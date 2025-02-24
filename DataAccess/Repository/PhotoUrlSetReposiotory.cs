using DataAccess.Repository.IRepository;
using Models.DatabaseRelatedModels;


namespace DataAccess.Repository
{
    public class PhotoUrlSetReposiotory: Repository<PhotoUrlSet>, IPhotoUrlsSetRepository
    {
        public PhotoUrlSetReposiotory(ApplicationDbContext applicationDbContext) : base(applicationDbContext) { }
    }
}
