using System;
using System.Diagnostics;

namespace NAudio.Utils
{
    /// <summary>
    /// A very basic circular buffer implementation
    /// </summary>
    public class CircularBuffer
    {
        private readonly byte[] buffer;
        private readonly object lockObject;
        private int writePosition;
        private int readPosition;
        private int byteCount;

        /// <summary>
        /// Create a new circular buffer
        /// </summary>
        /// <param name="size">Max buffer size in bytes</param>
        public CircularBuffer(int size)
        {
            buffer = new byte[size];
            lockObject = new object();
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public int Write(byte[] data, int offset, int count)
        {
            lock (lockObject)
            {
                var bytesWritten = 0;
                if (count > buffer.Length - byteCount)
                {
                    count = buffer.Length - byteCount;
                }
                // write to end
                int writeToEnd = Math.Min(buffer.Length - writePosition, count);
                Array.Copy(data, offset, buffer, writePosition, writeToEnd);
                writePosition += writeToEnd;
                writePosition %= buffer.Length;
                bytesWritten += writeToEnd;
                if (bytesWritten < count)
                {
                    Debug.Assert(writePosition == 0);
                    // must have wrapped round. Write to start
                    Array.Copy(data, offset + bytesWritten, buffer, writePosition, count - bytesWritten);
                    writePosition += (count - bytesWritten);
                    bytesWritten = count;
                }
                byteCount += bytesWritten;
                return bytesWritten;
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public int Read(byte[] data, int offset, int count)
        {
            lock (lockObject)
            {
                if (count > byteCount)
                {
                    count = byteCount;
                }
                int bytesRead = 0;
                int readToEnd = Math.Min(buffer.Length - readPosition, count);
                Array.Copy(buffer, readPosition, data, offset, readToEnd);
                bytesRead += readToEnd;
                readPosition += readToEnd;
                readPosition %= buffer.Length;

                if (bytesRead < count)
                {
                    // must have wrapped round. Read from start
                    Debug.Assert(readPosition == 0);
                    Array.Copy(buffer, readPosition, data, offset + bytesRead, count - bytesRead);
                    readPosition += (count - bytesRead);
                    bytesRead = count;
                }

                byteCount -= bytesRead;
                Debug.Assert(byteCount >= 0);
                return bytesRead;
            }
        }

        /// <summary>
        /// Maximum length of this circular buffer
        /// </summary>
        public int MaxLength => buffer.Length;

        /// <summary>
        /// Number of bytes currently stored in the circular buffer
        /// </summary>
        public int Count
        {
            get
            {
                lock (lockObject)
                {
                    return byteCount;
                }
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void Reset()
        {
            lock (lockObject)
            {
                ResetInner();
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        private void ResetInner()
        {
            byteCount = 0;
            readPosition = 0;
            writePosition = 0;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void Advance(int count)
        {
            lock (lockObject)
            {
                if (count >= byteCount)
                {
                    ResetInner();
                }
                else
                {
                    byteCount -= count;
                    readPosition += count;
                    readPosition %= MaxLength;
                }
            }
        }
    }
}
