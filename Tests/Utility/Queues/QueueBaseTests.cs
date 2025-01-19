
using Models.ContractInterfaces;
using NUnit.Framework;
using Utility.Queues;


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

        private class TestQueue : SortedQueue<TestItem>
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
        public async Task Enqueue_ShouldAddItemToQueue_WhenQueueIsEmpty()
        {
            // Arrange
            var item = new TestItem { Id = 1, Name = "Item1" };

            // Act
            await _queue.EnqueueAsync(item);

            // Assert
            Assert.That(await _queue.PeekAsync(), Is.EqualTo(item));
        }

        [Test]
        public async Task Enqueue_ShouldMaintainOrder_WhenItemsAreAdded()
        {
            // Arrange
            var item1 = new TestItem { Id = 2, Name = "Item2" };
            var item2 = new TestItem { Id = 1, Name = "Item1" };

            // Act
            await _queue.EnqueueAsync(item1);
            await _queue.EnqueueAsync(item2);

            // Assert
            Assert.That(await _queue.PeekAsync(), Is.EqualTo(item2));
        }

        #endregion

        #region Peek Tests

        [Test]
        public async Task Peek_ShouldReturnFirstElement_WhenQueueHasItems()
        {
            // Arrange
            var item = new TestItem { Id = 1, Name = "Item1" };
            await _queue.EnqueueAsync(item);

            // Act
            var result = await _queue.PeekAsync();

            // Assert
            Assert.That(result, Is.EqualTo(item));
        }

        [Test]
        public async Task Peek_ShouldNotRemoveElementFromQueue_WhenQueueHasItems()
        {
            // Arrange
            var item = new TestItem { Id = 1, Name = "Item1" };
            await _queue.EnqueueAsync(item);

            // Act
            await _queue.PeekAsync();

            // Assert
            Assert.That(_queue.IsEmpty(), Is.False);
        }
        

        [Test]
        public async Task Peek_ShouldReturnNull_WhenQueueIsEmpty()
        {
            // Act
            var result = await _queue.PeekAsync();

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region Dequeue Tests

        [Test]
        public async Task Dequeue_ShouldReturnAndRemoveFirstElement_WhenQueueHasItems()
        {
            // Arrange
            var item = new TestItem { Id = 1, Name = "Item1" };
            await _queue.EnqueueAsync(item);

            // Act
            var result = await _queue.DequeueAsync();

            // Assert
            Assert.That(result, Is.EqualTo(item));
        }

        [Test]
        public async Task Dequeue_ShouldMakeQueueEmpty_WhenQueueHasOnlyOneItem()
        {
            // Arrange
            var item = new TestItem { Id = 1, Name = "Item1" };
            await _queue.EnqueueAsync(item);

            // Act
            await _queue.DequeueAsync();

            // Assert
            Assert.That(_queue.IsEmpty(), Is.True);
        }

        [Test]
        public async Task Dequeue_ShouldReturnNull_WhenQueueIsEmpty()
        {
            // Act
            var result = await _queue.DequeueAsync();

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region IsEmpty Tests

        [Test]
        public async Task IsEmpty_ShouldReturnTrue_WhenQueueIsEmpty()
        {
            // Act
            var result = _queue.IsEmpty();

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task IsEmpty_ShouldReturnFalse_WhenQueueHasItems()
        {
            // Arrange
            var item = new TestItem { Id = 1, Name = "Item1" };
            await _queue.EnqueueAsync(item);

            // Act
            var result = _queue.IsEmpty();

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region RemoveById Tests

        [Test]
        public async Task RemoveById_ShouldReturnTrue_WhenItemWithIdExists()
        {
            // Arrange
            var item = new TestItem { Id = 1, Name = "Item1" };
            await _queue.EnqueueAsync(item);

            // Act
            var result = await _queue.RemoveByIdAsync(1);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task RemoveById_ShouldRemoveItem_WhenItemWithIdExists()
        {
            // Arrange
            var item = new TestItem { Id = 1, Name = "Item1" };
            await _queue.EnqueueAsync(item);

            // Act
            await _queue.RemoveByIdAsync(1);

            // Assert
            Assert.That(_queue.IsEmpty(), Is.True);
        }

        [Test]
        public async Task RemoveById_ShouldReturnFalse_WhenItemWithIdDoesNotExist()
        {
            // Arrange
            var item = new TestItem { Id = 1, Name = "Item1" };
            await _queue.EnqueueAsync(item);

            // Act
            var result = await _queue.RemoveByIdAsync(2);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task RemoveById_ShouldNotRemoveAnyItem_WhenItemWithIdDoesNotExist()
        {
            // Arrange
            var item = new TestItem { Id = 1, Name = "Item1" };
            await _queue.EnqueueAsync(item);

            // Act
            await _queue.RemoveByIdAsync(2);

            // Assert
            Assert.That(await _queue.PeekAsync(), Is.EqualTo(item));
        }

        #endregion
    }
}
