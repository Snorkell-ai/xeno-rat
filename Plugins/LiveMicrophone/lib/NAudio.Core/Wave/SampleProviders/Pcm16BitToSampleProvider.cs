using System;

namespace NAudio.Wave.SampleProviders
{
    /// <summary>
    /// Converts an IWaveProvider containing 16 bit PCM to an
    /// ISampleProvider
    /// </summary>
    public class Pcm16BitToSampleProvider : SampleProviderConverterBase
    {
        /// <summary>
        /// Initialises a new instance of Pcm16BitToSampleProvider
        /// </summary>
        /// <param name="source">Source wave provider</param>
        public Pcm16BitToSampleProvider(IWaveProvider source)
            : base(source)
        {
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public override int Read(float[] buffer, int offset, int count)
        {
            int sourceBytesRequired = count * 2;
            EnsureSourceBuffer(sourceBytesRequired);
            int bytesRead = source.Read(sourceBuffer, 0, sourceBytesRequired);
            int outIndex = offset;
            for(int n = 0; n < bytesRead; n+=2)
            {
                buffer[outIndex++] = BitConverter.ToInt16(sourceBuffer, n) / 32768f;
            }
            return bytesRead / 2;
        }
    }
}
