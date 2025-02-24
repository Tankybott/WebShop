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
    public class CategoryUpserterTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ICategoryRepository> _mockCategoryRepository;
        private Mock<ICategoryHierarchyManager> _mockCategoryHierarchyManager;
        private ICategoryUpserter _categoryUpserter;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockCategoryHierarchyManager = new Mock<ICategoryHierarchyManager>();

            _mockUnitOfWork.Setup(u => u.Category).Returns(_mockCategoryRepository.Object);

            _categoryUpserter = new CategoryUpserter(_mockUnitOfWork.Object, _mockCategoryHierarchyManager.Object);
        }

        private void SetupMockForGetAllAsync(IEnumerable<Category> categories)
        {
            _mockCategoryRepository
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<Category, bool>>>(),
                    It.IsAny<string?>(),
                    It.IsAny<bool>(),
                    It.IsAny<Expression<Func<Category, object>>>()))
                .ReturnsAsync(categories);
        }

        private void SetupMockForGetAsync(Category? result)
        {
            _mockCategoryRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Category, bool>>>(),
                    It.IsAny<string?>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(result);
        }

        [Test]
        public async Task UpsertAsync_ShouldAddNewCategory_WhenCategoryIdIsZero()
        {
            // Arrange
            var categoryVM = new CategoryVM
            {
                Category = new Category { Id = 0 },
                AllCategories = new List<Category>()
            };

            SetupMockForGetAllAsync(categoryVM.AllCategories);

            // Act
            await _categoryUpserter.UpsertAsync(categoryVM);

            // Assert
            _mockCategoryRepository.Verify(r => r.Add(categoryVM.Category), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task UpsertAsync_ShouldUpdateExistingCategory_WhenCategoryIdIsNotZero()
        {
            // Arrange
            var categoryVM = new CategoryVM
            {
                Category = new Category { Id = 1 },
                AllCategories = new List<Category>()
            };

            SetupMockForGetAllAsync(categoryVM.AllCategories);

            // Act
            await _categoryUpserter.UpsertAsync(categoryVM);

            // Assert
            _mockCategoryRepository.Verify(r => r.Update(categoryVM.Category), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task UpsertAsync_ShouldAddSubcategory_WhenCreatingNewCategoryWithParentCategory()
        {
            // Arrange
            var parentCategory = new Category { Id = 1 };
            var childCategory = new Category { Id = 0, ParentCategoryId = 1 }; // Id 0 means it's a new category
            var categoryVM = new CategoryVM { Category = childCategory };

            SetupMockForGetAllAsync(new List<Category>());
            SetupMockForGetAsync(parentCategory);

            // Act
            await _categoryUpserter.UpsertAsync(categoryVM);

            // Assert
            _mockCategoryHierarchyManager.Verify(c => c.AddSubcategoryToCategoryAsync(parentCategory, childCategory), Times.Once);
        }

        [Test]
        public async Task UpsertAsync_ShouldNotCallHierarchyManager_WhenNewCategoryHasNoParent()
        {
            // Arrange
            var newCategory = new Category { Id = 0, ParentCategoryId = null };
            var categoryVM = new CategoryVM { Category = newCategory };

            SetupMockForGetAllAsync(new List<Category>());

            // Act
            await _categoryUpserter.UpsertAsync(categoryVM);

            // Assert
            _mockCategoryHierarchyManager.Verify(c => c.AddSubcategoryToCategoryAsync(It.IsAny<Category>(), It.IsAny<Category>()), Times.Never);
        }

        [Test]
        public async Task UpsertAsync_ShouldNotCallHierarchyManager_WhenUpdatingCategory()
        {
            // Arrange
            var existingCategory = new Category { Id = 1, ParentCategoryId = 2 };
            var categoryVM = new CategoryVM { Category = existingCategory };

            SetupMockForGetAllAsync(new List<Category>());

            // Act
            await _categoryUpserter.UpsertAsync(categoryVM);

            // Assert
            _mockCategoryHierarchyManager.Verify(c => c.AddSubcategoryToCategoryAsync(It.IsAny<Category>(), It.IsAny<Category>()), Times.Never);
        }
    }
}
