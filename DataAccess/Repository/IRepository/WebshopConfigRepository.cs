using Microsoft.EntityFrameworkCore;
using Models.DatabaseRelatedModels;
using Serilog;


namespace DataAccess.Repository.IRepository
{
    public class WebshopConfigRepository : IWebshopConfigRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly DbSet<WebshopConfig> _dbSet;

        public WebshopConfigRepository(ApplicationDbContext db)
        {
            _db = db;
            _dbSet = _db.Set<WebshopConfig>();
        }

        public async Task<WebshopConfig> GetAsync()
        {
            try
            {
                return await _dbSet.FirstOrDefaultAsync(wc => wc.Id == 1) ?? new WebshopConfig { Id = 1 };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to retrieve WebshopConfig from the database");
                throw new Exception("An error occurred while retrieving the WebshopConfig entity.", ex);
            }
        }

        public void Update(WebshopConfig config)
        {
            try
            {
                _dbSet.Update(config);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to update WebshopConfig in the database");
                throw new Exception("An error occurred while updating the WebshopConfig entity.", ex);
            }
        }
    }
}
