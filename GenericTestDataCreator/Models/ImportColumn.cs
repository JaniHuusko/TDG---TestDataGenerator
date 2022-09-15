using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericTestDataCreator.Models
{
    public class ImportColumn
    {
        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!;
        public int MaxLength { get; set; }
        public bool IsNullable { get; set; }
        public bool IsComputed { get; set; }
        public bool IsIdentity { get; set; }
        public ForeignKeyInfo? ForeignKeyInfo { get; set; }
    }
}