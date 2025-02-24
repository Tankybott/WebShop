using DataAccess.Repository.IRepository;
using Models.DatabaseRelatedModels;


namespace DataAccess.Repository
{
    public class OrderDetailRepository: Repository<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext) { }
    }
}
