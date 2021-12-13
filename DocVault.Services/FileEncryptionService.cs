using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DocVault.Models;

namespace DocVault.Services
{
    public sealed class FileEncryptionService : INotifyPropertyChanged
    {
        private readonly UserSettings _userSettings;
        
        private byte[] PasswordBytes { get; set; }

        public bool EncryptionKeyIsSet => 
            PasswordBytes is {Length: > 0};

        public event PropertyChangedEventHandler PropertyChanged;

        public FileEncryptionService(UserSettings userSettings)
        {
            _userSettings = userSettings;
        }

        public void SetUserEncryptionKey(string encryptionKey)
        {
            PasswordBytes = Encoding.ASCII.GetBytes(encryptionKey);
        }

        public async Task EncryptAndStoreDocument(Document document)
        {
            using RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;

            var key = new Rfc2898DeriveBytes(PasswordBytes, document.Salt, 1000);
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8);

            aes.Mode = CipherMode.CBC;

            if (!Directory.Exists(_userSettings.EncryptedStorageLocation.URI))
            {
                Directory.CreateDirectory(_userSettings.EncryptedStorageLocation.URI);
            }

            string outputFileName =
                Path.Combine(_userSettings.EncryptedStorageLocation.URI, document.NameInStorage);

            await using FileStream outputFileStream =
                new(outputFileName, FileMode.Create);

            await using var cryptoStream = 
                new CryptoStream(outputFileStream, aes.CreateEncryptor(), CryptoStreamMode.Write);

            int blockSizeBytes = aes.BlockSize / 8;
            byte[] data = new byte[blockSizeBytes];

            await using (FileStream inputFileStream =
                new(document.FileInfo.FullName, FileMode.Open))
            {
                int count = 0;

                do
                {
                    count = inputFileStream.Read(data, 0, blockSizeBytes);
                    cryptoStream.Write(data, 0, count);
                } while (count > 0);

                inputFileStream.Close();
            }

            await cryptoStream.FlushFinalBlockAsync();

            cryptoStream.Close();

            outputFileStream.Close();
        }

        public async Task RetrieveAndDecryptDocument(Document document)
        {
            using RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;

            var key = new Rfc2898DeriveBytes(PasswordBytes, document.Salt, 1000);
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8);

            aes.Mode = CipherMode.CBC;

            if (!Directory.Exists(_userSettings.DecryptedStorageLocation.URI))
            {
                Directory.CreateDirectory(_userSettings.DecryptedStorageLocation.URI);
            }

            // Decrypt the cipher text from from the encrypted file stream (inputFileStream)
            // into the decrypted file stream (outputFileStream).
            string decryptedFileName =
                Path.Combine(_userSettings.DecryptedStorageLocation.URI, document.OriginalName);

            await using FileStream outputFileStream = new(decryptedFileName, FileMode.Create);
            // blockSizeBytes can be any arbitrary size.
            int blockSizeBytes = aes.BlockSize / 8;
            byte[] data = new byte[blockSizeBytes];

            // Start at the beginning of the cipher text.
            await using FileStream inputFileStream =
                new(Path.Combine(_userSettings.EncryptedStorageLocation.URI, document.NameInStorage), FileMode.Open);

            inputFileStream.Seek(0, SeekOrigin.Begin);

            await using (CryptoStream decryptedOutputStream =
                new(outputFileStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
            {
                int count = 0;
                do
                {
                    count = inputFileStream.Read(data, 0, blockSizeBytes);
                    decryptedOutputStream.Write(data, 0, count);
                } while (count > 0);

                await decryptedOutputStream.FlushFinalBlockAsync();
                decryptedOutputStream.Close();
            }

            outputFileStream.Close();

            inputFileStream.Close();
        }
    }
}