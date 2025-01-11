using ControllersServices.CategoryService;
using DataAccess.Repository.IRepository;
using Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.ControllerServicesTests.CategoryTests
{
    [TestFixture]
    public class CategoryHierarchyCreatorTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ICategoryRepository> _mockCategoryRepository;
        private CategoryHierarchyCreator _categoryHierarchyCreator;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockUnitOfWork.Setup(u => u.Category).Returns(_mockCategoryRepository.Object);
            _categoryHierarchyCreator = new CategoryHierarchyCreator(_mockUnitOfWork.Object);
        }

        [Test]
        public async Task AddSubcategoryToCategoryAsync_ShouldAddSubcategoryToParentCategory()
        {
            // Arrange
            var parentCategory = new Category { Id = 1, Name = "Parent" };
            var childCategory = new Category { Id = 2, Name = "Child", ParentCategoryId = 1 };

            // Act
            await _categoryHierarchyCreator.AddSubcategoryToCategoryAsync(parentCategory, childCategory);

            // Assert
            Assert.That(parentCategory.SubCategories.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task AddSubcategoryToCategoryAsync_ShouldContainSubcategoryAfterAddition()
        {
            // Arrange
            var parentCategory = new Category { Id = 1, Name = "Parent" };
            var childCategory = new Category { Id = 2, Name = "Child", ParentCategoryId = 1 };

            // Act
            await _categoryHierarchyCreator.AddSubcategoryToCategoryAsync(parentCategory, childCategory);

            // Assert
            Assert.That(parentCategory.SubCategories.Contains(childCategory), Is.True);
        }

        [Test]
        public async Task AddSubcategoryToCategoryAsync_ShouldCallSaveAsyncOnce()
        {
            // Arrange
            var parentCategory = new Category { Id = 1, Name = "Parent" };
            var childCategory = new Category { Id = 2, Name = "Child", ParentCategoryId = 1 };

            // Mock the SaveAsync method to track its call
            _mockUnitOfWork.Setup(u => u.SaveAsync()).Verifiable();

            // Act
            await _categoryHierarchyCreator.AddSubcategoryToCategoryAsync(parentCategory, childCategory);

            // Assert
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task DeleteSubcategoryFromCategoryAsync_ShouldHaveZeroSubcategoriesAfterDeletingExistingSubcategory()
        {
            // Arrange
            var parentCategory = new Category { Id = 1, Name = "Parent" };
            var childCategory = new Category { Id = 2, Name = "Child", ParentCategoryId = 1 };
            parentCategory.SubCategories.Add(childCategory);

            // Act
            await _categoryHierarchyCreator.DeleteSubcategoryFromCategoryAsync(parentCategory, childCategory);

            // Assert
            Assert.That(parentCategory.SubCategories.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task DeleteSubcategoryFromCategoryAsync_ShouldNotContainSubcategoryAfterDeletion()
        {
            // Arrange
            var parentCategory = new Category { Id = 1, Name = "Parent" };
            var childCategory = new Category { Id = 2, Name = "Child", ParentCategoryId = 1 };
            parentCategory.SubCategories.Add(childCategory);

            // Act
            await _categoryHierarchyCreator.DeleteSubcategoryFromCategoryAsync(parentCategory, childCategory);

            // Assert
            Assert.That(parentCategory.SubCategories.Contains(childCategory), Is.False);
        }

        [Test]
        public async Task DeleteSubcategoryFromCategoryAsync_ShouldCallSaveAsyncOnceAfterDeletion()
        {
            // Arrange
            var parentCategory = new Category { Id = 1, Name = "Parent" };
            var childCategory = new Category { Id = 2, Name = "Child", ParentCategoryId = 1 };
            parentCategory.SubCategories.Add(childCategory);

            // Mock the SaveAsync method to track its call
            _mockUnitOfWork.Setup(u => u.SaveAsync()).Verifiable();

            // Act
            await _categoryHierarchyCreator.DeleteSubcategoryFromCategoryAsync(parentCategory, childCategory);

            // Assert
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}
