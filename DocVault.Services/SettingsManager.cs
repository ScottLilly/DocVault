using System;
using System.IO;
using Newtonsoft.Json;

namespace DocVault.Services
{
    public class SettingsManager<T> where T : class
    {
        private readonly string _filePath;

        public SettingsManager(string fileName)
        {
            _filePath = GetLocalFilePath(fileName);
        }

        private string GetLocalFilePath(string fileName)
        {
            string appData =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DocVault");

            return Path.Combine(appData, fileName);
        }

        public T LoadSettings() =>
            File.Exists(_filePath) ?
                JsonConvert.DeserializeObject<T>(File.ReadAllText(_filePath)) :
                null;

        public void SaveSettings(T settings)
        {
            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);

            string directoryName = new FileInfo(_filePath).DirectoryName;

            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            File.WriteAllText(_filePath, json);
        }
    }
}