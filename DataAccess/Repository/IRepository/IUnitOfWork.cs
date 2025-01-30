using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        Task SaveAsync();
        void DetachEntity<T>(T entity) where T : class;
    }
}
