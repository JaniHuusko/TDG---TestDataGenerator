using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericTestDataCreator.Models
{
    public class ExportColumn
    {
        public string Name { get; set; } = null!;
        public ForeignKeyInfo? ForeignKeyInfo { get; set; }
        public bool IsNullable { get; set; } = false;
        public List<string?> Values { get; set; } = new List<string?>();
    }
}
