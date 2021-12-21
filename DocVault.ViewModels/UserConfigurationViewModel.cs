using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using DocVault.Models;
using DocVault.Services;

namespace DocVault.ViewModels
{
    public class UserConfigurationViewModel : INotifyPropertyChanged
    {
        private readonly SettingsManager<UserSettings> _settingsManager;

        private UserSettings _savedUserSettings;

        public StorageLocation.LocationType NewEncryptedLocationType { get; set; }
        public string NewEncryptedLocationURI { get; set; }
        public StorageLocation.LocationType NewDecryptedLocationType { get; set; }
        public string NewDecryptedLocationURI { get; set; }

        public bool HasNoChanges =>
            EncryptedFileLocationIsUnchanged && DecryptedFileLocationIsUnchanged;

        public bool EncryptedFileLocationIsUnchanged =>
            _savedUserSettings.EncryptedStorageLocation.URI.Trim().Equals(NewEncryptedLocationURI.Trim(),
                StringComparison.InvariantCultureIgnoreCase);
        public bool DecryptedFileLocationIsUnchanged =>
            _savedUserSettings.DecryptedStorageLocation.URI.Trim().Equals(NewDecryptedLocationURI.Trim(),
                StringComparison.InvariantCultureIgnoreCase);
        public bool EncryptedFileLocationChanged => !EncryptedFileLocationIsUnchanged;
        public bool DecryptedFileLocationChanged => !DecryptedFileLocationIsUnchanged;

        private DirectoryInfo EncryptedFilesDirectoryInfo =>
            new DirectoryInfo(_savedUserSettings.EncryptedStorageLocation.URI);
        private DirectoryInfo NewEncryptedFilesDirectoryInfo =>
            new DirectoryInfo(NewEncryptedLocationURI);

        public bool NewEncryptedLocationExists => NewEncryptedFilesDirectoryInfo.Exists;
        public long NewEncryptedLocationExistingFiles =>
            NewEncryptedLocationExists ? NewEncryptedFilesDirectoryInfo.GetFiles().Length : 0;

        public int EncryptedFilesCount =>
            EncryptedFilesDirectoryInfo.GetFiles().Length;
        public long EncryptedFilesSize =>
            EncryptedFilesDirectoryInfo.GetFiles().Sum(f => f.Length);

        public string FormattedEncryptedFileSize =>
            EncryptedFilesSize < 1000 ? $"{EncryptedFilesSize} bytes" :
            EncryptedFilesSize < 1000000 ? $"{EncryptedFilesSize / 1000:N1} KBs" :
            EncryptedFilesSize < 1000000000 ? $"{EncryptedFilesSize / 1000000:N1} MBs" :
            $"{EncryptedFilesSize / 1000000000:N1} GB";

        public event PropertyChangedEventHandler PropertyChanged;

        public UserConfigurationViewModel(SettingsManager<UserSettings> settingsManager,
            UserSettings userSettings)
        {
            _settingsManager = settingsManager;
            _savedUserSettings = userSettings;

            SetEditingValuesToSavedValues();
        }

        public void SetEditingValuesToSavedValues()
        {
            NewEncryptedLocationType = _savedUserSettings.EncryptedStorageLocation.Type;
            NewEncryptedLocationURI = _savedUserSettings.EncryptedStorageLocation.URI;
            NewDecryptedLocationType = _savedUserSettings.DecryptedStorageLocation.Type;
            NewDecryptedLocationURI = _savedUserSettings.DecryptedStorageLocation.URI;
        }

        public void SaveChanges()
        {
            _savedUserSettings.EncryptedStorageLocation.Type = NewEncryptedLocationType;
            _savedUserSettings.EncryptedStorageLocation.URI = NewEncryptedLocationURI;
            _savedUserSettings.DecryptedStorageLocation.Type = NewDecryptedLocationType;
            _savedUserSettings.DecryptedStorageLocation.URI = NewDecryptedLocationURI;

            _settingsManager.SaveSettings(_savedUserSettings);

            _savedUserSettings = _settingsManager.LoadSettings();
        }

        public void MoveEncryptedFiles()
        {
            if (NewEncryptedLocationExists)
            {
                foreach (string file in Directory.EnumerateFiles(_savedUserSettings.EncryptedStorageLocation.URI))
                {
                    string destinationFile =
                        Path.Combine(NewEncryptedLocationURI, Path.GetFileName(file));

                    // TODO: See issue to check that files doesn't already exist in new EncryptedLocation
                    if (!File.Exists(destinationFile))
                    {
                        File.Move(file, destinationFile);
                    }
                }
            }
            else
            {
                Directory.Move(_savedUserSettings.EncryptedStorageLocation.URI, NewEncryptedLocationURI);
            }
        }
    }
}