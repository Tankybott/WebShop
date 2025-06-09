using DataAccess.Repository.IRepository;
using Models;
using Models.DatabaseRelatedModels;
using Moq;
using NUnit.Framework;
using Services.OrderServices;
using System.Linq.Expressions;

[TestFixture]
public class OrderStockReducerTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IProductRepository> _mockProductRepo;
    private OrderStockReducer _reducer;
    private Product _product1;
    private Product _product2;

    [SetUp]
    public void Setup()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockProductRepo = new Mock<IProductRepository>();

        _mockUnitOfWork.Setup(u => u.Product).Returns(_mockProductRepo.Object);

        _reducer = new OrderStockReducer(_mockUnitOfWork.Object);
        _product1 = new Product { Id = 1, StockQuantity = 10 };
        _product2 = new Product { Id = 2, StockQuantity = 20 };
    }

    [Test]
    public async Task ReduceStockByOrderDetailsAsync_ShouldDecreaseStockOfEachProduct_WhenCalled()
    {
        var details = new List<OrderDetail>
        {
            new OrderDetail { ProductId = 1, Quantity = 3 },
            new OrderDetail { ProductId = 2, Quantity = 5 }
        };

        SetupGetAsyncForProduct(1, _product1);
        SetupGetAsyncForProduct(2, _product2);

        await _reducer.ReduceStockByOrderDetailsAsync(details);

        Assert.That(_product1.StockQuantity, Is.EqualTo(7));
    }

    [Test]
    public async Task ReduceStockByOrderDetailsAsync_ShouldDecreaseStockOfSecondProduct_WhenCalled()
    {
        var details = new List<OrderDetail>
        {
            new OrderDetail { ProductId = 1, Quantity = 3 },
            new OrderDetail { ProductId = 2, Quantity = 5 }
        };

        SetupGetAsyncForProduct(1, _product1);
        SetupGetAsyncForProduct(2, _product2);

        await _reducer.ReduceStockByOrderDetailsAsync(details);

        Assert.That(_product2.StockQuantity, Is.EqualTo(15));
    }

    [Test]
    public async Task ReduceStockByOrderDetailsAsync_ShouldCallUpdateRange_WhenCalled()
    {
        var details = new List<OrderDetail>
        {
            new OrderDetail { ProductId = 1, Quantity = 3 }
        };

        SetupGetAsyncForProduct(1, _product1);

        await _reducer.ReduceStockByOrderDetailsAsync(details);

        _mockProductRepo.Verify(r => r.UpdateRange(It.Is<IEnumerable<Product>>(list =>
            list.Count() == 1 && list.First().Id == 1 && list.First().StockQuantity == 7)), Times.Once);
    }

    [Test]
    public async Task ReduceStockByOrderDetailsAsync_ShouldCallSaveAsync_WhenCalled()
    {
        var details = new List<OrderDetail>
        {
            new OrderDetail { ProductId = 1, Quantity = 3 }
        };

        SetupGetAsyncForProduct(1, _product1);

        await _reducer.ReduceStockByOrderDetailsAsync(details);

        _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Test]
    public async Task ReduceStockByOrderDetailsAsync_ShouldSkipNullProduct_WhenProductNotFound()
    {
        var details = new List<OrderDetail>
        {
            new OrderDetail { ProductId = 999, Quantity = 1 }
        };

        SetupGetAsyncForProduct(999, null);

        await _reducer.ReduceStockByOrderDetailsAsync(details);

        _mockProductRepo.Verify(r => r.UpdateRange(It.IsAny<IEnumerable<Product>>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
    }

    private void SetupGetAsyncForProduct(int productId, Product? result)
    {
        _mockProductRepo
            .Setup(r => r.GetAsync(It.Is<Expression<Func<Product, bool>>>(expr => TestProductId(expr, productId)), null, false))
            .ReturnsAsync(result);
    }

    private bool TestProductId(Expression<Func<Product, bool>> expr, int expectedId)
    {
        var compiled = expr.Compile();
        return compiled(new Product { Id = expectedId });
    }
}
