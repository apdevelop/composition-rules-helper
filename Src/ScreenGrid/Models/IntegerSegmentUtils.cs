using System;
using System.Collections.Generic;

namespace ScreenGrid.Models
{
    /// <summary>
    /// Extended operations on IntegerSegment
    /// </summary>
    public static class IntegerSegmentUtils
    {
        /// <summary>
        /// Search for all-zero segments of array with given minimal length
        /// </summary>
        /// <param name="array">Input array</param>
        /// <param name="minimalSegmentLength">Minimal length of segment</param>
        /// <returns>List of segments, which contains only zeros</returns>
        public static IList<IntegerSegment> FindZeroSegments(UInt32[] array, int minimalSegmentLength)
        {
            var result = new List<IntegerSegment>();

            if (array.Length < minimalSegmentLength)
            {
                return result;
            }

            int lastZero = -1;
            for (var i = 0; i < array.Length; i++)
            {
                if (array[i] == 0)
                {
                    if (lastZero == -1)
                    {
                        lastZero = i;
                    }

                    if (i == (array.Length - 1))
                    {
                        var segment = new IntegerSegment(lastZero, i);
                        if (CheckSegmentLength(segment, minimalSegmentLength))
                        {
                            result.Add(segment);
                        }
                    }
                }
                else
                {
                    if (lastZero != -1)
                    {
                        var segment = new IntegerSegment(lastZero, i - 1);
                        lastZero = -1;
                        if (CheckSegmentLength(segment, minimalSegmentLength))
                        {
                            result.Add(segment);
                        }
                    }
                }
            }

            return result;
        }

        private static bool CheckSegmentLength(IntegerSegment segment, int minimalSegmentLength)
        {
            return (segment.End - segment.Start + 1) >= minimalSegmentLength;
        }

        public static IntegerSegment IntersectionOfSegments(IntegerSegment segment1, IntegerSegment segment2)
        {
            if ((segment1.Start > segment2.End) || (segment1.End < segment2.Start))
            {
                return IntegerSegment.Zero;
            }
            else
            {
                var start = Math.Max(segment1.Start, segment2.Start);
                var end = Math.Min(segment1.End, segment2.End);
                return new IntegerSegment(start, end);
            }
        }

        public static IList<IntegerSegment> IntersectionOfSegments(IEnumerable<IList<IntegerSegment>> lines)
        {
            var sum = new List<IntegerSegment>();
            foreach (var line in lines)
            {
                if (sum.Count == 0)
                {
                    sum.AddRange(line);
                }
                else
                {
                    // Perform intersection checks
                    var newSum = new List<IntegerSegment>();
                    foreach (var segmentA in sum)
                    {
                        foreach (var segmentB in line)
                        {
                            var r = IntersectionOfSegments(segmentA, segmentB);
                            if (r != IntegerSegment.Zero)
                            {
                                newSum.Add(r);
                            }
                        }
                    }

                    sum = newSum;
                    if (sum.Count == 0)
                    {
                        break;
                    }
                }
            }

            return sum;
        }

        public static IntegerSegment SegmentsWithMaxDistance(IList<IntegerSegment> segments)
        {
            if (segments.Count < 2)
            {
                throw new ArgumentException();
            }

            var i1 = 0;
            var i2 = 0;
            var maxDistance = 0;
            for (var i = 0; i < segments.Count - 1; i++)
            {
                var distance = segments[i + 1].Start - segments[i].End;
                if (distance > maxDistance)
                {
                    i1 = segments[i].End;
                    i2 = segments[i + 1].Start;
                    maxDistance = distance;
                }
            }

            return new IntegerSegment(i1, i2);
        }
    }
}
