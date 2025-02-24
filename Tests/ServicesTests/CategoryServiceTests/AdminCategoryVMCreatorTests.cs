using ControllersServices.CategoryService;
using ControllersServices.CategoryService.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.ViewModels;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Tests.ControllerServicesTests.CategoryTests
{
    [TestFixture]
    public class AdminCategoryVMCreatorTests
    {
        private Mock<ICategoryHierarchyManager> _mockCategoryHierarchyCreator;
        private ICategoryVMCreator _adminCategoryVMCreator;

        [SetUp]
        public void Setup()
        {
            _mockCategoryHierarchyCreator = new Mock<ICategoryHierarchyManager>();
            _adminCategoryVMCreator = new CategoryVMCreator(_mockCategoryHierarchyCreator.Object);
        }

        [Test]
        public void CreateCategoryVM_ShouldReturnAdminCategoryVMWithCategoryListItems()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category1" },
                new Category { Id = 2, Name = "Category2" }
            };

            // Act
            var result = _adminCategoryVMCreator.CreateCategoryVM(categories);

            // Assert
            Assert.That(result.CategoryListItems.Count(), Is.EqualTo(3)); // Should include 2 categories + "Root" item
        }

        [Test]
        public void CreateCategoryVM_ShouldAddRootOptionAsFirstItem()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category1" }
            };

            // Act
            var result = _adminCategoryVMCreator.CreateCategoryVM(categories);

            // Assert
            Assert.That(result.CategoryListItems.First().Text, Is.EqualTo("Root")); // First item should be "Root"
        }

        [Test]
        public void CreateCategoryVM_ShouldContainCategoryNameInSelectListItem()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category1" }
            };

            // Act
            var result = _adminCategoryVMCreator.CreateCategoryVM(categories);

            // Assert
            Assert.That(result.CategoryListItems.ElementAt(1).Text, Is.EqualTo("Category1")); // Category name should be "Category1"
        }

        [Test]
        public void CreateCategoryVM_ShouldIncludeCategoryIdInSelectListItemValue()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category1" }
            };

            // Act
            var result = _adminCategoryVMCreator.CreateCategoryVM(categories);

            // Assert
            Assert.That(result.CategoryListItems.ElementAt(1).Value, Is.EqualTo("1")); // Category value should be "1"
        }

        [Test]
        public void CreateCategoryVM_ShouldReturnEmptyCategoryListItems_WhenNoCategoriesProvided()
        {
            // Arrange
            var categories = new List<Category>();

            // Act
            var result = _adminCategoryVMCreator.CreateCategoryVM(categories);

            // Assert
            Assert.That(result.CategoryListItems.Count(), Is.EqualTo(1)); // Should only include "Root"
        }

        [Test]
        public void CreateCategoryVM_ShouldIncludeAllCategoriesInCategoryListItems()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category1" },
                new Category { Id = 2, Name = "Category2" },
                new Category { Id = 3, Name = "Category3" }
            };

            // Act
            var result = _adminCategoryVMCreator.CreateCategoryVM(categories);

            // Assert
            Assert.That(result.CategoryListItems.Count(), Is.EqualTo(4)); // 3 categories + "Root"
        }

        [Test]
        public void CreateCategoryVM_ShouldCorrectlySetOtherProperties()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category1" }
            };

            // Act
            var result = _adminCategoryVMCreator.CreateCategoryVM(categories);

            // Assert
            Assert.That(result.AllCategories, Is.EqualTo(categories)); // AllCategories should be equal to the input categories
        }

        [Test]
        public void CreateCategoryVM_ShouldReturnNewCategoryForCategoryProperty()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category1" }
            };

            // Act
            var result = _adminCategoryVMCreator.CreateCategoryVM(categories);

            // Assert
            Assert.That(result.Category, Is.InstanceOf<Category>()); // Category should be a new Category instance
        }
    }
}
