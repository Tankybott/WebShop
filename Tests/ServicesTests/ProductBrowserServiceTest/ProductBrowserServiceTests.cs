using AutoMapper;
using ControllersServices.CategoryService.Interfaces;
using ControllersServices.ProductBrowserService;
using ControllersServices.ProductBrowserService.Interfaces;
using DataAccess.Repository.IRepository;
using DataAccess.Repository.Utility;
using Models;
using Models.DTOs;
using Models.ProductFilterOptions;
using Models.ViewModels;
using Moq;
using NUnit.Framework;
using Services.CategoryService.Interfaces;
using System.Linq.Expressions;
using Utility.Common;

namespace ControllersServices.ProductBrowserService.Tests
{
    [TestFixture]
    public class ProductBrowserServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IProductBrowserVmCreator> _mockVmCreator;
        private Mock<ICategoryHierarchyRetriver> _mockHierarchyRetriever;
        private Mock<ICategoryIdRetriver> _mockCategoryIdRetriever;
        private Mock<IMapper> _mockMapper;
        private Mock<ICategoryRepository> _mockCategoryRepository;
        private Mock<IProductRepository> _mockProductRepository;
        private ProductBrowserService _productBrowserService;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockVmCreator = new Mock<IProductBrowserVmCreator>();
            _mockHierarchyRetriever = new Mock<ICategoryHierarchyRetriver>();
            _mockCategoryIdRetriever = new Mock<ICategoryIdRetriver>();
            _mockMapper = new Mock<IMapper>();
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockProductRepository = new Mock<IProductRepository>();

            _mockUnitOfWork.Setup(u => u.Category).Returns(_mockCategoryRepository.Object);
            _mockUnitOfWork.Setup(u => u.Product).Returns(_mockProductRepository.Object);

            _productBrowserService = new ProductBrowserService(
                _mockUnitOfWork.Object,
                _mockVmCreator.Object,
                _mockHierarchyRetriever.Object,
                _mockMapper.Object,
                _mockCategoryIdRetriever.Object
            );
        }

        #region GetProductBrowserVM

        [Test]
        public async Task GetProductBrowserVM_ShouldCallVmCreator_WhenCalled()
        {
            // Arrange
            var expectedVm = new ProductBrowserVM();
            _mockVmCreator.Setup(v => v.CreateProductBrowserVM()).ReturnsAsync(expectedVm);

            // Act
            var result = await _productBrowserService.GetProductBrowserVM();

            // Assert
            Assert.That(result, Is.EqualTo(expectedVm));
        }

        #endregion

        #region GetFilteredProductsDTO

        [Test]
        public async Task GetFilteredProductsDTO_ShouldReturnProducts_WhenCategoryIsNull()
        {
            // Arrange
            var filter = new ProductFilterOptionsRequest { CategoryIDFilter = 999 };
            _mockCategoryRepository.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<Category, bool>>>(),
                It.IsAny<string?>(),
                It.IsAny<bool>()
            )).ReturnsAsync((Category?)null);

            _mockMapper.Setup(m => m.Map(filter, It.IsAny<ProductFilterOptionsQuery>()));
            _mockProductRepository.Setup(r => r.GetProductCardDTOsAsync(It.IsAny<ProductFilterOptionsQuery>()))
                                  .ReturnsAsync(new PaginatedResult<ProductCardDTO>());

            // Act
            var result = await _productBrowserService.GetFilteredProductsDTO(filter);

            // Assert
            _mockProductRepository.Verify(r => r.GetProductCardDTOsAsync(It.Is<ProductFilterOptionsQuery>(q => q.CategoriesFilteredIds != null)), Times.Once);
        }

        [Test]
        public async Task GetFilteredProductsDTO_ShouldReturnProducts_WhenCategoryIsPresent()
        {
            // Arrange
            var filter = new ProductFilterOptionsRequest { CategoryIDFilter = 1 };
            var category = new Category();
            var categoriesTree = new List<Category> { category };
            var categoryIds = new List<int> { 1, 2, 3 };

            _mockCategoryRepository.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<Category, bool>>>(),
                It.IsAny<string?>(),
                It.IsAny<bool>()
            )).ReturnsAsync(category);

            _mockHierarchyRetriever.Setup(h => h.GetCategoryTreeAsync(category)).ReturnsAsync(categoriesTree);
            _mockCategoryIdRetriever.Setup(r => r.GetIdsOfCategories(categoriesTree)).Returns(categoryIds);
            _mockMapper.Setup(m => m.Map(filter, It.IsAny<ProductFilterOptionsQuery>()));

            _mockProductRepository.Setup(r => r.GetProductCardDTOsAsync(It.IsAny<ProductFilterOptionsQuery>()))
                                  .ReturnsAsync(new PaginatedResult<ProductCardDTO>());

            // Act
            var result = await _productBrowserService.GetFilteredProductsDTO(filter);

            // Assert
            _mockCategoryIdRetriever.Verify(r => r.GetIdsOfCategories(categoriesTree), Times.Once);
        }

        #endregion
    }
}
