// -----------------------------------------
// milligan22963 - implemented to work with nAudio
// 12/2014
// -----------------------------------------

using System;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.CoreAudioApi
{
    /// <summary>
    /// AudioSessionEvents callback implementation
    /// </summary>
    public class AudioSessionEventsCallback : IAudioSessionEvents
    {
        private readonly IAudioSessionEventsHandler audioSessionEventsHandler;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="handler"></param>
        public AudioSessionEventsCallback(IAudioSessionEventsHandler handler)
        {
            audioSessionEventsHandler = handler;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public int OnDisplayNameChanged(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string displayName,
            [In] ref Guid eventContext)
        {
            audioSessionEventsHandler.OnDisplayNameChanged(displayName);

            return 0;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public int OnIconPathChanged(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string iconPath,
            [In] ref Guid eventContext)
        {
            audioSessionEventsHandler.OnIconPathChanged(iconPath);

            return 0;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public int OnSimpleVolumeChanged(
            [In] [MarshalAs(UnmanagedType.R4)] float volume,
            [In] [MarshalAs(UnmanagedType.Bool)] bool isMuted,
            [In] ref Guid eventContext)
        {
            audioSessionEventsHandler.OnVolumeChanged(volume, isMuted);

            return 0;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public int OnChannelVolumeChanged(
            [In] [MarshalAs(UnmanagedType.U4)] UInt32 channelCount,
            [In] [MarshalAs(UnmanagedType.SysInt)] IntPtr newVolumes, // Pointer to float array
            [In] [MarshalAs(UnmanagedType.U4)] UInt32 channelIndex,
            [In] ref Guid eventContext)
        {
            audioSessionEventsHandler.OnChannelVolumeChanged(channelCount, newVolumes, channelIndex);

            return 0;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public int OnGroupingParamChanged(
            [In] ref Guid groupingId,
            [In] ref Guid eventContext)
        {
            audioSessionEventsHandler.OnGroupingParamChanged(ref groupingId);

            return 0;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public int OnStateChanged(
            [In] AudioSessionState state)
        {
            audioSessionEventsHandler.OnStateChanged(state);

            return 0;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public int OnSessionDisconnected(
            [In] AudioSessionDisconnectReason disconnectReason)
        {
            audioSessionEventsHandler.OnSessionDisconnected(disconnectReason);

            return 0;
        }
    }
}
