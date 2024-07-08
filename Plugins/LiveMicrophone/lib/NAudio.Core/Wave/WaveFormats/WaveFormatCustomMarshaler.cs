using System;
using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace NAudio.Wave
{
    /// <summary>
    /// Custom marshaller for WaveFormat structures
    /// </summary>
    public sealed class WaveFormatCustomMarshaler : ICustomMarshaler
    {
        private static WaveFormatCustomMarshaler marshaler = null;

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static ICustomMarshaler GetInstance(string cookie)
        {
            if (marshaler == null)
            {
                marshaler = new WaveFormatCustomMarshaler();
            }
            return marshaler;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void CleanUpManagedData(object ManagedObj)
        {
            
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void CleanUpNativeData(IntPtr pNativeData)
        {
            Marshal.FreeHGlobal(pNativeData);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public int GetNativeDataSize()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public IntPtr MarshalManagedToNative(object ManagedObj)
        {
            return WaveFormat.MarshalToPtr((WaveFormat)ManagedObj);            
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            return WaveFormat.MarshalFromPtr(pNativeData);
        }
    }
}
