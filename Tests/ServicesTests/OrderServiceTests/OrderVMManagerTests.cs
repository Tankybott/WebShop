using Moq;
using NUnit.Framework;
using Services.OrderServices;
using Models.DatabaseRelatedModels;
using DataAccess.Repository.IRepository;
using Utility.Common.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Expressions;
using Models.ViewModels;
using DataAccess.Repository;
using Models;
using Services.OrderServices.Interfaces;

namespace Services.OrderServices.Tests
{
    [TestFixture]
    public class OrderVMManagerTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IOrderHeaderManager> _mockOrderHeaderManager;
        private Mock<IUserRetriver> _mockUserRetriever;

        private Mock<IApplicationUserRepository> _mockUserRepo;
        private Mock<ICartRepository> _mockCartRepo;
        private Mock<IWebshopConfigRepository> _mockConfigRepo;
        private Mock<ICarrierRepository> _mockCarrierRepo;
        private Mock<IOrderHeaderRepository> _mockOrderHeaderRepo;

        private OrderVMManager _vmManager;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockOrderHeaderManager = new Mock<IOrderHeaderManager>();
            _mockUserRetriever = new Mock<IUserRetriver>();

            _mockUserRepo = new Mock<IApplicationUserRepository>();
            _mockCartRepo = new Mock<ICartRepository>();
            _mockConfigRepo = new Mock<IWebshopConfigRepository>();
            _mockCarrierRepo = new Mock<ICarrierRepository>();
            _mockOrderHeaderRepo = new Mock<IOrderHeaderRepository>();

            _mockUnitOfWork.Setup(u => u.ApplicationUser).Returns(_mockUserRepo.Object);
            _mockUnitOfWork.Setup(u => u.Cart).Returns(_mockCartRepo.Object);
            _mockUnitOfWork.Setup(u => u.WebshopConfig).Returns(_mockConfigRepo.Object);
            _mockUnitOfWork.Setup(u => u.Carrier).Returns(_mockCarrierRepo.Object);
            _mockUnitOfWork.Setup(u => u.OrderHeader).Returns(_mockOrderHeaderRepo.Object);

            _vmManager = new OrderVMManager(_mockUnitOfWork.Object, _mockOrderHeaderManager.Object, _mockUserRetriever.Object);
        }

        #region CreateVmForNewOrderAsync

        [Test]
        public async Task CreateVmForNewOrderAsync_ShouldThrowException_WhenUserIsNull()
        {
            // Arrange
            _mockUserRetriever.Setup(r => r.GetCurrentUserId()).Returns("user-id");
            _mockUserRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                         .ReturnsAsync((ApplicationUser)null!);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(() => _vmManager.CreateVmForNewOrderAsync());
        }

        [Test]
        public async Task CreateVmForNewOrderAsync_ShouldSetOrderHeaderUser_WhenUserExists()
        {
            // Arrange
            var user = new ApplicationUser { Id = "user-id" };
            _mockUserRetriever.Setup(r => r.GetCurrentUserId()).Returns("user-id");
            _mockUserRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                         .ReturnsAsync(user);

            _mockCartRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Cart, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                         .ReturnsAsync((Cart)null!);
            _mockConfigRepo.Setup(r => r.GetAsync()).ReturnsAsync(new WebshopConfig { Currency = "PLN" });
            _mockCarrierRepo.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Carrier, bool>>>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Expression<Func<Carrier, object>>>()))
                            .ReturnsAsync(new List<Carrier>());

            // Act
            var result = await _vmManager.CreateVmForNewOrderAsync();

            // Assert
            _mockOrderHeaderManager.Verify(m => m.AddUserDataToOrderHeader(It.IsAny<OrderHeader>(), user), Times.Once);
        }

        [Test]
        public async Task CreateVmForNewOrderAsync_ShouldSetCurrency_WhenConfigExists()
        {
            _mockUserRetriever.Setup(r => r.GetCurrentUserId()).Returns("uid");
            _mockUserRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                         .ReturnsAsync(new ApplicationUser { Id = "uid" });
            _mockCartRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Cart, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                         .ReturnsAsync((Cart)null!);
            _mockConfigRepo.Setup(r => r.GetAsync()).ReturnsAsync(new WebshopConfig { Currency = "EUR" });
            _mockCarrierRepo.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Carrier, bool>>>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Expression<Func<Carrier, object>>>()))
                            .ReturnsAsync(new List<Carrier>());

            var result = await _vmManager.CreateVmForNewOrderAsync();

            Assert.That(result.Currency, Is.EqualTo("EUR"));
        }

        #endregion

        #region GetVmByIdAsync

        [Test]
        public async Task GetVmByIdAsync_ShouldSetOrderHeaderAndCurrency_WhenOrderExists()
        {
            var order = new OrderHeader { Id = 5 };
            _mockOrderHeaderRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<OrderHeader, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                                .ReturnsAsync(order);
            _mockConfigRepo.Setup(r => r.GetAsync()).ReturnsAsync(new WebshopConfig { Currency = "USD" });

            var result = await _vmManager.GetVmByIdAsync(5);

            Assert.That(result.OrderHeader, Is.EqualTo(order));
            Assert.That(result.Currency, Is.EqualTo("USD"));
        }

        [Test]
        public async Task GetVmByIdAsync_ShouldNotFail_WhenOrderHeaderIsNull()
        {
            _mockOrderHeaderRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<OrderHeader, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                                .ReturnsAsync((OrderHeader)null!);
            _mockConfigRepo.Setup(r => r.GetAsync()).ReturnsAsync(new WebshopConfig { Currency = "USD" });

            var result = await _vmManager.GetVmByIdAsync(1);

            Assert.That(result.OrderHeader, Is.Null);
            Assert.That(result.Currency, Is.EqualTo("USD"));
        }

        #endregion
    }
}
