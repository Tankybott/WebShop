using Microsoft.AspNetCore.Http;


namespace Utility.Common.Interfaces
{
    public interface IFileNameCreator
    {
        string CreateFileName(IFormFile file);
        string CreateProductPhotoName();
        string CreateFileName(string extension);
    }
}
