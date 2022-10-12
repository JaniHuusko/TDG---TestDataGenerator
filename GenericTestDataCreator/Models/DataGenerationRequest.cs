using GenericTestDataCreator.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericTestDataCreator.Models
{
    public class DataGenerationRequest
    {
        public string ConnectionString { get; set; } = null!;
        public int? DataRowCount { get; set; }
        public int PermutationCount { get; set; }
        public List<PermutationConfiguration> PermutationConfiguration { get; set; } = new List<PermutationConfiguration>();
        public List<ImportTable> AllTables { get; set; } = new List<ImportTable>();
        public List<ImportTable> SelectedTables { get; set; } = new List<ImportTable>();
    }
}
