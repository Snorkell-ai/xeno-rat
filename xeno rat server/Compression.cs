using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace xeno_rat_server
{
    class Compression
    {
        const ushort COMPRESSION_FORMAT_LZNT1 = 2;
        const ushort COMPRESSION_ENGINE_MAXIMUM = 0x100;

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("ntdll.dll")]
        private static extern uint RtlGetCompressionWorkSpaceSize(ushort CompressionFormat, out uint pNeededBufferSize, out uint Unknown);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("ntdll.dll")]
        private static extern uint RtlDecompressBuffer(ushort CompressionFormat, byte[] UncompressedBuffer, int UncompressedBufferSize, byte[] CompressedBuffer,
            int CompressedBufferSize, out int FinalUncompressedSize);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("ntdll.dll")]
        private static extern uint RtlCompressBuffer(ushort CompressionFormat, byte[] SourceBuffer, int SourceBufferLength, byte[] DestinationBuffer,
            int DestinationBufferLength, uint Unknown, out int pDestinationSize, IntPtr WorkspaceBuffer);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr LocalAlloc(int uFlags, IntPtr sizetdwBytes);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LocalFree(IntPtr hMem);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static byte[] Compress(byte[] buffer)
        {
            var outBuf = new byte[buffer.Length * 6];
            uint dwSize = 0, dwRet = 0;
            uint ret = RtlGetCompressionWorkSpaceSize(COMPRESSION_FORMAT_LZNT1 | COMPRESSION_ENGINE_MAXIMUM, out dwSize, out dwRet);
            if (ret != 0)
            {
                return null;
            }
            int dstSize = 0;
            IntPtr hWork = LocalAlloc(0, new IntPtr(dwSize));
            ret = RtlCompressBuffer(COMPRESSION_FORMAT_LZNT1 | COMPRESSION_ENGINE_MAXIMUM, buffer,
                buffer.Length, outBuf, outBuf.Length, 0, out dstSize, hWork);
            if (ret != 0)
            {
                return null;
            }
            LocalFree(hWork);
            Array.Resize(ref outBuf, dstSize);
            return outBuf;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static byte[] Decompress(byte[] buffer, int original_size)
        {
            int dwRet = 0;
            byte[] a = new byte[original_size];
            RtlDecompressBuffer(COMPRESSION_FORMAT_LZNT1, a, original_size, buffer, buffer.Length, out dwRet);
            return a;
        }
    }
}
