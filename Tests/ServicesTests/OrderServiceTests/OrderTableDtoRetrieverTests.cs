using DataAccess.Repository.IRepository;
using Models.DTOs;
using Moq;
using NUnit.Framework;
using Services.OrderServices;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Utility.Common.Interfaces;
using Utility.Constants;

namespace ServicesTests.OrderServices
{
    [TestFixture]
    public class OrderTableDtoRetrieverTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IOrderHeaderRepository> _mockOrderHeaderRepository;
        private Mock<IUserRetriver> _mockUserRetriever;
        private OrderTableDtoRetriever _retriever;

        [SetUp]
        public void Setup()
        {
            _mockOrderHeaderRepository = new Mock<IOrderHeaderRepository>();
            _mockUserRetriever = new Mock<IUserRetriver>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUnitOfWork.Setup(u => u.OrderHeader).Returns(_mockOrderHeaderRepository.Object);

            _retriever = new OrderTableDtoRetriever(_mockUnitOfWork.Object, _mockUserRetriever.Object);
        }

        #region GetEntitiesAsync

        [Test]
        public async Task GetEntitiesAsync_ShouldReturnAllDtos_WhenUserIsAdmin()
        {
            // Arrange
            var dtos = new List<OrderDTO> { new OrderDTO(), new OrderDTO() };
            _mockOrderHeaderRepository.Setup(r => r.GetOrderTableDtoAsync()).ReturnsAsync(dtos);

            var user = new ClaimsPrincipal(new ClaimsIdentity());
            _mockUserRetriever.Setup(r => r.GetCurrentUser()).Returns(user);
            _mockUserRetriever.Setup(r => r.GetCurrentUserId()).Returns("user-id");

            // Act
            _mockUserRetriever.Setup(r => r.GetCurrentUser().IsInRole(IdentityRoleNames.AdminRole)).Returns(true);
            var result = await _retriever.GetEntitiesAsync();

            // Assert
            Assert.That(result, Is.EqualTo(dtos));
        }

        [Test]
        public async Task GetEntitiesAsync_ShouldReturnAllDtos_WhenUserIsHeadAdmin()
        {
            // Arrange
            var dtos = new List<OrderDTO> { new OrderDTO(), new OrderDTO() };
            _mockOrderHeaderRepository.Setup(r => r.GetOrderTableDtoAsync()).ReturnsAsync(dtos);

            var user = new ClaimsPrincipal(new ClaimsIdentity());
            _mockUserRetriever.Setup(r => r.GetCurrentUser()).Returns(user);
            _mockUserRetriever.Setup(r => r.GetCurrentUserId()).Returns("user-id");

            // Act
            _mockUserRetriever.Setup(r => r.GetCurrentUser().IsInRole(IdentityRoleNames.HeadAdminRole)).Returns(true);
            var result = await _retriever.GetEntitiesAsync();

            // Assert
            Assert.That(result, Is.EqualTo(dtos));
        }

        [Test]
        public async Task GetEntitiesAsync_ShouldFilterDtosByCurrentUserId_WhenUserIsNotAdmin()
        {
            // Arrange
            var dtos = new List<OrderDTO>
            {
                new OrderDTO { ApplicationUserId = "user-1" },
                new OrderDTO { ApplicationUserId = "user-2" },
                new OrderDTO { ApplicationUserId = "user-3" }
            };

            _mockOrderHeaderRepository.Setup(r => r.GetOrderTableDtoAsync()).ReturnsAsync(dtos);
            var user = new ClaimsPrincipal(new ClaimsIdentity());
            _mockUserRetriever.Setup(r => r.GetCurrentUser()).Returns(user);
            _mockUserRetriever.Setup(r => r.GetCurrentUserId()).Returns("user-2");

            _mockUserRetriever.Setup(r => r.GetCurrentUser().IsInRole(It.IsAny<string>())).Returns(false);

            // Act
            var result = await _retriever.GetEntitiesAsync();

            // Assert
            Assert.That(result.Select(r => r.ApplicationUserId), Is.EqualTo(new[] { "user-2" }));
        }

        #endregion
    }
}
