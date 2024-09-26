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

namespace PPNewsletterFilter
{
    public partial class LoginView : Window
    {
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
            // Show the loading overlay
            loadingOverlay.Visibility = Visibility.Visible;

            // Run long operation asynchronously
            bool success = await Task.Run(() => ConnectToServer());

            if (success)
            {
                // If successful, open the main window
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close(); // Close the login window
            }
        }

        private bool ConnectToServer()
        {

                string mail = string.Empty;
                string password = string.Empty;

                this.Dispatcher.Invoke(() =>
                {
                    mail = txtEmail.Text;
                    password = pwdPassword.Password;
                });

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
                    return;
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
                    return;
                }

                try
                {
                    // The Inbox folder is always available on all IMAP servers...
                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadWrite);

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
                        }

                        // Progresscounter
                        this.Dispatcher.Invoke(() =>
                        {
                            LoadingText.Text = "Loading... " + (i + 1).ToString() + " / " + inbox.Count.ToString();
                        });
                    }

                    // Open MainWindow and update email list
                    this.Dispatcher.Invoke(() =>
                    {
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.UpdateEmailList(map);
                    });
                    return true;

                }            
                catch (Exception ex)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"Error: {ex.Message}", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);

                    });
                    return false;
                }
            }
        }
    }
}
