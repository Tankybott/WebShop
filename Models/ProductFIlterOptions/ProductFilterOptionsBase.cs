namespace Models.ProductFilterOptions
{
    public abstract class ProductFilterOptionsBase
    {
        public string? TypedTextFilter { get; set; }
        public decimal? MinimalPriceFilter { get; set; }
        public decimal? MaximalPriceFilter { get; set; }
        public bool? ShowOnlyDiscountFilter { get; set; }
        public string? SortByValueFilter { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}
