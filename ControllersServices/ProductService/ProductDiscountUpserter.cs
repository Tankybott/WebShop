using Models;
using Models.DiscountCreateModel;
using Serilog;
using Services.PhotoService.Interfaces.DiscountService.Interfaces;
using Services.ProductManagement.Interfaces;

namespace Services.ProductService
{
    public class ProductDiscountUpserter : IProductDiscountUpserter
    {
        private readonly IDiscountService _discountService;

        public ProductDiscountUpserter(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        public async Task HandleDiscountUpsertAsync(
           Product product,
           DiscountCreateModel discountCreateModel)
        {
            if (IsDiscountValid(discountCreateModel))
            {
                if (discountCreateModel.IsDiscountChanged != null && discountCreateModel.IsDiscountChanged == false)
                {
                    return;
                }
                else if (product.DiscountId == 0 || product.DiscountId == null)
                {
                    var discount = await _discountService.CreateDiscountAsync(discountCreateModel.StartTime.Value, discountCreateModel.EndTime.Value, discountCreateModel.Percentage.Value);
                    product.DiscountId = discount.Id;
                }
                else
                {
                    var discount = await _discountService.UpdateDiscountAsync(product.DiscountId.Value, discountCreateModel.StartTime.Value, discountCreateModel.EndTime.Value, discountCreateModel.Percentage.Value);
                    product.DiscountId = discount.Id;
                }
            }
            else
            {
                // Deletes 
                if (IsDiscountCleaned(discountCreateModel))
                {
                    if (discountCreateModel.DiscountId != 0 && discountCreateModel.DiscountId != null)
                    {
                        await _discountService.DeleteByIdAsync(discountCreateModel.DiscountId.Value);
                    }
                    product.DiscountId = null;
                }
                else
                {
                    Log.Error("Failed to add discount because of invalid data");
                    throw new ArgumentException("Failed to add discount because of invalid data ");
                }
            }
        }


        private bool IsDiscountCleaned(DiscountCreateModel model)
        {
            return (model.StartTime == null && model.EndTime == null && model.Percentage == null);
        }

        private bool IsDiscountValid(DiscountCreateModel model) 
        {
            return (model.StartTime != null && model.EndTime != null && model.Percentage != null);
        }
    }
}
