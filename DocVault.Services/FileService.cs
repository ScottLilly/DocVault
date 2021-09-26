using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;

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

        public static void SaveFormattedJson(string filename, string jsonText)
        {
            File.WriteAllText(filename, JsonPrettify(jsonText));
        }

        private static string JsonPrettify(string json)
        {
            using StringReader stringReader = new StringReader(json);
            using StringWriter stringWriter = new StringWriter();

            JsonTextReader jsonReader =
                new JsonTextReader(stringReader);

            JsonTextWriter jsonWriter =
                new JsonTextWriter(stringWriter)
                {
                    Formatting = Formatting.Indented
                };

            jsonWriter.WriteToken(jsonReader);

            return stringWriter.ToString();
        }
    }
}