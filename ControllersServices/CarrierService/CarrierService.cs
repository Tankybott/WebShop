using DataAccess.Repository.IRepository;
using Models.DatabaseRelatedModels;
using Services.CarrierService.Interfaces;


namespace Services.CarrierService
{
    public class CarrierService : ICarrierService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CarrierService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Carrier>> GetAllCarriers()
        {
            return await _unitOfWork.Carrier.GetAllAsync();
        }

        public async Task<Carrier?> GetCarrierAsync(int? id) 
        {
            Carrier? carrier = null;
            if (id != null)
            {
                carrier = await _unitOfWork.Carrier.GetAsync(c => c.Id == id);
            }
            else if (id == null || carrier == null) 
            {
                carrier = new Carrier();
            }

            return carrier;
        }

        public async Task DeleteById(int carreirId)
        {
            var carrierToDelete = await _unitOfWork.Carrier.GetAsync(c => c.Id == carreirId);
            if (carrierToDelete != null)
            {
                _unitOfWork.Carrier.Remove(carrierToDelete);
                await _unitOfWork.SaveAsync();
            }
            else
            {
                throw new ArgumentException("No carrier with matching id to delete");
            }
        }

        public async Task Upsert(Carrier carrier)
        {
            if (carrier.Id == 0)
            {
                _unitOfWork.Carrier.Add(carrier);
                await _unitOfWork.SaveAsync();
            }
            else
            {
                _unitOfWork.Carrier.Update(carrier);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
