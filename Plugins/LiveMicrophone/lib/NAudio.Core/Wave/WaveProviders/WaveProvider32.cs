using System;
using System.Collections.Generic;
using System.Text;

namespace NAudio.Wave
{
    /// <summary>
    /// Base class for creating a 32 bit floating point wave provider
    /// Can also be used as a base class for an ISampleProvider that can 
    /// be plugged straight into anything requiring an IWaveProvider
    /// </summary>
    public abstract class WaveProvider32 : IWaveProvider, ISampleProvider
    {
        private WaveFormat waveFormat;

        /// <summary>
        /// Initializes a new instance of the WaveProvider32 class 
        /// defaulting to 44.1kHz mono
        /// </summary>
        public WaveProvider32()
            : this(44100, 1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the WaveProvider32 class with the specified
        /// sample rate and number of channels
        /// </summary>
        public WaveProvider32(int sampleRate, int channels)
        {
            SetWaveFormat(sampleRate, channels);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void SetWaveFormat(int sampleRate, int channels)
        {
            this.waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public int Read(byte[] buffer, int offset, int count)
        {
            WaveBuffer waveBuffer = new WaveBuffer(buffer);
            int samplesRequired = count / 4;
            int samplesRead = Read(waveBuffer.FloatBuffer, offset / 4, samplesRequired);
            return samplesRead * 4;
        }

        /// <summary>
        /// Method to override in derived classes
        /// Supply the requested number of samples into the buffer
        /// </summary>
        public abstract int Read(float[] buffer, int offset, int sampleCount);

        /// <summary>
        /// The Wave Format
        /// </summary>
        public WaveFormat WaveFormat
        {
            get { return waveFormat; }
        }
    }
}
