using DataAccess.Repository.IRepository;
using DataAccess.Repository.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models;
using Models.DatabaseRelatedModels;
using Models.DTOs;
using Models.ProductFilterOptions;
using Serilog;
using System.Linq.Expressions;
using Utility.Constants;




namespace DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ICategoryRepository _categoryRepository;
        public ProductRepository(ApplicationDbContext applicationDbContext, ICategoryRepository categoryRepository) : base(applicationDbContext)
        {
            _categoryRepository = categoryRepository;
        }

        public override void Add(Product product)
        {
            try
            {
                var hasSubCategories = _categoryRepository.Any(c => c.ParentCategoryId == product.CategoryId);
                if (hasSubCategories) throw new ArgumentException("Products can be added only on highest level of category tree"); 
                dbSet.Add(product);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to add entity to database");
                throw new Exception("An error occurred while adding the entity.", ex);
            }
        }

        public override void Update(Product product)
        {
            var hasSubCategories = _categoryRepository.Any(c => c.ParentCategoryId == product.CategoryId);
            if (hasSubCategories) throw new ArgumentException("Products can be added only on highest level of category tree");
            base.Update(product);
        }

        public override void UpdateRange(IEnumerable<Product> products)
        {
            foreach (var product in products) 
            {
                var hasSubCategories = _categoryRepository.Any(c => c.ParentCategoryId == product.CategoryId);
                if (hasSubCategories) throw new ArgumentException("Products can be added only on highest level of category tree");
            }
            base.UpdateRange(products);
        }

        public override async Task<IEnumerable<Product>> GetAllAsync(Expression<Func<Product, bool>>? filter = null, string? includeProperties = null, bool tracked = false, Expression<Func<Product, object>>? sortBy = null)
        {
            includeProperties = "Discount,PhotosUrlSets";
            return await base.GetAllAsync(filter, includeProperties, tracked, sortBy);
        }

        public override async Task<Product?> GetAsync(Expression<Func<Product, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            includeProperties = "Discount,PhotosUrlSets";
            return await base.GetAsync(filter, includeProperties, tracked);
        }

        public async Task<IEnumerable<ProductTableDTO>> GetProductsTableDtoAsync(IEnumerable<int>? categoryIds, string? productFilterOption) 
        {
            var query = _db.Products.Include(p => p.Category).Include(p => p.Discount).AsQueryable(); ;

            if (categoryIds != null && categoryIds.Any())
            {
                query = query.Where(p => categoryIds.Contains(p.CategoryId));
            }

            switch (productFilterOption)
            {
                case ProductListFilteringOptions.All:
                    break;
                case ProductListFilteringOptions.OnlyDiscounted:
                    query = query.Where(p => p.DiscountId != null && p.DiscountId != 0);
                    break;
                case ProductListFilteringOptions.OnlyWithActiveDiscount:
                    query = query.Where(p => p.Discount != null && p.Discount.isActive);
                    break;
            }

            var result = query.Select(p => new ProductTableDTO
            {
                Id = p.Id,
                Name = p.Name,
                CategoryName = p.Category.Name,
                DiscountId = p.DiscountId,
                IsDiscountActive = (p.Discount != null && p.Discount.isActive) ? true : null,
            });
            
            return await result.ToListAsync();
        }

        public async Task<PaginatedResult<ProductCardDTO>> GetProductCardDTOsAsync(ProductFilterOptionsQuery productFilterOptions)
        {
            var query = _db.Products
            .Include(p => p.Category)
            .Include(p => p.Discount)
            .Include(p => p.PhotosUrlSets)
            .AsQueryable();

            query = ApplyCategoryFilter(query, productFilterOptions.CategoriesFilteredIds);
            query = ApplyTextFilter(query, productFilterOptions.TypedTextFilter);
            query = ApplyPriceFilters(query, productFilterOptions.MinimalPriceFilter, productFilterOptions.MaximalPriceFilter);
            query = ApplyDiscountFilter(query, productFilterOptions.ShowOnlyDiscountFilter);
            query = ApplySorting(query, productFilterOptions.SortByValueFilter);

            int totalItemCount = await query.CountAsync();

            query = ApplyPagination(query, productFilterOptions.PageSize, productFilterOptions.PageNumber);

            var productCardDTOCollection = await query.Select(p => new ProductCardDTO
            {
                Id = p.Id,
                Name = p.Name,
                CategoryName = p.Category.Name,
                Price = p.Price,
                MainPhotoUrl = p.PhotosUrlSets.FirstOrDefault(p => p.IsMainPhoto == true)!.ThumbnailPhotoUrl,
                DiscountPercentage = (p.Discount != null && p.Discount.isActive) ? p.Discount.Percentage : null,
                StockQuantity = p.StockQuantity
            }).ToListAsync();

            var result = new PaginatedResult<ProductCardDTO>
            {
                Items = productCardDTOCollection,
                TotalItemCount = totalItemCount,
            };

            return result;
        }

        private IQueryable<Product> ApplyCategoryFilter(IQueryable<Product> query, IEnumerable<int>? categoriesFilteredIds)
        {
            if (categoriesFilteredIds != null && categoriesFilteredIds.Any())
            {
                query = query.Where(p => categoriesFilteredIds.Contains(p.CategoryId));
            }
            return query;
        }

        private IQueryable<Product> ApplyTextFilter(IQueryable<Product> query, string? typedTextFilter)
        {
            if (!string.IsNullOrEmpty(typedTextFilter))
            {
                query = query.Where(p => p.Name.Contains(typedTextFilter));
            }
            return query;
        }
        private IQueryable<Product> ApplyPriceFilters(IQueryable<Product> query, decimal? minimalPriceFilter, decimal? maximalPriceFilter)
        {
            if (minimalPriceFilter != null)
            {
                query = query.Where(p => p.Price > minimalPriceFilter);
            }
            if (maximalPriceFilter != null)
            {
                query = query.Where(p => p.Price < maximalPriceFilter);
            }
            return query;
        }

        private IQueryable<Product> ApplyDiscountFilter(IQueryable<Product> query, bool? showOnlyDiscountFilter)
        {
            if (showOnlyDiscountFilter != null)
            {
                if (showOnlyDiscountFilter == true)
                {
                    query = query.Where(p => p.Discount != null && p.Discount.isActive);
                }
            }
            return query;
        }

        private IQueryable<Product> ApplySorting(IQueryable<Product> query, string? sortByValueFilter)
        {
            if (!String.IsNullOrEmpty(sortByValueFilter))
            {
                return sortByValueFilter switch
                {
                    ProductBrowserFilteringOptions.AlphabeticOption => query.OrderBy(p => p.Name),
                    ProductBrowserFilteringOptions.PriceAscendingOption => query.OrderBy(p => p.Price),
                    ProductBrowserFilteringOptions.PriceDescendingOption => query.OrderByDescending(p => p.Price),
                    _ => query
                };
            }
            else 
            {
                return query;
            }
        }

        private IQueryable<Product> ApplyPagination(IQueryable<Product> query, int? pageSize, int? pageNumber)  
        {
            if (pageSize != null && pageSize != 0 && pageNumber != null && pageNumber != 0)
            {
                int skip = (pageNumber.Value - 1) * pageSize.Value;
                return query.Skip(skip)
                    .Take(pageSize.Value);
            }
            else 
            {
                return query;
            }
        }
    }
}
