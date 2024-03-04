using System;
using System.Linq;
using System.Text;

namespace NAudio.Utils
{
    /// <summary>
    /// Chunk Identifier helpers
    /// </summary>
    public class ChunkIdentifier
    {

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static int ChunkIdentifierToInt32(string s)
        {
            if (s.Length != 4) throw new ArgumentException("Must be a four character string");
            var bytes = Encoding.UTF8.GetBytes(s);
            if (bytes.Length != 4) throw new ArgumentException("Must encode to exactly four bytes");
            return BitConverter.ToInt32(bytes, 0);
        }
    }
}
