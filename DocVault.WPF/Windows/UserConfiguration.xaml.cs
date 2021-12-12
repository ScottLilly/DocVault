using System.Windows;
using DocVault.ViewModels;

namespace DocVault.WPF.Windows
{
    public partial class UserConfiguration : Window
    {
        private UserConfigurationViewModel VM => DataContext as UserConfigurationViewModel;

        public UserConfiguration(UserConfigurationViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }

        private void EncryptedFileLocationSelection_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void DecryptedFileLocationSelection_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void Revert_OnClick(object sender, RoutedEventArgs e)
        {
            VM.Revert();
        }

        private void SaveChanges_OnClick(object sender, RoutedEventArgs e)
        {
            VM.SaveChanges();

            Close();
        }
    }
}