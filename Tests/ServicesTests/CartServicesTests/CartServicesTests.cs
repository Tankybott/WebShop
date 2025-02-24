using NUnit.Framework;
using Moq;
using Models.DTOs;
using Models.FormModel;
using Services.CartServices;
using Services.CartServices.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.Services.CartServicesTests
{
    [TestFixture]
    public class CartServicesTests
    {
        private Mock<ICartPriceSynchronizer> _mockCartPriceSynchronizer;
        private Mock<ICartItemsQuantityRetriver> _mockCartItemsQuantityRetriver;
        private Mock<ICartItemRemover> _mockCartItemRemover;
        private Mock<ICartItemAdder> _mockCartItemAdder;
        private Mock<ICartItemQuantityUpdater> _mockCartItemQuantityUpdater;
        private Mock<ICartItemQuantityValidator> _mockCartItemQuantityValidator;
        private ICartServices _cartServices;

        [SetUp]
        public void Setup()
        {
            _mockCartPriceSynchronizer = new Mock<ICartPriceSynchronizer>();
            _mockCartItemsQuantityRetriver = new Mock<ICartItemsQuantityRetriver>();
            _mockCartItemRemover = new Mock<ICartItemRemover>();
            _mockCartItemAdder = new Mock<ICartItemAdder>();
            _mockCartItemQuantityUpdater = new Mock<ICartItemQuantityUpdater>();
            _mockCartItemQuantityValidator = new Mock<ICartItemQuantityValidator>();

            _cartServices = new CartServices(
                _mockCartPriceSynchronizer.Object,
                _mockCartItemsQuantityRetriver.Object,
                _mockCartItemRemover.Object,
                _mockCartItemAdder.Object,
                _mockCartItemQuantityUpdater.Object,
                _mockCartItemQuantityValidator.Object
            );
        }

        [Test]
        public async Task AddItemToCartAsync_ShouldCallCartItemAdder()
        {
            // Arrange
            var formModel = new CartItemFormModel { ProductId = 1, ProductQuantity = 2 };

            // Act
            await _cartServices.AddItemToCartAsync(formModel);

            // Assert
            _mockCartItemAdder.Verify(a => a.AddItemAsync(formModel), Times.Once);
        }

        [Test]
        public async Task RemoveCartItemAsync_ShouldCallCartItemRemover()
        {
            // Arrange
            int cartItemId = 1;

            // Act
            await _cartServices.RemoveCartItemAsync(cartItemId);

            // Assert
            _mockCartItemRemover.Verify(r => r.RemoveAsync(cartItemId), Times.Once);
        }

        [Test]
        public async Task UpdateCartItemQantityAsync_ShouldCallCartItemQuantityUpdater()
        {
            // Arrange
            int cartItemId = 1;
            int newQuantity = 5;

            // Act
            await _cartServices.UpdateCartItemQantityAsync(cartItemId, newQuantity);

            // Assert
            _mockCartItemQuantityUpdater.Verify(u => u.UpdateQantityAsync(cartItemId, newQuantity), Times.Once);
        }

        [Test]
        public async Task ValidateCartProductsQuantityAsync_ShouldCallCartItemQuantityValidator()
        {
            // Arrange
            var cartItemsToCheck = new List<CartItemQuantityDTO>
            {
                new CartItemQuantityDTO { CartItemId = 1, Quantity = 5 },
                new CartItemQuantityDTO { CartItemId = 2, Quantity = 2 }
            };

            _mockCartItemQuantityValidator
                .Setup(v => v.ValidateItemsQuantity(cartItemsToCheck))
                .ReturnsAsync(cartItemsToCheck);

            // Act
            var result = await _cartServices.ValidateCartProductsQuantityAsync(cartItemsToCheck);

            // Assert
            Assert.That(result, Is.EqualTo(cartItemsToCheck));
            _mockCartItemQuantityValidator.Verify(v => v.ValidateItemsQuantity(cartItemsToCheck), Times.Once);
        }

        [Test]
        public async Task SynchronizeCartPrices_ShouldCallCartPriceSynchronizer()
        {
            // Arrange
            int cartId = 1;
            var synchronizedIds = new List<int> { 1, 2 };

            _mockCartPriceSynchronizer
                .Setup(s => s.Synchronize(cartId))
                .ReturnsAsync(synchronizedIds);

            // Act
            var result = await _cartServices.SynchronizeCartPrices(cartId);

            // Assert
            Assert.That(result, Is.EqualTo(synchronizedIds));
            _mockCartPriceSynchronizer.Verify(s => s.Synchronize(cartId), Times.Once);
        }

        [Test]
        public async Task GetCartItemsQantityAsync_ShouldCallCartItemsQuantityRetriver()
        {
            // Arrange
            int expectedQuantity = 5;

            _mockCartItemsQuantityRetriver
                .Setup(q => q.GetItemsQantityAsync())
                .ReturnsAsync(expectedQuantity);

            // Act
            var result = await _cartServices.GetCartItemsQantityAsync();

            // Assert
            Assert.That(result, Is.EqualTo(expectedQuantity));
            _mockCartItemsQuantityRetriver.Verify(q => q.GetItemsQantityAsync(), Times.Once);
        }
    }
}
