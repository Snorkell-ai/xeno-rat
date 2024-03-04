using System;
using System.Runtime.InteropServices;

namespace NAudio.Dmo
{
    /// <summary>
    /// Media Object InPlace
    /// </summary>
    public class MediaObjectInPlace : IDisposable
    {
        private IMediaObjectInPlace mediaObjectInPlace;

        /// <summary>
        /// Creates a new Media Object InPlace
        /// </summary>
        /// <param name="mediaObjectInPlace">Media Object InPlace COM Interface</param>
        internal MediaObjectInPlace(IMediaObjectInPlace mediaObjectInPlace)
        {
            this.mediaObjectInPlace = mediaObjectInPlace;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public DmoInPlaceProcessReturn Process(int size, int offset, byte[] data, long timeStart, DmoInPlaceProcessFlags inPlaceFlag)
        {
            var pointer = Marshal.AllocHGlobal(size);
            Marshal.Copy(data, offset, pointer, size);

            var result = mediaObjectInPlace.Process(size, pointer, timeStart, inPlaceFlag);
            Marshal.ThrowExceptionForHR(result);

            Marshal.Copy(pointer, data, offset, size);
            Marshal.FreeHGlobal(pointer);

            return (DmoInPlaceProcessReturn) result;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public MediaObjectInPlace Clone()
        {
            Marshal.ThrowExceptionForHR(this.mediaObjectInPlace.Clone(out var cloneObj));
            return new MediaObjectInPlace(cloneObj);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public long GetLatency()
        {
            Marshal.ThrowExceptionForHR(this.mediaObjectInPlace.GetLatency(out var latencyTime));
            return latencyTime;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public MediaObject GetMediaObject()
        {
            return new MediaObject((IMediaObject) mediaObjectInPlace);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void Dispose()
        {
            if (mediaObjectInPlace != null)
            {
                Marshal.ReleaseComObject(mediaObjectInPlace);
                mediaObjectInPlace = null;
            }
        }
    }
}