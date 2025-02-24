using NUnit.Framework;
using Moq;
using DataAccess.Repository.IRepository;
using Models;
using Models.ViewModels;
using Services.CategoryService;
using Services.CategoryService.Interfaces;
using ControllersServices.CategoryService.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System;

namespace Tests.Services.CategoryServiceTests
{
    [TestFixture]
    public class CategoryVmRetriverTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ICategoryRepository> _mockCategoryRepository;
        private Mock<ICategoryVMCreator> _mockCategoryVMCreator;
        private ICategoryVmRetriver _categoryVmRetriver;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockCategoryVMCreator = new Mock<ICategoryVMCreator>();

            _mockUnitOfWork.Setup(u => u.Category).Returns(_mockCategoryRepository.Object);

            _categoryVmRetriver = new CategoryVmRetriver(_mockUnitOfWork.Object, _mockCategoryVMCreator.Object);
        }

        private void SetupMockForGetAllAsync(IEnumerable<Category> categories)
        {
            _mockCategoryRepository
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<Category, bool>>>(),
                    It.IsAny<string?>(),
                    true,
                    It.IsAny<Expression<Func<Category, object>>>()))
                .ReturnsAsync(categories);
        }

        private void SetupMockForGetAsync(Category? result)
        {
            _mockCategoryRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Category, bool>>>(),
                    It.IsAny<string?>(), 
                    It.IsAny<bool>()   
                ))
                .ReturnsAsync(result);
        }


        [Test]
        public async Task GetVMAsync_ShouldReturnCategoryVMWithAllCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category1" },
                new Category { Id = 2, Name = "Category2" }
            };

            var expectedVM = new CategoryVM { AllCategories = categories };

            SetupMockForGetAllAsync(categories);
            _mockCategoryVMCreator.Setup(c => c.CreateCategoryVM(categories)).Returns(expectedVM);

            // Act
            var result = await _categoryVmRetriver.GetVMAsync();

            // Assert
            Assert.That(result, Is.EqualTo(expectedVM));
        }

        [Test]
        public async Task GetVMAsync_ShouldRetrieveAndSetCategory_WhenIdIsProvided()
        {
            // Arrange
            var categoryId = 1;
            var expectedCategory = new Category { Id = categoryId, Name = "Category1" };
            var categories = new List<Category> { expectedCategory, new Category { Id = 2, Name = "Category2" } };

            var expectedVM = new CategoryVM { AllCategories = categories, Category = expectedCategory };

            SetupMockForGetAllAsync(categories);
            SetupMockForGetAsync(expectedCategory);

            _mockCategoryVMCreator.Setup(c => c.CreateCategoryVM(categories)).Returns(expectedVM);

            // Act
            var result = await _categoryVmRetriver.GetVMAsync(categoryId);

            // Assert
            Assert.That(result.Category, Is.EqualTo(expectedCategory));
        }

        [Test]
        public async Task GetVMAsync_ShouldSetRootCategory_WhenBindedParentIdIsZero()
        {
            // Arrange
            var bindedParentId = 0;
            var categories = new List<Category> { new Category { Id = 1, Name = "Category1" } };

            var expectedVM = new CategoryVM { Category = new Category(), BindedParentName = "Root" };

            SetupMockForGetAllAsync(categories);
            _mockCategoryVMCreator.Setup(c => c.CreateCategoryVM(categories)).Returns(expectedVM);

            // Act
            var result = await _categoryVmRetriver.GetVMAsync(bindedParentId: bindedParentId);

            // Assert
            Assert.That(result.Category.ParentCategoryId, Is.Null);
            Assert.That(result.BindedParentName, Is.EqualTo("Root"));
        }

        [Test]
        public async Task GetVMAsync_ShouldSetBindedParentCategory_WhenBindedParentIdIsNotZero()
        {
            // Arrange
            var bindedParentId = 1;
            var parentCategory = new Category { Id = bindedParentId, Name = "Parent" };
            var categories = new List<Category> { parentCategory };

            var expectedVM = new CategoryVM { Category = new Category { ParentCategoryId = bindedParentId }, BindedParentName = "Parent" };

            SetupMockForGetAllAsync(categories);
            SetupMockForGetAsync(parentCategory);

            _mockCategoryVMCreator.Setup(c => c.CreateCategoryVM(categories)).Returns(expectedVM);

            // Act
            var result = await _categoryVmRetriver.GetVMAsync(bindedParentId: bindedParentId);

            // Assert
            Assert.That(result.Category.ParentCategoryId, Is.EqualTo(bindedParentId));
            Assert.That(result.BindedParentName, Is.EqualTo("Parent"));
        }
    }
}
