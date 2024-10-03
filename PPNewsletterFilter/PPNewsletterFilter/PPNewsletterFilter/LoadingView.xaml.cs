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
            Data.map = new Dictionary<string, int>();
            inbox.Open(FolderAccess.ReadWrite);
            int messageCount = inbox.Count;

            // Set ProgressBar maximum value based on message count
            progressBar.Maximum = messageCount;

            for (int i = 0; i < messageCount; i++)
            {
                // Fetch email message asynchronously
                var message = await Task.Run(() => inbox.GetMessage(i));

                // Process email (update dictionary with the sender)
                if (Data.map.ContainsKey(message.From.ToString()))
                {
                    Data.map[message.From.ToString()]++;
                }
                else
                {
                    Data.map.Add(message.From.ToString(), 1);
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

    }
}
