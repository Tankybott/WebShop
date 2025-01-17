using Microsoft.AspNetCore.Http;


namespace Utility.Common.Interfaces
{
    public interface IFileNameCreator
    {
        public string CreateFileName(IFormFile file);
        public string CreateJpegFileName();
    }
}
