using Microsoft.AspNetCore.Http;
using Utility.Common.Interfaces;

namespace Utility.Common
{
    public class BaseUrlRetriever : IBaseUrlRetriever
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BaseUrlRetriever(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetBaseUrl()
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null)
                return string.Empty;

            return $"{request.Scheme}://{request.Host}";
        }
    }
}