namespace B2bOrder.Models.Common
{
    public class PagedResult<T>
    {
        public bool Success { get; set; } = true;
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<T> Rows { get; set; } = new List<T>();
        public string Message { get; set; }
    }
}
