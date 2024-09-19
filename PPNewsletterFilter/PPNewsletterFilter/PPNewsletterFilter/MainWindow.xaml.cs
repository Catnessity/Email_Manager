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
        public MainWindow()
        {
            InitializeComponent();
        }

        public void UpdateEmailList(Dictionary<string, int> emailMap)
        {
            lstEmails.Items.Clear();
            foreach (var pair in emailMap)
            {
                lstEmails.Items.Add($"{pair.Key}: {pair.Value} Email Count");
            }
        }
    }
}