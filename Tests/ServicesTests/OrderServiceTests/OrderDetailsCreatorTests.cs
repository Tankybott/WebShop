using Moq;
using NUnit.Framework;
using Services.OrderServices;
using DataAccess.Repository.IRepository;
using Models;
using Models.DatabaseRelatedModels;
using System.Linq.Expressions;

namespace ServicesTests.OrderServices
{
    [TestFixture]
    public class OrderDetailsCreatorTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ICartRepository> _mockCartRepository;
        private Mock<IOrderDetailRepository> _mockOrderDetailRepository;
        private OrderDetailsCreator _creator;
        private Cart _testCart;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCartRepository = new Mock<ICartRepository>();
            _mockOrderDetailRepository = new Mock<IOrderDetailRepository>();

            _mockUnitOfWork.Setup(u => u.Cart).Returns(_mockCartRepository.Object);
            _mockUnitOfWork.Setup(u => u.OrderDetail).Returns(_mockOrderDetailRepository.Object);

            _creator = new OrderDetailsCreator(_mockUnitOfWork.Object);

            _testCart = new Cart
            {
                Id = 1,
                Items = new List<CartItem>
                {
                    new CartItem
                    {
                        CurrentPrice = 50,
                        ProductQuantity = 2,
                        Product = new Product
                        {
                            Id = 1,
                            Name = "Test Product",
                            ShippingPriceFactor = 1.5m
                        }
                    }
                }
            };
        }

        #region CreateDetailsAsync

        [Test]
        public void CreateDetailsAsync_ShouldThrowArgumentNullException_WhenCartNotFound()
        {
            _mockCartRepository.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<Cart, bool>>>(),
                It.IsAny<string?>(),
                It.IsAny<bool>()
            )).ReturnsAsync((Cart?)null);

            var act = () => _creator.CreateDetailsAsync(1, 1);

            Assert.ThrowsAsync<ArgumentNullException>(async () => await act());
        }

        [Test]
        public void CreateDetailsAsync_ShouldThrowInvalidOperationException_WhenNoValidCartItems()
        {
            var cartWithZeroQty = new Cart
            {
                Id = 2,
                Items = new List<CartItem>
                {
                    new CartItem
                    {
                        CurrentPrice = 30,
                        ProductQuantity = 0,
                        Product = new Product
                        {
                            Id = 2,
                            Name = "ZeroQty Product",
                            ShippingPriceFactor = 1.0m
                        }
                    }
                }
            };

            _mockCartRepository.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<Cart, bool>>>(),
                It.IsAny<string?>(),
                It.IsAny<bool>()
            )).ReturnsAsync(cartWithZeroQty);

            var act = () => _creator.CreateDetailsAsync(1, 1);

            Assert.ThrowsAsync<InvalidOperationException>(async () => await act());
        }

        [Test]
        public async Task CreateDetailsAsync_ShouldCallAddRange_WhenValidCartProvided()
        {
            _mockCartRepository.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<Cart, bool>>>(),
                It.IsAny<string?>(),
                It.IsAny<bool>()
            )).ReturnsAsync(_testCart);

            await _creator.CreateDetailsAsync(1, 99);

            _mockOrderDetailRepository.Verify(r => r.AddRange(It.Is<IEnumerable<OrderDetail>>(d =>
                d.Count() == 1 &&
                d.First().ProductId == _testCart.Items.First().Product.Id &&
                d.First().OrderHeaderId == 99
            )), Times.Once);
        }

        [Test]
        public async Task CreateDetailsAsync_ShouldCallSaveTwice_WhenValidCartProvided()
        {
            _mockCartRepository.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<Cart, bool>>>(),
                It.IsAny<string?>(),
                It.IsAny<bool>()
            )).ReturnsAsync(_testCart);

            await _creator.CreateDetailsAsync(1, 99);

            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Exactly(2));
        }

        [Test]
        public async Task CreateDetailsAsync_ShouldCallCartRemove_WhenValidCartProvided()
        {
            _mockCartRepository.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<Cart, bool>>>(),
                It.IsAny<string?>(),
                It.IsAny<bool>()
            )).ReturnsAsync(_testCart);

            await _creator.CreateDetailsAsync(1, 99);

            _mockCartRepository.Verify(r => r.Remove(_testCart), Times.Once);
        }

        #endregion
    }
}
