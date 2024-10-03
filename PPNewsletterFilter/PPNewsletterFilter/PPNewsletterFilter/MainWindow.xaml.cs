using PPNewsletterFilter;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PPNewsletterFilter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<EmailInfo> Emails { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Emails = new ObservableCollection<EmailInfo>();
            DataContext = this;

        }

        public void UpdateEmailList(Dictionary<string, int> emailMap)
        {
            Emails.Clear();
            foreach (var pair in emailMap)
            {
                Emails.Add(new EmailInfo { Sender = pair.Key, Count = pair.Value });
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Event handler for mouse on the window
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private void btnFilterLoad_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btnUnsubscribe_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

        }

    }

    public class EmailInfo
    {
        public string? Sender { get; set; }
        public int Count { get; set; }
    }


}
