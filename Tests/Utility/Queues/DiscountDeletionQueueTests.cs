using NUnit.Framework;
using Models.DatabaseRelatedModels;
using System;
using System.Collections.Generic;

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
        public void Items_ShouldBeSortedByEndTime()
        {
            // Arrange
            var discount1 = new Discount { Id = 1, EndTime = new DateTime(2024, 12, 25, 12, 0, 0) };
            var discount2 = new Discount { Id = 2, EndTime = new DateTime(2024, 12, 24, 12, 0, 0) }; // Earlier EndTime
            var discount3 = new Discount { Id = 3, EndTime = new DateTime(2024, 12, 26, 12, 0, 0) }; // Later EndTime

            // Act
            _queue.Enqueue(discount1);
            _queue.Enqueue(discount2);
            _queue.Enqueue(discount3);

            var result = _queue.Peek();

            // Assert
            Assert.That(result, Is.EqualTo(discount2)); // The earliest EndTime should be first
        }

        [Test]
        public void Items_WithSameEndTime_ShouldBeSortedById()
        {
            // Arrange
            var discount1 = new Discount { Id = 1, EndTime = new DateTime(2024, 12, 25, 12, 0, 0) };
            var discount2 = new Discount { Id = 2, EndTime = new DateTime(2024, 12, 25, 12, 0, 0) }; // Same EndTime as discount1

            // Act
            _queue.Enqueue(discount2);
            _queue.Enqueue(discount1);

            var result1 = _queue.Dequeue(); // First item
            var result2 = _queue.Dequeue(); // Second item

            // Assert
            Assert.That(result1, Is.EqualTo(discount1)); // Lower Id should come first
            Assert.That(result2, Is.EqualTo(discount2)); // Higher Id should come later
        }

        [Test]
        public void Items_WithSameEndTimeAndDifferentId_ShouldAllBeInserted()
        {
            // Arrange
            var discount1 = new Discount { Id = 1, EndTime = new DateTime(2024, 12, 25, 12, 0, 0) };
            var discount2 = new Discount { Id = 2, EndTime = new DateTime(2024, 12, 25, 12, 0, 0) };
            var discount3 = new Discount { Id = 3, EndTime = new DateTime(2024, 12, 25, 12, 0, 0) };

            // Act
            _queue.Enqueue(discount1);
            _queue.Enqueue(discount2);
            _queue.Enqueue(discount3);

            // Assert
            Assert.That(_queue.Dequeue(), Is.EqualTo(discount1)); // Order by Id
            Assert.That(_queue.Dequeue(), Is.EqualTo(discount2));
            Assert.That(_queue.Dequeue(), Is.EqualTo(discount3));
        }
    }
}