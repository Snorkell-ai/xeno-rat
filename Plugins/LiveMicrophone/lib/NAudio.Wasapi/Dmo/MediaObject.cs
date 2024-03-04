using System;
using System.Collections.Generic;
using NAudio.Utils;
using System.Runtime.InteropServices;
using NAudio.Wave;
using System.Diagnostics;

namespace NAudio.Dmo
{
    /// <summary>
    /// Media Object
    /// </summary>
    public class MediaObject : IDisposable
    {
        private IMediaObject mediaObject;
        private readonly int inputStreams;
        private readonly int outputStreams;

        #region Construction

        /// <summary>
        /// Creates a new Media Object
        /// </summary>
        /// <param name="mediaObject">Media Object COM interface</param>
        internal MediaObject(IMediaObject mediaObject)
        {
            this.mediaObject = mediaObject;
            mediaObject.GetStreamCount(out inputStreams, out outputStreams);
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Number of input streams
        /// </summary>
        public int InputStreamCount
        {
            get { return inputStreams; }
        }

        /// <summary>
        /// Number of output streams
        /// </summary>
        public int OutputStreamCount
        {
            get { return outputStreams; }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public DmoMediaType? GetInputType(int inputStream, int inputTypeIndex)
        {
            try
            {
                DmoMediaType mediaType;
                int hresult = mediaObject.GetInputType(inputStream, inputTypeIndex, out mediaType);
                if (hresult == HResult.S_OK)
                {
                    // this frees the format (if present)
                    // we should therefore come up with a way of marshaling the format
                    // into a completely managed structure
                    DmoInterop.MoFreeMediaType(ref mediaType);
                    return mediaType;
                }
            }
            catch (COMException e)
            {
                if (e.GetHResult() != (int)DmoHResults.DMO_E_NO_MORE_ITEMS)
                {
                    throw;
                }
            }
            return null;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public DmoMediaType? GetOutputType(int outputStream, int outputTypeIndex)
        {
            try
            {
                DmoMediaType mediaType;
                int hresult = mediaObject.GetOutputType(outputStream, outputTypeIndex, out mediaType);
                if (hresult == HResult.S_OK)
                {
                    // this frees the format (if present)
                    // we should therefore come up with a way of marshaling the format
                    // into a completely managed structure
                    DmoInterop.MoFreeMediaType(ref mediaType);
                    return mediaType;
                }
            }
            catch (COMException e)
            {
                if (e.GetHResult() != (int)DmoHResults.DMO_E_NO_MORE_ITEMS)
                {
                    throw;
                }
            }
            return null;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public DmoMediaType GetOutputCurrentType(int outputStreamIndex)
        {
            DmoMediaType mediaType;
            int hresult = mediaObject.GetOutputCurrentType(outputStreamIndex, out mediaType);
            if (hresult == HResult.S_OK)
            {
                // this frees the format (if present)
                // we should therefore come up with a way of marshaling the format
                // into a completely managed structure
                DmoInterop.MoFreeMediaType(ref mediaType);
                return mediaType;
            }
            else
            {
                if (hresult == (int)DmoHResults.DMO_E_TYPE_NOT_SET)
                {
                    throw new InvalidOperationException("Media type was not set.");
                }
                else
                {
                    throw Marshal.GetExceptionForHR(hresult);
                }
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public IEnumerable<DmoMediaType> GetInputTypes(int inputStreamIndex)
        {
            int typeIndex = 0;
            DmoMediaType? mediaType;
            while ((mediaType = GetInputType(inputStreamIndex,typeIndex)) != null)
            {
                yield return mediaType.Value;
                typeIndex++;
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public IEnumerable<DmoMediaType> GetOutputTypes(int outputStreamIndex)
        {
            int typeIndex = 0;
            DmoMediaType? mediaType;
            while ((mediaType = GetOutputType(outputStreamIndex, typeIndex)) != null)
            {
                yield return mediaType.Value;
                typeIndex++;
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public bool SupportsInputType(int inputStreamIndex, DmoMediaType mediaType)
        {
            return SetInputType(inputStreamIndex, mediaType, DmoSetTypeFlags.DMO_SET_TYPEF_TEST_ONLY);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        private bool SetInputType(int inputStreamIndex, DmoMediaType mediaType, DmoSetTypeFlags flags)
        {
            int hResult = mediaObject.SetInputType(inputStreamIndex, ref mediaType, flags);
            if (hResult != HResult.S_OK)
            {
                if (hResult == (int)DmoHResults.DMO_E_INVALIDSTREAMINDEX)
                {
                    throw new ArgumentException("Invalid stream index");
                }
                if (hResult == (int)DmoHResults.DMO_E_TYPE_NOT_ACCEPTED)
                {
                    Debug.WriteLine("Media type was not accepted");
                }

                return false;
            }
            return true;
        }

        /// <summary>
        /// Sets the input type
        /// </summary>
        /// <param name="inputStreamIndex">Input stream index</param>
        /// <param name="mediaType">Media Type</param>
        public void SetInputType(int inputStreamIndex, DmoMediaType mediaType)
        {
            if(!SetInputType(inputStreamIndex,mediaType,DmoSetTypeFlags.None))
            {
                throw new ArgumentException("Media Type not supported");
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void SetInputWaveFormat(int inputStreamIndex, WaveFormat waveFormat)
        {
            DmoMediaType mediaType = CreateDmoMediaTypeForWaveFormat(waveFormat);
            bool set = SetInputType(inputStreamIndex, mediaType, DmoSetTypeFlags.None);
            DmoInterop.MoFreeMediaType(ref mediaType);
            if (!set)
            {
                throw new ArgumentException("Media Type not supported");
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public bool SupportsInputWaveFormat(int inputStreamIndex, WaveFormat waveFormat)
        {
            DmoMediaType mediaType = CreateDmoMediaTypeForWaveFormat(waveFormat);
            bool supported = SetInputType(inputStreamIndex, mediaType, DmoSetTypeFlags.DMO_SET_TYPEF_TEST_ONLY);
            DmoInterop.MoFreeMediaType(ref mediaType);
            return supported;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        private DmoMediaType CreateDmoMediaTypeForWaveFormat(WaveFormat waveFormat)
        {
            DmoMediaType mediaType = new DmoMediaType();
            int waveFormatExSize = Marshal.SizeOf(waveFormat);  // 18 + waveFormat.ExtraSize;
            DmoInterop.MoInitMediaType(ref mediaType, waveFormatExSize);
            mediaType.SetWaveFormat(waveFormat);
            return mediaType;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public bool SupportsOutputType(int outputStreamIndex, DmoMediaType mediaType)
        {
            return SetOutputType(outputStreamIndex, mediaType, DmoSetTypeFlags.DMO_SET_TYPEF_TEST_ONLY);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public bool SupportsOutputWaveFormat(int outputStreamIndex, WaveFormat waveFormat)
        {
            DmoMediaType mediaType = CreateDmoMediaTypeForWaveFormat(waveFormat);
            bool supported = SetOutputType(outputStreamIndex, mediaType, DmoSetTypeFlags.DMO_SET_TYPEF_TEST_ONLY);
            DmoInterop.MoFreeMediaType(ref mediaType);
            return supported;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        private bool SetOutputType(int outputStreamIndex, DmoMediaType mediaType, DmoSetTypeFlags flags)
        {
            int hresult = mediaObject.SetOutputType(outputStreamIndex, ref mediaType, flags);
            if (hresult == (int)DmoHResults.DMO_E_TYPE_NOT_ACCEPTED)
            {
                return false;
            }
            else if (hresult == HResult.S_OK)
            {
                return true;
            }
            else
            {
                throw Marshal.GetExceptionForHR(hresult);
            }
        }

        /// <summary>
        /// Sets the output type
        /// n.b. may need to set the input type first
        /// </summary>
        /// <param name="outputStreamIndex">Output stream index</param>
        /// <param name="mediaType">Media type to set</param>
        public void SetOutputType(int outputStreamIndex, DmoMediaType mediaType)
        {
            if (!SetOutputType(outputStreamIndex, mediaType, DmoSetTypeFlags.None))
            {
                throw new ArgumentException("Media Type not supported");
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void SetOutputWaveFormat(int outputStreamIndex, WaveFormat waveFormat)
        {
            DmoMediaType mediaType = CreateDmoMediaTypeForWaveFormat(waveFormat);
            bool succeeded = SetOutputType(outputStreamIndex, mediaType, DmoSetTypeFlags.None);
            DmoInterop.MoFreeMediaType(ref mediaType);
            if (!succeeded)
            {
                throw new ArgumentException("Media Type not supported");
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public MediaObjectSizeInfo GetInputSizeInfo(int inputStreamIndex)
        {
            int size;
            int maxLookahead;
            int alignment;
            Marshal.ThrowExceptionForHR(mediaObject.GetInputSizeInfo(inputStreamIndex, out size, out maxLookahead, out alignment));
            return new MediaObjectSizeInfo(size, maxLookahead, alignment);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public MediaObjectSizeInfo GetOutputSizeInfo(int outputStreamIndex)
        {
            int size;
            int alignment;
            Marshal.ThrowExceptionForHR(mediaObject.GetOutputSizeInfo(outputStreamIndex, out size, out alignment));
            return new MediaObjectSizeInfo(size, 0, alignment);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void ProcessInput(int inputStreamIndex, IMediaBuffer mediaBuffer, DmoInputDataBufferFlags flags,
            long timestamp, long duration)
        {
            Marshal.ThrowExceptionForHR(mediaObject.ProcessInput(inputStreamIndex, mediaBuffer, flags, timestamp, duration));
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void ProcessOutput(DmoProcessOutputFlags flags, int outputBufferCount, DmoOutputDataBuffer[] outputBuffers)
        {
            int reserved;
            Marshal.ThrowExceptionForHR(mediaObject.ProcessOutput(flags, outputBufferCount, outputBuffers, out reserved));
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void AllocateStreamingResources()
        {
            Marshal.ThrowExceptionForHR(mediaObject.AllocateStreamingResources());
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void FreeStreamingResources()
        {
            Marshal.ThrowExceptionForHR(mediaObject.FreeStreamingResources());
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public long GetInputMaxLatency(int inputStreamIndex)
        {
            long maxLatency;
            Marshal.ThrowExceptionForHR(mediaObject.GetInputMaxLatency(inputStreamIndex, out maxLatency));
            return maxLatency;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void Flush()
        {
            Marshal.ThrowExceptionForHR(mediaObject.Flush());
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void Discontinuity(int inputStreamIndex)
        {
            Marshal.ThrowExceptionForHR(mediaObject.Discontinuity(inputStreamIndex));
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public bool IsAcceptingData(int inputStreamIndex)
        {
            DmoInputStatusFlags flags;
            int hresult = mediaObject.GetInputStatus(inputStreamIndex, out flags);
            Marshal.ThrowExceptionForHR(hresult);
            return (flags & DmoInputStatusFlags.DMO_INPUT_STATUSF_ACCEPT_DATA) == DmoInputStatusFlags.DMO_INPUT_STATUSF_ACCEPT_DATA;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void Dispose()
        {
            if (mediaObject != null)
            {
                Marshal.ReleaseComObject(mediaObject);
                mediaObject = null;
            }
        }

        #endregion
    }
}
