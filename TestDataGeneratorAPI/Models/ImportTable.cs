namespace TestDataGeneratorAPI.Models
{
    public class ImportTable
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = null!;
        public int CurrentDataRowCount { get; set; }
        public int ForeignKeyCount { get; set; } = 0;
        public List<ImportColumn> Columns { get; set; } = new List<ImportColumn>();
    }
}
