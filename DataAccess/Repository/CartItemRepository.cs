using DataAccess.Repository.IRepository;
using Models;


namespace DataAccess.Repository
{
    public class CartItemRepository : Repository<CartItem>, ICartItemRepository
    {
         public CartItemRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext) { }
    }
}
