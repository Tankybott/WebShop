using NUnit.Framework;
using Services.CategoryService.Interfaces;
using Models;
namespace Services.CategoryService.Tests
{
    [TestFixture]
    public class CategoryIdRetriverTests
    {
        private ICategoryIdRetriver _categoryIdRetriver;

        [SetUp]
        public void SetUp()
        {
            _categoryIdRetriver = new CategoryIdRetriver();
        }

        [Test]
        public void GetIdsOfCategories_ShouldReturnIds_WhenCategoriesHaveIds()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category1" },
                new Category { Id = 2, Name = "Category2" }
            };

            // Act
            var result = _categoryIdRetriver.GetIdsOfCategories(categories).ToList();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetIdsOfCategories_ShouldReturnCorrectFirstId_WhenCategoriesProvided()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category1" },
                new Category { Id = 2, Name = "Category2" }
            };

            // Act
            var result = _categoryIdRetriver.GetIdsOfCategories(categories).ToList();

            // Assert
            Assert.That(result[0], Is.EqualTo(1));
        }

        [Test]
        public void GetIdsOfCategories_ShouldReturnEmptyList_WhenNoCategoriesProvided()
        {
            // Arrange
            var categories = new List<Category>();

            // Act
            var result = _categoryIdRetriver.GetIdsOfCategories(categories);

            // Assert
            Assert.That(result.Any(), Is.False);
        }

        [Test]
        public void GetIdsOfCategories_ShouldThrowArgumentNullException_WhenCategoriesIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _categoryIdRetriver.GetIdsOfCategories(null));
        }
    }
}