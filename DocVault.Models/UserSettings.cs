namespace DocVault.Models
{
    public class UserSettings
    {
        public StorageLocation EncryptedStorageLocation { get; set; }
        public StorageLocation DecryptedStorageLocation { get; set; }
    }
}