namespace B2bOrder.Models
{
    public class ImportResult
    {
        public int InsertCount { get; set; }

        public int UpdateCount { get; set; }

        public int SkipCount { get; set; }

        public List<string> Errors { get; set; } = new();
    }
}