using DataAccess.Repository.IRepository;
using Models.DatabaseRelatedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class PhotoUrlSetReposiotory: Repository<PhotoUrlSet>, IPhotoUrlsSetRepository
    {
        public PhotoUrlSetReposiotory(ApplicationDbContext applicationDbContext) : base(applicationDbContext) { }
    }
}
