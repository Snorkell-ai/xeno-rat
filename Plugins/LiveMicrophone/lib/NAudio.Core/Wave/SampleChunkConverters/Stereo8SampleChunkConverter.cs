using NAudio.Utils;

namespace NAudio.Wave.SampleProviders
{
    class Stereo8SampleChunkConverter : ISampleChunkConverter
    {
        private int offset;
        private byte[] sourceBuffer;
        private int sourceBytes;

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public bool Supports(WaveFormat waveFormat)
        {
            return waveFormat.Encoding == WaveFormatEncoding.Pcm &&
                waveFormat.BitsPerSample == 8 &&
                waveFormat.Channels == 2;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void LoadNextChunk(IWaveProvider source, int samplePairsRequired)
        {
            int sourceBytesRequired = samplePairsRequired * 2;
            sourceBuffer = BufferHelpers.Ensure(sourceBuffer, sourceBytesRequired);
            sourceBytes = source.Read(sourceBuffer, 0, sourceBytesRequired);
            offset = 0;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public bool GetNextSample(out float sampleLeft, out float sampleRight)
        {
            if (offset < sourceBytes)
            {
                sampleLeft = sourceBuffer[offset++] / 256f;
                sampleRight = sourceBuffer[offset++] / 256f;
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
