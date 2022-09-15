using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericTestDataCreator.Models
{
    public class ImportTable
    {
        public string Name { get; set; } = null!;
        public int CurrentDataRowCount { get; set; }
        public int ForeignKeyCount { get; set; } = 0;
        public List<ImportColumn> Columns { get; set; } = new List<ImportColumn>();
    }
}
