using NAudio.Wasapi.CoreAudioApi.Interfaces;
using System;
using System.Runtime.InteropServices;

namespace NAudio.Wasapi.CoreAudioApi
{
    static class NativeMethods
    {

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("Mmdevapi.dll", ExactSpelling = true, PreserveSig = false)]
        public static extern void ActivateAudioInterfaceAsync(
            [In, MarshalAs(UnmanagedType.LPWStr)] string deviceInterfacePath,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [In] IntPtr activationParams, // n.b. is actually a pointer to a PropVariant, but we never need to pass anything but null
            [In] IActivateAudioInterfaceCompletionHandler completionHandler,
            out IActivateAudioInterfaceAsyncOperation activationOperation);
    }
}
