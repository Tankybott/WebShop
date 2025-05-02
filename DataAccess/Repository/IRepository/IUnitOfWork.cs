using Microsoft.EntityFrameworkCore;


namespace DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }
        IDiscountRepository Discount { get; }
        IPhotoUrlsSetRepository PhotoUrlSets { get; }
        ICartItemRepository CartItem { get; }
        ICartRepository Cart { get; }
        IApplicationUserRepository ApplicationUser { get;}
        IOrderHeaderRepository OrderHeader { get; }
        IOrderDetailRepository OrderDetail { get; }
        ICarrierRepository Carrier { get; }
        IWebshopConfigRepository WebshopConfig { get; }
        Task SaveAsync();
        DbContext GetDbContext();
    }
}
