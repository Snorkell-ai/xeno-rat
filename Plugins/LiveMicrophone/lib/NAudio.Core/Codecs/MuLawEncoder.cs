namespace NAudio.Codecs
{
    /// <summary>
    /// mu-law encoder
    /// based on code from:
    /// http://hazelware.luggle.com/tutorials/mulawcompression.html
    /// </summary>
    public static class MuLawEncoder
    {
        private const int cBias = 0x84;
        private const int cClip = 32635;

        private static readonly byte[] MuLawCompressTable = new byte[256] 
        {
             0,0,1,1,2,2,2,2,3,3,3,3,3,3,3,3,
             4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,
             5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,
             5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,
             6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,
             6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,
             6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,
             6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,
             7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
             7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
             7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
             7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
             7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
             7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
             7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
             7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7
        };

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static byte LinearToMuLawSample(short sample)
        {
            int sign = (sample >> 8) & 0x80;
            if (sign != 0)
                sample = (short)-sample;
            if (sample > cClip)
                sample = cClip;
            sample = (short)(sample + cBias);
            int exponent = (int)MuLawCompressTable[(sample >> 7) & 0xFF];
            int mantissa = (sample >> (exponent + 3)) & 0x0F;
            int compressedByte = ~(sign | (exponent << 4) | mantissa);

            return (byte)compressedByte;
        }
    }
}
