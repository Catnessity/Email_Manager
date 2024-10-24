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
using System.Xml.Linq;
using System.ComponentModel;

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
            this.DataContext = this;
            Data.InitializeUnsubscribedSenders();
            Data.CheckUnsubscribedSendersAndNotify();
          
        }

        public void UpdateEmailList(List<EmailInfo> emailMap)
        {
            Data.map = emailMap;
            Emails.Clear();
            foreach (var entry in emailMap)
            {
                if (entry.HasLink)
                {
                    Emails.Add(new EmailInfo(entry.Sender, entry.Count, entry.UnsubscribeLink, entry.UniqueIDs, entry.DateLastSent));
                }
                else
                {
                    var mail = new EmailInfo(entry.Sender, entry.Count, entry.UnsubscribeLink, entry.UniqueIDs, entry.DateLastSent);
                    mail.UnsubscribeButtonVisibility = Visibility.Hidden;
                    Emails.Add(mail);
                }
                if (Data.CheckIfSenderIsUnsubscribed(entry.Sender))
                {
                    entry.IsUnsubscribed = true;
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

            //show only newsletter
            if (filterNewsletter.IsChecked == true && filterKeyWord.Text == "")
            {
                foreach (var entry in Data.map)
                {
                    if (entry.HasLink)
                    {
                        Emails.Add(new EmailInfo(entry.Sender, entry.Count, entry.UnsubscribeLink, entry.UniqueIDs, entry.DateLastSent));
                    }
                }
            }
            //show newsletter that contain text
            else if (filterNewsletter.IsChecked == true && filterKeyWord.Text != "")
            {
                foreach (var entry in Data.map)
                {
                    if (entry.HasLink && entry.Sender.ToLower().Contains(filterKeyWord.Text.ToLower()))
                    {
                        Emails.Add(new EmailInfo(entry.Sender, entry.Count, entry.UnsubscribeLink, entry.UniqueIDs, entry.DateLastSent));
                    }                    
                }
            }
            //show all mails that contain text
            else if (filterNewsletter.IsChecked == false && filterKeyWord.Text != "")
            {
                foreach (var entry in Data.map)
                {
                    if (entry.Sender.ToLower().Contains(filterKeyWord.Text.ToLower()))
                    {
                        Emails.Add(new EmailInfo(entry.Sender, entry.Count, entry.UnsubscribeLink, entry.UniqueIDs, entry.DateLastSent));
                    }
                }
            }
            else
            {
                //no filters selected
                foreach (var entry in Data.map)
                {
                    if (entry.HasLink)
                    {
                        Emails.Add(new EmailInfo(entry.Sender, entry.Count, entry.UnsubscribeLink, entry.UniqueIDs, entry.DateLastSent));
                    }
                    else
                    {
                        var mail = new EmailInfo(entry.Sender, entry.Count, entry.UnsubscribeLink, entry.UniqueIDs, entry.DateLastSent);
                        mail.UnsubscribeButtonVisibility = Visibility.Hidden;
                        Emails.Add(mail);
                    }
                }

            }
        }
        private void FilterField_Enter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnFilterLoad_Click(sender, e);
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
                        info.IsUnsubscribed = true;
                        //somehow update gui

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
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
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


            //// Stop the stopwatch
            //stopwatch.Stop();

            //// Get the elapsed time in milliseconds
            //long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

        }
    


    }

    public class EmailInfo
    {


        public string? Sender { get; set; }
        public int Count { get; set; }

        public bool HasLink { get; set; }

        public bool IsUnsubscribed { get; set; }
        public Visibility UnsubscribeButtonVisibility { get; set; } = Visibility.Hidden;
        public string? UnsubscribeLink { get; set; }
        public List<UniqueId>? UniqueIDs { get; set; }
        public string? DateLastSent { get; set; }
       

        public EmailInfo(string? sender, int count, string? unsubscribeLink, List<UniqueId> uids, string? dateLastSent)
        {
            Sender = sender;
            Count = count;
            if (UnsubscribeLink == null) { UnsubscribeLink = unsubscribeLink; };
            HasLink = (UnsubscribeLink != null) ? true : false;
            if (UnsubscribeLink != null) { UnsubscribeButtonVisibility = Visibility.Visible; }; 
            UniqueIDs = uids;
            DateLastSent = dateLastSent;
            IsUnsubscribed = false; //default
        }

    }
   
}
