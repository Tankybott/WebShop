using DataAccess.Repository.IRepository;
using Models;
using Models.FormModel;
using Services.CartServices.CustomExeptions;
using Services.CartServices.Interfaces;
using Utility.CalculationClasses;

namespace Services.CartServices
{
    public class CartItemCreator : ICartItemCreator
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartItemCreator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CartItem> CreateCartItemAsync(CartItemFormModel formModel)
        {
            var cartItem = new CartItem()
            {
                ProductId = formModel.ProductId,
                ProductQuantity = formModel.ProductQuantity,
            };

            var product = await _unitOfWork.Product.GetAsync(p => p.Id == formModel.ProductId);

            if (product != null && product.StockQuantity < formModel.ProductQuantity)
                throw new ArgumentException("Requested quantity exceeds available stock.");

            if (product != null)
            {
                AdjustCartProdcutsPrice(formModel.IsAddedWithDiscount, product, cartItem);
            }

            return cartItem;
        }

        private void AdjustCartProdcutsPrice(bool isAddedWithDiscount, Product product, CartItem cartItem)
        {
            if (isAddedWithDiscount)
            {
                if (isDiscountStillActive(product.DiscountId))
                {
                    cartItem.CurrentPrice = DiscountedPriceCalculator.CalculatePriceOfDiscount(product.Price, product.Discount!.Percentage);
                }
                else
                {
                    throw new DiscountOutOfDateException();
                }
            }
            else
            {
                cartItem.CurrentPrice = product.Price;
            }
        }

        private bool isDiscountStillActive(int? discountId)
        {
            return (discountId != 0 && discountId != null);
        }
    }
}
