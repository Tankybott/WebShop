using Models.DTOs;

namespace Services.OrderServices.Interfaces
{
    public interface IOrderTableDtoRetriever
    {
        Task<IEnumerable<OrderDTO>> GetEntitiesAsync();
    }
}