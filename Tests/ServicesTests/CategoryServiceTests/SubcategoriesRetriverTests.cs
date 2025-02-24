using NUnit.Framework;
using Moq;
using DataAccess.Repository.IRepository;
using Models;
using Services.CategoryService;
using Services.CategoryService.Interfaces;
using System.Linq.Expressions;


namespace Tests.Services.CategoryServiceTests
{
    [TestFixture]
    public class SubcategoriesRetriverTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ICategoryRepository> _mockCategoryRepository;
        private ISubcategoriesRetriver _subcategoriesRetriver;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCategoryRepository = new Mock<ICategoryRepository>();

            _mockUnitOfWork.Setup(u => u.Category).Returns(_mockCategoryRepository.Object);

            _subcategoriesRetriver = new SubcategoriesRetriver(_mockUnitOfWork.Object);
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

        [Test]
        public async Task GetSubcategoriesAsync_ShouldReturnAllCategories_WhenFilterIsAll()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category1" },
                new Category { Id = 2, Name = "Category2" }
            };
            SetupMockForGetAllAsync(categories);

            // Act
            var result = await _subcategoriesRetriver.GetSubcategoriesAsync("all");

            // Assert
            Assert.That(result, Is.EqualTo(categories));
        }

        [Test]
        public async Task GetSubcategoriesAsync_ShouldReturnRootCategories_WhenFilterIsEmpty()
        {
            // Arrange
            var rootCategories = new List<Category>
            {
                new Category { Id = 1, Name = "Root", ParentCategoryId = null },
                new Category { Id = 2, Name = "Root2", ParentCategoryId = 0 }
            };
            SetupMockForGetAllAsync(rootCategories);

            // Act
            var result = await _subcategoriesRetriver.GetSubcategoriesAsync("");

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(c => c.ParentCategoryId == null || c.ParentCategoryId == 0), Is.True);
        }

        [Test]
        public async Task GetSubcategoriesAsync_ShouldReturnFilteredCategories_WhenFilterIsSpecificParentId()
        {
            // Arrange
            var parentCategoryId = 1;
            var categories = new List<Category>
            {
                new Category { Id = 2, ParentCategoryId = parentCategoryId },
                new Category { Id = 3, ParentCategoryId = parentCategoryId }
            };
            SetupMockForGetAllAsync(categories);

            // Act
            var result = await _subcategoriesRetriver.GetSubcategoriesAsync(parentCategoryId.ToString());

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(c => c.ParentCategoryId == parentCategoryId), Is.True);
        }

        [Test]
        public async Task GetSubcategoriesAsync_ShouldReturnAllCategories_WhenFilterIsInvalid()
        {
            // Arrange
            var allCategories = new List<Category>
            {
                new Category { Id = 1, Name = "Category1" },
                new Category { Id = 2, Name = "Category2" }
            };
            SetupMockForGetAllAsync(allCategories);

            // Act
            var result = await _subcategoriesRetriver.GetSubcategoriesAsync("invalid_filter");

            // Assert
            Assert.That(result, Is.EqualTo(allCategories));
        }
    }
}
