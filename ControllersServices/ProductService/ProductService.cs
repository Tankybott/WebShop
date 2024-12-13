using AutoMapper;
using ControllersServices.ProductService.Interfaces;
using ControllersServices.Utilities.Interfaces;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Models;
using Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllersServices.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductVMCreator _productVMCreator;
        private readonly IProductUpserter _productUpserter;
        private readonly IProductPhotoService _productPhotoService;

        public ProductService(IUnitOfWork unitOfWork,
            IProductVMCreator productVMCreator,
            IProductUpserter productUpserter,
            IProductPhotoService productPhotoService)
        {
            _unitOfWork = unitOfWork;
            _productVMCreator = productVMCreator;
            _productUpserter = productUpserter;
            _productPhotoService = productPhotoService;
        }

        public async Task<ProductVM> GetProductVMForIndexAsync()
        {
            var categories = await _unitOfWork.Category.GetAllAsync(tracked: true, sortBy: c => c.Name);
            var products = await _unitOfWork.Product.GetAllAsync(tracked: true);

            ProductVM producVM = _productVMCreator.CreateProductVM(categories, products);

            return producVM;
        }

        public async Task<ProductVM> GetProductVMAsync(int? id = null)
        {
            var categories = await _unitOfWork.Category.GetAllAsync(tracked: true, sortBy: c => c.Name);

            ProductVM producVM = _productVMCreator.CreateProductVM(categories);

            if (id != null) 
            {
               producVM.Product = await _unitOfWork.Product.GetAsync(p => p.Id == id);
            }

            return producVM;
        }

        public async Task UpsertAsync(ProductFormModel model)
        {
            await _productUpserter.HandleUpsertAsync(model);
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsForTableAsync(string? categoryFilter) 
        {
            int filterValueInt;
            if (categoryFilter != null && int.TryParse(categoryFilter, out filterValueInt))
            {
                return await _unitOfWork.Product.GetProductsDtoOfCategoryAsync(filterValueInt);
            }

            var products = await _unitOfWork.Product.GetProductsDtoAsync();
            return products;
        }

        public async Task Delete(int? id)
        {
            Product productToDelete = await _unitOfWork.Product.GetAsync(p => p.Id == id, tracked: true);
            if (productToDelete != null)
            {
                await _productPhotoService.DeletePhotoAsync(productToDelete!.MainPhotoUrl);
                if (productToDelete.OtherPhotosUrls != null)
                {
                    foreach (var url in productToDelete.OtherPhotosUrls)
                    {
                        await _productPhotoService.DeletePhotoAsync(url);
                    }
                }
                _unitOfWork.Product.Remove(productToDelete);
                await _unitOfWork.SaveAsync();
            }
        }
        //public async Task UpdateDiscount(int? id, int discount) 
        //{
        //    Product productToUpdate = await _unitOfWork.Product.GetAsync(p => p.Id == id);
        //    productToUpdate.Discount = discount;
        //    _unitOfWork.Product.Update(productToUpdate);
        //    await _unitOfWork.SaveAsync();
        //}
    }
}

