﻿using ControllersServices.ProductBrowserService.Interfaces;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Models.ViewModels;

namespace ControllersServices.ProductBrowserService
{
    public class ProductBrowserVmCreator : IProductBrowserVmCreator
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductBrowserVmCreator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProductBrowserVM> CreateProductBrowserVM()
        {
            var vm = new ProductBrowserVM();
            vm.Categories = await _unitOfWork.Category.GetAllAsync(tracked: true, sortBy: c => c.Name);
            var webshopCofig = await _unitOfWork.WebshopConfig.GetAsync();
            vm.Currency = webshopCofig.Currency;
            return vm;
        }
    }
}
