using NUnit.Framework;
using Moq;
using ControllersServices.AdminCategoryService;
using ControllersServices.ProductManagement.Interfaces;
using DataAccess.Repository.IRepository;
using Models;

using System.Linq.Expressions;
using ControllersServices.AdminCategoryService.Interfaces;


namespace Tests.ControllerServicesTests.AdminCategoryTests
{
    [TestFixture]
    public class CategoryReletedProductRemoverTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IProductRemover> _mockProductRemover;
        private Mock<IProductRepository> _mockProductRepository;
        private CategoryReletedProductRemover _categoryReletedProductRemover;


        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockProductRemover = new Mock<IProductRemover>();
            _mockUnitOfWork.Setup(p => p.Product).Returns(_mockProductRepository.Object);

            _categoryReletedProductRemover = new CategoryReletedProductRemover(_mockProductRemover.Object, _mockUnitOfWork.Object);

        }

        [Test]
        public async Task DeleteProductsOfCategories_ShouldCallRemoveAsync_ForAllProductsOfAllCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1 },
                new Category { Id = 2 }
            };


            var products = new List<Product>
            {
                new Product { Id = 1, CategoryId = 2 },
                new Product { Id = 2, CategoryId = 2 },
            };


            _mockProductRepository.Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<Product, bool>>>(),
                    It.IsAny<string?>(),
                    It.IsAny<bool>(),
                    It.IsAny<Expression<Func<Product, object>>>()
                ))
                .ReturnsAsync(products);

            _mockProductRemover.Setup(p => p.RemoveAsync(It.IsAny<Product>()))
                .Returns(Task.CompletedTask);

            // Act
            await _categoryReletedProductRemover.DeleteProductsOfCategories(categories);

            // Assert
            _mockProductRemover.Verify(p => p.RemoveAsync(It.IsAny<Product>()), Times.Exactly(4)); // Total calls
        }

        [Test]
        public async Task DeleteProductsOfCategories_ShouldNotCallRemoveAsync_WhenNoProductsExistForCategory()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1 }
            };

            var products = new List<Product>
            {
            };

            _mockProductRepository.Setup(r => r.GetAllAsync(
                                It.IsAny<Expression<Func<Product, bool>>>(),
                                It.IsAny<string?>(),
                                It.IsAny<bool>(),
                                It.IsAny<Expression<Func<Product, object>>>()
                            ))
                            .ReturnsAsync(products);

            // Act
            await _categoryReletedProductRemover.DeleteProductsOfCategories(categories);

            // Assert
            _mockProductRemover.Verify(p => p.RemoveAsync(It.IsAny<Product>()), Times.Never); // Should not be called
        }
    }
}