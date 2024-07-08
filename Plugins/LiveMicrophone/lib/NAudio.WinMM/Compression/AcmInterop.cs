using System;
using System.Runtime.InteropServices;

namespace NAudio.Wave.Compression
{
    /// <summary>
    /// Interop definitions for Windows ACM (Audio Compression Manager) API
    /// </summary>
    class AcmInterop
    {
        // http://msdn.microsoft.com/en-us/library/dd742891%28VS.85%29.aspx
        public delegate bool AcmDriverEnumCallback(IntPtr hAcmDriverId, IntPtr instance, AcmDriverDetailsSupportFlags flags);

        public delegate bool AcmFormatEnumCallback(IntPtr hAcmDriverId, ref AcmFormatDetails formatDetails, IntPtr dwInstance, AcmDriverDetailsSupportFlags flags);

        public delegate bool AcmFormatTagEnumCallback(IntPtr hAcmDriverId, ref AcmFormatTagDetails formatTagDetails, IntPtr dwInstance, AcmDriverDetailsSupportFlags flags);

        /// <summary>
        /// http://msdn.microsoft.com/en-us/library/dd742910%28VS.85%29.aspx
        /// UINT ACMFORMATCHOOSEHOOKPROC acmFormatChooseHookProc(
        ///   HWND hwnd,     
        ///   UINT uMsg,     
        ///   WPARAM wParam, 
        ///   LPARAM lParam  
        /// </summary>        
        public delegate bool AcmFormatChooseHookProc(IntPtr windowHandle, int message, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("msacm32.dll")]
        public static extern MmResult acmDriverAdd(out IntPtr driverHandle,
            IntPtr driverModule,
            IntPtr driverFunctionAddress,
            int priority,
            AcmDriverAddFlags flags);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("msacm32.dll")]
        public static extern MmResult acmDriverRemove(IntPtr driverHandle,
            int removeFlags);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("Msacm32.dll")]
        public static extern MmResult acmDriverClose(IntPtr hAcmDriver, int closeFlags);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("Msacm32.dll")]
        public static extern MmResult acmDriverEnum(AcmDriverEnumCallback fnCallback, IntPtr dwInstance, AcmDriverEnumFlags flags);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("Msacm32.dll")]
        public static extern MmResult acmDriverDetails(IntPtr hAcmDriver, ref AcmDriverDetails driverDetails, int reserved);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("Msacm32.dll")]
        public static extern MmResult acmDriverOpen(out IntPtr pAcmDriver, IntPtr hAcmDriverId, int openFlags);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("Msacm32.dll", EntryPoint = "acmFormatChooseW")]
        public static extern MmResult acmFormatChoose(ref AcmFormatChoose formatChoose);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("Msacm32.dll")]
        public static extern MmResult acmFormatEnum(IntPtr hAcmDriver, ref AcmFormatDetails formatDetails, AcmFormatEnumCallback callback, IntPtr instance, AcmFormatEnumFlags flags);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("Msacm32.dll",EntryPoint="acmFormatSuggest")]
        public static extern MmResult acmFormatSuggest2(
            IntPtr hAcmDriver,
            IntPtr sourceFormatPointer,
            IntPtr destFormatPointer,
            int sizeDestFormat,
            AcmFormatSuggestFlags suggestFlags);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("Msacm32.dll")]
        public static extern MmResult acmFormatTagEnum(IntPtr hAcmDriver, ref AcmFormatTagDetails formatTagDetails, AcmFormatTagEnumCallback callback, IntPtr instance, int reserved);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("Msacm32.dll")]
        public static extern MmResult acmMetrics(IntPtr hAcmObject, AcmMetrics metric, out int output);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("Msacm32.dll",EntryPoint="acmStreamOpen")]
        public static extern MmResult acmStreamOpen2(
            out IntPtr hAcmStream,
            IntPtr hAcmDriver,
            IntPtr sourceFormatPointer,
            IntPtr destFormatPointer,
            [In] WaveFilter waveFilter,
            IntPtr callback,
            IntPtr instance,
            AcmStreamOpenFlags openFlags);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("Msacm32.dll")]
        public static extern MmResult acmStreamClose(IntPtr hAcmStream, int closeFlags);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("Msacm32.dll")]
        public static extern MmResult acmStreamConvert(IntPtr hAcmStream, [In, Out] AcmStreamHeaderStruct streamHeader, AcmStreamConvertFlags streamConvertFlags);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("Msacm32.dll")]
        public static extern MmResult acmStreamPrepareHeader(IntPtr hAcmStream, [In, Out] AcmStreamHeaderStruct streamHeader, int prepareFlags);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("Msacm32.dll")]
        public static extern MmResult acmStreamReset(IntPtr hAcmStream, int resetFlags);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("Msacm32.dll")]
        public static extern MmResult acmStreamSize(IntPtr hAcmStream, int inputBufferSize, out int outputBufferSize, AcmStreamSizeFlags flags);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("Msacm32.dll")]
        public static extern MmResult acmStreamUnprepareHeader(IntPtr hAcmStream, [In, Out] AcmStreamHeaderStruct streamHeader, int flags);
    }
}
