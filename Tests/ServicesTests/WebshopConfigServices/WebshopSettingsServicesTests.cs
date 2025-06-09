using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Models.DatabaseRelatedModels;
using Models.ViewModels;
using Moq;
using NUnit.Framework;


namespace Services.WebshopConfigServices.Tests
{
    [TestFixture]
    public class WebshopSettingsServicesTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IWebshopConfigRepository> _mockWebshopConfigRepository;
        private WebshopSettingsServices _settingsService;

        [SetUp]
        public void Setup()
        {
            _mockWebshopConfigRepository = new Mock<IWebshopConfigRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockUnitOfWork.Setup(u => u.WebshopConfig).Returns(_mockWebshopConfigRepository.Object);
            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

            _settingsService = new WebshopSettingsServices(_mockUnitOfWork.Object);
        }

        #region GetVmAsync

        [Test]
        public async Task GetVmAsync_ShouldReturnVmWithCurrencyAndSiteName_WhenCalled()
        {
            // Arrange
            var config = new WebshopConfig { Currency = "USD", SiteName = "Shop" };
            SetupMockForGetAsync(config);

            // Act
            var result = await _settingsService.GetVmAsync();

            // Assert
            Assert.That(result.Currency, Is.EqualTo("USD"));
        }

        [Test]
        public async Task GetVmAsync_ShouldIncludeAllCurrencies_WhenCalled()
        {
            // Arrange
            SetupMockForGetAsync(new WebshopConfig());

            // Act
            var result = await _settingsService.GetVmAsync();

            // Assert
            Assert.That(result.Currencies.Count(), Is.GreaterThan(0));
        }

        #endregion

        #region UpdateAsync

        [Test]
        public async Task UpdateAsync_ShouldUpdateSiteName_WhenCalled()
        {
            // Arrange
            var config = new WebshopConfig();
            SetupMockForGetAsync(config);

            var vm = new WebshopSettingsVm { SiteName = "UpdatedName", Currency = "EUR" };

            // Act
            await _settingsService.UpdateAsync(vm);

            // Assert
            Assert.That(config.SiteName, Is.EqualTo("UpdatedName"));
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateCurrency_WhenCalled()
        {
            // Arrange
            var config = new WebshopConfig();
            SetupMockForGetAsync(config);

            var vm = new WebshopSettingsVm { SiteName = "Any", Currency = "PLN" };

            // Act
            await _settingsService.UpdateAsync(vm);

            // Assert
            Assert.That(config.Currency, Is.EqualTo("PLN"));
        }

        [Test]
        public async Task UpdateAsync_ShouldCallUpdateAndSaveAsync_WhenCalled()
        {
            // Arrange
            var config = new WebshopConfig();
            SetupMockForGetAsync(config);

            var vm = new WebshopSettingsVm { SiteName = "x", Currency = "x" };

            // Act
            await _settingsService.UpdateAsync(vm);

            // Assert
            _mockWebshopConfigRepository.Verify(r => r.Update(config), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        #endregion

        #region Helpers

        private void SetupMockForGetAsync(WebshopConfig config)
        {
            _mockWebshopConfigRepository
                .Setup(r => r.GetAsync())
                .ReturnsAsync(config);
        }

        #endregion
    }
}
