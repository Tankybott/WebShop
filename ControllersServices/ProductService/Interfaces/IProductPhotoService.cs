using Microsoft.AspNetCore.Http;
using Models;
using Models.DatabaseRelatedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllersServices.ProductManagement.Interfaces
{
    public interface IProductPhotoService
    {
        Task AddPhotoSetAsync(IFormFile photo, string thumbnailPhotoName, string FullSizePhotName, string imageDirectory);
        Task DeletePhotoSetAsync(PhotoUrlSet photoSet);
        Task SynchronizeMainPhotosAsync(string newPhotoThumbnailUrl);
    }
}
