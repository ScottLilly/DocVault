﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace DocVault.Models
{
    public class Document
    {
        private static readonly RNGCryptoServiceProvider s_generator = 
            new RNGCryptoServiceProvider();

        [NotMapped]
        public FileInfo FileInfo { get; set; }

        public Guid Id { get; set; }
        public DateTime StoredDateTime { get; set; } = DateTime.UtcNow;
        public string NameInStorage { get; set; }
        public string OriginalName { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public long FileSize { get; set; }
        public byte[] Checksum { get; set; }
        public byte[] Salt { get; set; }

        public ICollection<Tag> Tags { get; set; } =
            new List<Tag>();

        public string TagList =>
            string.Join(", ", Tags.OrderBy(t => t.Value).Select(t => t.Value));

        public Document()
        {
            byte[] randomByteArray = new byte[8];

            s_generator.GetBytes(randomByteArray);

            Salt = randomByteArray;
        }
    }
}