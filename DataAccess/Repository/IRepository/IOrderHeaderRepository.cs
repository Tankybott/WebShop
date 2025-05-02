using Models.DatabaseRelatedModels;
using Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository: IRepository<OrderHeader>
    {
        Task<IEnumerable<OrderDTO>> GetOrderTableDtoAsync();
    }
}
