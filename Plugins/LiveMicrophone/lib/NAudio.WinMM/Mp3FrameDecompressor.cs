using System;
using NAudio.Wave.Compression;

namespace NAudio.Wave
{
    /// <summary>
    /// MP3 Frame Decompressor using ACM
    /// </summary>
    public class AcmMp3FrameDecompressor : IMp3FrameDecompressor
    {
        private readonly AcmStream conversionStream;
        private readonly WaveFormat pcmFormat;
        private bool disposed;

        /// <summary>
        /// Creates a new ACM frame decompressor
        /// </summary>
        /// <param name="sourceFormat">The MP3 source format</param>
        public AcmMp3FrameDecompressor(WaveFormat sourceFormat)
        {
            this.pcmFormat = AcmStream.SuggestPcmFormat(sourceFormat);
            try
            {
                conversionStream = new AcmStream(sourceFormat, pcmFormat);
            }
            catch (Exception)
            {
                disposed = true;
                GC.SuppressFinalize(this);
                throw;
            }
        }

        /// <summary>
        /// Output format (PCM)
        /// </summary>
        public WaveFormat OutputFormat { get { return pcmFormat; } }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public int DecompressFrame(Mp3Frame frame, byte[] dest, int destOffset)
        {
            if (frame == null)
            {
                throw new ArgumentNullException("frame", "You must provide a non-null Mp3Frame to decompress");
            }
            Array.Copy(frame.RawData, conversionStream.SourceBuffer, frame.FrameLength);
            int converted = conversionStream.Convert(frame.FrameLength, out int sourceBytesConverted);
            if (sourceBytesConverted != frame.FrameLength)
            {
                throw new InvalidOperationException(String.Format("Couldn't convert the whole MP3 frame (converted {0}/{1})",
                    sourceBytesConverted, frame.FrameLength));
            }
            Array.Copy(conversionStream.DestBuffer, 0, dest, destOffset, converted);
            return converted;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void Reset()
        {
            conversionStream.Reposition();
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
				if(conversionStream != null)
					conversionStream.Dispose();
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Finalizer ensuring that resources get released properly
        /// </summary>
        ~AcmMp3FrameDecompressor()
        {
            System.Diagnostics.Debug.Assert(false, "AcmMp3FrameDecompressor Dispose was not called");
            Dispose();
        }
    }
}
