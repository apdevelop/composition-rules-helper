namespace ScreenGrid.Models.Tests
{
    using System;

    using NUnit.Framework;

    [TestFixture]
    public class IntegerSegmentTests
    {
        [Test]
        public void IntegerSegmentsAreEqualTest()
        {
            var a = new IntegerSegment(1, 2);
            var b = new IntegerSegment(1, 2);

            Assert.IsTrue(a == b);
            Assert.AreEqual(a, b);
        }

        [Test]
        public void IntegerSegmentsAreNotEqualTest()
        {
            var a = new IntegerSegment(1, 2);
            var b = new IntegerSegment(3, 5);

            Assert.IsTrue(a != b);
            Assert.AreNotEqual(a, b);
        }

        [Test]
        public void FindZeroSegmentsTest1()
        {
            var array = new UInt32[] { 1, 0, 0, 0, 0, 0, 1, 2, 1, 0, 3, 4, 0, 3, 5 };
            var result = IntegerSegmentUtils.FindZeroSegments(array, 4);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].Start);
            Assert.AreEqual(5, result[0].End);
        }

        [Test]
        public void FindZeroSegmentsTestThreeSegments()
        {
            var array = new UInt32[] { 1, 0, 0, 0, 0, 0, 1, 2, 1, 0, 0, 3, 4, 0, 0, 3, 0, 5 };
            var result = IntegerSegmentUtils.FindZeroSegments(array, 2);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(1, result[0].Start);
            Assert.AreEqual(5, result[0].End);
            Assert.AreEqual(9, result[1].Start);
            Assert.AreEqual(10, result[1].End);
            Assert.AreEqual(13, result[2].Start);
            Assert.AreEqual(14, result[2].End);
        }

        [Test]
        public void FindZeroSegmentsTestNoSegments()
        {
            var array = new UInt32[] { 1, 0, 0, 1, 2, 3 };
            var result = IntegerSegmentUtils.FindZeroSegments(array, 3);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void FindZeroSegmentsTestShortAllZeroSegment()
        {
            var array = new UInt32[] { 0, 0, 0, 0, };
            var result = IntegerSegmentUtils.FindZeroSegments(array, 4);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(0, result[0].Start);
            Assert.AreEqual(3, result[0].End);
        }

        [Test]
        public void FindZeroSegmentsTestAllZeroSegment()
        {
            var array = new UInt32[] { 0, 0, 0, 0, 0, 0 };
            var result = IntegerSegmentUtils.FindZeroSegments(array, 3);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(0, result[0].Start);
            Assert.AreEqual(5, result[0].End);
        }

        [Test]
        public void FindZeroSegmentsTestSimpleSegment1()
        {
            var array = new UInt32[] { 1, 0, 0, 0, 0, 0 };
            var result = IntegerSegmentUtils.FindZeroSegments(array, 3);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].Start);
            Assert.AreEqual(5, result[0].End);
        }

        [Test]
        public void FindZeroSegmentsTestSimpleSegment2()
        {
            var array = new UInt32[] { 0, 0, 0, 0, 0, 1 };
            var result = IntegerSegmentUtils.FindZeroSegments(array, 3);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(0, result[0].Start);
            Assert.AreEqual(4, result[0].End);
        }

        [Test]
        public void IntersectionOfSegmentsFullIntersection()
        {
            var result = IntegerSegmentUtils.IntersectionOfSegments(
                new IntegerSegment(1, 2),
                new IntegerSegment(1, 2));
            Assert.AreEqual(new IntegerSegment(1, 2), result);
        }

        [Test]
        public void IntersectionOfSegmentsPartialIntersection()
        {
            var a = new IntegerSegment(1, 4);
            var b = new IntegerSegment(2, 8);

            Assert.AreEqual(new IntegerSegment(2, 4), IntegerSegmentUtils.IntersectionOfSegments(a, b));
            Assert.AreEqual(new IntegerSegment(2, 4), IntegerSegmentUtils.IntersectionOfSegments(b, a));
        }

        [Test]
        public void IntersectionOfSegmentsNoIntersection()
        {
            var result = IntegerSegmentUtils.IntersectionOfSegments(
                new IntegerSegment(1, 2),
                new IntegerSegment(4, 5));
            Assert.AreEqual(IntegerSegment.Zero, result);
        }

        [Test]
        public void IntersectionOfSegments()
        {
            var result = IntegerSegmentUtils.IntersectionOfSegments(new[]
                {
                    new[] { new IntegerSegment(1, 2), new IntegerSegment(4, 6) },
                    new[] { new IntegerSegment(1, 2), new IntegerSegment(5, 10) },
                });

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(new IntegerSegment(1, 2), result[0]);
            Assert.AreEqual(new IntegerSegment(5, 6), result[1]);
        }

    }
}
