using DataAccess.Repository.IRepository;
using Services.WebshopConfigServices.Interfaces;

namespace Services.WebshopConfigServices
{
    public class FreeShippingThresholdManager : IFreeShippingThresholdManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public FreeShippingThresholdManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task UpdateFreeShippingThresholdAsync(decimal? price)
        {
            var webshopConfig = await _unitOfWork.WebshopConfig.GetAsync();
            if (webshopConfig.FreeShippingFrom != price)
            {
                webshopConfig.FreeShippingFrom = price;
                _unitOfWork.WebshopConfig.Update(webshopConfig);
                await _unitOfWork.SaveAsync();
            }
        }

        public async Task<decimal?> GetFreeShippingThresholdAsync()
        {
            var freeShippingThreshold = await _unitOfWork.WebshopConfig.GetAsync();
            return freeShippingThreshold.FreeShippingFrom;
        }
    }
}
