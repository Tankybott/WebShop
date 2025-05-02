using DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Models.DatabaseRelatedModels;
using Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Constants;

namespace DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        public OrderHeaderRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext) { }

        public async Task<IEnumerable<OrderDTO>> GetOrderTableDtoAsync()
        {
            var query = _db.OrderHeaders.AsQueryable();

            var result = query.Select(o => new OrderDTO
            {
                Id = o.Id,
                FullName = o.Name,
                CreationDate = o.CreationDate,
                ApplicationUserId = o.ApplicationUserId,
                OrderStatus = o.OrderStatus
            });

            return await result.ToListAsync();
        }
    }
} 

