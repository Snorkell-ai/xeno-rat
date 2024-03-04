using System;
using NAudio.Wave;

namespace NAudio.Utils
{
    /// <summary>
    /// WavePosition extension methods
    /// </summary>
    public static class WavePositionExtensions
    {

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static TimeSpan GetPositionTimeSpan(this IWavePosition @this)
        {
            var pos = @this.GetPosition() / (@this.OutputWaveFormat.Channels * @this.OutputWaveFormat.BitsPerSample / 8);
            return TimeSpan.FromMilliseconds(pos * 1000.0 / @this.OutputWaveFormat.SampleRate);
        }
    }
}
