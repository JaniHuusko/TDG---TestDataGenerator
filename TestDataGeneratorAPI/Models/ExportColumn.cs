using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDataGeneratorAPI.Models
{
    public class ExportColumn
    {
        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!;
        public bool IsNullable { get; set; } = false;
        public ForeignKeyInfo? ForeignKeyInfo { get; set; }
        public List<string?> Values { get; set; } = new List<string?>();
    }
}
