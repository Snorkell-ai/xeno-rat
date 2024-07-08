namespace NAudio.Wave
{
    /// <summary>
    /// Buffered WaveProvider taking source data from WaveIn
    /// </summary>
    public class WaveInProvider : IWaveProvider
    {
        private readonly IWaveIn waveIn;
        private readonly BufferedWaveProvider bufferedWaveProvider;

        /// <summary>
        /// Creates a new WaveInProvider
        /// n.b. Should make sure the WaveFormat is set correctly on IWaveIn before calling
        /// </summary>
        /// <param name="waveIn">The source of wave data</param>
        public WaveInProvider(IWaveIn waveIn)
        {
            this.waveIn = waveIn;
            waveIn.DataAvailable += OnDataAvailable;
            bufferedWaveProvider = new BufferedWaveProvider(WaveFormat);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            bufferedWaveProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public int Read(byte[] buffer, int offset, int count)
        {
            return bufferedWaveProvider.Read(buffer, offset, count);
        }

        /// <summary>
        /// The WaveFormat
        /// </summary>
        public WaveFormat WaveFormat => waveIn.WaveFormat;
    }
}
