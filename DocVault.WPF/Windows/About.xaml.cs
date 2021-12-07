using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace DocVault.WPF.Windows
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            ProcessStartInfo processStartInfo =
                new(e.Uri.ToString())
                {
                    UseShellExecute = true
                };

            Process.Start(processStartInfo);
        }

        private void OK_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}