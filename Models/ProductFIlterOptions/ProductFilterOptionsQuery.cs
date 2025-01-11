namespace Models.ProductFilterOptions
{
    public class ProductFilterOptionsQuery : ProductFilterOptionsBase
    {
        public IEnumerable<int>? CategoriesFilteredIds { get; set; }
    }
}
