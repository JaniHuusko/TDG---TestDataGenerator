using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericTestDataCreator.Models
{
    // Scalable class for adding configurations related to enums, datetimes, pictures and more.
    public class DataGenerationRequest
    {
        public string ConnectionString { get; set; } = null!;
        public int? DataRowCount { get; set; }
        public List<ImportTable> Tables { get; set; } = new List<ImportTable>();
        public ImportTable? ImportTable { get; set; }
    }
}
