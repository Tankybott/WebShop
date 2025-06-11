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

        public string CreateFileName(string extension)
        {
            return Guid.NewGuid().ToString() + "." + extension;
        }

        public string CreateJpegFileName()
        {
            return CreateFileName("jpeg");
        }


    }
}