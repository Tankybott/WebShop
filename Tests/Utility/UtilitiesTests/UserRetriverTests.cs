using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System.Security.Claims;


namespace Utility.Common.Tests
{
    [TestFixture]
    public class UserRetriverTests
    {
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private UserRetriver _userRetriver;

        [SetUp]
        public void Setup()
        {
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _userRetriver = new UserRetriver(_mockHttpContextAccessor.Object);
        }

        #region GetCurrentUser

        [Test]
        public void GetCurrentUser_ShouldReturnUser_WhenUserIsAuthenticated()
        {
            // Arrange
            var user = CreateAuthenticatedUser("123");
            var context = new DefaultHttpContext { User = user };
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            // Act
            var result = _userRetriver.GetCurrentUser();

            // Assert
            Assert.That(result, Is.EqualTo(user));
        }

        [Test]
        public void GetCurrentUser_ShouldThrowException_WhenHttpContextIsNull()
        {
            // Arrange
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext)null);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _userRetriver.GetCurrentUser());
        }

        [Test]
        public void GetCurrentUser_ShouldThrowException_WhenUserIsUnauthenticated()
        {
            // Arrange
            var unauthenticatedUser = new ClaimsPrincipal(new ClaimsIdentity());
            var context = new DefaultHttpContext { User = unauthenticatedUser };
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _userRetriver.GetCurrentUser());
        }

        #endregion

        #region GetCurrentUserId

        [Test]
        public void GetCurrentUserId_ShouldReturnId_WhenClaimIsPresent()
        {
            // Arrange
            var expectedUserId = "user-123";
            var user = CreateAuthenticatedUser(expectedUserId);
            var context = new DefaultHttpContext { User = user };
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            // Act
            var userId = _userRetriver.GetCurrentUserId();

            // Assert
            Assert.That(userId, Is.EqualTo(expectedUserId));
        }

        [Test]
        public void GetCurrentUserId_ShouldThrowException_WhenClaimIsMissing()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new Claim("other", "data")
            }, "mock", ClaimTypes.Name, ClaimTypes.Role));
            var context = new DefaultHttpContext { User = user };
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _userRetriver.GetCurrentUserId());
        }

        #endregion

        #region Helpers

        private ClaimsPrincipal CreateAuthenticatedUser(string userId)
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock");

            return new ClaimsPrincipal(identity);
        }

        #endregion
    }
}
