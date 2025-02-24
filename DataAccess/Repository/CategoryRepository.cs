using DataAccess.Repository.IRepository;
using DataAccess.Repository.Utility;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTOs;
using Models.ProductFilterOptions;
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
