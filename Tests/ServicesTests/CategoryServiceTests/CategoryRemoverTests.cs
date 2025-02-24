using NUnit.Framework;
using Moq;
using DataAccess.Repository.IRepository;
using Models;
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
    public class CategoryRemoverTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ICategoryRepository> _mockCategoryRepository;
        private Mock<ICategoryHierarchyManager> _mockCategoryHierarchyManager;
        private Mock<ICategoryHierarchyRetriver> _mockCategoryHierarchyRetriver;
        private Mock<ICategoryReletedProductRemover> _mockProductRemover;
        private ICategoryRemover _categoryRemover;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockCategoryHierarchyManager = new Mock<ICategoryHierarchyManager>();
            _mockCategoryHierarchyRetriver = new Mock<ICategoryHierarchyRetriver>();
            _mockProductRemover = new Mock<ICategoryReletedProductRemover>();

            _mockUnitOfWork.Setup(u => u.Category).Returns(_mockCategoryRepository.Object);

            _categoryRemover = new CategoryRemover(
                _mockUnitOfWork.Object,
                _mockProductRemover.Object,
                _mockCategoryHierarchyManager.Object,
                _mockCategoryHierarchyRetriver.Object
            );
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
        public void DeleteAsync_ShouldThrowArgumentNullException_WhenIdIsNull()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _categoryRemover.DeleteAsync(null));
        }

        [Test]
        public async Task DeleteAsync_ShouldNotProceed_WhenCategoryIsNotFound()
        {
            // Arrange
            SetupMockForGetAsync(null);

            // Act
            await _categoryRemover.DeleteAsync(1);

            // Assert
            _mockCategoryHierarchyManager.Verify(m => m.DeleteSubcategoryFromCategoryAsync(It.IsAny<Category>(), It.IsAny<Category>()), Times.Never);
            _mockCategoryHierarchyRetriver.Verify(r => r.GetCategoryTreeAsync(It.IsAny<Category>()), Times.Never);
            _mockProductRemover.Verify(p => p.DeleteProductsOfCategoriesAsync(It.IsAny<IEnumerable<Category>>()), Times.Never);
            _mockCategoryRepository.Verify(r => r.RemoveRange(It.IsAny<IEnumerable<Category>>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Never);
        }

        [Test]
        public async Task DeleteAsync_ShouldCallDeleteSubcategoryFromCategory_WhenCategoryHasParent()
        {
            // Arrange
            var parentCategory = new Category { Id = 2 };
            var targetCategory = new Category { Id = 1, ParentCategoryId = 2, ParentCategory = parentCategory };

            SetupMockForGetAsync(targetCategory);

            _mockCategoryHierarchyRetriver
                .Setup(r => r.GetCategoryTreeAsync(targetCategory))
                .ReturnsAsync(new List<Category> { targetCategory });

            _mockProductRemover
                .Setup(p => p.DeleteProductsOfCategoriesAsync(It.IsAny<IEnumerable<Category>>()))
                .Returns(Task.CompletedTask);

            // Act
            await _categoryRemover.DeleteAsync(targetCategory.Id);

            // Assert
            _mockCategoryHierarchyManager.Verify(m => m.DeleteSubcategoryFromCategoryAsync(targetCategory, parentCategory), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_ShouldDeleteCategoryAndSubcategories_WhenCategoryExists()
        {
            // Arrange
            var targetCategory = new Category { Id = 1, ParentCategoryId = null };
            var subcategory1 = new Category { Id = 3, ParentCategoryId = targetCategory.Id };
            var subcategory2 = new Category { Id = 4, ParentCategoryId = targetCategory.Id };
            var categoriesToBeDeleted = new List<Category> { targetCategory, subcategory1, subcategory2 };

            SetupMockForGetAsync(targetCategory);
            _mockCategoryHierarchyRetriver.Setup(r => r.GetCategoryTreeAsync(targetCategory)).ReturnsAsync(categoriesToBeDeleted);
            _mockProductRemover.Setup(p => p.DeleteProductsOfCategoriesAsync(categoriesToBeDeleted)).Returns(Task.CompletedTask);

            // Act
            await _categoryRemover.DeleteAsync(targetCategory.Id);

            // Assert
            _mockCategoryRepository.Verify(r => r.RemoveRange(It.Is<List<Category>>(list =>
                list.Contains(targetCategory) &&
                list.Contains(subcategory1) &&
                list.Contains(subcategory2))), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_ShouldDeleteProductsBeforeRemovingCategories()
        {
            // Arrange
            var targetCategory = new Category { Id = 1 };
            var categoriesToBeDeleted = new List<Category> { targetCategory };

            SetupMockForGetAsync(targetCategory);
            _mockCategoryHierarchyRetriver.Setup(r => r.GetCategoryTreeAsync(targetCategory)).ReturnsAsync(categoriesToBeDeleted);
            _mockProductRemover.Setup(p => p.DeleteProductsOfCategoriesAsync(categoriesToBeDeleted)).Returns(Task.CompletedTask);

            // Act
            await _categoryRemover.DeleteAsync(targetCategory.Id);

            // Assert
            _mockProductRemover.Verify(p => p.DeleteProductsOfCategoriesAsync(categoriesToBeDeleted), Times.Once);
            _mockCategoryRepository.Verify(r => r.RemoveRange(categoriesToBeDeleted), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}
