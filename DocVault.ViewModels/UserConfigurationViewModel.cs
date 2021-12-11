using System.ComponentModel;
using DocVault.Models;
using DocVault.Services;

namespace DocVault.ViewModels
{
    public class UserConfigurationViewModel : INotifyPropertyChanged
    {
        private readonly SettingsManager<UserSettings> _settingsManager;
        private readonly UserSettings _originalUserSettings;

        public UserSettings UserSettings { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public UserConfigurationViewModel(SettingsManager<UserSettings> settingsManager,
            UserSettings userSettings)
        {
            _originalUserSettings = userSettings.Clone();

            UserSettings = userSettings;

            _settingsManager = settingsManager;
        }

        public void SaveSettings()
        {
            _settingsManager.SaveSettings(UserSettings);
        }

        public void RevertChanges()
        {
            UserSettings.EncryptedStorageLocation.Type = 
                _originalUserSettings.EncryptedStorageLocation.Type;
            UserSettings.EncryptedStorageLocation.URI =
                _originalUserSettings.EncryptedStorageLocation.URI;
            UserSettings.DecryptedStorageLocation.Type =
                _originalUserSettings.DecryptedStorageLocation.Type;
            UserSettings.DecryptedStorageLocation.URI =
                _originalUserSettings.DecryptedStorageLocation.URI;
        }
    }
}