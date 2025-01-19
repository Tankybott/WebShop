using Serilog;
using Utility.Common.Interfaces;


namespace Utility.Common
{
    public class FileService : IFileService
    {
        public async Task CreateFileAsync(Stream inputStream, string outputPath)
        {
            var directory = Path.GetDirectoryName(outputPath);

            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await using var outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
            await inputStream.CopyToAsync(outputStream);
        }

        public async Task DeleteFileAsync(string filePath)
        {
            if (File.Exists(filePath))
            {

                await Task.Run(() => File.Delete(filePath));
            }
            else
            {
                Log.Error("Tried to delete not existing file");
            }
        }
    }
}