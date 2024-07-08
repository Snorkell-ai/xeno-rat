using NAudio.CoreAudioApi.Interfaces;
using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace NAudio.CoreAudioApi
{
    /// <summary>
    /// Manages the AudioStreamVolume for the <see cref="AudioClient"/>.
    /// </summary>
    public class AudioStreamVolume : IDisposable
    {
        IAudioStreamVolume audioStreamVolumeInterface;

        internal AudioStreamVolume(IAudioStreamVolume audioStreamVolumeInterface)
        {
            this.audioStreamVolumeInterface = audioStreamVolumeInterface;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        private void CheckChannelIndex(int channelIndex, string parameter)
        {
            int channelCount = ChannelCount;
            if (channelIndex >= channelCount)
            {
                throw new ArgumentOutOfRangeException(parameter, "You must supply a valid channel index < current count of channels: " + channelCount.ToString());
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public float[] GetAllVolumes()
        {
            Marshal.ThrowExceptionForHR(audioStreamVolumeInterface.GetChannelCount(out var channels));
            var levels = new float[channels];
            Marshal.ThrowExceptionForHR(audioStreamVolumeInterface.GetAllVolumes(channels, levels));
            return levels;
        }

        /// <summary>
        /// Returns the current number of channels in this audio stream.
        /// </summary>
        public int ChannelCount
        {
            get
            {
                Marshal.ThrowExceptionForHR(audioStreamVolumeInterface.GetChannelCount(out var channels));
                unchecked
                {
                    return (int)channels;
                }
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public float GetChannelVolume(int channelIndex)
        {
            CheckChannelIndex(channelIndex, "channelIndex");

            uint index;
            unchecked 
            {
                index = (uint)channelIndex;
            }
            Marshal.ThrowExceptionForHR(audioStreamVolumeInterface.GetChannelVolume(index, out var level));
            return level;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void SetAllVolumes(float[] levels)
        {
            // Make friendly Net exceptions for common problems:
            int channelCount = ChannelCount;
            if (levels == null)
            {
                throw new ArgumentNullException(nameof(levels));
            }
            if (levels.Length != channelCount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(levels),
                    String.Format(CultureInfo.InvariantCulture, "SetAllVolumes MUST be supplied with a volume level for ALL channels. The AudioStream has {0} channels and you supplied {1} channels.",
                                  channelCount, levels.Length));
            }
            for (int i = 0; i < levels.Length; i++)
            {
                float level = levels[i];
                if (level < 0.0f) throw new ArgumentOutOfRangeException(nameof(levels), "All volumes must be between 0.0 and 1.0. Invalid volume at index: " + i.ToString());
                if (level > 1.0f) throw new ArgumentOutOfRangeException(nameof(levels), "All volumes must be between 0.0 and 1.0. Invalid volume at index: " + i.ToString());
            }
            unchecked
            {
                Marshal.ThrowExceptionForHR(audioStreamVolumeInterface.SetAllVoumes((uint)channelCount, levels));
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void SetChannelVolume(int index, float level)
        {
            CheckChannelIndex(index, "index");

            if (level < 0.0f) throw new ArgumentOutOfRangeException(nameof(level), "Volume must be between 0.0 and 1.0");
            if (level > 1.0f) throw new ArgumentOutOfRangeException(nameof(level), "Volume must be between 0.0 and 1.0");
            unchecked
            {
                Marshal.ThrowExceptionForHR(audioStreamVolumeInterface.SetChannelVolume((uint)index, level));
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Release/cleanup objects during Dispose/finalization.
        /// </summary>
        /// <param name="disposing">True if disposing and false if being finalized.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (audioStreamVolumeInterface != null)
                {
                    // although GC would do this for us, we want it done now
                    Marshal.ReleaseComObject(audioStreamVolumeInterface);
                    audioStreamVolumeInterface = null;
                }
            }
        }

        #endregion
    }
}
