using DataAccess.Repository.IRepository;
using Models;
using System.Linq.Expressions;


namespace DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext db): base(db) { }

        public bool Any(Expression<Func<Category, bool>> predicate)
        {
            return dbSet.Any(predicate);
        }
    }
}
