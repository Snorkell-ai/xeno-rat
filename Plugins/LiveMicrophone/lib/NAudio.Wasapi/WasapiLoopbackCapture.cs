using System;
using NAudio.CoreAudioApi;

// ReSharper disable once CheckNamespace
namespace NAudio.Wave
{
    /// <summary>
    /// WASAPI Loopback Capture
    /// based on a contribution from "Pygmy" - http://naudio.codeplex.com/discussions/203605
    /// </summary>
    public class WasapiLoopbackCapture : WasapiCapture
    {
        /// <summary>
        /// Initialises a new instance of the WASAPI capture class
        /// </summary>
        public WasapiLoopbackCapture() :
            this(GetDefaultLoopbackCaptureDevice())
        {
        }

        /// <summary>
        /// Initialises a new instance of the WASAPI capture class
        /// </summary>
        /// <param name="captureDevice">Capture device to use</param>
        public WasapiLoopbackCapture(MMDevice captureDevice) :
            base(captureDevice)
        {
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static MMDevice GetDefaultLoopbackCaptureDevice()
        {
            MMDeviceEnumerator devices = new MMDeviceEnumerator();
            return devices.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        protected override AudioClientStreamFlags GetAudioClientStreamFlags()
        {
            return AudioClientStreamFlags.Loopback | base.GetAudioClientStreamFlags();
        }        
    }
}
