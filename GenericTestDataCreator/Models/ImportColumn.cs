using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericTestDataCreator.Models
{
    public class ImportColumn
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!;
        public int MaxLength { get; set; }
        public byte? NumericPrecision { get; set; }
        public byte? NumericScale { get; set; }
        public bool IsNullable { get; set; }
        public bool IsGenerated { get; set; }
        public ForeignKeyInfo? ForeignKeyInfo { get; set; }
    }
}