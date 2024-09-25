using System;
using System.Windows;
using System.Windows.Controls;
using MailKit.Net.Imap;
using MailKit;
using System.Windows.Input;
using System.Xml.Schema;
using System.Windows.Media;

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

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string mail = txtEmail.Text;
            string password = pwdPassword.Password;

            //if email empty return with color
            if (mail == "") {
                Mail.Foreground = new SolidColorBrush(Colors.Red);
            }else
            {
                Mail.Foreground = new SolidColorBrush(Colors.WhiteSmoke);
            }
            //if password empty return with color
            if (password == "")
            {
                Password.Foreground = new SolidColorBrush(Colors.Red);
                return;
            }else
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
                    }

                    // Open MainWindow and update email list
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    mainWindow.UpdateEmailList(map);

                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Something went wrong while processing the incoming messages", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}