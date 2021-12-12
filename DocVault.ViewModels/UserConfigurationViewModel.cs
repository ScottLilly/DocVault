using System.ComponentModel;
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