using MailKit;
using PPNewsletterFilter;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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

        public void UpdateEmailList(List<Tuple<string, int, bool, string>> emailMap)
        {
            Emails.Clear();
            foreach (var entry in emailMap)
            {
                if (entry.Item3)
                {
                    Emails.Add(new EmailInfo { Sender = entry.Item1, Count = entry.Item2, UnsubscribeButtonVisibility = Visibility.Visible, UnsubscribeLink = entry.Item4 });
                }
                else
                {
                    Emails.Add(new EmailInfo { Sender = entry.Item1, Count = entry.Item2, UnsubscribeButtonVisibility = Visibility.Hidden });
                }
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
            if (sender is Button button && button.CommandParameter is string unsubscribeLink)
            {
                if (!string.IsNullOrEmpty(unsubscribeLink))
                {
                    try
                    {
                        // Open the link in the default browser
                        System.Diagnostics.Process.Start(new ProcessStartInfo
                        {
                            FileName = unsubscribeLink,
                            UseShellExecute = true // UseShellExecute ensures the link opens in the default browser
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to open unsubscribe link: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Unsubscribe link is not available.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is string adressDelete)
            {

                var inbox = Data.Client.Inbox;
                for (int i = 0; i < inbox.Count; i++)
                {
                    var message = inbox.GetMessage(i);

                    if (message.From.ToString().Contains(adressDelete))
                    {
                        inbox.Store(i, new StoreFlagsRequest(StoreAction.Add, MessageFlags.Deleted) { Silent = true });
                    }
                }
                inbox.Expunge();

                var popup = FindParentPopup(button);
                if (popup != null)
                {
                    popup.IsOpen = false; // Close the popup after the delete logic is complete
                }

            }

        }
        private Popup FindParentPopup(DependencyObject child)
        {
            while (child != null)
            {
                if (child is Popup popup)
                    return popup;

                child = VisualTreeHelper.GetParent(child);
            }
            return null;
        }

    }

    public class EmailInfo
    {
        public string? Sender { get; set; }
        public int Count { get; set; }
        public Visibility UnsubscribeButtonVisibility { get; set; } = Visibility.Visible;
        public string? UnsubscribeLink { get; set; }

    }


}
