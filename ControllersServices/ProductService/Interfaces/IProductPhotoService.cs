using ControllersServices.Utilities.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllersServices.ProductManagement.Interfaces
{
    public interface IProductPhotoService
    {
        Task AddPhotoAsync(IFormFile photo, string fileName, string imageDirectory);
        Task DeletePhotoAsync(string photoUrl);
    }
}
