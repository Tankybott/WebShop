using DataAccess.Repository.IRepository;
using Models;
using Models.DatabaseRelatedModels;


namespace DataAccess.Repository
{
    public class CarrierRepository : Repository<Carrier>, ICarrierRepository
    {
        public CarrierRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext) { }
    }
}
