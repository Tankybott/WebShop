using DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Serilog;
namespace DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public IDiscountRepository Discount { get; private set; }
        public IPhotoUrlsSetRepository PhotoUrlSets { get; private set; }
        public ICartRepository Cart { get; private set; }
        public ICartItemRepository CartItem { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public ICarrierRepository Carrier { get; private set; }
        public IWebshopConfigRepository WebshopConfig { get; private set; }

        public UnitOfWork(ApplicationDbContext db, ICategoryRepository category,
            IProductRepository product,
            IDiscountRepository discount,
            IPhotoUrlsSetRepository photoUrlsSet,
            ICartRepository cart,
            ICartItemRepository cartItem,
            IApplicationUserRepository applicationUser,
            IOrderDetailRepository orderDetail,
            IOrderHeaderRepository orderHeader,
            ICarrierRepository carrier,
            IWebshopConfigRepository webshopConfig)
        {
            _db = db;
            Category = category;
            Product = product;
            Discount = discount;
            PhotoUrlSets = photoUrlsSet;
            Cart = cart;
            CartItem = cartItem;
            ApplicationUser = applicationUser;
            OrderDetail = orderDetail;
            OrderHeader = orderHeader;
            Carrier = carrier;
            WebshopConfig = webshopConfig;
        }

        public async Task SaveAsync()
        {
            try
            {
                await _db.SaveChangesAsync();
                _db.ChangeTracker.Clear(); 
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to save changes to database");
                throw new Exception("An error occurred while saving changes to the database.", ex);
            }
        }

        public DbContext GetDbContext()
        {
            return _db;
        }
    }
}
