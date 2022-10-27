namespace TestDataGeneratorAPI.Models
{
    public class ColumnConfiguration
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public bool NotNull { get; set; } = false;
        public bool OnlyNull { get; set; } = false;
        public string? MaxValue { get; set; }
        public string? MinValue { get; set; }
        public List<string>? OnlyPossibleValues { get; set; }
        public string ImportColumnId { get; set; } = null!;
        public virtual ImportColumn Column { get; set; } = null!;
    }
}
