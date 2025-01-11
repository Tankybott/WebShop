using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Models.DatabaseRelatedModels;
using Utility.DiscountQueues;

namespace Tests.Utility.DiscountQueues
{
    [TestFixture]
    public class ActivationDiscountQueueTests
    {
        private ActivationDiscountQueue _queue;

        [SetUp]
        public void Setup()
        {
            _queue = new ActivationDiscountQueue();
        }

        [Test]
        public async Task Items_ShouldBeSortedByStartTime()
        {
            // Arrange
            var discount1 = new Discount { Id = 1, StartTime = new DateTime(2024, 12, 25, 10, 0, 0) };
            var discount2 = new Discount { Id = 2, StartTime = new DateTime(2024, 12, 24, 10, 0, 0) }; // Earlier StartTime
            var discount3 = new Discount { Id = 3, StartTime = new DateTime(2024, 12, 26, 10, 0, 0) }; // Later StartTime

            // Act
            await _queue.EnqueueAsync(discount1);
            await _queue.EnqueueAsync(discount2);
            await _queue.EnqueueAsync(discount3);

            var result = await _queue.PeekAsync();

            // Assert
            Assert.That(result, Is.EqualTo(discount2)); // The earliest StartTime should be first
        }

        [Test]
        public async Task Items_WithSameStartTime_ShouldBeSortedById()
        {
            // Arrange
            var discount1 = new Discount { Id = 1, StartTime = new DateTime(2024, 12, 25, 10, 0, 0) };
            var discount2 = new Discount { Id = 2, StartTime = new DateTime(2024, 12, 25, 10, 0, 0) }; // Same StartTime as discount1

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
        public async Task Items_WithSameStartTimeAndDifferentId_ShouldAllBeInserted()
        {
            // Arrange
            var discount1 = new Discount { Id = 1, StartTime = new DateTime(2024, 12, 25, 10, 0, 0) };
            var discount2 = new Discount { Id = 2, StartTime = new DateTime(2024, 12, 25, 10, 0, 0) };
            var discount3 = new Discount { Id = 3, StartTime = new DateTime(2024, 12, 25, 10, 0, 0) };

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