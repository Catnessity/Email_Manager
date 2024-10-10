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
using System.Windows.Media.Imaging;


namespace PPNewsletterFilter
{

    public partial class LoginView : Window
    {

        public LoginView()
        {
            InitializeComponent();
        }
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            ConnectToServer();
        }

        private void ConnectToServer()
        {

            string mail = string.Empty;
            string password = string.Empty;
            mail = txtEmail.Text;
            password = pwdPassword.Password;
            string vispassword = pwdPasswordShow.Text;

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
            if (password == "" && vispassword == "")
            {
                Password.Foreground = new SolidColorBrush(Colors.Red);
                return;
            }

            else
            {
                Password.Foreground = new SolidColorBrush(Colors.WhiteSmoke);
            }

            // Connect to IMAP server and authenticate
            Data.Client = new ImapClient();

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
                    imap_address = "outlook.office365.com";
                }
                else
                {
                    feedback.Text = "Please check your email, either there is a typo or your provider is not supported.";
                    return;
                }

                //try to connect to the imap server with the given password
                try
                {
                    Data.Client.Connect(imap_address, 993, true);
                    if(password != "")
                    Data.Client.Authenticate(mail, password);
                    else if(vispassword != "")
                    Data.Client.Authenticate(mail, vispassword);
            }
                catch (Exception ex)
                {
                    feedback.Text = "The connection to the server or the authentication failed. \n Check your password and try again.";
                    return;
                }

                try
                {
                    ShowLoadingView();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Something went wrong while processing the incoming messages", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

            
        }

        public void ShowLoadingView()
        {
            LoadingView loadingview = new LoadingView();
            this.Close(); // Close the login window
            loadingview.Show();

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
        
        private void PasswordField_Enter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }

        //private void ShowPassword_Checked(object sender, RoutedEventArgs e)
        //{
        //    pwdPasswordShow.Text = pwdPassword.Password;
        //    pwdPassword.Visibility = Visibility.Hidden;
        //    pwdPasswordShow.Visibility = Visibility.Visible;
        //}

        //private void HidePassword_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    pwdPassword.Password = pwdPasswordShow.Text;
        //    pwdPassword.Visibility = Visibility.Visible;
        //    pwdPasswordShow.Visibility = Visibility.Hidden;
        //}

        private bool isPasswordVisible = false;

        private void TogglePasswordVisibility(object sender, MouseButtonEventArgs e)
        {
            if (isPasswordVisible)
            {
                // Hide the password
                pwdPassword.Password = pwdPasswordShow.Text;
                pwdPassword.Visibility = Visibility.Visible;
                pwdPasswordShow.Visibility = Visibility.Hidden;
                imgTogglePassword.Source = new BitmapImage(new Uri("images/eye_closed.png", UriKind.Relative)); // Change to "eye-closed" image
                isPasswordVisible = false;
            }
            else
            {
                // Show the password
                pwdPasswordShow.Text = pwdPassword.Password;
                pwdPassword.Visibility = Visibility.Hidden;
                pwdPasswordShow.Visibility = Visibility.Visible;
                imgTogglePassword.Source = new BitmapImage(new Uri("images/eye_open.png", UriKind.Relative)); // Change to "eye-open" image
                isPasswordVisible = true;
            }
        }


    }
}
