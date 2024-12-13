namespace Tests.AdminCategoryTests
{
    using NUnit.Framework;
    using Moq;
    using ControllersServices.AdminCategoryService;
    using ControllersServices.AdminCategoryService.Interfaces;
    using DataAccess.Repository.IRepository;
    using Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Linq.Expressions;

    namespace Tests.AdminCategoryTests
    {


        [TestFixture]
        public class CategoryHierarchyRetriverTests
        {
            private Mock<IUnitOfWork> _mockUnitOfWork;
            private Mock<ICategoryRepository> _mockCategoryRepository;
            private CategoryHierarchyRetriver _categoryHierarchyRetriver;

            [SetUp]
            public void Setup()
            {
                _mockUnitOfWork = new Mock<IUnitOfWork>();

                _mockCategoryRepository = new Mock<ICategoryRepository>();

                _mockUnitOfWork.Setup(u => u.Category).Returns(_mockCategoryRepository.Object);

                _categoryHierarchyRetriver = new CategoryHierarchyRetriver(_mockUnitOfWork.Object);
            }

            private void SetupMockForGetAllAsync(IEnumerable<Category> categories)
            {
                _mockCategoryRepository.Setup(r => r.GetAllAsync(
                        It.IsAny<Expression<Func<Category, bool>>?>(),
                        It.IsAny<string?>(),
                        It.IsAny<bool>(),
                        It.IsAny<Expression<Func<Category, object>>?>()
                    ))
                    .ReturnsAsync(categories);
            }

            [Test]
            public async Task GetCollectionOfAllHigherLevelSubcategoriesAsync_ReturnsCorrectCount()
            {
                // Arrange
                var parentCategory = new Category { Id = 1, Name = "Parent" };
                var childCategory1 = new Category { Id = 2, Name = "Child1", ParentCategoryId = 1 };
                var childCategory2 = new Category { Id = 3, Name = "Child2", ParentCategoryId = 1 };
                var subChildCategory = new Category { Id = 4, Name = "SubChild", ParentCategoryId = 2 };

                var categories = new List<Category> { parentCategory, childCategory1, childCategory2, subChildCategory };
                SetupMockForGetAllAsync(categories);

                // Act
                var result = await _categoryHierarchyRetriver.GetCollectionOfAllHigherLevelSubcategoriesAsync(parentCategory);

                // Assert
                Assert.That(result.Count(), Is.EqualTo(3));
            }

            [Test]
            public async Task GetCollectionOfAllHigherLevelSubcategoriesAsync_ContainsChildCategory1()
            {
                // Arrange
                var parentCategory = new Category { Id = 1, Name = "Parent" };
                var childCategory1 = new Category { Id = 2, Name = "Child1", ParentCategoryId = 1 };

                var categories = new List<Category> { parentCategory, childCategory1 };
                SetupMockForGetAllAsync(categories);

                // Act
                var result = await _categoryHierarchyRetriver.GetCollectionOfAllHigherLevelSubcategoriesAsync(parentCategory);

                // Assert
                Assert.That(result.Contains(childCategory1), Is.True);
            }

            [Test]
            public async Task GetCollectionOfAllHigherLevelSubcategoriesAsync_ContainsChildCategory2()
            {
                // Arrange
                var parentCategory = new Category { Id = 1, Name = "Parent" };
                var childCategory2 = new Category { Id = 3, Name = "Child2", ParentCategoryId = 1 };

                var categories = new List<Category> { parentCategory, childCategory2 };
                SetupMockForGetAllAsync(categories);

                // Act
                var result = await _categoryHierarchyRetriver.GetCollectionOfAllHigherLevelSubcategoriesAsync(parentCategory);

                // Assert
                Assert.That(result.Contains(childCategory2), Is.True);
            }

            [Test]
            public async Task GetCollectionOfAllHigherLevelSubcategoriesAsync_ContainsSubChildCategory()
            {
                // Arrange
                var parentCategory = new Category { Id = 1, Name = "Parent" };
                var childCategory1 = new Category { Id = 2, Name = "Child1", ParentCategoryId = 1 };
                var subChildCategory = new Category { Id = 4, Name = "SubChild", ParentCategoryId = 2 };

                var categories = new List<Category> { parentCategory, childCategory1, subChildCategory };
                SetupMockForGetAllAsync(categories);

                // Act
                var result = await _categoryHierarchyRetriver.GetCollectionOfAllHigherLevelSubcategoriesAsync(parentCategory);

                // Assert
                Assert.That(result.Contains(subChildCategory), Is.True);
            }

            [Test]
            public async Task GetCollectionOfAllHigherLevelSubcategoriesAsync_ReturnsEmptyWhenNoSubcategories()
            {
                // Arrange
                var parentCategory = new Category { Id = 1, Name = "Parent" };
                var categories = new List<Category> { parentCategory };
                SetupMockForGetAllAsync(categories);

                // Act
                var result = await _categoryHierarchyRetriver.GetCollectionOfAllHigherLevelSubcategoriesAsync(parentCategory);

                // Assert
                Assert.That(result, Is.Empty);
            }
        }
    }
}
