namespace TestDataGeneratorAPI.Models
{
    public class DataGenerationRequest
    {
        public string ConnectionString { get; set; } = null!;
        public int? DataRowCount { get; set; }
        public int PermutationCount { get; set; }
        public PermutationConfiguration PermutationConfiguration { get; set; } = new();
        public List<ImportTable> Tables { get; set; } = new();
        public List<ColumnConfiguration> ColumnConfigurations { get; set; } = new();

    }
}
