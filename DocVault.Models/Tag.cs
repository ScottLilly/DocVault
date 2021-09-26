using System;
using System.Collections.Generic;

namespace DocVault.Models
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string Value { get; set; }

        public ICollection<Document> Documents { get; set; } =
            new List<Document>();
    }
}