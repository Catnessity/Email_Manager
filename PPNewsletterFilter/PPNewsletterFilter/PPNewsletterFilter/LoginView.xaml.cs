using System;
using System.Windows;
using System.Windows.Controls;
using MailKit.Net.Imap;
using MailKit;
using System.Windows.Input;

namespace PPNewsletterFilter
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string mail = txtEmail.Text;
            string password = pwdPassword.Password;

            

            // Connect to IMAP server and authenticate
            using (var client = new ImapClient())
            {
                try
                {
                    client.Connect("imap.gmx.net", 993, true);
                    client.Authenticate(mail, password);

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
                    }

                    // Open MainWindow and update email list
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    mainWindow.UpdateEmailList(map);

                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}