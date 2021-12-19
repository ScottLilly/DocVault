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

        public bool EncryptedFileLocationChanged =>
            !_savedUserSettings.EncryptedStorageLocation.URI.Trim().Equals(NewEncryptedLocationURI.Trim(),
                StringComparison.InvariantCultureIgnoreCase);

        private DirectoryInfo EncryptedFilesDirectoryInfo =>
            new DirectoryInfo(_savedUserSettings.EncryptedStorageLocation.URI);

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

            // On startup, set new editable values from currently-saved values
            Revert();
        }

        public void Revert()
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
    }
}