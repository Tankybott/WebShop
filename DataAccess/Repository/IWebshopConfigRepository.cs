using Models.DatabaseRelatedModels;

namespace DataAccess.Repository
{
    public interface IWebshopConfigRepository
    {
        Task<WebshopConfig> GetAsync();
        void Update(WebshopConfig config);
    }
}