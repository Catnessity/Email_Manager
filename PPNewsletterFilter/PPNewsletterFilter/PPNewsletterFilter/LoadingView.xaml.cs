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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Loading();
        }

        async void Loading()
        {
            
            if (Application.Current.Dispatcher.CheckAccess())
            {
                await StartLoading();
            }
            else
            {
                // Otherwise, marshal the function back to the UI thread asynchronously
                Application.Current.Dispatcher.BeginInvoke((Action)(() => StartLoading()));
            }

            if (loadingDone == true)
            {
                ShowMainWindow();
            }
        }

        async Task StartLoading()
        {
            var inbox = Data.Client.Inbox;
            Data.map = new Dictionary<string, int>();
            inbox.Open(FolderAccess.ReadWrite);
            var MessageCount = inbox.Count;
            Data.map = new Dictionary<string, int>();

            for (int i = 0; i < inbox.Count; i++)
            {
                var message = inbox.GetMessage(i);
                if (Data.map.ContainsKey(message.From.ToString()))
                {
                    Data.map[message.From.ToString()]++;
                }
                else
                {
                    Data.map.Add(message.From.ToString(), 1);
                    // Progresscounter

                    LoadingText.Text = "Loading... " + (i + 1).ToString() + " / " + inbox.Count.ToString();
                }
            }
            //set value to continue to mainwindow
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
