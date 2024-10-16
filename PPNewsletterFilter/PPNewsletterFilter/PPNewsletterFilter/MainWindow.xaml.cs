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
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PPNewsletterFilter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<EmailInfo> Emails { get; set; }
        public List<Tuple<string, int, bool, string, List<UniqueId>, string>> allMails { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Emails = new ObservableCollection<EmailInfo>();
            this.DataContext = this;
            Data.InitializeUnsubscribedSenders();
            List<string> senderHaveIgnoredUnsubscribe = new List<string>();

            foreach (var mailinfo in Data.map)
            {
                if (mailinfo.Item1 != null)
                {

                    if (Data.SenderIgnoredUnsubscribe(mailinfo.Item1, mailinfo.Item6))
                    {
                        senderHaveIgnoredUnsubscribe.Add(mailinfo.Item1);
                    }
                }

            }
            if (senderHaveIgnoredUnsubscribe.Count > 0)
            {
                //open notification window
                MessageBox.Show(
                $"The following senders have ignored your previous unsubscription:\n{string.Join(", ", senderHaveIgnoredUnsubscribe)}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
                );
            }
        }

        public void UpdateEmailList(List<Tuple<string, int, bool, string, List<UniqueId>, string>> emailMap)
        {
            allMails = emailMap;
            Emails.Clear();
            foreach (var entry in emailMap)
            {
                if (entry.Item3)
                {
                    Emails.Add(new EmailInfo { Sender = entry.Item1, Count = entry.Item2, UnsubscribeButtonVisibility = Visibility.Visible, UnsubscribeLink = entry.Item4, UniqueIDs = entry.Item5, DateLastSent = entry.Item6 });
                }
                else
                {
                    Emails.Add(new EmailInfo { Sender = entry.Item1, Count = entry.Item2, UnsubscribeButtonVisibility = Visibility.Hidden, UniqueIDs = entry.Item5, DateLastSent = entry.Item6 });
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
            Emails.Clear();
            if (filterNewsletter.IsChecked == true && filterKeyWord.Text == "")
            {
                foreach (var entry in allMails)
                {
                    if (entry.Item3)
                    {
                        Emails.Add(new EmailInfo { Sender = entry.Item1, Count = entry.Item2, UnsubscribeButtonVisibility = Visibility.Visible, UnsubscribeLink = entry.Item4, UniqueIDs = entry.Item5, DateLastSent = entry.Item6 });
                    }
                }
            }
            else if (filterNewsletter.IsChecked == true && filterKeyWord.Text != "")
            {
                foreach (var entry in allMails)
                {
                    if (entry.Item3 && entry.Item1.Contains(filterKeyWord.Text))
                    {
                        Emails.Add(new EmailInfo { Sender = entry.Item1, Count = entry.Item2, UnsubscribeButtonVisibility = Visibility.Visible, UnsubscribeLink = entry.Item4, UniqueIDs = entry.Item5, DateLastSent = entry.Item6 });
                    }
                }
            }
            else if (filterNewsletter.IsChecked == false && filterKeyWord.Text != "")
            {
                foreach (var entry in allMails)
                {
                    if (entry.Item3 && entry.Item1.Contains(filterKeyWord.Text))
                    {
                        Emails.Add(new EmailInfo { Sender = entry.Item1, Count = entry.Item2, UnsubscribeButtonVisibility = Visibility.Visible, UnsubscribeLink = entry.Item4, UniqueIDs = entry.Item5, DateLastSent = entry.Item6 });
                    }
                    else if (!entry.Item3 && entry.Item1.Contains(filterKeyWord.Text))
                    {
                        Emails.Add(new EmailInfo { Sender = entry.Item1, Count = entry.Item2, UnsubscribeButtonVisibility = Visibility.Hidden, UniqueIDs = entry.Item5, DateLastSent = entry.Item6 });

                    }
                }
            }
            else
            {
                foreach (var entry in allMails)
                {
                    if (entry.Item3)
                    {
                        Emails.Add(new EmailInfo { Sender = entry.Item1, Count = entry.Item2, UnsubscribeButtonVisibility = Visibility.Visible, UnsubscribeLink = entry.Item4, UniqueIDs = entry.Item5, DateLastSent = entry.Item6 });
                    }
                    else
                    {
                        Emails.Add(new EmailInfo { Sender = entry.Item1, Count = entry.Item2, UnsubscribeButtonVisibility = Visibility.Hidden, UniqueIDs = entry.Item5, DateLastSent = entry.Item6 });
                    }
                }
            }
        }
        private void btnUnsubscribe_Click(object sender, RoutedEventArgs e)
        {


            if (sender is Button button)
            {
                if (button.CommandParameter is EmailInfo info)
                {


                    if (info == null || info.UnsubscribeLink == null || info.Sender == null)
                    {
                        return;
                    }
                    try
                    {
                        // Open the link in the default browser
                        System.Diagnostics.Process.Start(new ProcessStartInfo
                        {
                            FileName = info.UnsubscribeLink,
                            UseShellExecute = true // UseShellExecute ensures the link opens in the default browser
                        });

                        Data.AddDataToUnsubscribedSenders(info.Sender, info.UnsubscribeLink);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to open unsubscribe link: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }
            }
            else
            {
                MessageBox.Show("Unsubscribe link is not available.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }



        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var selectedItem = EmailDataGrid.SelectedItem as EmailInfo;
            if (selectedItem != null)
            {
                // Remove the selected item from the collection
                Emails.Remove(selectedItem);
            }
            if (sender is Button button && button.CommandParameter is List<UniqueId> uids)
            {

                var inbox = Data.Client.Inbox;

                inbox.Store(uids, new StoreFlagsRequest(StoreAction.Add, MessageFlags.Deleted) { Silent = true });

                inbox.Expunge();

            }


            // Stop the stopwatch
            stopwatch.Stop();

            // Get the elapsed time in milliseconds
            long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            filterKeyWord.Text = $"Elapsed time: {elapsedMilliseconds} milliseconds";
        }

    }

    public class EmailInfo
    {
        public string? Sender { get; set; }
        public int Count { get; set; }
        public Visibility UnsubscribeButtonVisibility { get; set; } = Visibility.Visible;
        public string? UnsubscribeLink { get; set; }
        public List<UniqueId>? UniqueIDs { get; set; }
        public string? DateLastSent { get; set; }

    }


}
