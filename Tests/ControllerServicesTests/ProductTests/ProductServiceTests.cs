using NUnit.Framework;
using Moq;
using DataAccess.Repository.IRepository;
using Models;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using ControllersServices.ProductManagement.Interfaces;
using ProductManagementNamespace = ControllersServices.ProductManagement;
using Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.DTOs;
using Models.ProductModel;

namespace Tests.ControllersServices.ProductManagement
{
    [TestFixture]
    public class ProductServiceTests
    {
        private Mock<IProductRepository> _mockProductRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IProductVMCreator> _mockProductVMCreator;
        private Mock<IProductUpserter> _mockProductUpserter;
        private Mock<IProductRemover> _mockProductRemover;
        private ProductManagementNamespace.ProductService _productService;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockUnitOfWork.Setup(u => u.Product).Returns(_mockProductRepository.Object);

            _mockProductVMCreator = new Mock<IProductVMCreator>();
            _mockProductUpserter = new Mock<IProductUpserter>();
            _mockProductRemover = new Mock<IProductRemover>();

            _productService = new ProductManagementNamespace.ProductService(
                _mockUnitOfWork.Object,
                _mockProductVMCreator.Object,
                _mockProductUpserter.Object,
                _mockProductRemover.Object
            );
        }

        private void SetupMockForProductGetAllAsync(IEnumerable<Product> products)
        {
            _mockProductRepository.Setup(u => u.GetAllAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<string?>(),
                It.IsAny<bool>(),
                It.IsAny<Expression<Func<Product, object>>>()
            )).ReturnsAsync(products);
        }

        private void SetupMockForProductGetAsync(Product? product)
        {
            _mockProductRepository.Setup(u => u.GetAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<string>(),
                false
            )).ReturnsAsync(product);
        }

        private void SetupMockForCategoryGetAll(IEnumerable<Category> categories) 
        {
            _mockUnitOfWork.Setup(u => u.Category.GetAllAsync(
                It.IsAny<Expression<Func<Category, bool>>>(),
                It.IsAny<string?>(),
                true,
                It.IsAny<Expression<Func<Category, object>>>()
            )).ReturnsAsync(categories);
        }

        private void SetupMockForProductVMCreator()
        {
            var productVM = new ProductVM
            {
                Products = null,
                CategoryListItems = null
            };

            _mockProductVMCreator.Setup(p => p.CreateProductVM(It.IsAny<IEnumerable<Category>>(), It.IsAny<IEnumerable<Product>>()))
                                 .Returns(productVM);
        }

        #region Helper Methods

        private ProductFormModel CreateValidProductFormModel()
        {
            return new ProductFormModel
            {
                Id = 1,
                CategoryId = 10,
                Name = "Test Product",
                Price = 100.50m,
                ShortDescription = "Test Short Description",
                FullDescription = "Test Full Description",
                DiscountId = null,
                MainPhoto = Mock.Of<IFormFile>(),
                OtherPhotos = new List<IFormFile>(),
                UrlsToDelete = new List<string>(),
                DiscountStartDate = null,
                DiscountEndDate = null,
                DiscountPercentage = null,
                IsDisocuntChanged = false
            };
        }

        #endregion

        #region GetProductVMForIndexAsync Tests

        [Test]
        public async Task GetProductVMForIndexAsync_ShouldCallGetAllAsyncOnCategoryAndProduct()
        {   // Arrange 
            _mockUnitOfWork.Setup(u => u.Category.GetAllAsync(
                It.IsAny<Expression<Func<Category, bool>>>(),
                It.IsAny<string?>(),
                It.IsAny<bool>(),
                It.IsAny<Expression<Func<Category, object>>>()
            )).ReturnsAsync(new List<Category>());
            SetupMockForProductGetAllAsync(new List<Product>());

            // Act
            await _productService.GetProductVMForIndexAsync();

            // Assert
            _mockUnitOfWork.Verify(u => u.Category.GetAllAsync(
                It.IsAny<Expression<Func<Category, bool>>>(),
                It.IsAny<string?>(),
                true,
                It.IsAny<Expression<Func<Category, object>>>()
            ), Times.Once);

            _mockUnitOfWork.Verify(u => u.Product.GetAllAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<string?>(),
                true,
                It.IsAny<Expression<Func<Product, object>>>()
            ), Times.Once);
        }

        [Test]
        public async Task GetProductVMForIndexAsync_ShouldCallCreateProductVM()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category1" },
                new Category { Id = 2, Name = "Category2" }
            };

            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product1", CategoryId = 1 },
                new Product { Id = 2, Name = "Product2", CategoryId = 2 }
            };

            SetupMockForProductGetAllAsync(products);

            SetupMockForCategoryGetAll(categories);

            // Act
            await _productService.GetProductVMForIndexAsync();

            // Assert
            _mockProductVMCreator.Verify(p => p.CreateProductVM(categories, products), Times.Once);
        }

        [Test]
        public async Task GetProductVMForIndexAsync_ShouldReturnCorrectProductVM()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category1" },
                new Category { Id = 2, Name = "Category2" }
            };

            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product1", CategoryId = 1 },
                new Product { Id = 2, Name = "Product2", CategoryId = 2 }
            };

            var expectedVM = new ProductVM
            {
                Products = products,
                CategoryListItems = categories.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
            };

            SetupMockForProductGetAllAsync(products);

            _mockUnitOfWork.Setup(u => u.Category.GetAllAsync(
                It.IsAny<Expression<Func<Category, bool>>>(),
                It.IsAny<string?>(),
                true,
                It.IsAny<Expression<Func<Category, object>>>()
            )).ReturnsAsync(categories);

            _mockProductVMCreator.Setup(p => p.CreateProductVM(categories, products))
                                 .Returns(expectedVM);

            // Act
            var result = await _productService.GetProductVMForIndexAsync();

            // Assert
            Assert.That(result, Is.EqualTo(expectedVM));
        }

        #endregion
        #region GetProductVMAsync Tests

        [Test]
        public async Task GetProductVMAsync_ShouldFetchCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category1" },
                new Category { Id = 2, Name = "Category2" }
            };

            SetupMockForCategoryGetAll(categories);

            // Act
            await _productService.GetProductVMAsync();

            // Assert
            _mockUnitOfWork.Verify(u => u.Category.GetAllAsync(
                It.IsAny<Expression<Func<Category, bool>>>(),
                It.IsAny<string?>(),
                true,
                It.IsAny<Expression<Func<Category, object>>>()
            ), Times.Once);
        }

        [Test]
        public async Task GetProductVMAsync_ShouldCreateProductVM_WithoutProduct()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category1" },
                new Category { Id = 2, Name = "Category2" }
            };

            var expectedVM = new ProductVM
            {
                CategoryListItems = categories.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
            };

            SetupMockForCategoryGetAll(categories);

            _mockProductVMCreator.Setup(p => p.CreateProductVM(categories, null))
                                 .Returns(expectedVM);

            // Act
            var result = await _productService.GetProductVMAsync();

            // Assert
            Assert.That(result, Is.EqualTo(expectedVM));
        }

        [Test]
        public async Task GetProductVMAsync_ShouldFetchProduct_WhenIdIsProvided()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category1" },
                new Category { Id = 2, Name = "Category2" }
            };

            var product = new Product { Id = 10, Name = "Test Product" };

            SetupMockForCategoryGetAll(categories);
            SetupMockForProductGetAsync(product);
            SetupMockForProductVMCreator();

            // Act
            var result = await _productService.GetProductVMAsync(10);

            // Assert
            Assert.That(result.Product, Is.EqualTo(product));
        }

        [Test]
        public async Task GetProductVMAsync_ShouldNotFetchProduct_WhenIdIsNull()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category1" },
                new Category { Id = 2, Name = "Category2" }
            };

            SetupMockForCategoryGetAll(categories);
            SetupMockForProductVMCreator();

            // Act
            var result = await _productService.GetProductVMAsync();

            // Assert
            Assert.That(result.Product, Is.Null);
            _mockUnitOfWork.Verify(u => u.Product.GetAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<string?>(),
                It.IsAny<bool>()
            ), Times.Never);
        }

        #endregion
        #region GetProductsForTableAsync Tests

        [Test]
        public async Task GetProductsForTableAsync_ShouldCallGetProductsDtoOfCategoryAsync_WhenCategoryFilterIsValid()
        {
            // Arrange
            var categoryFilter = "10";
            _mockProductRepository.Setup(p => p.GetProductsTableDtoAsync(10))
                                  .ReturnsAsync(new List<ProductTableDTO>());

            // Act
            await _productService.GetProductsForTableAsync(categoryFilter);

            // Assert
            _mockProductRepository.Verify(p => p.GetProductsTableDtoAsync(10), Times.Once);
        }

        [Test]
        public async Task GetProductsForTableAsync_ShouldReturnFilteredProducts_WhenCategoryFilterIsValid()
        {
            // Arrange
            var categoryFilter = "10";
            var expectedProducts = new List<ProductTableDTO>
    {
        new ProductTableDTO { Id = 1, Name = "Product1", CategoryName = "Category1" },
        new ProductTableDTO { Id = 2, Name = "Product2", CategoryName = "Category1" }
    };

            _mockProductRepository.Setup(p => p.GetProductsTableDtoAsync(10))
                                  .ReturnsAsync(expectedProducts);

            // Act
            var result = await _productService.GetProductsForTableAsync(categoryFilter);

            // Assert
            Assert.That(result, Is.EqualTo(expectedProducts));
        }

        [Test]
        public async Task GetProductsForTableAsync_ShouldCallGetProductsDtoAsync_WhenCategoryFilterIsNull()
        {
            // Arrange
            string? categoryFilter = null;
            _mockProductRepository.Setup(p => p.GetProductsTableDtoAsync(null))
                                  .ReturnsAsync(new List<ProductTableDTO>());

            // Act
            await _productService.GetProductsForTableAsync(categoryFilter);

            // Assert
            _mockProductRepository.Verify(p => p.GetProductsTableDtoAsync(null), Times.Once);
        }

        [Test]
        public async Task GetProductsForTableAsync_ShouldReturnAllProducts_WhenCategoryFilterIsNull()
        {
            // Arrange
            string? categoryFilter = null;
            var expectedProducts = new List<ProductTableDTO>
    {
        new ProductTableDTO { Id = 1, Name = "Product1", CategoryName = "Category1" },
        new ProductTableDTO { Id = 2, Name = "Product2", CategoryName = "Category2" }
    };

            _mockProductRepository.Setup(p => p.GetProductsTableDtoAsync(null))
                                  .ReturnsAsync(expectedProducts);

            // Act
            var result = await _productService.GetProductsForTableAsync(categoryFilter);

            // Assert
            Assert.That(result, Is.EqualTo(expectedProducts));
        }

        [Test]
        public async Task GetProductsForTableAsync_ShouldNotCallGetProductsDtoOfCategoryAsync_WhenCategoryFilterIsInvalid()
        {
            // Arrange
            var categoryFilter = "invalid";
            _mockProductRepository.Setup(p => p.GetProductsTableDtoAsync(null))
                                  .ReturnsAsync(new List<ProductTableDTO>());

            // Act
            await _productService.GetProductsForTableAsync(categoryFilter);

            // Assert
            _mockProductRepository.Verify(p => p.GetProductsTableDtoAsync(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task GetProductsForTableAsync_ShouldCallGetProductsDtoAsync_WhenCategoryFilterIsInvalid()
        {
            // Arrange
            var categoryFilter = "invalid";
            _mockProductRepository.Setup(p => p.GetProductsTableDtoAsync(null))
                                  .ReturnsAsync(new List<ProductTableDTO>());

            // Act
            await _productService.GetProductsForTableAsync(categoryFilter);

            // Assert
            _mockProductRepository.Verify(p => p.GetProductsTableDtoAsync(null), Times.Once);
        }

        [Test]
        public async Task GetProductsForTableAsync_ShouldReturnAllProducts_WhenCategoryFilterIsInvalid()
        {
            // Arrange
            var categoryFilter = "invalid";
            var expectedProducts = new List<ProductTableDTO>
    {
        new ProductTableDTO { Id = 1, Name = "Product1", CategoryName = "Category1" },
        new ProductTableDTO { Id = 2, Name = "Product2", CategoryName = "Category2" }
    };

            _mockProductRepository.Setup(p => p.GetProductsTableDtoAsync(null))
                                  .ReturnsAsync(expectedProducts);

            // Act
            var result = await _productService.GetProductsForTableAsync(categoryFilter);

            // Assert
            Assert.That(result, Is.EqualTo(expectedProducts));
        }

        #endregion
        #region DeleteAsync Tests

        [Test]
        public async Task DeleteAsync_ShouldFetchProductById()
        {
            // Arrange
            var productId = 1;
            var product = new Product { Id = productId, Name = "Test Product" };

            _mockProductRepository.Setup(p => p.GetAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<string?>(), false))
                                  .ReturnsAsync(product);

            // Act
            await _productService.DeleteAsync(productId);

            // Assert
            _mockProductRepository.Verify(p => p.GetAsync(It.Is<Expression<Func<Product, bool>>>(f => f.Compile().Invoke(product)), null, false), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_ShouldCallRemoveAsync_WhenProductExists()
        {
            // Arrange
            var productId = 1;
            var product = new Product { Id = productId, Name = "Test Product" };

            _mockProductRepository.Setup(p => p.GetAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<string?>(), false))
                                  .ReturnsAsync(product);

            // Act
            await _productService.DeleteAsync(productId);

            // Assert
            _mockProductRemover.Verify(r => r.RemoveAsync(product), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_ShouldNotCallRemoveAsync_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = 1;

            _mockProductRepository.Setup(p => p.GetAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<string?>(), false))
                                  .ReturnsAsync((Product?)null);

            // Act
            await _productService.DeleteAsync(productId);

            // Assert
            _mockProductRemover.Verify(r => r.RemoveAsync(It.IsAny<Product>()), Times.Never);
        }

        [Test]
        public async Task DeleteAsync_ShouldHandleNullProductGracefully()
        {
            // Arrange
            var productId = 1;

            _mockProductRepository.Setup(p => p.GetAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<string?>(), false))
                                  .ReturnsAsync((Product?)null);

            // Act & Assert
            Assert.DoesNotThrowAsync(() => _productService.DeleteAsync(productId));
        }

        #endregion
    }
}