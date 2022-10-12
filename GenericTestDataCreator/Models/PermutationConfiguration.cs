using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericTestDataCreator.Models
{
    public class PermutationConfiguration
    {
        public ImportColumn? Column { get; set; } = null;
        public int? ColumnSeperator { get; set; } = null;
        public Dictionary<int, int?>? EveryUntil { get; set; } = null;
        public bool OnlyNulls { get; set; } = false;
        public bool NoNulls { get; set; } = false;
    }
}
