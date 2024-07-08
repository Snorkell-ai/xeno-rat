using System;
using System.IO;
using NAudio.MediaFoundation;

// ReSharper disable once CheckNamespace
namespace NAudio.Wave
{
    /// <summary>
    /// MediaFoundationReader supporting reading from a stream
    /// </summary>
    public class StreamMediaFoundationReader : MediaFoundationReader
    {
        private readonly Stream stream;

        /// <summary>
        /// Constructs a new media foundation reader from a stream
        /// </summary>
        public StreamMediaFoundationReader(Stream stream, MediaFoundationReaderSettings settings = null)
        {
            this.stream = stream;
            Init(settings);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        protected override IMFSourceReader CreateReader(MediaFoundationReaderSettings settings)
        {
            var ppSourceReader = MediaFoundationApi.CreateSourceReaderFromByteStream(MediaFoundationApi.CreateByteStream(new ComStream(stream)));

            ppSourceReader.SetStreamSelection(-2, false);
            ppSourceReader.SetStreamSelection(-3, true);
            ppSourceReader.SetCurrentMediaType(-3, IntPtr.Zero, new MediaType
            {
                MajorType = MediaTypes.MFMediaType_Audio,
                SubType = settings.RequestFloatOutput ? AudioSubtypes.MFAudioFormat_Float : AudioSubtypes.MFAudioFormat_PCM
            }.MediaFoundationObject);

            return ppSourceReader;
        }
    }
}
