﻿using DataAccess.Repository.IRepository;
using Models.DatabaseRelatedModels;
using Services.ProductService.Interfaces;

namespace Services.ProductService
{
    public class ProductMainPhotoSynchronizer : IProductMainPhotoSynchronizer
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductMainPhotoSynchronizer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task SynchronizeMainPhotoSetAsync(string newMainPhotoThumbnailUrl)
        {
            var currentMainPhotoSet = await _unitOfWork.PhotoUrlSets.GetAsync(s => s.IsMainPhoto == true);
            if (currentMainPhotoSet?.ThumbnailPhotoUrl == newMainPhotoThumbnailUrl) return;

            var newMainPhotoSet = await _unitOfWork.PhotoUrlSets.GetAsync(s => s.ThumbnailPhotoUrl == newMainPhotoThumbnailUrl);

            if (newMainPhotoSet != null && currentMainPhotoSet != null)
            {
                newMainPhotoSet.IsMainPhoto = true;
                currentMainPhotoSet.IsMainPhoto = false;

                _unitOfWork.PhotoUrlSets.Update(newMainPhotoSet);
                _unitOfWork.PhotoUrlSets.Update(currentMainPhotoSet);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
