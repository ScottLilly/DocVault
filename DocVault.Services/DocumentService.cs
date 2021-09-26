using System;
using System.IO;
using DocVault.Models;

namespace DocVault.Services
{
    public static class DocumentService
    {
        public static Document CreateDocumentFromFile(string fileName)
        {
            FileInfo fileInfo = new(fileName);

            return new Document
            {
                FileInfo = fileInfo,
                OriginalName = fileInfo.Name,
                CreatedDateTime = fileInfo.CreationTime,
                FileSize = fileInfo.Length,
                Checksum = FileService.ComputeMD5ChecksumForFile(fileName),
                NameInStorage = Guid.NewGuid().ToString()
            };
        }
    }
}