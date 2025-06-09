using Moq;
using NUnit.Framework;
using Services.OrderServices;
using DataAccess.Repository.IRepository;
using Models.DatabaseRelatedModels;
using Microsoft.AspNetCore.Identity.UI.Services;
using Services.EmailFactory.interfaces;
using Utility.Constants;
using System.Linq.Expressions;
using Models;

namespace ServicesTests.OrderServices
{
    [TestFixture]
    public class OrderStatusManagerTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IEmailFactory> _mockEmailFactory;
        private Mock<IEmailSender> _mockEmailSender;
        private OrderStatusManager _manager;
        private OrderHeader _orderHeader;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockEmailFactory = new Mock<IEmailFactory>();
            _mockEmailSender = new Mock<IEmailSender>();

            _manager = new OrderStatusManager(
                _mockUnitOfWork.Object,
                _mockEmailFactory.Object,
                _mockEmailSender.Object
            );

            _orderHeader = new OrderHeader
            {
                Id = 1,
                OrderStatus = "Created",
                ApplicationUser = new ApplicationUser { Email = "test@example.com" }
            };
        }

        #region StartProcessingAsync

        [Test]
        public async Task StartProcessingAsync_ShouldCallUpdateAndSaveAsync_WhenOrderHeaderExists()
        {
            SetupMockGetOrderHeader(_orderHeader);

            await _manager.StartProcessingAsync(1);

            _mockUnitOfWork.Verify(u => u.OrderHeader.Update(_orderHeader), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task StartProcessingAsync_ShouldSendEmail_WhenEmailExists()
        {
            SetupMockGetOrderHeader(_orderHeader);
            _mockEmailFactory.Setup(f => f.BuildInformationEmail(It.IsAny<string>(), It.IsAny<List<string>>()))
                .Returns("<html>email</html>");

            await _manager.StartProcessingAsync(1);

            _mockEmailSender.Verify(e => e.SendEmailAsync(
                "test@example.com",
                "Your Order is Now Processing",
                "<html>email</html>"), Times.Once);
        }

        [Test]
        public async Task StartProcessingAsync_ShouldNotSendEmail_WhenEmailIsNull()
        {
            _orderHeader.ApplicationUser!.Email = null;
            SetupMockGetOrderHeader(_orderHeader);

            await _manager.StartProcessingAsync(1);

            _mockEmailSender.Verify(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region SendAsync

        [Test]
        public async Task SendAsync_ShouldThrowException_WhenValidationFails()
        {
            _orderHeader.TrackingLink = null; // cause validation to fail
            SetupMockGetOrderHeader(_orderHeader);

            Assert.ThrowsAsync<InvalidOperationException>(() => _manager.SendAsync(1));
        }

        [Test]
        public async Task SendAsync_ShouldUpdateStatusAndSave_WhenOrderIsValid()
        {
            MakeOrderHeaderValid(_orderHeader);
            SetupMockGetOrderHeader(_orderHeader);

            await _manager.SendAsync(1);

            _mockUnitOfWork.Verify(u => u.OrderHeader.Update(_orderHeader), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task SendAsync_ShouldSendEmail_WhenOrderIsValid()
        {
            MakeOrderHeaderValid(_orderHeader);
            SetupMockGetOrderHeader(_orderHeader);
            _mockEmailFactory.Setup(f => f.BuildInformationEmail(It.IsAny<string>(), It.IsAny<List<string>>()))
                .Returns("<html>shipped</html>");

            await _manager.SendAsync(1);

            _mockEmailSender.Verify(e => e.SendEmailAsync(
                "test@example.com",
                "Your Order Is On Its Way",
                "<html>shipped</html>"), Times.Once);
        }

        #endregion

        private void SetupMockGetOrderHeader(OrderHeader header)
        {
            _mockUnitOfWork.Setup(u => u.OrderHeader.GetAsync(
                It.IsAny<Expression<Func<OrderHeader, bool>>>(),
                It.IsAny<string?>(),
                It.IsAny<bool>())).ReturnsAsync(header);
        }

        private void MakeOrderHeaderValid(OrderHeader header)
        {
            header.ApplicationUserId = "user-id";
            header.OrderStatus = "Created";
            header.TrackingLink = "track";
            header.SessionId = "session";
            header.PaymentIntentId = "intent";
            header.PaymentLink = "link";
            header.Name = "John";
            header.PhoneNumber = "123";
            header.StreetAdress = "Street";
            header.City = "City";
            header.Region = "Region";
            header.PostalCode = "Code";
            header.Country = "Country";
            header.ShippingDate = DateTime.Now;
            header.CreationDate = DateTime.Now;
            header.PaymentDate = DateTime.Now;
            header.CarrierId = 1;
        }
    }
}
