using System.IO;
using System.Security.Cryptography;
using DocVault.Models;

namespace DocVault.Services
{
    public static class FileService
    {
        public static byte[] ComputeMD5ChecksumForFile(string fileName)
        {
            using MD5 md5 = MD5.Create();
            using FileStream stream = File.OpenRead(fileName);

            return md5.ComputeHash(stream);
        }

        public static void CreateDocumentDirectoriesIfMissing(UserSettings userSettings)
        {
            CreateDirectoryIfMissing(userSettings.EncryptedStorageLocation.URI);
            CreateDirectoryIfMissing(userSettings.DecryptedStorageLocation.URI);
        }

        private static void CreateDirectoryIfMissing(string path)
        {
            if (!string.IsNullOrWhiteSpace(path) &&
                !Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}