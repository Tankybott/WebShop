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
    public interface IPhotoService
    {
        Task UploadPhotosFromSet(IFormFile photo, string thumbnailPhotoName, string FullSizePhotName, string imageDirectory);
        Task DeletePhotosFromServer(PhotoUrlSet photoSet);
        Task SynchronizeMainPhotosAsync(string newPhotoThumbnailUrl);
    }
}
