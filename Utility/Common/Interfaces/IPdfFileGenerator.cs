namespace Utility.Common.Interfaces
{
    public interface IPdfFileGenerator
    {
        Task<string> GeneratePdfFromHtmlAsync(string html);
    }
}