

using DataAccess.Repository.IRepository;
using Models.DTOs;
using Services.OrderServices.Interfaces;
using Utility.Common.Interfaces;
using Utility.Constants;

namespace Services.OrderServices
{
    public class OrderTableDtoRetriever : IOrderTableDtoRetriever
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRetriver _userRetriver;
        public OrderTableDtoRetriever(IUnitOfWork unitOfWork, IUserRetriver userRetriver)
        {
            _unitOfWork = unitOfWork;
            _userRetriver = userRetriver;
        }

        public async Task<IEnumerable<OrderDTO>> GetEntitiesAsync()
        {
            var DTOs = await _unitOfWork.OrderHeader.GetOrderTableDtoAsync();
            var validDateDtos = DTOs.Select(d =>
            {
                d.CreationDate = d.CreationDate.ToLocalTime();
                return d;
            }).ToList();

            var currentUser = _userRetriver.GetCurrentUser();
            var currentUserId = _userRetriver.GetCurrentUserId();
            if (currentUser.IsInRole(IdentityRoleNames.HeadAdminRole) || currentUser.IsInRole(IdentityRoleNames.AdminRole))
            {
                return validDateDtos;
            }
            else
            {
                return validDateDtos.Where(d => d.ApplicationUserId == currentUserId);
            }
        }
    }
}
