using NAudio.Utils;

namespace NAudio.Wave.SampleProviders
{
    /// <summary>
    /// Helper base class for classes converting to ISampleProvider
    /// </summary>
    public abstract class SampleProviderConverterBase : ISampleProvider
    {
        /// <summary>
        /// Source Wave Provider
        /// </summary>
        protected IWaveProvider source;
        private readonly WaveFormat waveFormat;

        /// <summary>
        /// Source buffer (to avoid constantly creating small buffers during playback)
        /// </summary>
        protected byte[] sourceBuffer;

        /// <summary>
        /// Initialises a new instance of SampleProviderConverterBase
        /// </summary>
        /// <param name="source">Source Wave provider</param>
        public SampleProviderConverterBase(IWaveProvider source)
        {
            this.source = source;
            waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(source.WaveFormat.SampleRate, source.WaveFormat.Channels);
        }

        /// <summary>
        /// Wave format of this wave provider
        /// </summary>
        public WaveFormat WaveFormat => waveFormat;

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public abstract int Read(float[] buffer, int offset, int count);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        protected void EnsureSourceBuffer(int sourceBytesRequired)
        {
            sourceBuffer = BufferHelpers.Ensure(sourceBuffer, sourceBytesRequired);
        }
    }
}
