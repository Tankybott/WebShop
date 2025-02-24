using DataAccess.Repository.IRepository;
using Serilog;
using Services.ProductService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.CalculationClasses;

namespace Services.ProductService
{
    public class ProductPriceRetriver : IProductPriceRetriver
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductPriceRetriver(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<decimal> GetProductPriceAsync(int productId)
        {
            var product = await _unitOfWork.Product.GetAsync(p => p.Id == productId);
            if (product != null && product.DiscountId != null && product.DiscountId != 0)
            {
                return DiscountedPriceCalculator.CalculatePriceOfDiscount(product.Price, product.Discount.Percentage);
            }
            else if (product != null) {
                return product.Price;
            }
            else
            {
                Log.Error("cannot get product price ,product with id not fount");
                throw new ArgumentException("Product with id not fount");
            }
        }
    }
}
