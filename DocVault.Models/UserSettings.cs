using System.ComponentModel;

namespace DocVault.Models
{
    public class UserSettings : INotifyPropertyChanged
    {
        public StorageLocation EncryptedStorageLocation { get; }
        public StorageLocation DecryptedStorageLocation { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public UserSettings(StorageLocation encryptedStorageLocation,
            StorageLocation decryptedStorageLocation)
        {
            EncryptedStorageLocation = encryptedStorageLocation;
            DecryptedStorageLocation = decryptedStorageLocation;
        }
    }
}