using Models.DatabaseRelatedModels;

namespace Services.CarrierService.Interfaces
{
    public interface ICarrierService
    {
        Task DeleteById(int carreirId);
        Task<IEnumerable<Carrier>> GetAllCarriers();
        Task Upsert(Carrier carrier);
        Task<Carrier?> GetCarrierAsync(int? id);
    }
}