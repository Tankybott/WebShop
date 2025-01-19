using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Models.DatabaseRelatedModels;
using Services.PhotoSetService.Interfaces;
using Services.ProductService;
using Models;

namespace Services.ProductService.Tests
{
    [TestFixture]
    public class ProductPhotoUpserterTests
    {
        private Mock<IPhotoSetService> _mockPhotoSetService;
        private Mock<IMainPhotoSetSynchronizer> _mockMainPhotoSetSynchronizer;
        private ProductPhotoUpserter _productPhotoUpserter;

        [SetUp]
        public void Setup()
        {
            _mockPhotoSetService = new Mock<IPhotoSetService>();
            _mockMainPhotoSetSynchronizer = new Mock<IMainPhotoSetSynchronizer>();
            _productPhotoUpserter = new ProductPhotoUpserter(_mockPhotoSetService.Object, _mockMainPhotoSetSynchronizer.Object);
        }

        #region UploadMainPhotoSetAsync

        [Test]
        public async Task UploadMainPhotoSetAsync_ShouldAddPhotoSetToProduct_WhenCalled()
        {
            // Arrange
            var product = new Product { PhotosUrlSets = new List<PhotoUrlSet>() };
            var mainPhoto = new Mock<IFormFile>().Object;
            var photoSet = new PhotoUrlSet { ThumbnailPhotoUrl = "thumbnail.jpg", BigPhotoUrl = "bigphoto.jpg", IsMainPhoto = true };

            _mockPhotoSetService.Setup(ps => ps.CreatePhotoSetAsync(mainPhoto, true)).ReturnsAsync(photoSet);

            // Act
            await _productPhotoUpserter.UploadMainPhotoSetAsync(product, mainPhoto);

            // Assert
            Assert.That(product.PhotosUrlSets.Contains(photoSet), Is.True, "The main photo set should be added to the product's photo sets.");
        }

        #endregion

        #region UploadOtherPhotoSetsAsync

        [Test]
        public async Task UploadOtherPhotoSetsAsync_ShouldAddPhotoSetsToProduct_WhenOtherPhotosProvided()
        {
            // Arrange
            var product = new Product { PhotosUrlSets = new List<PhotoUrlSet>() };
            var otherPhotos = new List<IFormFile> { new Mock<IFormFile>().Object, new Mock<IFormFile>().Object };
            var photoSet = new PhotoUrlSet { ThumbnailPhotoUrl = "thumbnail.jpg", BigPhotoUrl = "bigphoto.jpg", IsMainPhoto = false };

            _mockPhotoSetService.Setup(ps => ps.CreatePhotoSetAsync(It.IsAny<IFormFile>(), false)).ReturnsAsync(photoSet);

            // Act
            await _productPhotoUpserter.UploadOtherPhotoSetsAsync(product, otherPhotos);

            // Assert
            Assert.That(product.PhotosUrlSets.Count, Is.EqualTo(2), "The product should have photo sets added for each photo provided.");
        }

        [Test]
        public async Task UploadOtherPhotoSetsAsync_ShouldInitializePhotoUrlSets_WhenProductPhotosAreNull()
        {
            // Arrange
            var product = new Product { PhotosUrlSets = null };
            var otherPhotos = new List<IFormFile> { new Mock<IFormFile>().Object };

            var photoSet = new PhotoUrlSet { ThumbnailPhotoUrl = "thumbnail.jpg", BigPhotoUrl = "bigphoto.jpg", IsMainPhoto = false };
            _mockPhotoSetService.Setup(ps => ps.CreatePhotoSetAsync(It.IsAny<IFormFile>(), false)).ReturnsAsync(photoSet);

            // Act
            await _productPhotoUpserter.UploadOtherPhotoSetsAsync(product, otherPhotos);

            // Assert
            Assert.That(product.PhotosUrlSets, Is.Not.Null, "Product.PhotosUrlSets should be initialized if it is null.");
        }

        [Test]
        public async Task UploadOtherPhotoSetsAsync_ShouldNotAddPhotoSets_WhenOtherPhotosAreEmpty()
        {
            // Arrange
            var product = new Product { PhotosUrlSets = new List<PhotoUrlSet>() };

            // Act
            await _productPhotoUpserter.UploadOtherPhotoSetsAsync(product, null);

            // Assert
            Assert.That(product.PhotosUrlSets.Count, Is.EqualTo(0), "No photo sets should be added when no photos are provided.");
        }

        #endregion

        #region DeletePhotoSetsAsync

        [Test]
        public async Task DeletePhotoSetsAsync_ShouldCallDeletePhotoSetForEachThumbnail_WhenCalled()
        {
            // Arrange
            var thumbnailPhotoUrls = new List<string> { "thumbnail1.jpg", "thumbnail2.jpg" };

            // Act
            await _productPhotoUpserter.DeletePhotoSetsAsync(thumbnailPhotoUrls);

            // Assert
            _mockPhotoSetService.Verify(ps => ps.DeletePhotoSetAsync("thumbnail1.jpg"), Times.Once, "DeletePhotoSetAsync should be called for the first thumbnail.");
            _mockPhotoSetService.Verify(ps => ps.DeletePhotoSetAsync("thumbnail2.jpg"), Times.Once, "DeletePhotoSetAsync should be called for the second thumbnail.");
        }

        #endregion

        #region SetPhotoMain

        [Test]
        public async Task SetPhotoMain_ShouldCallSynchronizeMainPhotoSet_WhenCalledWithNewMainPhotoThumbnailUrl()
        {
            // Arrange
            var newMainPhotoThumbnailUrl = "new-thumbnail.jpg";

            // Act
            await _productPhotoUpserter.SetPhotoMain(newMainPhotoThumbnailUrl);

            // Assert
            _mockMainPhotoSetSynchronizer.Verify(s => s.SynchronizeMainPhotoSetAsync(newMainPhotoThumbnailUrl), Times.Once, "SynchronizeMainPhotoSetAsync should be called with the new main photo thumbnail URL.");
        }

        #endregion
    }
}
