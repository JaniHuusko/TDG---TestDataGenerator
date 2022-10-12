namespace GenericTestDataCreator.Models
{
    public class ColumnConfiguration<T> where T : class
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public bool NotNull { get; set; } = false;
        public bool OnlyNull { get; set; } = false;
        public T? MaxValue { get; set; }
        public T? MinValue { get; set; }
        public List<T>? PossibleValues { get; set; }
        public string ImportColumnId { get; set; } = null!;
        public virtual ImportColumn Column { get; set; } = null!;
    }
}
