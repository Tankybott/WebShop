//using ControllersServices.ProductManagement;
//using DataAccess.Repository.IRepository;
//using Microsoft.AspNetCore.Http;
//using Moq;
//using NUnit.Framework;
//using Services.ProductService.Interfaces;
//using Utility.Common.Interfaces;

//[TestFixture]
//public class ProductPhotoServiceTests
//{
//    private Mock<IPathCreator> _mockPathCreator;
//    private Mock<IUnitOfWork> _mockUnitOfWork;
//    private Mock<IImageProcessor> _mockImageProcessor;
//    private Mock<IProductPhotoSetSynchronizer> _mockPhotoSetsRepairer;
//    private ProductPhotoService _productPhotoService;

//    [SetUp]
//    public void Setup()
//    {
//        _mockPathCreator = new Mock<IPathCreator>();


//        SetupMockForPathCreator();
//        //SetupMockForFileService();
//        _productPhotoService = new ProductPhotoService(_mockPathCreator.Object, _mockImageProcessor.Object, _mockUnitOfWork.Object, _mockPhotoSetsRepairer.Object);
//    }

//    private void SetupMockForPathCreator()
//    {
//        _mockPathCreator.Setup(p => p.GetRootPath()).Returns("C:/TestRootPath");
//        _mockPathCreator.Setup(p => p.CombinePaths(It.IsAny<string>(), It.IsAny<string>()))
//                        .Returns((string firstPath, string secondPath) => Path.Combine(firstPath, secondPath));
//    }

//    //private void SetupMockForFileService()
//    //{
//    //    _mockFileService.Setup(f => f.CreateFileAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<string>()))
//    //                    .Returns(Task.CompletedTask);
//    //    _mockFileService.Setup(f => f.DeleteFileAsync(It.IsAny<string>()))
//    //                    .Returns(Task.CompletedTask);
//    //}

//    #region AddPhotoAsync Tests

//    //[Test]
//    //public async Task AddPhotoAsync_ShouldCallGetRootPath()
//    //{
//    //    // Arrange
//    //    var photo = Mock.Of<IFormFile>();
//    //    var fileName = "test.jpg";
//    //    var imageDirectory = "images";

//    //    // Act
//    //    await _productPhotoService.AddPhotoAsync(photo, fileName, imageDirectory);

//    //    // Assert
//    //    _mockPathCreator.Verify(p => p.GetRootPath(), Times.Once);
//    //}

//    //[Test]
//    //public async Task AddPhotoAsync_ShouldCallCombinePathsWithCorrectArguments()
//    //{
//    //    // Arrange
//    //    var photo = Mock.Of<IFormFile>();
//    //    var fileName = "test.jpg";
//    //    var imageDirectory = "images";

//    //    // Act
//    //    await _productPhotoService.AddPhotoAsync(photo, fileName, imageDirectory);

//    //    // Assert
//    //    _mockPathCreator.Verify(p => p.CombinePaths("C:/TestRootPath", imageDirectory), Times.Once);
//    //}

//    //[Test]
//    //public async Task AddPhotoAsync_ShouldCallCreateFileAsyncWithCorrectArguments()
//    //{
//    //    // Arrange
//    //    var photo = Mock.Of<IFormFile>();
//    //    var fileName = "test.jpg";
//    //    var imageDirectory = "images";
//    //    var combinedPath = Path.Combine("C:/TestRootPath", imageDirectory);

//    //    _mockPathCreator.Setup(p => p.CombinePaths(It.IsAny<string>(), It.IsAny<string>())).Returns(combinedPath);

//    //    // Act
//    //    await _productPhotoService.AddPhotoAsync(photo, fileName, imageDirectory);

//    //    // Assert
//    //    _mockFileService.Verify(f => f.CreateFileAsync(photo, combinedPath, fileName), Times.Once);
//    //}

//    //#endregion
//    //#region DeletePhotoAsync Tests

//    //[Test]
//    //public async Task DeletePhotoAsync_ShouldCallGetRootPath()
//    //{
//    //    // Arrange
//    //    var photoUrl = "images/test.jpg";

//    //    // Act
//    //    await _productPhotoService.DeletePhotoAsync(photoUrl);

//    //    // Assert
//    //    _mockPathCreator.Verify(p => p.GetRootPath(), Times.Once);
//    //}

//    //[Test]
//    //public async Task DeletePhotoAsync_ShouldCallCombinePathsWithCorrectArguments()
//    //{
//    //    // Arrange
//    //    var photoUrl = "images/test.jpg";
//    //    var normalizedPhotoUrl = photoUrl.Replace('/', Path.DirectorySeparatorChar);

//    //    // Act
//    //    await _productPhotoService.DeletePhotoAsync(photoUrl);

//    //    // Assert
//    //    _mockPathCreator.Verify(p => p.CombinePaths("C:/TestRootPath", normalizedPhotoUrl.TrimStart(Path.DirectorySeparatorChar)), Times.Once);
//    //}

//    //[Test]
//    //public async Task DeletePhotoAsync_ShouldCallDeleteFileAsyncWithCorrectFullPath()
//    //{
//    //    // Arrange
//    //    var photoUrl = "images/test.jpg";
//    //    var normalizedPhotoUrl = photoUrl.Replace('/', Path.DirectorySeparatorChar);
//    //    var combinedPath = Path.Combine("C:/TestRootPath", normalizedPhotoUrl.TrimStart(Path.DirectorySeparatorChar));

//    //    _mockPathCreator.Setup(p => p.CombinePaths(It.IsAny<string>(), It.IsAny<string>())).Returns(combinedPath);

//    //    // Act
//    //    await _productPhotoService.DeletePhotoAsync(photoUrl);

//    //    // Assert
//    //    _mockFileService.Verify(f => f.DeleteFileAsync(combinedPath), Times.Once);
//    //}

//    #endregion
//}
