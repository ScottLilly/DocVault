using System.ComponentModel;
using DocVault.Models;
using DocVault.Services;

namespace DocVault.ViewModels
{
    public class UserConfigurationViewModel : INotifyPropertyChanged
    {
        private readonly SettingsManager<UserSettings> _settingsManager;

        public UserSettings UserSettings { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public UserConfigurationViewModel(SettingsManager<UserSettings> settingsManager,
            UserSettings userSettings)
        {
            UserSettings = userSettings;

            _settingsManager = settingsManager;
        }

        public void SaveSettings()
        {
            _settingsManager.SaveSettings(UserSettings);
        }
    }
}