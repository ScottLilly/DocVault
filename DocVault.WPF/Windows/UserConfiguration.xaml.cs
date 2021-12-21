using System.Collections.Generic;
using System.Windows;
using DocVault.Services;
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
            VM.SetEditingValuesToSavedValues();
        }

        private void SaveChanges_OnClick(object sender, RoutedEventArgs e)
        {
            if (VM.HasNoChanges)
            {
                Close();
            }

            // No additional check needed, if only change is DecryptedLocation
            if (VM.EncryptedFileLocationIsUnchanged)
            {
                VM.SaveChanges();
                Close();
            }

            // EncryptedLocation has changed, so do additional checks

            // EncryptedLocation property is not null/empty
            // TODO: Popup warning and remain on UserConfiguration window
            // Maybe do in textbox in the UI

            // New EncryptedLocation already exists, and has files
            if (VM.NewEncryptedLocationExists && VM.NewEncryptedLocationExistingFiles > 0)
            {
                YesNo useExistingDirectory =
                    new YesNo("Use Existing Directory", new List<string>
                    {
                        $"The directory '{VM.NewEncryptedLocationURI}' already exists",
                        $"and contains {VM.NewEncryptedLocationExistingFiles:N0} files",
                        "Are you sure you want to use it?"
                    });

                useExistingDirectory.Owner = this;

                useExistingDirectory.ShowDialog();

                if (useExistingDirectory.ResponseIsYes)
                {
                    VM.MoveEncryptedFiles();
                    VM.SaveChanges();
                    Close();
                }
                else
                {
                    return;
                }
            }

            // No encrypted files to move, allow change to EncryptedLocation
            if (VM.EncryptedFilesCount == 0)
            {
                VM.SaveChanges();
                Close();
            }
            else
            {
                // Show message and do not continue
                // if NewEncryptedLocation does not have enough available disk space for files.
                if (VM.EncryptedFilesSize > VM.NewEncryptedLocationAvailableSpace)
                {
                    OK notEnoughDiskSpace =
                        new OK("Move Existing Files", new List<string>
                        {
                            "Do you want to move the existing files?",
                            $"{VM.EncryptedFilesCount} files",
                            $"{VM.FormattedEncryptedFilesSize} in files"
                        });
                    notEnoughDiskSpace.Owner = this;

                    notEnoughDiskSpace.ShowDialog();

                    return;
                }

                // Notify user files will be moved
                // Don't allow user to change EncryptedLocation without moving existing files.
                YesNo moveExistingFiles =
                    new YesNo("Move Existing Files", new List<string>
                    {
                        "Do you want to move the existing files?",
                        $"{VM.EncryptedFilesCount} files",
                        $"{VM.FormattedEncryptedFilesSize} in total size"
                    });
                moveExistingFiles.Owner = this;

                moveExistingFiles.ShowDialog();

                if (moveExistingFiles.ResponseIsYes)
                {
                    VM.MoveEncryptedFiles();
                    VM.SaveChanges();
                    Close();
                }
                else
                {
                    // Don't allow change to EncryptedLocation without moving existing encrypted files.
                    VM.SetEditingValuesToSavedValues();
                }
            }
        }
    }
}