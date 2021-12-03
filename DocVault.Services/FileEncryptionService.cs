using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DocVault.Models;

namespace DocVault.Services
{
    public sealed class FileEncryptionService : INotifyPropertyChanged
    {
        private UserSettings _userSettings;

        private string ENCRYPTED_FILE_LOCATION => _userSettings.EncryptedStorageLocation.URI;
        private const string DECRYPTED_FILE_LOCATION = @"E:\Decrypt\";

        private static readonly RSACryptoServiceProvider s_rsaCryptoServiceProvider =
            new RSACryptoServiceProvider();

        private byte[] _aesKey;
        private byte[] _aesIV;

        private bool _encryptionKeyIsSet;

        public bool EncryptionKeyIsSet
        {
            get => _encryptionKeyIsSet;
            private set
            {
                _encryptionKeyIsSet = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public FileEncryptionService(UserSettings userSettings)
        {
            _userSettings = userSettings;
        }

        public void SetUserEncryptionKey(string encryptionKey)
        {
            if (string.IsNullOrWhiteSpace(encryptionKey))
            {
                EncryptionKeyIsSet = false;
                return;
            }

            byte[] encryptionKeyBytes =
                new UnicodeEncoding().GetBytes(encryptionKey);

            _aesKey = SHA256.Create().ComputeHash(encryptionKeyBytes);
            _aesIV = MD5.Create().ComputeHash(encryptionKeyBytes);

            EncryptionKeyIsSet = true;
        }

        public async Task EncryptDocumentAsync(Document document)
        {
            Aes aes = Aes.Create();
            aes.Key = _aesKey;
            aes.IV = _aesIV;

            // Use RSACryptoServiceProvider to encrypt the AES key.
            byte[] encryptedKey = s_rsaCryptoServiceProvider.Encrypt(aes.Key, false);

            // Create byte arrays to contain the length values of the key and IV.
            byte[] lengthKeyArray = BitConverter.GetBytes(encryptedKey.Length);
            byte[] lengthIVArray = BitConverter.GetBytes(aes.IV.Length);

            // Write the following to the FileStream for the encrypted file (outputFileStream):
            // - length of the key
            // - length of the IV
            // - encrypted key
            // - the IV
            // - the encrypted cipher content

            if (!Directory.Exists(ENCRYPTED_FILE_LOCATION))
            {
                Directory.CreateDirectory(ENCRYPTED_FILE_LOCATION);
            }

            string outputFileName =
                Path.Combine(ENCRYPTED_FILE_LOCATION, document.NameInStorage);

            await using FileStream outputFileStream =
                new(outputFileName, FileMode.Create);

            outputFileStream.Write(lengthKeyArray, 0, 4);
            outputFileStream.Write(lengthIVArray, 0, 4);
            outputFileStream.Write(encryptedKey, 0, encryptedKey.Length);
            outputFileStream.Write(aes.IV, 0, aes.IV.Length);

            ICryptoTransform transform = aes.CreateEncryptor();

            // Now write the cipher text using a CryptoStream for encrypting.
            await using (CryptoStream encryptedOutputStream =
                new(outputFileStream, transform, CryptoStreamMode.Write))
            {
                int blockSizeBytes = aes.BlockSize / 8;
                byte[] data = new byte[blockSizeBytes];

                await using (FileStream inputFileStream =
                    new(document.FileInfo.FullName, FileMode.Open))
                {
                    int count = 0;

                    do
                    {
                        count = inputFileStream.Read(data, 0, blockSizeBytes);
                        encryptedOutputStream.Write(data, 0, count);
                    } while (count > 0);

                    inputFileStream.Close();
                }

                await encryptedOutputStream.FlushFinalBlockAsync();

                encryptedOutputStream.Close();
            }

            outputFileStream.Close();

            await WriteEncryptedFileToStorageAsync(document);
        }

        private async Task WriteEncryptedFileToStorageAsync(Document document)
        {
            //BlobServiceClient blobServiceClient =
            //    new BlobServiceClient(_blobConnectionString);

            //BlobContainerClient containerClient =
            //    blobServiceClient.GetBlobContainerClient(_containerName);

            //BlobClient blobClient =
            //    containerClient.GetBlobClient(document.NameInStorage);

            //await using FileStream uploadFileStream =
            //    File.OpenRead(Path.Combine(ENCRYPTED_FILE_LOCATION, document.NameInStorage));

            //await blobClient.UploadAsync(uploadFileStream, true);

            //uploadFileStream.Close();
        }

        public void DecryptDocument(Document document)
        {
            Aes aes = Aes.Create();
            aes.Key = _aesKey;
            aes.IV = _aesIV;

            using FileStream inputFileStream =
                new(ENCRYPTED_FILE_LOCATION + document.NameInStorage, FileMode.Open);

            // Create byte arrays to get the length of the encrypted key and IV.
            // These values were stored as 4 bytes each at the beginning of the encrypted file.
            byte[] lengthKeyArray = new byte[4];
            byte[] lengthIVArray = new byte[4];

            inputFileStream.Seek(0, SeekOrigin.Begin);
            inputFileStream.Read(lengthKeyArray, 0, 3);
            inputFileStream.Seek(4, SeekOrigin.Begin);
            inputFileStream.Read(lengthIVArray, 0, 3);

            int lengthKey = BitConverter.ToInt32(lengthKeyArray, 0);
            int lengthIV = BitConverter.ToInt32(lengthIVArray, 0);

            // Determine the start position of the cipher text (startC) and its length(lenC).
            int startOfEncryptedContent = lengthKey + lengthIV + 8;

            // Create the byte arrays for the encrypted Aes key and the IV.
            byte[] encryptionKey = new byte[lengthKey];
            byte[] iv = new byte[lengthIV];

            // Extract the key and IV starting from index 8 after the length values.
            inputFileStream.Seek(8, SeekOrigin.Begin);
            inputFileStream.Read(encryptionKey, 0, lengthKey);
            inputFileStream.Seek(8 + lengthKey, SeekOrigin.Begin);
            inputFileStream.Read(iv, 0, lengthIV);

            Directory.CreateDirectory(DECRYPTED_FILE_LOCATION);

            // Use RSACryptoServiceProvider to decrypt the AES key.
            byte[] decryptedKey = s_rsaCryptoServiceProvider.Decrypt(encryptionKey, false);

            // Decrypt the key.
            ICryptoTransform transform = aes.CreateDecryptor(decryptedKey, iv);

            // Decrypt the cipher text from from the encrypted file stream (inputFileStream)
            // into the decrypted file stream (outputFileStream).
            string decryptedFileName =
                Path.Combine(DECRYPTED_FILE_LOCATION, document.OriginalName);

            using (FileStream outputFileStream = new(decryptedFileName, FileMode.Create))
            {
                // blockSizeBytes can be any arbitrary size.
                int blockSizeBytes = aes.BlockSize / 8;
                byte[] data = new byte[blockSizeBytes];

                // Start at the beginning of the cipher text.
                inputFileStream.Seek(startOfEncryptedContent, SeekOrigin.Begin);

                using (CryptoStream decryptedOutputStream =
                    new(outputFileStream, transform, CryptoStreamMode.Write))
                {
                    int count = 0;
                    do
                    {
                        count = inputFileStream.Read(data, 0, blockSizeBytes);
                        decryptedOutputStream.Write(data, 0, count);
                    } while (count > 0);

                    decryptedOutputStream.FlushFinalBlock();
                    decryptedOutputStream.Close();
                }

                outputFileStream.Close();
            }

            inputFileStream.Close();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}