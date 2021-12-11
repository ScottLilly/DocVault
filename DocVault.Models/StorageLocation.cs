using System.ComponentModel;

namespace DocVault.Models
{
    public class StorageLocation : INotifyPropertyChanged
    {
        public enum LocationType
        {
            LocalDisk,
            NetworkShare,
            AWS,
            Azure
        }

        public LocationType Type { get; set; }
        public string URI { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}