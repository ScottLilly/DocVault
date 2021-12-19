using System.Collections.Generic;
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
            if (VM.EncryptedFileLocationChanged)
            {
                YesNo yesNoWindow =
                    new YesNo("Move Existing Documents", new List<string>
                    {
                        "Do you want to move the existing files?",
                        $"{VM.EncryptedFilesCount} files",
                        $"{VM.FormattedEncryptedFileSize} in total size"
                    });
                yesNoWindow.Owner = this;

                yesNoWindow.ShowDialog();

                if (yesNoWindow.Response)
                {

                }
            }

            VM.SaveChanges();

            Close();
        }
    }
}