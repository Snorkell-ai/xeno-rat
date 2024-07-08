using NAudio.Utils;

namespace NAudio.Wave.SampleProviders
{
    class StereoFloatSampleChunkConverter : ISampleChunkConverter
    {
        private int sourceSample;
        private byte[] sourceBuffer;
        private WaveBuffer sourceWaveBuffer;
        private int sourceSamples;

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public bool Supports(WaveFormat waveFormat)
        {
            return waveFormat.Encoding == WaveFormatEncoding.IeeeFloat &&
                waveFormat.Channels == 2;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void LoadNextChunk(IWaveProvider source, int samplePairsRequired)
        {
            int sourceBytesRequired = samplePairsRequired * 8;
            sourceBuffer = BufferHelpers.Ensure(sourceBuffer, sourceBytesRequired);
            sourceWaveBuffer = new WaveBuffer(sourceBuffer);
            sourceSamples = source.Read(sourceBuffer, 0, sourceBytesRequired) / 4;
            sourceSample = 0;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public bool GetNextSample(out float sampleLeft, out float sampleRight)
        {
            if (sourceSample < sourceSamples)
            {
                sampleLeft = sourceWaveBuffer.FloatBuffer[sourceSample++];
                sampleRight = sourceWaveBuffer.FloatBuffer[sourceSample++];
                return true;
            }
            else
            {
                sampleLeft = 0.0f;
                sampleRight = 0.0f;
                return false;
            }
        }
    }
}
