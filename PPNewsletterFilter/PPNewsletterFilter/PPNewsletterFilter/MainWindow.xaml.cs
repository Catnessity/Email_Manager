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
    }

        public class EmailInfo
        {
            public string? Sender { get; set; }
            public int Count { get; set; }
        }
}