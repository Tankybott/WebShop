using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllersServices.Utilities.Interfaces
{
    public interface IFileService
    {
        Task CreateFileAsync(IFormFile file, string directory, string fileName);
        Task DeleteFileAsync(string filePath);
    }
}
