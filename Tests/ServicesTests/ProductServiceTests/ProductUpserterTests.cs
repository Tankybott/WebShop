using NUnit.Framework;
using Moq;
using AutoMapper;
using DataAccess.Repository.IRepository;
using Models;
using Models.DiscountCreateModel;
using Microsoft.AspNetCore.Http;
using Services.ProductManagement.Interfaces;
using Models.FormModel;
using System.Linq.Expressions;
using Models.DatabaseRelatedModels;
using Services.DiscountService.Interfaces;

namespace ControllersServices.ProductManagement.Tests
{
    [TestFixture]
    public class ProductUpserterTests
    {
        private Mock<IMapper> _mockMapper;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IDiscountService> _mockDiscountService;
        private Mock<IProductPhotoUpserter> _mockProductPhotoUpserter;
        private Mock<IProductDiscountUpserter> _mockProductDiscountUpserter;
        private ProductUpserter _productUpserter;

        [SetUp]
        public void Setup()
        {
            _mockMapper = new Mock<IMapper>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.Product.Add(It.IsAny<Product>()));
            _mockUnitOfWork.Setup(u => u.Product.Update(It.IsAny<Product>()));
            _mockUnitOfWork.Setup(u => u.PhotoUrlSets.GetAsync(
                It.IsAny<Expression<Func<PhotoUrlSet, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
            )).ReturnsAsync(new PhotoUrlSet());

            _mockDiscountService = new Mock<IDiscountService>();
            _mockProductPhotoUpserter = new Mock<IProductPhotoUpserter>();
            _mockProductDiscountUpserter = new Mock<IProductDiscountUpserter>();

            _productUpserter = new ProductUpserter(
                _mockMapper.Object,
                _mockUnitOfWork.Object,
                _mockDiscountService.Object,
                _mockProductPhotoUpserter.Object,
                _mockProductDiscountUpserter.Object
            );
        }

        #region HandleUpsertAsync

        [Test]
        public async Task HandleUpsertAsync_ShouldMapModelToProduct_WhenCalled()
        {
            // Arrange
            var model = new ProductFormModel();
            var product = new Product();
            _mockMapper.Setup(m => m.Map(model, It.IsAny<Product>())).Callback<ProductFormModel, Product>((src, dest) => dest.Id = product.Id);

            // Act
            await _productUpserter.HandleUpsertAsync(model);

            // Assert
            _mockMapper.Verify(m => m.Map(model, It.IsAny<Product>()), Times.Once, "Mapper should be called to map the model to a Product.");
        }

        [Test]
        public async Task HandleUpsertAsync_ShouldHandleDiscountUpsert_WhenCalled()
        {
            // Arrange
            var model = new ProductFormModel { DiscountStartDate = DateTime.Now, DiscountEndDate = DateTime.Now.AddDays(1), DiscountPercentage = 10 };
            var product = new Product();
            _mockMapper.Setup(m => m.Map(model, It.IsAny<Product>())).Callback<ProductFormModel, Product>((src, dest) => dest.Id = product.Id);

            // Act
            await _productUpserter.HandleUpsertAsync(model);

            // Assert
            _mockProductDiscountUpserter.Verify(d => d.HandleDiscountUpsertAsync(It.IsAny<Product>(), It.IsAny<DiscountCreateModel>()), Times.Once, "Discount upsert should be handled.");
        }

        [Test]
        public async Task HandleUpsertAsync_ShouldUploadMainPhoto_WhenMainPhotoIsProvided()
        {
            // Arrange
            var model = new ProductFormModel { MainPhoto = Mock.Of<IFormFile>() };
            var product = new Product();
            _mockMapper.Setup(m => m.Map(model, It.IsAny<Product>())).Callback<ProductFormModel, Product>((src, dest) => dest.Id = product.Id);

            // Act
            await _productUpserter.HandleUpsertAsync(model);

            // Assert
            _mockProductPhotoUpserter.Verify(p => p.UploadMainPhotoSetAsync(It.IsAny<Product>(), model.MainPhoto), Times.Once, "Main photo should be uploaded if provided.");
        }

        [Test]
        public async Task HandleUpsertAsync_ShouldUploadOtherPhotos_WhenOtherPhotosAreProvided()
        {
            // Arrange
            var model = new ProductFormModel { OtherPhotos = new List<IFormFile> { Mock.Of<IFormFile>() } };
            var product = new Product();
            _mockMapper.Setup(m => m.Map(model, It.IsAny<Product>())).Callback<ProductFormModel, Product>((src, dest) => dest.Id = product.Id);

            // Act
            await _productUpserter.HandleUpsertAsync(model);

            // Assert
            _mockProductPhotoUpserter.Verify(p => p.UploadOtherPhotoSetsAsync(It.IsAny<Product>(), model.OtherPhotos), Times.Once, "Other photos should be uploaded if provided.");
        }

