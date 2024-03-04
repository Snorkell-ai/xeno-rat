using System;

namespace NAudio.Wave
{
    /// <summary>
    /// Wave Callback Info
    /// </summary>
    public class WaveCallbackInfo
    {
        /// <summary>
        /// Callback Strategy
        /// </summary>
        public WaveCallbackStrategy Strategy { get; private set; }
        /// <summary>
        /// Window Handle (if applicable)
        /// </summary>
        public IntPtr Handle { get; private set; }

        private WaveWindow waveOutWindow;
        private WaveWindowNative waveOutWindowNative;

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static WaveCallbackInfo FunctionCallback()
        {
            return new WaveCallbackInfo(WaveCallbackStrategy.FunctionCallback, IntPtr.Zero);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static WaveCallbackInfo NewWindow()
        {
            return new WaveCallbackInfo(WaveCallbackStrategy.NewWindow, IntPtr.Zero);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static WaveCallbackInfo ExistingWindow(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
            {
                throw new ArgumentException("Handle cannot be zero");
            }
            return new WaveCallbackInfo(WaveCallbackStrategy.ExistingWindow, handle);
        }

        private WaveCallbackInfo(WaveCallbackStrategy strategy, IntPtr handle)
        {
            this.Strategy = strategy;
            this.Handle = handle;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        internal void Connect(WaveInterop.WaveCallback callback)
        {
            if (Strategy == WaveCallbackStrategy.NewWindow)
            {
                waveOutWindow = new WaveWindow(callback);
                waveOutWindow.CreateControl();
                this.Handle = waveOutWindow.Handle;
            }
            else if (Strategy == WaveCallbackStrategy.ExistingWindow)
            {
                waveOutWindowNative = new WaveWindowNative(callback);
                waveOutWindowNative.AssignHandle(this.Handle);
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        internal MmResult WaveOutOpen(out IntPtr waveOutHandle, int deviceNumber, WaveFormat waveFormat, WaveInterop.WaveCallback callback)
        {
            MmResult result;
            if (Strategy == WaveCallbackStrategy.FunctionCallback)
            {
                result = WaveInterop.waveOutOpen(out waveOutHandle, (IntPtr)deviceNumber, waveFormat, callback, IntPtr.Zero, WaveInterop.WaveInOutOpenFlags.CallbackFunction);
            }
            else
            {
                result = WaveInterop.waveOutOpenWindow(out waveOutHandle, (IntPtr)deviceNumber, waveFormat, this.Handle, IntPtr.Zero, WaveInterop.WaveInOutOpenFlags.CallbackWindow);
            }
            return result;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        internal MmResult WaveInOpen(out IntPtr waveInHandle, int deviceNumber, WaveFormat waveFormat, WaveInterop.WaveCallback callback)
        {
            MmResult result;
            if (Strategy == WaveCallbackStrategy.FunctionCallback)
            {
                result = WaveInterop.waveInOpen(out waveInHandle, (IntPtr)deviceNumber, waveFormat, callback, IntPtr.Zero, WaveInterop.WaveInOutOpenFlags.CallbackFunction);
            }
            else
            {
                result = WaveInterop.waveInOpenWindow(out waveInHandle, (IntPtr)deviceNumber, waveFormat, this.Handle, IntPtr.Zero, WaveInterop.WaveInOutOpenFlags.CallbackWindow);
            }
            return result;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        internal void Disconnect()
        {
            if (waveOutWindow != null)
            {
                waveOutWindow.Close();
                waveOutWindow = null;
            }
            if (waveOutWindowNative != null)
            {
                waveOutWindowNative.ReleaseHandle();
                waveOutWindowNative = null;
            }
        }
    }
}
