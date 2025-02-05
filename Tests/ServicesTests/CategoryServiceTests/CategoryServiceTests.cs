﻿namespace Tests.ControllerServicesTests.CategoryTests
{
    using NUnit.Framework;
    using Moq;

    using DataAccess.Repository.IRepository;
    using Models;
    using Models.ViewModels;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Linq.Expressions;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System;
    using global::ControllersServices.CategoryService.Interfaces;
    using global::ControllersServices.CategoryService;

    [TestFixture]
    public class CategoryServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ICategoryRepository> _mockCategoryRepository;
        private Mock<ICategoryVMCreator> _mockCategoryVMCreator;
        private Mock<ICategoryHierarchyCreator> _mockCategoryHierarchyCreator;
        private Mock<ICategoryHierarchyRetriver> _mockCategoryHierarchyRetriver;
        private Mock<ICategoryReletedProductRemover> _mockProductRemover;
        private CategoryService _categoryService;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockCategoryVMCreator = new Mock<ICategoryVMCreator>();
            _mockCategoryHierarchyCreator = new Mock<ICategoryHierarchyCreator>();
            _mockCategoryHierarchyRetriver = new Mock<ICategoryHierarchyRetriver>();
            _mockProductRemover = new Mock<ICategoryReletedProductRemover>();

            _mockUnitOfWork.Setup(u => u.Category).Returns(_mockCategoryRepository.Object);

            _categoryService = new CategoryService(
                _mockCategoryHierarchyCreator.Object,
                _mockUnitOfWork.Object,
                _mockCategoryVMCreator.Object,
                _mockCategoryHierarchyRetriver.Object,
                _mockProductRemover.Object

            );
        }
        private void SetupMockForGetAllAsync(IEnumerable<Category> categories)
        {
            _mockCategoryRepository.Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<Category, bool>>>(),
                    It.IsAny<string?>(),
                    It.IsAny<bool>(),
                    It.IsAny<Expression<Func<Category, object>>>()
                ))
                .ReturnsAsync(categories);
        }

        private void SetupMockForGetAsync(Category? result)
        {
            _mockCategoryRepository.Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Category, bool>>>(),
                    It.IsAny<string?>(),
                    It.IsAny<bool>()
                ))
                .ReturnsAsync(result);
        }

        private CategoryVM createExpectedVM(IEnumerable<Category> categories)
        {
            return new CategoryVM()
            {
                CategoryListItems = categories.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
                .Prepend(new SelectListItem
                {
                    Text = "Root",
                    Value = ""
                }),
                Category = new Category(),
                AllCategories = categories
            };
        }

        #region GetCategoryVMAsync Tests

        [Test]
        public async Task GetCategoryVMAsync_ShouldReturnCorrectViewModel()
        {
            // Arrange
            var categories = new List<Category> { new Category { Id = 1, Name = "Category1" }, new Category { Id = 2, Name = "Category2" } };
            var expectedVM = createExpectedVM(categories);

            SetupMockForGetAllAsync(categories);
            _mockCategoryVMCreator.Setup(c => c.CreateCategoryVM(categories)).Returns(expectedVM);

            // Act
            var result = await _categoryService.GetCategoryVMAsync();

            // Assert
            Assert.That(result, Is.EqualTo(expectedVM));
        }

        [Test]
        public async Task GetCategoryVMAsync_ShouldRetrieveCategories_WhenIdIsProvided()
        {
            // Arrange
            var categoryId = 1;
            Category expectedCategory = new Category() { Id = categoryId, Name = "Category1" };
            var categories = new List<Category> { expectedCategory, new Category { Id = 2, Name = "Category2" } };
            var expectedVM = createExpectedVM(categories);

            SetupMockForGetAllAsync(categories);

            _mockCategoryVMCreator.Setup(c => c.CreateCategoryVM(categories)).Returns(expectedVM);
            SetupMockForGetAsync(expectedCategory);

            var testVM = _mockCategoryVMCreator.Object.CreateCategoryVM(categories);
            var testcategory = await _mockUnitOfWork.Object.Category.GetAsync(c => c.Id == categoryId);

            // Act
            var result = await _categoryService.GetCategoryVMAsync(categoryId);

            // Assert
            Assert.That(result.Category, Is.EqualTo(expectedCategory));
        }

        [Test]
        public async Task GetCategoryVMAsync_ShouldSetRootCategory_WhenBindedParentIdIsZero()
        {
            // Arrange
            var bindedParentId = 0;
            var categories = new List<Category>
                {
                    new Category { Id = 1, Name = "Category1", ParentCategoryId = null }
                };

            // Set up the mock to return the list of categories
            SetupMockForGetAllAsync(categories);

            // Add the setup for _mockCategoryVMCreator
            _mockCategoryVMCreator
                .Setup(c => c.CreateCategoryVM(It.IsAny<IEnumerable<Category>>()))
                .Returns(new CategoryVM
                {
                    Category = new Category { ParentCategoryId = null },
                    BindedParentName = "Root"
                });

            // Act
            var result = await _categoryService.GetCategoryVMAsync(bindedParentId: bindedParentId);

            // Assert
            Assert.That(result.Category.ParentCategoryId, Is.Null);  // ParentCategoryId should be null
            Assert.That(result.BindedParentName, Is.EqualTo("Root")); // BindedParentName should be "Root"
        }

        [Test]
        public async Task GetCategoryVMAsync_ShouldSetBindedParentCategory_WhenBindedParentIdIsNotZero()
        {
            // Arrange
            var bindedParentId = 1;
            var parentCategory = new Category
            {
                Id = bindedParentId,
                Name = "Parent",
                ParentCategoryId = 1
            };

            // Set up the mock for GetAsync to return the parent category
            SetupMockForGetAsync(parentCategory);

            // Ensure _mockCategoryVMCreator is not null and returns expected values
            _mockCategoryVMCreator
                .Setup(c => c.CreateCategoryVM(It.IsAny<IEnumerable<Category>>()))
                .Returns(new CategoryVM
                {
                    Category = new Category { ParentCategoryId = bindedParentId },
                    BindedParentName = parentCategory.Name
                });

            // Act
            var result = await _categoryService.GetCategoryVMAsync(null, bindedParentId);

            // Assert
            Assert.That(result, Is.Not.Null, "The result should not be null."); // New check to verify result is not null
            Assert.That(result.Category.ParentCategoryId, Is.EqualTo(bindedParentId)); // Ensure ParentCategoryId is set correctly
            Assert.That(result.BindedParentName, Is.EqualTo(parentCategory.Name)); // Ensure BindedParentName is set correctly
        }

        #endregion

        #region UpsertAsync Tests

        [Test]
        public async Task UpsertAsync_ShouldAddNewCategory_WhenCategoryIdIsZero()
        {
            // Arrange
            var categoryVM = new CategoryVM
            {
                Category = new Category { Id = 0 },
                AllCategories = new List<Category>()
            };

            // Act
            await _categoryService.UpsertAsync(categoryVM);

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

            // Act
            await _categoryService.UpsertAsync(categoryVM);

            // Assert
            _mockCategoryRepository.Verify(r => r.Update(categoryVM.Category), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task UpsertAsync_ShouldAddSubcategory_WhenCreatingNewCategoryParentCategoryIsProvided()
        {
            // Arrange
            var parentCategory = new Category { Id = 1 };
            var childCategory = new Category { Id = 0, ParentCategoryId = 1 }; // id 0 means that new category is created
            var categoryVM = new CategoryVM { Category = childCategory };
            SetupMockForGetAsync(parentCategory);

            // Act
            await _categoryService.UpsertAsync(categoryVM);

            // Assert
            _mockCategoryHierarchyCreator.Verify(c => c.AddSubcategoryToCategoryAsync(parentCategory, childCategory), Times.Once);
        }

        #endregion

        #region GetSubcategoriesOfCategoryAsync Tests

        [Test]
        public async Task GetSubcategoriesOfCateogryAsync_ShouldReturnAllCategories_WhenFilterIsAll()
        {
            // Arrange
            var categories = new List<Category> { new Category { Id = 1, Name = "Category1" } };
            SetupMockForGetAllAsync(categories);

            // Act
            var result = await _categoryService.GetSubcategoriesOfCateogryAsync("all");

            // Assert
            Assert.That(result, Is.EqualTo(categories));
        }

        [Test]
        public async Task GetSubcategoriesOfCateogryAsync_ShouldReturnRootCategories_WhenFilterIsEmpty()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Root", ParentCategoryId = null }
            };
            SetupMockForGetAllAsync(categories);

            // Act
            var result = await _categoryService.GetSubcategoriesOfCateogryAsync("");

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Root"));
        }

        [Test]
        public async Task GetSubcategoriesOfCateogryAsync_ShouldFilterByParentCategoryId()
        {
            // Arrange
            var parentCategoryId = 1;
            var categories = new List<Category>
            {
                new Category { Id = 2, ParentCategoryId = parentCategoryId }
            };
            SetupMockForGetAllAsync(categories);

            // Act
            var result = await _categoryService.GetSubcategoriesOfCateogryAsync(parentCategoryId.ToString());

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().ParentCategoryId, Is.EqualTo(parentCategoryId));
        }
        #endregion
        #region DeleteCategoryWithAllSubcategories Tests

        [Test]
        public void DeleteCategoryWithAllSubcategories_ShouldThrowArgumentNullException_WhenIdIsNull()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _categoryService.DeleteCategoryWithAllSubcategoriesAsync(null));
        }
        [Test]
        public async Task DeleteCategoryWithAllSubcategories_ShouldCallRemoveRangeWithParentCategoryAndSubcategories()
        {
            // Arrange
            var targetCategory = new Category { Id = 1, ParentCategoryId = 2 };
            var subcategory1 = new Category { Id = 3, ParentCategoryId = targetCategory.Id };  // Subcategory with targetCategory as parent
            var subcategory2 = new Category { Id = 4, ParentCategoryId = targetCategory.Id };  // Another subcategory with targetCategory as parent
            var categoreis = new List<Category> { targetCategory, subcategory1, subcategory2 };  // List of all categories (to delete)


            SetupMockForGetAsync(targetCategory);
            _mockCategoryHierarchyRetriver.Setup(r => r.GetCategoryTreeAsync(targetCategory))
                .ReturnsAsync(categoreis);

            _mockProductRemover
                .Setup(p => p.DeleteProductsOfCategoriesAsync(It.IsAny<List<Category>>()))
                .Returns(Task.CompletedTask)
                .Verifiable();


            // Act
            await _categoryService.DeleteCategoryWithAllSubcategoriesAsync(targetCategory.Id);

            // Assert
            _mockCategoryRepository.Verify(r => r.RemoveRange(It.Is<List<Category>>(list =>
                list.Contains(targetCategory) &&
                list.Contains(subcategory1) &&
                list.Contains(subcategory2))), Times.Once);


        }

        [Test]
        public async Task DeleteCategoryWithAllSubcategories_ShouldUpdateParentCategorySubcategories_WhenCategoryIsDeleted()
        {
            // Arrange
            var targetCategory = new Category { Id = 1, ParentCategoryId = 2 };
            var parentCategory = new Category { Id = 2, SubCategories = new List<Category> { targetCategory } };
            targetCategory.ParentCategory = parentCategory;
            var categoriesToBeDeleted = new List<Category> { targetCategory };

            SetupMockForGetAsync(targetCategory);

            _mockCategoryHierarchyRetriver.Setup(r => r.GetCategoryTreeAsync(targetCategory))
                .ReturnsAsync(categoriesToBeDeleted);

            _mockCategoryHierarchyCreator.Setup(c => c.DeleteSubcategoryFromCategoryAsync(targetCategory, targetCategory.ParentCategory))
                .Verifiable();

            _mockProductRemover
                .Setup(p => p.DeleteProductsOfCategoriesAsync(It.IsAny<List<Category>>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _categoryService.DeleteCategoryWithAllSubcategoriesAsync(targetCategory.Id);

            // Assert
            _mockCategoryHierarchyCreator.Verify(c => c.DeleteSubcategoryFromCategoryAsync(targetCategory, targetCategory.ParentCategory), Times.Once);
        }

        #endregion
    }
}
