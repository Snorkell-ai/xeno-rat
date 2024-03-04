using System;
using System.Collections.Generic;
using System.IO;
namespace NAudio.SoundFont
{

    /// <summary>
    /// base class for structures that can read themselves
    /// </summary>
    internal abstract class StructureBuilder<T>
    {
        protected List<T> data;

        public StructureBuilder()
        {
            Reset();
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public abstract T Read(BinaryReader br);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public abstract void Write(BinaryWriter bw, T o);
        public abstract int Length { get; }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void Reset()
        {
            data = new List<T>();
        }

        public T[] Data => data.ToArray();
    }

}