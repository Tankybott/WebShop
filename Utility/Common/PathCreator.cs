using Microsoft.AspNetCore.Hosting;
using Utility.Common.Interfaces;

namespace Utility.Common
{
    public class PathCreator : IPathCreator
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string wwwRootPath;

        public PathCreator(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            wwwRootPath = _webHostEnvironment.WebRootPath;
        }

        public string GetRootPath()
        {
            return wwwRootPath;
        }

        public string CombinePaths(string firstPath, string secondPath)
        {
            var combinedPath = Path.Combine(NormalizePathToSystemFormat(firstPath), NormalizePathToSystemFormat(secondPath));
            return NormalizeToUniversalFormat(combinedPath);
        }

        public string CreateUrlPath(string directory, string fileName)
        {
            var combinedPath = CombinePaths(directory, fileName);
            return NormalizeToUniversalFormat(combinedPath, isUrl: true);
        }

        private string NormalizeToUniversalFormat(string path, bool isUrl = false)
        {
            var normalizedPath = path.Replace("\\", "/");

            if (isUrl && !normalizedPath.StartsWith("/"))
            {
                normalizedPath = "/" + normalizedPath;
            }

            return normalizedPath;
        }
        private string NormalizePathToSystemFormat(string path)
        {
            return path.Replace('/', Path.DirectorySeparatorChar)
                       .Replace('\\', Path.DirectorySeparatorChar)
                       .TrimStart(Path.DirectorySeparatorChar)
                       .TrimEnd(Path.DirectorySeparatorChar);
        }
    }
}