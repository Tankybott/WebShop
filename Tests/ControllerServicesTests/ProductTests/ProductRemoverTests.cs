using NUnit.Framework;
using Moq;
using ControllersServices.ProductManagement;
using ControllersServices.ProductManagement.Interfaces;
using DataAccess.Repository.IRepository;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.ControllerServicesTests.ProductTests
{
    [TestFixture]
    public class ProductRemoverTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IDiscountService> _mockDiscountService;
        private Mock<IProductPhotoService> _mockProductPhotoService;
        private ProductRemover _productRemover;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockDiscountService = new Mock<IDiscountService>();
            _mockProductPhotoService = new Mock<IProductPhotoService>();

            var mockProductRepository = new Mock<IProductRepository>();
            mockProductRepository.Setup(r => r.Remove(It.IsAny<Product>()));

            _mockUnitOfWork.Setup(u => u.Product).Returns(mockProductRepository.Object);

            _mockDiscountService.Setup(d => d.DeleteByIdAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
            _mockProductPhotoService.Setup(p => p.DeletePhotoAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

            _productRemover = new ProductRemover(_mockUnitOfWork.Object, _mockDiscountService.Object, _mockProductPhotoService.Object);
        }

        [Test]
        public async Task RemoveAsync_ShouldCallDeleteAsync_WhenProductHasDiscountId()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                DiscountId = 10,
                MainPhotoUrl = "main-photo.jpg",
                OtherPhotosUrls = null
            };

            // Act
            await _productRemover.RemoveAsync(product);

            // Assert
            _mockDiscountService.Verify(d => d.DeleteByIdAsync(10), Times.Once);
        }

        [Test]
        public async Task RemoveAsync_ShouldCallDeletePhotoAsync_ForMainPhoto()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                DiscountId = 10,
                MainPhotoUrl = "main-photo.jpg",
                OtherPhotosUrls = null
            };

            // Act
            await _productRemover.RemoveAsync(product);

            // Assert
            _mockProductPhotoService.Verify(p => p.DeletePhotoAsync("main-photo.jpg"), Times.Once);
        }

        [Test]
        public async Task RemoveAsync_ShouldCallDeletePhotoAsync_ForEachOtherPhotoUrl()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                DiscountId = null,
                MainPhotoUrl = "main-photo.jpg",
                OtherPhotosUrls = new List<string> { "photo1.jpg", "photo2.jpg" }
            };

            // Act
            await _productRemover.RemoveAsync(product);

            // Assert
            _mockProductPhotoService.Verify(p => p.DeletePhotoAsync("photo1.jpg"), Times.Once);
            _mockProductPhotoService.Verify(p => p.DeletePhotoAsync("photo2.jpg"), Times.Once);
        }

        [Test]
        public async Task RemoveAsync_ShouldCallProductRemove()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                DiscountId = 10,
                MainPhotoUrl = "main-photo.jpg",
                OtherPhotosUrls = null
            };

            // Act
            await _productRemover.RemoveAsync(product);

            // Assert
            _mockUnitOfWork.Verify(u => u.Product.Remove(product), Times.Once);
        }

        [Test]
        public async Task RemoveAsync_ShouldCallSaveAsync()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                DiscountId = 10,
                MainPhotoUrl = "main-photo.jpg",
                OtherPhotosUrls = null
            };

            // Act
            await _productRemover.RemoveAsync(product);

            // Assert
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task RemoveAsync_ShouldNotCallDeleteAsync_WhenProductHasNoDiscountId()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                DiscountId = null,
                MainPhotoUrl = "main-photo.jpg",
                OtherPhotosUrls = null
            };

            // Act
            await _productRemover.RemoveAsync(product);

            // Assert
            _mockDiscountService.Verify(d => d.DeleteByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task RemoveAsync_ShouldDoNothing_WhenProductIsNull()
        {
            // Act
            await _productRemover.RemoveAsync(null);

            // Assert
            _mockDiscountService.Verify(d => d.DeleteByIdAsync(It.IsAny<int>()), Times.Never);
            _mockProductPhotoService.Verify(p => p.DeletePhotoAsync(It.IsAny<string>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.Product.Remove(It.IsAny<Product>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Never);
        }
    }
}