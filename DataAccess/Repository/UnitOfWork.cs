using DataAccess.Repository.IRepository;
using Serilog;
namespace DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public IDiscountRepository Discount { get; private set; }

        public UnitOfWork(ApplicationDbContext db, ICategoryRepository category, IProductRepository product, IDiscountRepository discount)
        {
            _db = db;
            Category = category;
            Product = product;
            Discount = discount;
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
    }
}
