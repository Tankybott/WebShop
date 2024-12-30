using NUnit.Framework;
using ControllersServices.Utilities;
using Microsoft.AspNetCore.Hosting;
using Moq;

namespace Tests.ControllersServices.Utilities
{
    [TestFixture]
    public class PathCreatorTests
    {
        private Mock<IWebHostEnvironment> _mockWebHostEnvironment;
        private PathCreator _pathCreator;

        [SetUp]
        public void Setup()
        {
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _pathCreator = new PathCreator(_mockWebHostEnvironment.Object);
        }

        [Test]
        public void CombinePaths_ShouldReturnCombinedPath()
        {
            // Arrange
            var firstPath = "C:/TestRootPath";
            var secondPath = "Images/TestImage.jpg";

            // Act
            var combinedPath = _pathCreator.CombinePaths(firstPath, secondPath);

            // Assert
            Assert.That(combinedPath, Is.EqualTo(Path.Combine(firstPath, secondPath)));
        }
    }
}