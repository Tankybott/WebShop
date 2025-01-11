
namespace Models.DTOs
{
    public class ProductCardDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MainPhotoUrl { get; set; }
        public decimal Price { get; set; }
        public string ShortDescription { get; set; }
        public int? DiscountPercentage { get; set; }
        public string CategoryName { get; set; }
    }
}
