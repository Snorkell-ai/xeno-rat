using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace xeno_rat_client
{
    class Compression
    {
        const ushort COMPRESSION_FORMAT_LZNT1 = 2;
        const ushort COMPRESSION_ENGINE_MAXIMUM = 0x100;

        /// <summary>
        /// Retrieves the size of the workspace required for compression.
        /// </summary>
        /// <param name="CompressionFormat">The compression format to retrieve the workspace size for.</param>
        /// <param name="pNeededBufferSize">When this method returns, contains the size of the workspace required for compression.</param>
        /// <param name="Unknown">An unknown parameter.</param>
        /// <returns>The status of the method call.</returns>
        [DllImport("ntdll.dll")]
        private static extern uint RtlGetCompressionWorkSpaceSize(ushort CompressionFormat, out uint pNeededBufferSize, out uint Unknown);

        /// <summary>
        /// Decompresses a buffer using the specified compression format and returns the decompressed data.
        /// </summary>
        /// <param name="CompressionFormat">The compression format used for the input buffer.</param>
        /// <param name="UncompressedBuffer">The buffer containing the uncompressed data.</param>
        /// <param name="UncompressedBufferSize">The size of the uncompressed data buffer.</param>
        /// <param name="CompressedBuffer">The buffer containing the compressed data to be decompressed.</param>
        /// <param name="CompressedBufferSize">The size of the compressed data buffer.</param>
        /// <param name="FinalUncompressedSize">When this method returns, contains the size of the decompressed data.</param>
        /// <returns>The status of the decompression operation. Zero indicates success; otherwise, an error occurred.</returns>
        [DllImport("ntdll.dll")]
        private static extern uint RtlDecompressBuffer(ushort CompressionFormat, byte[] UncompressedBuffer, int UncompressedBufferSize, byte[] CompressedBuffer,
            int CompressedBufferSize, out int FinalUncompressedSize);

        /// <summary>
        /// Compresses the source buffer using the specified compression format and returns the compressed data in the destination buffer.
        /// </summary>
        /// <param name="CompressionFormat">The compression format to be used.</param>
        /// <param name="SourceBuffer">The buffer containing the data to be compressed.</param>
        /// <param name="SourceBufferLength">The length of the source buffer.</param>
        /// <param name="DestinationBuffer">The buffer to store the compressed data.</param>
        /// <param name="DestinationBufferLength">The length of the destination buffer.</param>
        /// <param name="Unknown">Unknown parameter.</param>
        /// <param name="pDestinationSize">The size of the compressed data in the destination buffer.</param>
        /// <param name="WorkspaceBuffer">The workspace buffer used for compression.</param>
        /// <returns>The status of the compression operation.</returns>
        [DllImport("ntdll.dll")]
        private static extern uint RtlCompressBuffer(ushort CompressionFormat, byte[] SourceBuffer, int SourceBufferLength, byte[] DestinationBuffer,
            int DestinationBufferLength, uint Unknown, out int pDestinationSize, IntPtr WorkspaceBuffer);

        /// <summary>
        /// Allocates the specified number of bytes in the local heap and returns a pointer to the allocated memory.
        /// </summary>
        /// <param name="uFlags">The memory allocation attributes. This parameter specifies the allocation and access protection attributes of the memory block. </param>
        /// <param name="sizetdwBytes">The size of the memory block, in bytes. If this parameter is zero, the LocalAlloc function allocates a zero-length item and returns a valid pointer.</param>
        /// <returns>A pointer to the allocated memory block if the function succeeds; otherwise, it returns NULL.</returns>
        /// <exception cref="System.ComponentModel.Win32Exception">Thrown when the function fails to allocate the requested memory.</exception>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr LocalAlloc(int uFlags, IntPtr sizetdwBytes);

        /// <summary>
        /// Frees the memory block allocated by LocalAlloc and LocalReAlloc and invalidates the handle.
        /// </summary>
        /// <param name="hMem">A handle to the local memory object. This handle is returned by either the LocalAlloc or LocalReAlloc function.</param>
        /// <returns>If the function succeeds, the return value is NULL. If the function fails, the return value is equal to a handle to the local memory object. To get extended error information, call GetLastError.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LocalFree(IntPtr hMem);

        /// <summary>
        /// Compresses the input buffer using the LZNT1 compression format and returns the compressed data.
        /// </summary>
        /// <param name="buffer">The input buffer to be compressed.</param>
        /// <returns>The compressed data in the LZNT1 format.</returns>
        /// <remarks>
        /// This method compresses the input buffer using the LZNT1 compression format and returns the compressed data.
        /// It first calculates the required workspace size for compression using RtlGetCompressionWorkSpaceSize function.
        /// Then, it compresses the input buffer using RtlCompressBuffer function and returns the compressed data.
        /// If any error occurs during compression, it returns null.
        /// </remarks>
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
            ret = RtlCompressBuffer(COMPRESSION_FORMAT_LZNT1 | COMPRESSION_ENGINE_MAXIMUM, buffer, buffer.Length, outBuf, outBuf.Length, 0, out dstSize, hWork);
            if (ret != 0)
            {
                LocalFree(hWork);
                return null;
            }
            LocalFree(hWork);
            Array.Resize(ref outBuf, dstSize);
            return outBuf;
        }

        /// <summary>
        /// Decompresses the input buffer using the LZNT1 compression format and returns the decompressed data.
        /// </summary>
        /// <param name="buffer">The compressed data buffer to be decompressed.</param>
        /// <param name="original_size">The size of the original data before compression.</param>
        /// <returns>The decompressed data as a byte array.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the input buffer is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the original size is less than or equal to 0.</exception>
        public static byte[] Decompress(byte[] buffer, int original_size)
        {
            int dwRet = 0;
            byte[] a = new byte[original_size];
            RtlDecompressBuffer(COMPRESSION_FORMAT_LZNT1, a, original_size, buffer, buffer.Length, out dwRet);
            return a;
        }
    }
}
