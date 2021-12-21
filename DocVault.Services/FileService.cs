using System.IO;
using System.Security.Cryptography;

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
    }
}