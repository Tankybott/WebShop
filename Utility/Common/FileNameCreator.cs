using Microsoft.AspNetCore.Http;
using Utility.Common.Interfaces;


namespace Utility.Common
{
    public class FileNameCreator : IFileNameCreator
    {
        public string CreateFileName(IFormFile file)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            return fileName;
        }

        public string CreateJpegFileName()
        {
            string fileName = Guid.NewGuid().ToString() + ".jpeg";
            return fileName;
        }
    }
}