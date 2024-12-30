using NUnit.Framework;
using ControllersServices.AdminCategoryService;
using Models;


namespace Tests.ControllersServices.AdminCategoryService
{
    [TestFixture]
    public class ProductVMCreatorTests
    {
        private ProductVMCreator _productVMCreator;

        [SetUp]
        public void Setup()
        {
            _productVMCreator = new ProductVMCreator();
        }

        #region CreateProductVM Tests

        [Test]
        public void CreateProductVM_ShouldPopulateCategoryListItems_WhenCategoriesAreProvided()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category1" },
                new Category { Id = 2, Name = "Category2" }
            };

            // Act
            var result = _productVMCreator.CreateProductVM(categories, null);

            // Assert
            Assert.That(result.CategoryListItems.Count(), Is.EqualTo(categories.Count));
            Assert.That(result.CategoryListItems.First().Text, Is.EqualTo("Category1"));
            Assert.That(result.CategoryListItems.First().Value, Is.EqualTo("1"));
        }

        [Test]
        public void CreateProductVM_ShouldPopulateProducts_WhenProductsAreProvided()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product1" },
                new Product { Id = 2, Name = "Product2" }
            };

            // Act
            var result = _productVMCreator.CreateProductVM(new List<Category>(), products);

            // Assert
            Assert.That(result.Products.Count, Is.EqualTo(products.Count));
            Assert.That(result.Products.First().Name, Is.EqualTo("Product1"));
        }

        [Test]
        public void CreateProductVM_ShouldCreateEmptyProductsList_WhenProductsAreNull()
        {
            // Arrange
            var categories = new List<Category>();

            // Act
            var result = _productVMCreator.CreateProductVM(categories, null);

            // Assert
            Assert.That(result.Products, Is.Empty);
        }

        [Test]
        public void CreateProductVM_ShouldInitializeProductProperty()
        {
            // Arrange
            var categories = new List<Category>();

            // Act
            var result = _productVMCreator.CreateProductVM(categories, null);

            // Assert
            Assert.That(result.Product, Is.Not.Null);
        }

        #endregion
    }
}