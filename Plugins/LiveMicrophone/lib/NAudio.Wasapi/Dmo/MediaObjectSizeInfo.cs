using System;

namespace NAudio.Dmo
{
    /// <summary>
    /// Media Object Size Info
    /// </summary>
    public class MediaObjectSizeInfo
    {
        /// <summary>
        /// Minimum Buffer Size, in bytes
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Max Lookahead
        /// </summary>
        public int MaxLookahead { get; }

        /// <summary>
        /// Alignment
        /// </summary>
        public int Alignment { get; }

        /// <summary>
        /// Media Object Size Info
        /// </summary>
        public MediaObjectSizeInfo(int size, int maxLookahead, int alignment)
        {
            Size = size;
            MaxLookahead = maxLookahead;
            Alignment = alignment;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public override string ToString()
        {
            return $"Size: {Size}, Alignment {Alignment}, MaxLookahead {MaxLookahead}";
        }

    }
}
