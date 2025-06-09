using Moq;
using NUnit.Framework;
using Services.OrderServices;
using DataAccess.Repository.IRepository;
using Models.DatabaseRelatedModels;
using Models;
using Models.FormModel;
using Utility.Common.Interfaces;
using Utility.Constants;
using System.Linq.Expressions;

namespace ServicesTests.OrderServices
{
    [TestFixture]
    public class OrderHeaderManagerTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IUserRetriver> _mockUserRetriever;
        private OrderHeaderManager _manager;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserRetriever = new Mock<IUserRetriver>();

            _manager = new OrderHeaderManager(_mockUnitOfWork.Object, _mockUserRetriever.Object);
        }

        #region CreateAsync

        [Test]
        public async Task CreateAsync_ShouldAddOrderHeader_WhenFormModelIsValid()
        {
            _mockUserRetriever.Setup(u => u.GetCurrentUserId()).Returns("userId");

            var formModel = new OrderFormModel
            {
                carrierId = 1,
                Name = "Test",
                PhoneNumber = "123",
                City = "City",
                Country = "Country",
                PostalCode = "00-000",
                Region = "Region",
                StreetAdress = "Street"
            };

            OrderHeader? addedOrder = null;
            _mockUnitOfWork.Setup(u => u.OrderHeader.Add(It.IsAny<OrderHeader>()))
                .Callback<OrderHeader>(o => addedOrder = o);

            var result = await _manager.CreateAsync(formModel);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(addedOrder?.ApplicationUserId, Is.EqualTo("userId"));
                Assert.That(addedOrder?.Name, Is.EqualTo("Test"));
            });
        }

        #endregion

        #region AddUserDataToOrderHeader

        [Test]
        public void AddUserDataToOrderHeader_ShouldSetProperties_WhenCalled()
        {
            var user = new ApplicationUser
            {
                Id = "id",
                Name = "name",
                PhoneNumber = "123",
                StreetAdress = "addr",
                City = "city",
                Region = "region",
                PostalCode = "code",
                Country = "country"
            };

            var header = new OrderHeader();

            _manager.AddUserDataToOrderHeader(header, user);

            Assert.Multiple(() =>
            {
                Assert.That(header.ApplicationUserId, Is.EqualTo("id"));
                Assert.That(header.Name, Is.EqualTo("name"));
                Assert.That(header.Country, Is.EqualTo("country"));
            });
        }

        #endregion

        #region UpdateAsync

        [Test]
        public async Task UpdateAsync_ShouldUpdateOrderHeader_WhenItExists()
        {
            var existing = new OrderHeader { Id = 1 };
            _mockUnitOfWork.Setup(u => u.OrderHeader.GetAsync(
                It.IsAny<Expression<Func<OrderHeader, bool>>>(), null, false))
                .ReturnsAsync(existing);

            var update = new OrderHeader
            {
                Id = 1,
                Name = "Updated",
                PhoneNumber = "987",
                StreetAdress = "New",
                City = "NewCity",
                Region = "NewRegion",
                PostalCode = "999",
                TrackingLink = "track",
                ShippingDate = DateTime.Today
            };

            await _manager.UpdateAsync(update);

            _mockUnitOfWork.Verify(u => u.OrderHeader.Update(It.Is<OrderHeader>(o =>
                o.Name == "Updated" &&
                o.PhoneNumber == "987" &&
                o.TrackingLink == "track"
            )), Times.Once);

            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_ShouldNotThrow_WhenOrderHeaderNotFound()
        {
            _mockUnitOfWork.Setup(u => u.OrderHeader.GetAsync(
                It.IsAny<Expression<Func<OrderHeader, bool>>>(), null, false))
                .ReturnsAsync((OrderHeader?)null);

            Assert.DoesNotThrowAsync(() => _manager.UpdateAsync(new OrderHeader { Id = 1 }));
        }

        #endregion
    }
}
