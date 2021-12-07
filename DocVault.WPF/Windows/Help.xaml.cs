using System.Windows;

namespace DocVault.WPF.Windows
{
    public partial class Help : Window
    {
        public Help()
        {
            InitializeComponent();
        }

        private void OK_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}