using System;
using System.Windows;
using System.Windows.Controls;
using MailKit.Net.Imap;
using MailKit;
using System.Windows.Input;
using PPNewsletterFilter;

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
        }

        private void txtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {

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

            try
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
                    this.Dispatcher.Invoke(() =>
                    {
                        // Open MainWindow and update email list
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.UpdateEmailList(map);
                    });
                }
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