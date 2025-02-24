using NUnit.Framework;
using Moq;
using DataAccess.Repository.IRepository;
using Models;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;

using Models.ViewModels;
using Models.DTOs;
using Models.ProductModel;
using Services.ProductManagement.Interfaces;
using ControllersServices.ProductManagement;
using Models.FormModel;

namespace Tests.ControllersServices.ProductManagement
{
    [TestFixture]
    public class ProductServicesTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IProductVMCreator> _mockProductVMCreator;
        private Mock<IProductUpserter> _mockProductUpserter;
        private Mock<IProductRemover> _mockProductRemover;
        private Mock<IProductTableDtoRetriver> _mockProductTableDtoRetriver;
        private ProductServices _productServices;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUnitOfWork.Setup(u => u.Category.GetAllAsync(
                It.IsAny<Expression<Func<Category, bool>>>(),
                It.IsAny<string?>(),
                It.IsAny<bool>(),
                It.IsAny<Expression<Func<Category, object>>>()
            )).ReturnsAsync(new List<Category>());

            _mockUnitOfWork.Setup(u => u.Product.GetAllAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<string?>(),
                It.IsAny<bool>(),
                It.IsAny<Expression<Func<Product, object>>>()
            )).ReturnsAsync(new List<Product>());

            _mockUnitOfWork.Setup(u => u.Product.GetAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
            )).ReturnsAsync(new Product());

            _mockProductVMCreator = new Mock<IProductVMCreator>();
            _mockProductUpserter = new Mock<IProductUpserter>();
            _mockProductRemover = new Mock<IProductRemover>();
            _mockProductTableDtoRetriver = new Mock<IProductTableDtoRetriver>();

            _productServices = new ProductServices(
                _mockUnitOfWork.Object,
                _mockProductVMCreator.Object,
                _mockProductUpserter.Object,
                _mockProductRemover.Object,
                _mockProductTableDtoRetriver.Object
            );
        }

        #region GetProductVMForIndexAsync Tests

        [Test]
        public async Task GetProductVMForIndexAsync_ShouldReturnProductVM_WhenCalled()
        {
            // Arrange
            var categories = new List<Category> { new Category { Id = 1, Name = "Category1" } };
            var products = new List<Product> { new Product { Id = 1, Name = "Product1" } };

            _mockUnitOfWork.Setup(u => u.Category.GetAllAsync(
                It.IsAny<Expression<Func<Category, bool>>>(),
                It.IsAny<string?>(),
                true,
                It.IsAny<Expression<Func<Category, object>>>()
            )).ReturnsAsync(categories);

            _mockUnitOfWork.Setup(u => u.Product.GetAllAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<string?>(),
                true,
                It.IsAny<Expression<Func<Product, object>>>()
            )).ReturnsAsync(products);

            var expectedVM = new ProductVM();
            _mockProductVMCreator.Setup(vm => vm.CreateProductVM(categories, products)).Returns(expectedVM);

            // Act
            var result = await _productServices.GetProductVMForIndexAsync();

            // Assert
            Assert.That(result, Is.EqualTo(expectedVM), "The returned ProductVM should match the expected VM.");
        }

        #endregion

        #region GetProductVMAsync Tests

        [Test]
        public async Task GetProductVMAsync_ShouldReturnProductVM_WhenCalledWithId()
        {
            // Arrange
            var categories = new List<Category> { new Category { Id = 1, Name = "Category1" } };
            var product = new Product { Id = 1, Name = "Product1" };
            var expectedProductVM = new ProductVM() 
            {
                Product = product,
            };

            _mockUnitOfWork.Setup(u => u.Category.GetAllAsync(
                It.IsAny<Expression<Func<Category, bool>>>(),
                It.IsAny<string?>(),
                true,
                It.IsAny<Expression<Func<Category, object>>>()
            )).ReturnsAsync(categories);


            _mockUnitOfWork.Setup(u => u.Product.GetAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<string>(),
                false
            )).ReturnsAsync(product);


            var expectedVM = new ProductVM { Product = product };

            _mockProductVMCreator.Setup(vm => vm.CreateProductVM(
                It.IsAny<IEnumerable<Category>>(),
                It.IsAny<IEnumerable<Product>>()
            )).Returns(expectedProductVM);


            // Act
            var result = await _productServices.GetProductVMAsync(1);

            // Assert
            Assert.That(result.Product.Id, Is.EqualTo(product.Id), "The returned ProductVM should contain the correct Product.");
        }

        [Test]
        public async Task GetProductVMAsync_ShouldReturnProductVMWithoutProduct_WhenCalledWithoutId()
        {
            // Arrange
            var categories = new List<Category> { new Category { Id = 1, Name = "Category1" } };

            _mockUnitOfWork.Setup(u => u.Category.GetAllAsync(
                It.IsAny<Expression<Func<Category, bool>>>(),
                It.IsAny<string?>(),
                true,
                It.IsAny<Expression<Func<Category, object>>>()
            )).ReturnsAsync(categories);

            var expectedVM = new ProductVM();
            _mockProductVMCreator.Setup(vm => vm.CreateProductVM(
                It.IsAny<IEnumerable<Category>>(),
                It.IsAny<IEnumerable<Product>>()
            )).Returns(new ProductVM());

            // Act
            var result = await _productServices.GetProductVMAsync();

            // Assert
            Assert.That(result.Product, Is.Null, "The returned ProductVM should not contain a Product.");
        }

        #endregion

        #region UpsertAsync Tests

        [Test]
        public async Task UpsertAsync_ShouldCallHandleUpsert_WhenCalled()
        {
            // Arrange
            var model = new ProductFormModel();

            // Act
            await _productServices.UpsertAsync(model);

            // Assert
            _mockProductUpserter.Verify(up => up.HandleUpsertAsync(model), Times.Once, "HandleUpsertAsync should be called when UpsertAsync is invoked.");
        }

        #endregion

        #region GetProductsForTableAsync Tests

        [Test]
        public async Task GetProductsForTableAsync_ShouldReturnProductTableDTOs_WhenCalled()
        {
            // Arrange
            var productTableDTOs = new List<ProductTableDTO> { new ProductTableDTO { Id = 1, Name = "Product1" } };

            _mockProductTableDtoRetriver.Setup(r => r.GetProductsTableDtoAsync(It.IsAny<int?>(), It.IsAny<string>()))
                                        .ReturnsAsync(productTableDTOs);

            // Act
            var result = await _productServices.GetProductsForTableAsync(null, null);

            // Assert
            Assert.That(result, Is.EqualTo(productTableDTOs), "The returned ProductTableDTOs should match the expected list.");
        }

        #endregion

        #region DeleteAsync Tests

        [Test]
        public async Task DeleteAsync_ShouldCallRemoveAsync_WhenProductExists()
        {
            // Arrange
            var product = new Product { Id = 1 };

            _mockUnitOfWork.Setup(u => u.Product.GetAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<string>(),
                false
            )).ReturnsAsync(product);

            // Act
            await _productServices.DeleteAsync(1);

            // Assert
            _mockProductRemover.Verify(pr => pr.RemoveAsync(product), Times.Once, "RemoveAsync should be called when the product exists.");
        }

        [Test]
        public async Task DeleteAsync_ShouldNotCallRemoveAsync_WhenProductDoesNotExist()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Product.GetAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<string>(),
                false
            )).ReturnsAsync((Product)null);

            // Act
            await _productServices.DeleteAsync(1);

            // Assert
            _mockProductRemover.Verify(pr => pr.RemoveAsync(It.IsAny<Product>()), Times.Never, "RemoveAsync should not be called when the product does not exist.");
        }

        #endregion
    }
}
