using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Common.Interfaces
{
    public interface IFileService
    {
        Task CreateFileAsync(Stream inputStream, string outputPath);
        Task DeleteFileAsync(string filePath);
    }
}
