using System;
using NAudio.Wave.SampleProviders;

namespace NAudio.Wave
{
    /// <summary>
    /// Useful extension methods to make switching between WaveAndSampleProvider easier
    /// </summary>
    public static class WaveExtensionMethods
    {

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static ISampleProvider ToSampleProvider(this IWaveProvider waveProvider)
        {
            return SampleProviderConverters.ConvertWaveProviderIntoSampleProvider(waveProvider);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static void Init(this IWavePlayer wavePlayer, ISampleProvider sampleProvider, bool convertTo16Bit = false)
        {
            IWaveProvider provider = convertTo16Bit ? (IWaveProvider)new SampleToWaveProvider16(sampleProvider) : new SampleToWaveProvider(sampleProvider);
            wavePlayer.Init(provider);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static WaveFormat AsStandardWaveFormat(this WaveFormat waveFormat)
        {
            var wfe = waveFormat as WaveFormatExtensible;
            return wfe != null ? wfe.ToStandardWaveFormat() : waveFormat;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static IWaveProvider ToWaveProvider(this ISampleProvider sampleProvider)
        {
            return new SampleToWaveProvider(sampleProvider);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static IWaveProvider ToWaveProvider16(this ISampleProvider sampleProvider)
        {
            return new SampleToWaveProvider16(sampleProvider);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static ISampleProvider FollowedBy(this ISampleProvider sampleProvider, ISampleProvider next)
        {
            return new ConcatenatingSampleProvider(new[] { sampleProvider, next});
        }

        /// <summary>
        /// Concatenates one Sample Provider on the end of another with silence inserted
        /// </summary>
        /// <param name="sampleProvider">The sample provider to play first</param>
        /// <param name="silenceDuration">Silence duration to insert between the two</param>
        /// <param name="next">The sample provider to play next</param>
        /// <returns>A single sample provider</returns>
        public static ISampleProvider FollowedBy(this ISampleProvider sampleProvider, TimeSpan silenceDuration, ISampleProvider next)
        {
            var silenceAppended = new OffsetSampleProvider(sampleProvider) {LeadOut = silenceDuration};
            return new ConcatenatingSampleProvider(new[] { silenceAppended, next });
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static ISampleProvider Skip(this ISampleProvider sampleProvider, TimeSpan skipDuration)
        {
            return new OffsetSampleProvider(sampleProvider) { SkipOver = skipDuration};            
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static ISampleProvider Take(this ISampleProvider sampleProvider, TimeSpan takeDuration)
        {
            return new OffsetSampleProvider(sampleProvider) { Take = takeDuration };
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static ISampleProvider ToMono(this ISampleProvider sourceProvider, float leftVol = 0.5f, float rightVol = 0.5f)
        {
            if(sourceProvider.WaveFormat.Channels == 1) return sourceProvider;
            return new StereoToMonoSampleProvider(sourceProvider) {LeftVolume = leftVol, RightVolume = rightVol};
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static ISampleProvider ToStereo(this ISampleProvider sourceProvider, float leftVol = 1.0f, float rightVol = 1.0f)
        {
            if (sourceProvider.WaveFormat.Channels == 2) return sourceProvider;
            return new MonoToStereoSampleProvider(sourceProvider) { LeftVolume = leftVol, RightVolume = rightVol };
        }
    }
}
