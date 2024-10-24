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
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PPNewsletterFilter
{
    public partial class LoadingView : Window
    {
        bool loadingDone = false;
 
        public LoadingView()
        {
            InitializeComponent();
            DataContext = this; // Set data context to this window for progress
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await Loading();
            //Loading();
        }

        async Task Loading()
        {
            try
            {
                // Ensure the ProgressBar is in the correct state
                progressBar.IsIndeterminate = false;

                // Start loading emails and updating the progress bar
                await StartLoading();

                // After loading is complete, show the main window
                ShowMainWindow();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Something went wrong", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        async Task StartLoading()
        {
            var inbox = Data.Client.Inbox;
            Data.map = new List<EmailInfo>();
            inbox.Open(FolderAccess.ReadWrite);
            int messageCount = inbox.Count;

            // Set ProgressBar maximum value based on message count
            progressBar.Maximum = messageCount;
            var summaries = inbox.Fetch(0, -1, MessageSummaryItems.UniqueId);
            for (int i = 0; i < messageCount; i++)
            {
                // Fetch email message asynchronously
                var message = await Task.Run(() => inbox.GetMessage(i));
                var uid = summaries[i].UniqueId;
                var sender = message.From.ToString();
                var listUnsubscribeHeader = message.Headers["List-Unsubscribe"];
                string unsubscribeLink = null;
                var emaildate = message.Date.ToString("yyyy-MM-dd");
                if (listUnsubscribeHeader != null)
                    unsubscribeLink = ExtractUnsubscribeLink(listUnsubscribeHeader);

                // Find if the sender already exists in the map
                var existingEntry = Data.map.FirstOrDefault(x => x.Sender == sender);
                if (existingEntry != null)
                {
                    List<UniqueId> existingUids = existingEntry.UniqueIDs;
                    existingUids.Add(uid);
                    // Update the existing entry
                    var updatedEntry = new EmailInfo(
                        sender,
                        existingEntry.Count + 1,  // Increment the count
                        unsubscribeLink,  // Update the unsubscribe header if available
                        existingUids,
                        emaildate
                    );

                    // Replace the old entry with the updated one
                    Data.map[Data.map.IndexOf(existingEntry)] = updatedEntry;
                }
                else
                {
                    // Create a new entry if the sender is not in the map yet
                    List<UniqueId> list = new List<UniqueId>();
                    list.Add(uid);
                    var newEntry = new EmailInfo(
                        sender,
                        1,  // Start with a count of 1
                        unsubscribeLink, 
                        list,
                        emaildate
                    );
                    Data.map.Add(newEntry);
                }

                // Update the UI with progress (must be done on the UI thread)
                Application.Current.Dispatcher.Invoke(() =>
                {
                    LoadingText.Text = $"Loading... {i + 1} / {messageCount}";
                    progressBar.Value = i + 1; // Update progress bar value
                });

                // Optionally, add a small delay to visualize the progress
                await Task.Delay(10);  // Short delay for better UI experience
            }

            // Mark loading as complete (optional if needed elsewhere)
            loadingDone = true;

        }




        public void ShowMainWindow()
        {
            MainWindow mainWindow = new MainWindow();
            // Open MainWindow and update email list
            mainWindow.Show();
            mainWindow.UpdateEmailList(Data.map);
            this.Close();
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
        private string ExtractUnsubscribeLink(string input)
        {
            // Define a regex pattern to match only HTTP(S) URLs
            string pattern = @"https?://[^\s,<>]+";
            var matches = Regex.Matches(input, pattern);

            // If a match is found, return the first match (the URL)
            if (matches.Count > 0)
            {
                return matches[0].Value;  // Adjust index if you want to prioritize a specific match
            }

            // Return an empty string if no URL is found
            return string.Empty;
        }

    }
}
