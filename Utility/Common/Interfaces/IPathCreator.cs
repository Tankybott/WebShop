
namespace Utility.Common.Interfaces
{
    public interface IPathCreator
    {
        public string GetRootPath();
        string CombinePaths(string firstPath, string secondPath);
        string CreateUrlPath(string directory, string fileName);
    }
}
