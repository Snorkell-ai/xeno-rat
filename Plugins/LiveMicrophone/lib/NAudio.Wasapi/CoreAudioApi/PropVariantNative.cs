using System;
using System.Runtime.InteropServices;

namespace NAudio.CoreAudioApi.Interfaces
{
    class PropVariantNative
    {

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("ole32.dll")]
#endif
        internal static extern int PropVariantClear(ref PropVariant pvar);

#if WINDOWS_UWP
        [DllImport("api-ms-win-core-com-l1-1-1.dll")]
#else
        [DllImport("ole32.dll")]
#endif
        internal static extern int PropVariantClear(IntPtr pvar);
    }
}
