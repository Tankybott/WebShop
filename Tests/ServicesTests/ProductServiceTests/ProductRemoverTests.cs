using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess.Repository.IRepository;
using Models;
using Models.DatabaseRelatedModels;
using Services.PhotoService.Interfaces;
using ControllersServices.ProductManagement;
using Services.DiscountService.Interfaces;

namespace ControllersServices.ProductManagement.Tests
{
    [TestFixture]
    public class ProductRemoverTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IDiscountService> _mockDiscountService;
        private Mock<IPhotoService> _mockPhotoService;
        private ProductRemover _productRemover;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUnitOfWork.Setup(u => u.Product.Remove(It.IsAny<Product>())); // Mocking Remove method
            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask); // Mock SaveAsync to avoid null reference
            _mockDiscountService = new Mock<IDiscountService>();
            _mockPhotoService = new Mock<IPhotoService>();

            _productRemover = new ProductRemover(
                _mockUnitOfWork.Object,
                _mockDiscountService.Object,
                _mockPhotoService.Object
            );
        }

        #region RemoveAsync

        [Test]
        public async Task RemoveAsync_ShouldCallDeleteDiscount_WhenProductHasDiscountId()
        {
            // Arrange
            var product = new Product { DiscountId = 1 };

            // Act
            await _productRemover.RemoveAsync(product);

            // Assert
            _mockDiscountService.Verify(ds => ds.DeleteByIdAsync(1), Times.Once, "DeleteByIdAsync should be called when the product has a DiscountId.");
        }

        [Test]
        public async Task RemoveAsync_ShouldNotCallDeleteDiscount_WhenProductHasNoDiscountId()
        {
            // Arrange
            var product = new Product { DiscountId = null };

            // Act
            await _productRemover.RemoveAsync(product);

            // Assert
            _mockDiscountService.Verify(ds => ds.DeleteByIdAsync(It.IsAny<int>()), Times.Never, "DeleteByIdAsync should not be called when the product has no DiscountId.");
        }

        [Test]
        public async Task RemoveAsync_ShouldCallDeletePhotosFromServer_ForEachPhotoUrlSet_WhenPhotosExist()
        {
            // Arrange
            var photoUrlSets = new List<PhotoUrlSet>
            {
                new PhotoUrlSet { ThumbnailPhotoUrl = "thumbnail1.jpg", BigPhotoUrl = "bigphoto1.jpg" },
                new PhotoUrlSet { ThumbnailPhotoUrl = "thumbnail2.jpg", BigPhotoUrl = "bigphoto2.jpg" }
            };
            var product = new Product { PhotosUrlSets = photoUrlSets };

            // Act
            await _productRemover.RemoveAsync(product);

            // Assert
            foreach (var photoUrlSet in photoUrlSets)
            {
                _mockPhotoService.Verify(ps => ps.DeletePhotosFromServer(photoUrlSet), Times.Once, $"DeletePhotosFromServer should be called for {photoUrlSet.ThumbnailPhotoUrl}.");
            }
        }

        [Test]
        public async Task RemoveAsync_ShouldNotCallDeletePhotosFromServer_WhenProductHasNoPhotos()
        {
            // Arrange
            var product = new Product { PhotosUrlSets = null };

            // Act
            await _productRemover.RemoveAsync(product);

            // Assert
            _mockPhotoService.Verify(ps => ps.DeletePhotosFromServer(It.IsAny<PhotoUrlSet>()), Times.Never, "DeletePhotosFromServer should not be called when the product has no photos.");
        }

        [Test]
        public async Task RemoveAsync_ShouldCallRemoveProduct_WhenCalled()
        {
            // Arrange
            var product = new Product();

            // Act
            await _productRemover.RemoveAsync(product);

            // Assert
            _mockUnitOfWork.Verify(uow => uow.Product.Remove(product), Times.Once, "Remove should be called to delete the product.");
        }

        [Test]
        public async Task RemoveAsync_ShouldCallSaveChanges_WhenCalled()
        {
            // Arrange
            var product = new Product();

            // Act
            await _productRemover.RemoveAsync(product);

            // Assert
            _mockUnitOfWork.Verify(uow => uow.SaveAsync(), Times.Once, "SaveAsync should be called to persist changes.");
        }

        [Test]
        public async Task RemoveAsync_ShouldDoNothing_WhenProductIsNull()
        {
            // Act
            await _productRemover.RemoveAsync(null);

            // Assert
            _mockDiscountService.Verify(ds => ds.DeleteByIdAsync(It.IsAny<int>()), Times.Never, "DeleteByIdAsync should not be called when the product is null.");
            _mockPhotoService.Verify(ps => ps.DeletePhotosFromServer(It.IsAny<PhotoUrlSet>()), Times.Never, "DeletePhotosFromServer should not be called when the product is null.");
            _mockUnitOfWork.Verify(uow => uow.Product.Remove(It.IsAny<Product>()), Times.Never, "Remove should not be called when the product is null.");
            _mockUnitOfWork.Verify(uow => uow.SaveAsync(), Times.Never, "SaveAsync should not be called when the product is null.");
        }

        #endregion
    }
}