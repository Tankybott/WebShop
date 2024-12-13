using DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    internal class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext) { }

        public override async Task<IEnumerable<Product>> GetAllAsync(Expression<Func<Product, bool>>? filter = null, string? includeProperties = null, bool tracked = false, Expression<Func<Product, object>>? sortBy = null)
        {
            includeProperties = "Discount";
            return await base.GetAllAsync(filter, includeProperties, tracked, sortBy);
        }

        public override async Task<Product?> GetAsync(Expression<Func<Product, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            includeProperties = "Discount";
            return await base.GetAsync(filter, includeProperties, tracked);
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsDtoAsync() 
        {
            return await _db.Products
                .Include(p => p.Category)
                .Select(p => new ProductDTO 
                {
                    Id = p.Id,
                    Name = p.Name,
                    CategoryName = p.Category.Name
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsDtoOfCategoryAsync(int categoryId)
        {
            return await _db.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    CategoryName = p.Category.Name
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductOfCategoryAsync(int categoryId) 
        {
            return await _db.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }
    }
}
