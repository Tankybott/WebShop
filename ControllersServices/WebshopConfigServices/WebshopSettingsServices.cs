using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.ViewModels;
using Services.WebshopConfigServices.Interfaces;
using Utility.Common;

namespace Services.WebshopConfigServices
{
    public class WebshopSettingsServices : IWebshopSettingsServices
    {
        private readonly IUnitOfWork _unitOfWork;

        public WebshopSettingsServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<WebshopSettingsVm> GetVmAsync()
        {
            var vm = new WebshopSettingsVm();
            var currentConfig = await _unitOfWork.WebshopConfig.GetAsync();
            vm.Currency = currentConfig.Currency;
            vm.SiteName = currentConfig.SiteName;
            vm.Currencies = CurrenciesGetter.GetAllCurrencies().Select(c => new SelectListItem
            {
                Value = c.Key,
                Text = $"{c.Value} ({c.Key})"
            });

            return vm;
        }

        public async Task UpdateAsync(WebshopSettingsVm vm) 
        {
            var webshopConfig = await _unitOfWork.WebshopConfig.GetAsync();
            webshopConfig.SiteName = vm.SiteName;
            webshopConfig.Currency = vm.Currency;
            _unitOfWork.WebshopConfig.Update(webshopConfig);
            await _unitOfWork.SaveAsync();
        }
    }
}
