using NUnit.Framework;
using Models.DatabaseRelatedModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utility.Queues;

namespace Tests.Utility.DiscountQueues
{
    [TestFixture]
    public class DeletionDiscountQueueTests
    {
        private DeletionDiscountQueue _queue;

        [SetUp]
        public void Setup()
        {
            _queue = new DeletionDiscountQueue();
        }

        [Test]
        public async Task Items_ShouldBeSortedByEndTime()
        {
            // Arrange
            var discount1 = new Discount { Id = 1, EndTime = new DateTime(2024, 12, 25, 12, 0, 0) };
            var discount2 = new Discount { Id = 2, EndTime = new DateTime(2024, 12, 24, 12, 0, 0) }; // Earlier EndTime
            var discount3 = new Discount { Id = 3, EndTime = new DateTime(2024, 12, 26, 12, 0, 0) }; // Later EndTime

            // Act
            await _queue.EnqueueAsync(discount1);
            await _queue.EnqueueAsync(discount2);
            await _queue.EnqueueAsync(discount3);

            var result = await _queue.PeekAsync();

            // Assert
            Assert.That(result, Is.EqualTo(discount2)); // The earliest EndTime should be first
        }

        [Test]
        public async Task Items_WithSameEndTime_ShouldBeSortedById()
        {
            // Arrange
            var discount1 = new Discount { Id = 1, EndTime = new DateTime(2024, 12, 25, 12, 0, 0) };
            var discount2 = new Discount { Id = 2, EndTime = new DateTime(2024, 12, 25, 12, 0, 0) }; // Same EndTime as discount1

            // Act
            await _queue.EnqueueAsync(discount2);
            await _queue.EnqueueAsync(discount1);

            var result1 = await _queue.DequeueAsync(); // First item
            var result2 = await _queue.DequeueAsync(); // Second item

            // Assert
            Assert.That(result1, Is.EqualTo(discount1)); // Lower Id should come first
            Assert.That(result2, Is.EqualTo(discount2)); // Higher Id should come later
        }

        [Test]
        public async Task Items_WithSameEndTimeAndDifferentId_ShouldAllBeInserted()
        {
            // Arrange
            var discount1 = new Discount { Id = 1, EndTime = new DateTime(2024, 12, 25, 12, 0, 0) };
            var discount2 = new Discount { Id = 2, EndTime = new DateTime(2024, 12, 25, 12, 0, 0) };
            var discount3 = new Discount { Id = 3, EndTime = new DateTime(2024, 12, 25, 12, 0, 0) };

            // Act
            await _queue.EnqueueAsync(discount1);
            await _queue.EnqueueAsync(discount2);
            await _queue.EnqueueAsync(discount3);

            // Assert
            Assert.That(await _queue.DequeueAsync(), Is.EqualTo(discount1)); // Order by Id
            Assert.That(await _queue.DequeueAsync(), Is.EqualTo(discount2));
            Assert.That(await _queue.DequeueAsync(), Is.EqualTo(discount3));
        }
    }
}
