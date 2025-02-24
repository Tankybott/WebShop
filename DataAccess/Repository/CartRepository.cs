using DataAccess.Repository.IRepository;
using Models;
using System.Linq.Expressions;


namespace DataAccess.Repository
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        public CartRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext) {}
    }
}
