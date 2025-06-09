using Moq;
using NUnit.Framework;
using Services.OrderServices;
using DataAccess.Repository.IRepository;
using Services.OrderServices.Interfaces;
using Models.FormModel;
using Models.DatabaseRelatedModels;

namespace ServicesTests.OrderServices
{
    [TestFixture]
    public class OrderCreatorTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IOrderHeaderManager> _mockOrderHeaderManager;
        private Mock<IOrderDetailsCreator> _mockOrderDetailsCreator;
        private Mock<IStripePaymentManager> _mockStripePaymentManager;
        private OrderCreator _creator;
        private OrderFormModel _formModel;
        private OrderHeader _createdHeader;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockOrderHeaderManager = new Mock<IOrderHeaderManager>();
            _mockOrderDetailsCreator = new Mock<IOrderDetailsCreator>();
            _mockStripePaymentManager = new Mock<IStripePaymentManager>();

            _mockUnitOfWork.Setup(u => u.OrderHeader.Remove(It.IsAny<OrderHeader>()));
            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

            _creator = new OrderCreator(
                _mockUnitOfWork.Object,
                _mockOrderHeaderManager.Object,
                _mockOrderDetailsCreator.Object,
                _mockStripePaymentManager.Object
            );

            _formModel = new OrderFormModel
            {
                cartId = 5
            };

            _createdHeader = new OrderHeader
            {
                Id = 10,
                PaymentLink = "http://test-link"
            };

            _mockOrderHeaderManager
                .Setup(m => m.CreateAsync(It.IsAny<OrderFormModel>()))
                .ReturnsAsync(_createdHeader);
        }

        #region CreateAsync

        [Test]
        public async Task CreateAsync_ShouldCallCreateAsync_WhenCalled()
        {
            await _creator.CreateAsync(_formModel);

            _mockOrderHeaderManager.Verify(m => m.CreateAsync(_formModel), Times.Once);
        }

        [Test]
        public async Task CreateAsync_ShouldCallCreateDetailsAsync_WhenOrderHeaderCreated()
        {
            await _creator.CreateAsync(_formModel);

            _mockOrderDetailsCreator.Verify(m => m.CreateDetailsAsync(_formModel.cartId, _createdHeader.Id), Times.Once);
        }

        [Test]
        public async Task CreateAsync_ShouldCallMakeStripePaymentAsync_WhenDetailsCreated()
        {
            await _creator.CreateAsync(_formModel);

            _mockStripePaymentManager.Verify(m => m.MakeStripePaymentAsync(_createdHeader), Times.Once);
        }

        [Test]
        public void CreateAsync_ShouldRemoveOrderHeader_WhenCreateDetailsThrows()
        {
            _mockOrderDetailsCreator
                .Setup(m => m.CreateDetailsAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("fail"));

            var act = () => _creator.CreateAsync(_formModel);

            Assert.ThrowsAsync<Exception>(async () => await act());
        }

        [Test]
        public async Task CreateAsync_ShouldReturnPaymentLink_WhenAllStepsSucceed()
        {
            var result = await _creator.CreateAsync(_formModel);

            Assert.That(result, Is.EqualTo(_createdHeader.PaymentLink));
        }

        #endregion
    }
}
