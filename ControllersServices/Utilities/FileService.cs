using ControllersServices.Utilities.Interfaces;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.IO;
using System.Threading.Tasks;

namespace ControllersServices.Utilities
{
    public class FileService : IFileService
    {
        public async Task CreateFileAsync(IFormFile file, string directory, string fileName)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var filePath = Path.Combine(directory, fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
        }

        public async Task DeleteFileAsync(string filePath)
        {
            if (File.Exists(filePath))
            {

                await Task.Run(() => File.Delete(filePath));
            }
            else
            {
                Log.Error("Tried to delete not existing photo");
            }
        }
    }
}