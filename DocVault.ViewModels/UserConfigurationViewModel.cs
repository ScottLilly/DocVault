using System.ComponentModel;
using DocVault.Models;

namespace DocVault.ViewModels
{
    public class UserConfigurationViewModel : INotifyPropertyChanged
    {
        public UserSettings UserSettings { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public UserConfigurationViewModel(UserSettings userSettings)
        {
            UserSettings = userSettings;
        }
    }
}