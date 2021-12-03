namespace DocVault.Models
{
    public class StorageLocation
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
    }
}