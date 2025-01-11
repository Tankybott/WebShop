using DataAccess.Repository.IRepository;
using Models.DatabaseRelatedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class DiscountRepository : Repository<Discount>, IDiscountRepository
    {
        public DiscountRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext) { }
    }
}
