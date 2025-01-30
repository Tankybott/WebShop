using DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DatabaseRelatedModels;
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

        public UnitOfWork(ApplicationDbContext db, ICategoryRepository category, IProductRepository product, IDiscountRepository discount, IPhotoUrlsSetRepository photoUrlsSet, ICartRepository cart, ICartItemRepository cartItem)
        {
            _db = db;
            Category = category;
            Product = product;
            Discount = discount;
            PhotoUrlSets = photoUrlsSet;
            Cart = cart;
            CartItem = cartItem;
        }

        public async Task SaveAsync()
        {
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to save database");
                throw new Exception("An error occurred while saving changes.", ex);
            }
        }

        public void DetachEntity<T>(T entity) where T : class 
        {
            var entry = _db.Entry(entity);
            if (entry != null)
            {
                entry.State = EntityState.Detached;
            }
        }
    }
}
