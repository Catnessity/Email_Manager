using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PPNewsletterFilter
{
    /// <summary>
    /// Interaktionslogik für LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }


        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string mail = Mail.Text;
            string password = Password.Password;

            // Display values in LoginView
            PasswordLabel.Content = $"Password: {password}";

            //open MainWindow
            MainWindow mainWindow = new MainWindow();

            this.Close();
            mainWindow.Show();
        
            


        }
    }
}
