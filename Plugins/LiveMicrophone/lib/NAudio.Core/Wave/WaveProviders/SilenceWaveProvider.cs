using System;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace NAudio.Wave
{
    /// <summary>
    /// Silence producing wave provider
    /// Useful for playing silence when doing a WASAPI Loopback Capture
    /// </summary>
    public class SilenceProvider : IWaveProvider
    {
        /// <summary>
        /// Creates a new silence producing wave provider
        /// </summary>
        /// <param name="wf">Desired WaveFormat (should be PCM / IEE float</param>
        public SilenceProvider(WaveFormat wf) { WaveFormat = wf; }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public int Read(byte[] buffer, int offset, int count)
        {
            Array.Clear(buffer, offset, count);
            return count;
        }

        /// <summary>
        /// WaveFormat of this silence producing wave provider
        /// </summary>
        public WaveFormat WaveFormat { get; private set; }
    }
}
