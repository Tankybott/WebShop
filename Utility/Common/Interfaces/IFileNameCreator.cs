using Microsoft.AspNetCore.Http;


namespace Utility.Common.Interfaces
{
    public interface IFileNameCreator
    {
        string CreateFileName(IFormFile file);
        string CreateJpegFileName();
        string CreateFileName(string extension);
    }
}
