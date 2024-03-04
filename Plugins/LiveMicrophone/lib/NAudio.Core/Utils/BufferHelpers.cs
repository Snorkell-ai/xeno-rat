namespace NAudio.Utils
{
    /// <summary>
    /// Helper methods for working with audio buffers
    /// </summary>
    public static class BufferHelpers
    {

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static byte[] Ensure(byte[] buffer, int bytesRequired)
        {
            if (buffer == null || buffer.Length < bytesRequired)
            {
                buffer = new byte[bytesRequired];
            }
            return buffer;
        }

        /// <summary>
        /// Ensures the buffer is big enough
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="samplesRequired"></param>
        /// <returns></returns>
        public static float[] Ensure(float[] buffer, int samplesRequired)
        {
            if (buffer == null || buffer.Length < samplesRequired)
            {
                buffer = new float[samplesRequired];
            }
            return buffer;
        }
    }
}
