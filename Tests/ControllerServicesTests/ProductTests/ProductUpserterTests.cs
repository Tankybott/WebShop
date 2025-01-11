using NUnit.Framework;
using Moq;
using AutoMapper;
using ControllersServices.ProductManagement;
using ControllersServices.ProductManagement.Interfaces;
using ControllersServices.Utilities.Interfaces;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Models;
using Models.DatabaseRelatedModels;
using System.Linq.Expressions;
using Models.ProductModel;

namespace Tests.ControllersServices.ProductService
{
    [TestFixture]
    public class ProductUpserterTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IProductPhotoService> _mockProductPhotoService;
        private Mock<IFileNameCreator> _mockFileNameCreator;
        private Mock<IDiscountService> _mockDiscountService;

        private IMapper _mapper;
        private ProductUpserter _productUpserter;

        [SetUp]
        public void Setup()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductFormToProductMapper>();
            });

            _mapper = config.CreateMapper();

            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockProductPhotoService = new Mock<IProductPhotoService>();
            _mockFileNameCreator = new Mock<IFileNameCreator>();
            _mockDiscountService = new Mock<IDiscountService>();

            SetupMockUnitOfWork();
            SetupMockProductPhotoService();
            SetupMockDiscountService();

            _productUpserter = new ProductUpserter(
                _mapper,
                _mockUnitOfWork.Object,
                _mockFileNameCreator.Object,
                _mockProductPhotoService.Object,
                _mockDiscountService.Object
            );
        }

        private void SetupMockUnitOfWork()
        {
            _mockUnitOfWork.Setup(u => u.Product.Add(It.IsAny<Product>()));
            _mockUnitOfWork.Setup(u => u.Product.Update(It.IsAny<Product>()));
            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(u => u.Discount.GetAsync(
                    It.IsAny<Expression<Func<Discount, bool>>>(),
                    It.IsAny<string?>(),
                    It.IsAny<bool>()
                ))
                .ReturnsAsync(new Discount { Id = 1, StartTime = DateTime.Now, EndTime = DateTime.Now.AddDays(1), Percentage = 10 });
            _mockUnitOfWork.Setup(u => u.Discount.Remove(It.IsAny<Discount>()));
        }

        private void SetupMockProductPhotoService()
        {
            _mockProductPhotoService.Setup(p => p.AddPhotoAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<string>()))
                                    .Returns(Task.CompletedTask);

            _mockProductPhotoService.Setup(p => p.DeletePhotoAsync(It.IsAny<string>()))
                                    .Returns(Task.CompletedTask);
        }

        private void SetupMockDiscountService()
        {
            _mockDiscountService.Setup(d => d.CreateDiscountAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                                .ReturnsAsync(new Discount { Id = 1 });

            _mockDiscountService.Setup(d => d.UpdateDiscountAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                                .ReturnsAsync(new Discount { Id = 1 });
        }

        #region Helper Methods

        private ProductFormModel CreateValidProductFormModel()
        {
            return new ProductFormModel
            {
                Id = 10,
                CategoryId = 10,
                DiscountStartDate = DateTime.Now.AddDays(1),
                DiscountEndDate = DateTime.Now.AddDays(2),
                DiscountPercentage = 10,
                DiscountId = 12,
                IsDisocuntChanged = true,
                MainPhoto = Mock.Of<IFormFile>(),
                OtherPhotos = new List<IFormFile> { Mock.Of<IFormFile>() },
                UrlsToDelete = new List<string> { "/images/product/photo1.jpg" }
            };
        }

        #endregion

        #region HandleUpsertAsync Tests

        [Test]
        public async Task HandleUpsertAsync_ShouldAddNewDiscount_WhenDiscountIdIs0()
        {
            // Arrange
            var model = CreateValidProductFormModel();
            model.DiscountId = 0;

            // Act
            await _productUpserter.HandleUpsertAsync(model);

            // Assert
            _mockDiscountService.Verify(d => d.CreateDiscountAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()), Times.Once);
        }

        [Test]
        public async Task HandleUpsertAsync_ShouldUpadteDiscount_WhenDiscountIdIsNot0()
        {
            // Arrange
            var model = CreateValidProductFormModel();
            model.DiscountId = 11;

            // Act
            await _productUpserter.HandleUpsertAsync(model);

            // Assert
            _mockDiscountService.Verify(d => d.UpdateDiscountAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()), Times.Once);
        }

        [Test]
        public async Task HandleUpsertAsync_ShouldCallHandleMainPhotoUpload()
        {
            // Arrange
            var model = CreateValidProductFormModel();
            model.OtherPhotos = null; // No other photos provided, calling method with just one (MAIN) photo to upload

            // Act
            await _productUpserter.HandleUpsertAsync(model);

            // Assert
            _mockProductPhotoService.Verify(p => p.AddPhotoAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task HandleUpsertAsync_ShouldCallHandleOtherPhotosUpload()
        {
            // Arrange
            var model = CreateValidProductFormModel();

            // Act
            await _productUpserter.HandleUpsertAsync(model);

            // Assert
            _mockProductPhotoService.Verify(p => p.AddPhotoAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task HandleUpsertAsync_ShouldCallHandlePhotosDeletion()
        {
            // Arrange
            var model = CreateValidProductFormModel();

            // Act
            await _productUpserter.HandleUpsertAsync(model);

            // Assert
            _mockProductPhotoService.Verify(p => p.DeletePhotoAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task HandleUpsertAsync_ShouldAddNewProduct_WhenProductIdIsZero()
        {
            // Arrange
            var model = CreateValidProductFormModel();
            model.Id = 0;

            // Act
            await _productUpserter.HandleUpsertAsync(model);

            // Assert
            _mockUnitOfWork.Verify(u => u.Product.Add(It.IsAny<Product>()), Times.Once);
        }

        [Test]
        public async Task HandleUpsertAsync_ShouldUpdateExistingProduct_WhenProductIdIsNotZero()
        {
            // Arrange
            var model = CreateValidProductFormModel();
            model.Id = 1;

            // Act
            await _productUpserter.HandleUpsertAsync(model);

            // Assert
            _mockUnitOfWork.Verify(u => u.Product.Update(It.IsAny<Product>()), Times.Once);
        }

        [Test]
        public async Task HandleUpsertAsync_ShouldCallSaveAsync()
        {
            // Arrange
            var model = CreateValidProductFormModel();

            // Act
            await _productUpserter.HandleUpsertAsync(model);

            // Assert
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task HandleUpsertAsync_ShouldDeleteDiscount_WhenSaveThrowsExceptionAndProductHasDiscount()
        {
            // Arrange
            var model = CreateValidProductFormModel();
            model.DiscountPercentage = 10;
            model.DiscountStartDate = DateTime.Now.AddDays(1);
            model.DiscountEndDate = DateTime.Now.AddDays(2);
            _mockUnitOfWork.Setup(u => u.SaveAsync()).ThrowsAsync(new Exception());

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => _productUpserter.HandleUpsertAsync(model));

            _mockUnitOfWork.Verify(u => u.Discount.GetAsync(It.IsAny<Expression<Func<Discount, bool>>>(), null, false), Times.Once);
            _mockUnitOfWork.Verify(u => u.Discount.Remove(It.IsAny<Discount>()), Times.Once);
        }

        [Test]
        public async Task HandleUpsertAsync_ShouldNotDeleteDiscount_WhenSaveThrowsExceptionAndProductHasNoDiscount()
        {
            // Arrange
            var model = CreateValidProductFormModel();
            model.DiscountId = 0;
            model.DiscountPercentage = null;
            model.DiscountEndDate = null;
            model.DiscountStartDate = null;
            _mockUnitOfWork.Setup(u => u.SaveAsync()).ThrowsAsync(new Exception());

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => _productUpserter.HandleUpsertAsync(model));

            _mockUnitOfWork.Verify(u => u.Discount.GetAsync(It.IsAny<Expression<Func<Discount, bool>>>(), null, false), Times.Never);
            _mockUnitOfWork.Verify(u => u.Discount.Remove(It.IsAny<Discount>()), Times.Never);
        }
        [Test]
        public async Task HandleUpsertAsync_ShoudThrowArgumentException_WhenDiscountDataIsNotValidForAddingOrDeletingDiscount()
        {
            // Arrange
            var model = CreateValidProductFormModel();
            model.DiscountId = 0;
            model.DiscountPercentage = null;
            model.DiscountEndDate = null;
            //missing start date as null which will affect deleting discount, neither 3 of those values are not valid for adding new one 
            _mockUnitOfWork.Setup(u => u.SaveAsync()).ThrowsAsync(new Exception());

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _productUpserter.HandleUpsertAsync(model));

            _mockUnitOfWork.Verify(u => u.Discount.GetAsync(It.IsAny<Expression<Func<Discount, bool>>>(), null, false), Times.Never);
            _mockUnitOfWork.Verify(u => u.Discount.Remove(It.IsAny<Discount>()), Times.Never);
        }
        #endregion
    }
}