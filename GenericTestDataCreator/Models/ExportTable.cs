using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericTestDataCreator.Models
{
    public class ExportTable
    {
        public string Name { get; set; } = null!;
        public int ForeignKeyCount { get; set; } = 0;
        public List<ExportColumn> Columns { get; set; } = new List<ExportColumn>();
    }
}
