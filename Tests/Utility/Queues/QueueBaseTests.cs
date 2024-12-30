using Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;


namespace Tests.Utility.DiscountQueues
{
    [TestFixture]
    public class QueueBaseTests
    {
        private class TestItem : IHasId
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class TestItemComparer : IComparer<TestItem>
        {
            public int Compare(TestItem x, TestItem y)
            {
                return x.Id.CompareTo(y.Id);
            }
        }

        private class TestQueue : QueueBase<TestItem>
        {
            public TestQueue() : base(new TestItemComparer()) { }
        }

        private TestQueue _queue;

        [SetUp]
        public void Setup()
        {
            _queue = new TestQueue();
        }

        #region Enqueue Tests

        [Test]
        public void Enqueue_ShouldAddItemToQueue_WhenQueueIsEmpty()
        {
            // Arrange
            var item = new TestItem { Id = 1, Name = "Item1" };

            // Act
            _queue.Enqueue(item);

            // Assert
            Assert.That(_queue.Peek(), Is.EqualTo(item));
        }

        [Test]
        public void Enqueue_ShouldMaintainOrder_WhenItemsAreAdded()
        {
            // Arrange
            var item1 = new TestItem { Id = 2, Name = "Item2" };
            var item2 = new TestItem { Id = 1, Name = "Item1" };

            // Act
            _queue.Enqueue(item1);
            _queue.Enqueue(item2);

            // Assert
            Assert.That(_queue.Peek(), Is.EqualTo(item2));
        }

        #endregion

        #region Peek Tests

        [Test]
        public void Peek_ShouldReturnFirstElement_WhenQueueHasItems()
        {
            // Arrange
            var item = new TestItem { Id = 1, Name = "Item1" };
            _queue.Enqueue(item);

            // Act
            var result = _queue.Peek();

            // Assert
            Assert.That(result, Is.EqualTo(item));
        }

        [Test]
        public void Peek_ShouldNotRemoveElementFromQueue_WhenQueueHasItems()
        {
            // Arrange
            var item = new TestItem { Id = 1, Name = "Item1" };
            _queue.Enqueue(item);

            // Act
            _queue.Peek();

            // Assert
            Assert.That(_queue.IsEmpty, Is.False); 
        }

        [Test]
        public void Peek_ShouldReturnNull_WhenQueueIsEmpty()
        {
            // Act
            var result = _queue.Peek();

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region Dequeue Tests

        [Test]
        public void Dequeue_ShouldReturnAndRemoveFirstElement_WhenQueueHasItems()
        {
            // Arrange
            var item = new TestItem { Id = 1, Name = "Item1" };
            _queue.Enqueue(item);

            // Act
            var result = _queue.Dequeue();

            // Assert
            Assert.That(result, Is.EqualTo(item));
        }

        [Test]
        public void Dequeue_ShouldMakeQueueEmpty_WhenQueueHasOnlyOneItem()
        {
            // Arrange
            var item = new TestItem { Id = 1, Name = "Item1" };
            _queue.Enqueue(item);

            // Act
            _queue.Dequeue();

            // Assert
            Assert.That(_queue.IsEmpty, Is.True);
        }

        [Test]
        public void Dequeue_ShouldReturnNull_WhenQueueIsEmpty()
        {
            // Act
            var result = _queue.Dequeue();

            // Assert
            Assert.That(result, Is.Null); 
        }

        #endregion

        #region IsEmpty Tests

        [Test]
        public void IsEmpty_ShouldReturnTrue_WhenQueueIsEmpty()
        {
            // Act
            var result = _queue.IsEmpty;

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsEmpty_ShouldReturnFalse_WhenQueueHasItems()
        {
            // Arrange
            var item = new TestItem { Id = 1, Name = "Item1" };
            _queue.Enqueue(item);

            // Act
            var result = _queue.IsEmpty;

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region RemoveById Tests

        [Test]
        public void RemoveById_ShouldReturnTrue_WhenItemWithIdExists()
        {
            // Arrange
            var item = new TestItem { Id = 1, Name = "Item1" };
            _queue.Enqueue(item);

            // Act
            var result = _queue.RemoveById(1);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void RemoveById_ShouldRemoveItem_WhenItemWithIdExists()
        {
            // Arrange
            var item = new TestItem { Id = 1, Name = "Item1" };
            _queue.Enqueue(item);

            // Act
            _queue.RemoveById(1);

            // Assert
            Assert.That(_queue.IsEmpty, Is.True); 
        }

        [Test]
        public void RemoveById_ShouldReturnFalse_WhenItemWithIdDoesNotExist()
        {
            // Arrange
            var item = new TestItem { Id = 1, Name = "Item1" };
            _queue.Enqueue(item);

            // Act
            var result = _queue.RemoveById(2);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void RemoveById_ShouldNotRemoveAnyItem_WhenItemWithIdDoesNotExist()
        {
            // Arrange
            var item = new TestItem { Id = 1, Name = "Item1" };
            _queue.Enqueue(item);

            // Act
            _queue.RemoveById(2);

            // Assert
            Assert.That(_queue.Peek(), Is.EqualTo(item)); 
        }

        #endregion
    }
}