        [Test]
        public async Task HandleUpsertAsync_ShouldSetMainPhoto_WhenMainFlagIsIncorrect()
        {
            // Arrange
            var model = new ProductFormModel { MainPhotoUrl = "main-url", MainPhoto = null };
            var product = new Product { Id = 1 };
            _mockMapper.Setup(m => m.Map(model, It.IsAny<Product>())).Callback<ProductFormModel, Product>((src, dest) => dest.Id = product.Id);

            // Act
            await _productUpserter.HandleUpsertAsync(model);

            // Assert
            _mockProductPhotoUpserter.Verify(p => p.SetPhotoMain(model.MainPhotoUrl), Times.Once, "SetPhotoMain should be called when the main flag is incorrect.");
        }

        [Test]
        public async Task HandleUpsertAsync_ShouldntSetMainPhoto_WhenOldMainPhotoIsTheSame()
        {
            // Arrange
            var mainThumbnailPhotoUrl = "main-url";
            var model = new ProductFormModel { MainPhotoUrl = mainThumbnailPhotoUrl, MainPhoto = null };
            _mockUnitOfWork.Setup(u => u.PhotoUrlSets.GetAsync(
                It.IsAny<Expression<Func<PhotoUrlSet, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
            )).ReturnsAsync(new PhotoUrlSet() {
                ThumbnailPhotoUrl = mainThumbnailPhotoUrl

            });
            var product = new Product { Id = 1 };
            _mockMapper.Setup(m => m.Map(model, It.IsAny<Product>())).Callback<ProductFormModel, Product>((src, dest) => dest.Id = product.Id);

            // Act
            await _productUpserter.HandleUpsertAsync(model);

            // Assert
            _mockProductPhotoUpserter.Verify(p => p.SetPhotoMain(model.MainPhotoUrl), Times.Never, "SetPhotoMain shouldnt be called if old and new main photo the same.");
        }

        [Test]
        public async Task HandleUpsertAsync_ShouldDeletePhotoSets_WhenUrlsToDeleteAreProvided()
        {
            // Arrange
            var model = new ProductFormModel { UrlsToDelete = new List<string> { "url1", "url2" } };
            var product = new Product();
            _mockMapper.Setup(m => m.Map(model, It.IsAny<Product>())).Callback<ProductFormModel, Product>((src, dest) => dest.Id = product.Id);

            // Act
            await _productUpserter.HandleUpsertAsync(model);

            // Assert
            _mockProductPhotoUpserter.Verify(p => p.DeletePhotoSetsAsync(model.UrlsToDelete), Times.Once, "DeletePhotoSetsAsync should be called when URLs to delete are provided.");
        }

        [Test]
        public async Task HandleUpsertAsync_ShouldAddProduct_WhenProductIdIsZero()
        {
            // Arrange
            var model = new ProductFormModel();
            var product = new Product { Id = 0 };
            _mockMapper.Setup(m => m.Map(model, It.IsAny<Product>())).Callback<ProductFormModel, Product>((src, dest) => dest.Id = product.Id);

            // Act
            await _productUpserter.HandleUpsertAsync(model);

            // Assert
            _mockUnitOfWork.Verify(u => u.Product.Add(It.IsAny<Product>()), Times.Once, "Product should be added if its Id is zero.");
        }

        [Test]
        public async Task HandleUpsertAsync_ShouldUpdateProduct_WhenProductIdIsNonZero()
        {
            // Arrange
            var model = new ProductFormModel();
            var product = new Product { Id = 1 };
            _mockMapper.Setup(m => m.Map(model, It.IsAny<Product>())).Callback<ProductFormModel, Product>((src, dest) => dest.Id = product.Id);

            // Act
            await _productUpserter.HandleUpsertAsync(model);

            // Assert
            _mockUnitOfWork.Verify(u => u.Product.Update(It.IsAny<Product>()), Times.Once, "Product should be updated if its Id is non-zero.");
        }

        [Test]
        public void HandleUpsertAsync_ShouldThrowException_WhenSaveFails()
        {
            // Arrange
            var model = new ProductFormModel();
            _mockUnitOfWork.Setup(u => u.SaveAsync()).ThrowsAsync(new Exception("Save failed."));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => _productUpserter.HandleUpsertAsync(model), "An exception should be thrown if saving fails.");
        }

        #endregion
    }
}
