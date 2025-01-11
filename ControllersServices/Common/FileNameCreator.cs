using ControllersServices.Utilities.Interfaces;
using Microsoft.AspNetCore.Http;


namespace ControllersServices.Utilities
{
    public class FileNameCreator : IFileNameCreator
    {
        public string CreateFileName(IFormFile file)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            return fileName;
        }
    }
}