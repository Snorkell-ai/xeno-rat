namespace NAudio.Wave.SampleProviders
{
    /// <summary>
    /// Converts an IWaveProvider containing 8 bit PCM to an
    /// ISampleProvider
    /// </summary>
    public class Pcm8BitToSampleProvider : SampleProviderConverterBase
    {
        /// <summary>
        /// Initialises a new instance of Pcm8BitToSampleProvider
        /// </summary>
        /// <param name="source">Source wave provider</param>
        public Pcm8BitToSampleProvider(IWaveProvider source) :
            base(source)
        {
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public override int Read(float[] buffer, int offset, int count)
        {
            int sourceBytesRequired = count;
            EnsureSourceBuffer(sourceBytesRequired);
            int bytesRead = source.Read(sourceBuffer, 0, sourceBytesRequired);
            int outIndex = offset;
            for (int n = 0; n < bytesRead; n++)
            {
                buffer[outIndex++] = sourceBuffer[n] / 128f - 1.0f;
            }
            return bytesRead;
        }
    }
}
