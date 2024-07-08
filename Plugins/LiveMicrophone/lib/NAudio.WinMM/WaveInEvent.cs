using System;
using System.Runtime.InteropServices;
using NAudio.Mixer;
using System.Threading;
using NAudio.CoreAudioApi;

// ReSharper disable once CheckNamespace
namespace NAudio.Wave
{
    /// <summary>
    /// Recording using waveIn api with event callbacks.
    /// Use this for recording in non-gui applications
    /// Events are raised as recorded buffers are made available
    /// </summary>
    public class WaveInEvent : IWaveIn
    {
        private readonly AutoResetEvent callbackEvent;
        private readonly SynchronizationContext syncContext;
        private IntPtr waveInHandle;
        private volatile CaptureState captureState;
        private WaveInBuffer[] buffers;

        /// <summary>
        /// Indicates recorded data is available 
        /// </summary>
        public event EventHandler<WaveInEventArgs> DataAvailable;

        /// <summary>
        /// Indicates that all recorded data has now been received.
        /// </summary>
        public event EventHandler<StoppedEventArgs> RecordingStopped;

        /// <summary>
        /// Prepares a Wave input device for recording
        /// </summary>
        public WaveInEvent()
        {
            callbackEvent = new AutoResetEvent(false);
            syncContext = SynchronizationContext.Current;
            DeviceNumber = 0;
            WaveFormat = new WaveFormat(8000, 16, 1);
            BufferMilliseconds = 100;
            NumberOfBuffers = 3;
            captureState = CaptureState.Stopped;
        }

        /// <summary>
        /// Returns the number of Wave In devices available in the system
        /// </summary>
        public static int DeviceCount => WaveInterop.waveInGetNumDevs();

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static WaveInCapabilities GetCapabilities(int devNumber)
        {
            WaveInCapabilities caps = new WaveInCapabilities();
            int structSize = Marshal.SizeOf(caps);
            MmException.Try(WaveInterop.waveInGetDevCaps((IntPtr)devNumber, out caps, structSize), "waveInGetDevCaps");
            return caps;
        }

        /// <summary>
        /// Milliseconds for the buffer. Recommended value is 100ms
        /// </summary>
        public int BufferMilliseconds { get; set; }

        /// <summary>
        /// Number of Buffers to use (usually 2 or 3)
        /// </summary>
        public int NumberOfBuffers { get; set; }

        /// <summary>
        /// The device number to use
        /// </summary>
        public int DeviceNumber { get; set; }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        private void CreateBuffers()
        {
            // Default to three buffers of 100ms each
            int bufferSize = BufferMilliseconds * WaveFormat.AverageBytesPerSecond / 1000;
            if (bufferSize % WaveFormat.BlockAlign != 0)
            {
                bufferSize -= bufferSize % WaveFormat.BlockAlign;
            }

            buffers = new WaveInBuffer[NumberOfBuffers];
            for (int n = 0; n < buffers.Length; n++)
            {
                buffers[n] = new WaveInBuffer(waveInHandle, bufferSize);
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        private void OpenWaveInDevice()
        {
            CloseWaveInDevice();
            MmResult result = WaveInterop.waveInOpenWindow(out waveInHandle, (IntPtr)DeviceNumber, WaveFormat,
                callbackEvent.SafeWaitHandle.DangerousGetHandle(), 
                IntPtr.Zero, WaveInterop.WaveInOutOpenFlags.CallbackEvent);
            MmException.Try(result, "waveInOpen");
            CreateBuffers();
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void StartRecording()
        {
            if (captureState != CaptureState.Stopped)
                throw new InvalidOperationException("Already recording");
            OpenWaveInDevice();
            MmException.Try(WaveInterop.waveInStart(waveInHandle), "waveInStart");
            captureState = CaptureState.Starting;
            ThreadPool.QueueUserWorkItem((state) => RecordThread(), null);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        private void RecordThread()
        {
            Exception exception = null;
            try
            {
                DoRecording();
            }
            catch (Exception e)
            {
                exception = e;
            }
            finally
            {
                captureState = CaptureState.Stopped;
                RaiseRecordingStoppedEvent(exception);
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        private void DoRecording()
        {
            captureState = CaptureState.Capturing;
            foreach (var buffer in buffers)
            {
                if (!buffer.InQueue)
                {
                    buffer.Reuse();
                }
            }
            while (captureState == CaptureState.Capturing)
            {
                if (callbackEvent.WaitOne())
                {
                    // requeue any buffers returned to us
                    foreach (var buffer in buffers)
                    {
                        if (buffer.Done)
                        {
                            if (buffer.BytesRecorded > 0)
                            {
                                DataAvailable?.Invoke(this, new WaveInEventArgs(buffer.Data, buffer.BytesRecorded));
                            }

                            if (captureState == CaptureState.Capturing)
                            {
                                buffer.Reuse();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        private void RaiseRecordingStoppedEvent(Exception e)
        {
            var handler = RecordingStopped;
            if (handler != null)
            {
                if (syncContext == null)
                {
                    handler(this, new StoppedEventArgs(e));
                }
                else
                {
                    syncContext.Post(state => handler(this, new StoppedEventArgs(e)), null);
                }
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void StopRecording()
        {
            if (captureState != CaptureState.Stopped)
            {
                captureState = CaptureState.Stopping;
                MmException.Try(WaveInterop.waveInStop(waveInHandle), "waveInStop");

                //Reset, triggering the buffers to be returned
                MmException.Try(WaveInterop.waveInReset(waveInHandle), "waveInReset");

                callbackEvent.Set(); // signal the thread to exit
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public long GetPosition()
        {
            MmTime mmTime = new MmTime();
            mmTime.wType = MmTime.TIME_BYTES; // request results in bytes, TODO: perhaps make this a little more flexible and support the other types?
            MmException.Try(WaveInterop.waveInGetPosition(waveInHandle, out mmTime, Marshal.SizeOf(mmTime)), "waveInGetPosition");

            if (mmTime.wType != MmTime.TIME_BYTES)
                throw new Exception(string.Format("waveInGetPosition: wType -> Expected {0}, Received {1}", MmTime.TIME_BYTES, mmTime.wType));

            return mmTime.cb;
        }

        /// <summary>
        /// WaveFormat we are recording in
        /// </summary>
        public WaveFormat WaveFormat { get; set; }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (captureState != CaptureState.Stopped)
                    StopRecording();

                CloseWaveInDevice();
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        private void CloseWaveInDevice()
        {
            // Some drivers need the reset to properly release buffers
            WaveInterop.waveInReset(waveInHandle);
            if (buffers != null)
            {
                for (int n = 0; n < buffers.Length; n++)
                {
                    buffers[n].Dispose();
                }
                buffers = null;
            }
            WaveInterop.waveInClose(waveInHandle);
            waveInHandle = IntPtr.Zero;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public MixerLine GetMixerLine()
        {
            // TODO use mixerGetID instead to see if this helps with XP
            MixerLine mixerLine;
            if (waveInHandle != IntPtr.Zero)
            {
                mixerLine = new MixerLine(waveInHandle, 0, MixerFlags.WaveInHandle);
            }
            else
            {
                mixerLine = new MixerLine((IntPtr)DeviceNumber, 0, MixerFlags.WaveIn);
            }
            return mixerLine;
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

