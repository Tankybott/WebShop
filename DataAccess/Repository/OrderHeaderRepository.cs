using DataAccess.Repository.IRepository;
using Models.DatabaseRelatedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class OrderHeaderRepository: Repository<OrderHeader>, IOrderHeaderRepository
    {
        public OrderHeaderRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext) { }
    }
}
