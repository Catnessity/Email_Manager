using System;
using System.Windows;
using System.Windows.Controls;
using MailKit.Net.Imap;
using MailKit;
using System.Windows.Input;
using PPNewsletterFilter;
using System.ComponentModel;
using System.Xml.Schema;
using System.Windows.Media;
using System.Windows.Threading;
using System.Printing;

namespace PPNewsletterFilter
{
    public partial class LoginView : Window
    {
        static bool connectedToServer;

        public LoginView()
        {
            InitializeComponent();
            DataContext = this; // Set data context to this window for progress
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Event handler for mouse on the window
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void txtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnFullScreen_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            // Run long operation asynchronously
            //bool success = await Task.Run(() => ConnectToServer());
            if (Application.Current.Dispatcher.CheckAccess())
            {
                ConnectToServer();
            }
            else
            {
                // Otherwise, marshal the function back to the UI thread asynchronously
                Application.Current.Dispatcher.BeginInvoke((Action)(() => ConnectToServer()));
            }


            if (connectedToServer)
            {
                //Thread.Sleep(2000);
                //await Task.Run(() => StartLoading());

                // If successful, open the main window
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close(); // Close the login window
            }
        }

        //async switchToMainWindow((object sender, RoutedEventArgs e){

        //}

        private async void ConnectToServer()
        {

            string mail = string.Empty;
            string password = string.Empty;
            //this.Dispatcher.Invoke(() =>
            //        {
            mail = txtEmail.Text;
            password = pwdPassword.Password;
            //});

            //if email empty return with color
            if (mail == "")
            {
                Mail.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                Mail.Foreground = new SolidColorBrush(Colors.WhiteSmoke);
            }
            //if password empty return with color
            if (password == "")
            {
                Password.Foreground = new SolidColorBrush(Colors.Red);
                connectedToServer = false;
            }
            else
            {
                Password.Foreground = new SolidColorBrush(Colors.WhiteSmoke);
            }

            // Connect to IMAP server and authenticate
            using (var client = new ImapClient())
            {
                var imap_address = "";

                //get the corret imap address based on the given email provider
                if (mail.Contains("@gmx."))
                {
                    imap_address = "imap.gmx.net";
                }
                else if (mail.Contains("@gmail."))
                {
                    imap_address = "imap.gmail.com";
                }
                else if (mail.Contains("@outlook.") || password.Contains("@hotmail"))
                {
                    imap_address = "imap-mail.outlook.com";
                }
                else
                {
                    //MessageBox.Show($"Please check your email.", "Invalid Email", MessageBoxButton.OK, MessageBoxImage.Warning);
                    feedback.Text = "Please check your email, either there is a typo or your provider is not supported.";
                    connectedToServer = false;
                }

                //try to connect to the imap server with the given password
                try
                {
                    client.Connect(imap_address, 993, true);
                    client.Authenticate(mail, password);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show($"Please check your credentials.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                    feedback.Text = "The connection to the server or the authentication failed. \n Check your password and try again.";
                    connectedToServer = false;
                }

                try
                {
                    // The Inbox folder is always available on all IMAP servers...
                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadWrite);

                    await Task.Run(() => StartLoading(client, inbox));
                    connectedToServer = true;

                }
                catch (Exception ex)
                {

                    MessageBox.Show($"Error: {ex.Message}", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    connectedToServer = false;
                }
            }


        }



        private async void StartLoading(ImapClient client, IMailFolder inbox)
        {
            inbox.Open(FolderAccess.ReadWrite);
            var MessageCount = inbox.Count;
            Dictionary<string, int> map = new Dictionary<string, int>();

            for (int i = 0; i < inbox.Count; i++)
            {
                var message = inbox.GetMessage(i);
                if (map.ContainsKey(message.From.ToString()))
                {
                    map[message.From.ToString()]++;
                }
                else
                {
                    map.Add(message.From.ToString(), 1);
                    // Progresscounter
                    this.Dispatcher.Invoke(() =>
                    {
                        LoadingText.Text = "Loading... " + (i + 1).ToString() + " / " + inbox.Count.ToString();
                    });


                    // Open MainWindow and update email list
                    this.Dispatcher.Invoke(() =>
                    {
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.UpdateEmailList(map);
                    });
                }

            }
            //return true;
        }
    }
}
