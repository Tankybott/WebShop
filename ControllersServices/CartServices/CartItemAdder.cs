
using DataAccess.Repository.IRepository;
using Models.FormModel;
using Services.CartServices.Interfaces;

namespace Services.CartServices
{
    public class CartItemAdder : ICartItemAdder
    {
        private readonly ICartRetriver _cartRetriver;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartItemCreator _cartItemCreator;

        public CartItemAdder(ICartRetriver cartRetriver, IUnitOfWork unitOfWork, ICartItemCreator cartItemCreator)
        {
            _cartRetriver = cartRetriver;
            _unitOfWork = unitOfWork;
            _cartItemCreator = cartItemCreator;
        }

        public async Task AddItemAsync(CartItemFormModel formModel)
        {
            var usersCart = await _cartRetriver.RetriveUserCartAsync();
            var alredyExistingCartItem = usersCart.Items.FirstOrDefault(i => i.ProductId == formModel.ProductId);
            var itemToAdd = await _cartItemCreator.CreateCartItemAsync(formModel);
            if (alredyExistingCartItem != null)
            {
                alredyExistingCartItem.ProductQuantity += formModel.ProductQuantity;
            }
            else
            {
                usersCart.Items.Add(itemToAdd);
            }
            _unitOfWork.Cart.Update(usersCart);
            await _unitOfWork.SaveAsync();
        }
    }
}
