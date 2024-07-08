// created on 27/12/2002 at 20:20
using System;
using System.IO;

// ReSharper disable once CheckNamespace
namespace NAudio.Wave 
{
    /// <summary>
    /// Base class for all WaveStream classes. Derives from stream.
    /// </summary>
    public abstract class WaveStream : Stream, IWaveProvider
    {
        /// <summary>
        /// Retrieves the WaveFormat for this stream
        /// </summary>
        public abstract WaveFormat WaveFormat { get; }

        // base class includes long Position get; set
        // base class includes long Length get
        // base class includes Read
        // base class includes Dispose

        /// <summary>
        /// We can read from this stream
        /// </summary>
        public override bool CanRead => true;

        /// <summary>
        /// We can seek within this stream
        /// </summary>
        public override bool CanSeek => true;

        /// <summary>
        /// We can't write to this stream
        /// </summary>
        public override bool CanWrite => false;

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public override void Flush() { }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (origin == SeekOrigin.Begin)
                Position = offset;
            else if (origin == SeekOrigin.Current)
                Position += offset;
            else
                Position = Length + offset;
            return Position;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public override void SetLength(long length)
        {
            throw new NotSupportedException("Can't set length of a WaveFormatString");
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("Can't write to a WaveFormatString");
        }

        /// <summary>
        /// The block alignment for this wavestream. Do not modify the Position
        /// to anything that is not a whole multiple of this value
        /// </summary>
        public virtual int BlockAlign => WaveFormat.BlockAlign;

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void Skip(int seconds)
        {
            long newPosition = Position + WaveFormat.AverageBytesPerSecond*seconds;
            if (newPosition > Length)
                Position = Length;
            else if (newPosition < 0)
                Position = 0;
            else
                Position = newPosition;
        }

        /// <summary>
        /// The current position in the stream in Time format
        /// </summary>
        public virtual TimeSpan CurrentTime
        {
            get
            {
                return TimeSpan.FromSeconds((double)Position / WaveFormat.AverageBytesPerSecond);                
            }
            set
            {
                Position = (long) (value.TotalSeconds * WaveFormat.AverageBytesPerSecond);
            }
        }

        /// <summary>
        /// Total length in real-time of the stream (may be an estimate for compressed files)
        /// </summary>
        public virtual TimeSpan TotalTime
        {
            get
            {
                return TimeSpan.FromSeconds((double) Length / WaveFormat.AverageBytesPerSecond);
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public virtual bool HasData(int count)
        {
            return Position < Length;
        }
    }
}
