using NUnit.Framework;
using Moq;
using Models;
using Models.ViewModels;
using ControllersServices.CategoryService;
using ControllersServices.CategoryService.Interfaces;
using Services.CategoryService.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.ControllersServices.CategoryServiceTests
{
    [TestFixture]
    public class CategoryServiceTests
    {
        private Mock<ICategoryVmRetriver> _mockVmRetriver;
        private Mock<ICategoryUpserter> _mockCategoryUpserter;
        private Mock<ISubcategoriesRetriver> _mockSubcategoriesRetriver;
        private Mock<ICategoryRemover> _mockCategoryRemover;
        private ICategoryService _categoryService;

        [SetUp]
        public void Setup()
        {
            _mockVmRetriver = new Mock<ICategoryVmRetriver>();
            _mockCategoryUpserter = new Mock<ICategoryUpserter>();
            _mockSubcategoriesRetriver = new Mock<ISubcategoriesRetriver>();
            _mockCategoryRemover = new Mock<ICategoryRemover>();

            _categoryService = new CategoryService(
                _mockVmRetriver.Object,
                _mockCategoryUpserter.Object,
                _mockSubcategoriesRetriver.Object,
                _mockCategoryRemover.Object
            );
        }

        [Test]
        public async Task GetCategoryVMAsync_ShouldCallGetVMAsync_WhenCalled()
        {
            // Arrange
            var expectedVM = new CategoryVM();
            _mockVmRetriver.Setup(v => v.GetVMAsync(It.IsAny<int?>(), It.IsAny<int?>()))
                .ReturnsAsync(expectedVM);

            // Act
            var result = await _categoryService.GetCategoryVMAsync(1, 2);

            // Assert
            _mockVmRetriver.Verify(v => v.GetVMAsync(1, 2), Times.Once);
            Assert.That(result, Is.EqualTo(expectedVM));
        }

        [Test]
        public async Task UpsertCategoryAsync_ShouldCallUpsertAsync_WhenCalled()
        {
            // Arrange
            var categoryVM = new CategoryVM();
            _mockCategoryUpserter.Setup(u => u.UpsertAsync(categoryVM))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _categoryService.UpsertCategoryAsync(categoryVM);

            // Assert
            _mockCategoryUpserter.Verify(u => u.UpsertAsync(categoryVM), Times.Once);
        }

        [Test]
        public async Task GetSubcategoriesOfCategoryAsync_ShouldCallGetSubcategoriesAsync_WhenCalled()
        {
            // Arrange
            var categories = new List<Category> { new Category { Id = 1 } };
            _mockSubcategoriesRetriver.Setup(s => s.GetSubcategoriesAsync("filter"))
                .ReturnsAsync(categories);

            // Act
            var result = await _categoryService.GetSubcategoriesOfCateogryAsync("filter");

            // Assert
            _mockSubcategoriesRetriver.Verify(s => s.GetSubcategoriesAsync("filter"), Times.Once);
            Assert.That(result, Is.EqualTo(categories));
        }

        [Test]
        public async Task DeleteCategoryWithAllSubcategoriesAsync_ShouldCallDeleteAsync_WhenCalled()
        {
            // Arrange
            _mockCategoryRemover.Setup(r => r.DeleteAsync(1))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _categoryService.DeleteCategoryWithAllSubcategoriesAsync(1);

            // Assert
            _mockCategoryRemover.Verify(r => r.DeleteAsync(1), Times.Once);
        }
    }
}
