using System;

namespace NAudio.Utils
{
    /// <summary>
    /// A util class for conversions
    /// </summary>
    public class Decibels
    {
        // 20 / ln( 10 )
        private const double LOG_2_DB = 8.6858896380650365530225783783321;

        // ln( 10 ) / 20
        private const double DB_2_LOG = 0.11512925464970228420089957273422;

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static double LinearToDecibels(double lin)
        {
            return Math.Log(lin) * LOG_2_DB;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static double DecibelsToLinear(double dB)
        {
            return Math.Exp(dB * DB_2_LOG);
        }

    }
}
