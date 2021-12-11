using System.ComponentModel;

namespace DocVault.Models
{
    public class UserSettings : INotifyPropertyChanged
    {
        public StorageLocation EncryptedStorageLocation { get; set; }
        public StorageLocation DecryptedStorageLocation { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public UserSettings Clone()
        {
            UserSettings clonedSettings = new UserSettings();

            clonedSettings.EncryptedStorageLocation = new StorageLocation
            {
                Type = EncryptedStorageLocation.Type,
                URI = EncryptedStorageLocation.URI
            };

            clonedSettings.DecryptedStorageLocation = new StorageLocation
            {
                Type = DecryptedStorageLocation.Type,
                URI = DecryptedStorageLocation.URI
            };

            return clonedSettings;
        }
    }
}