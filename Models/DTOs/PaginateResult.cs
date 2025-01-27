namespace DataAccess.Repository.Utility
{
    public class PaginatedResult<T> where T : class
    {
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
        public int TotalItemCount { get; set; }
    }
}