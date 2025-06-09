using DataAccess.Repository.IRepository;
using Models.DatabaseRelatedModels;
using Moq;
using NUnit.Framework;
using System.Linq.Expressions;
using Models;
using DataAccess.Repository;

namespace ControllersServices.ProductBrowserService.Tests
{
    [TestFixture]
    public class ProductBrowserVmCreatorTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ICategoryRepository> _mockCategoryRepository;
        private Mock<IWebshopConfigRepository> _mockWebshopConfigRepository;
        private ProductBrowserVmCreator _vmCreator;

        [SetUp]
        public void Setup()
        {
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockWebshopConfigRepository = new Mock<IWebshopConfigRepository>();

            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUnitOfWork.Setup(u => u.Category).Returns(_mockCategoryRepository.Object);
            _mockUnitOfWork.Setup(u => u.WebshopConfig).Returns(_mockWebshopConfigRepository.Object);
            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

            _vmCreator = new ProductBrowserVmCreator(_mockUnitOfWork.Object);
        }

        #region CreateProductBrowserVM

        [Test]
        public async Task CreateProductBrowserVM_ShouldSetCategories_WhenCalled()
        {
            // Arrange
            var categories = new List<Category> { new Category { Name = "Test" } };
            _mockCategoryRepository.Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<Category, bool>>>(),
                It.IsAny<string?>(),
                It.IsAny<bool>(),
                It.IsAny<Expression<Func<Category, object>>>()
            )).ReturnsAsync(categories);

            _mockWebshopConfigRepository.Setup(r => r.GetAsync()).ReturnsAsync(new WebshopConfig { Currency = "USD" });

            // Act
            var result = await _vmCreator.CreateProductBrowserVM();

            // Assert
            Assert.That(result.Categories, Is.EqualTo(categories));
        }

        [Test]
        public async Task CreateProductBrowserVM_ShouldSetCurrency_WhenCalled()
        {
            // Arrange
            _mockCategoryRepository.Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<Category, bool>>>(),
                It.IsAny<string?>(),
                It.IsAny<bool>(),
                It.IsAny<Expression<Func<Category, object>>>()
            )).ReturnsAsync(new List<Category>());

            _mockWebshopConfigRepository.Setup(r => r.GetAsync()).ReturnsAsync(new WebshopConfig { Currency = "PLN" });

            // Act
            var result = await _vmCreator.CreateProductBrowserVM();

            // Assert
            Assert.That(result.Currency, Is.EqualTo("PLN"));
        }

        #endregion
    }
}
