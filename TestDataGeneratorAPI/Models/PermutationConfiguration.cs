namespace TestDataGeneratorAPI.Models
{
    public class PermutationConfiguration
    {
        public ImportColumn? Column { get; set; } = null;
        public int? ColumnSeperator { get; set; } = null;
        public Dictionary<int, int?>? EveryUntil { get; set; } = null;
        public bool OnlyNulls { get; set; } = false;
        public bool NoNulls { get; set; } = false;
        public bool PermutateWholeSchema { get; set; } = false;
    }
}
