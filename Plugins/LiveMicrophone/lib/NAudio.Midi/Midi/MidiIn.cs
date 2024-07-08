using System;
using System.Runtime.InteropServices;

namespace NAudio.Midi
{
    /// <summary>
    /// Represents a MIDI in device
    /// </summary>
    public class MidiIn : IDisposable 
    {
        private IntPtr hMidiIn = IntPtr.Zero;
        private bool disposed = false;
        private MidiInterop.MidiInCallback callback;

        //  Buffer headers created and marshalled to recive incoming Sysex mesages
        private IntPtr[] SysexBufferHeaders = new IntPtr[0];

        /// <summary>
        /// Called when a MIDI message is received
        /// </summary>
        public event EventHandler<MidiInMessageEventArgs> MessageReceived;

        /// <summary>
        /// An invalid MIDI message
        /// </summary>
        public event EventHandler<MidiInMessageEventArgs> ErrorReceived;

        /// <summary>
        /// Called when a Sysex MIDI message is received
        /// </summary>
        public event EventHandler<MidiInSysexMessageEventArgs> SysexMessageReceived;

        /// <summary>
        /// Gets the number of MIDI input devices available in the system
        /// </summary>
        public static int NumberOfDevices 
        {
            get 
            {
                return MidiInterop.midiInGetNumDevs();
            }
        }
        
        /// <summary>
        /// Opens a specified MIDI in device
        /// </summary>
        /// <param name="deviceNo">The device number</param>
        public MidiIn(int deviceNo) 
        {
            this.callback = new MidiInterop.MidiInCallback(Callback);
            MmException.Try(MidiInterop.midiInOpen(out hMidiIn, (IntPtr) deviceNo,this.callback,IntPtr.Zero,MidiInterop.CALLBACK_FUNCTION),"midiInOpen");
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void Close() 
        {
            Dispose();
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void Dispose() 
        {
            GC.KeepAlive(callback);
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void Start()
        {
            MmException.Try(MidiInterop.midiInStart(hMidiIn), "midiInStart");
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void Stop()
        {
            MmException.Try(MidiInterop.midiInStop(hMidiIn), "midiInStop");
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void Reset()
        {
            MmException.Try(MidiInterop.midiInReset(hMidiIn), "midiInReset");
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void CreateSysexBuffers(int bufferSize, int numberOfBuffers)
        {
            SysexBufferHeaders = new IntPtr[numberOfBuffers];

            var hdrSize = Marshal.SizeOf(typeof(MidiInterop.MIDIHDR));
            for (var i = 0; i < numberOfBuffers; i++)
            {
                var hdr = new MidiInterop.MIDIHDR();

                hdr.dwBufferLength = bufferSize;
                hdr.dwBytesRecorded = 0;
                hdr.lpData = Marshal.AllocHGlobal(bufferSize);
                hdr.dwFlags = 0;

                var lpHeader = Marshal.AllocHGlobal(hdrSize);
                Marshal.StructureToPtr(hdr, lpHeader, false);

                MmException.Try(MidiInterop.midiInPrepareHeader(hMidiIn, lpHeader, Marshal.SizeOf(typeof(MidiInterop.MIDIHDR))), "midiInPrepareHeader");
                MmException.Try(MidiInterop.midiInAddBuffer(hMidiIn, lpHeader, Marshal.SizeOf(typeof(MidiInterop.MIDIHDR))), "midiInAddBuffer");
                SysexBufferHeaders[i] = lpHeader;
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        private void Callback(IntPtr midiInHandle, MidiInterop.MidiInMessage message, IntPtr userData, IntPtr messageParameter1, IntPtr messageParameter2)
        {
            switch(message)
            {
                case MidiInterop.MidiInMessage.Open:
                    // message Parameter 1 & 2 are not used
                    break;
                case MidiInterop.MidiInMessage.Data:
                    // parameter 1 is packed MIDI message
                    // parameter 2 is milliseconds since MidiInStart
                    if (MessageReceived != null)
                    {
                        MessageReceived(this, new MidiInMessageEventArgs(messageParameter1.ToInt32(), messageParameter2.ToInt32()));
                    }
                    break;
                case MidiInterop.MidiInMessage.Error:
                    // parameter 1 is invalid MIDI message
                    if (ErrorReceived != null)
                    {
                        ErrorReceived(this, new MidiInMessageEventArgs(messageParameter1.ToInt32(), messageParameter2.ToInt32()));
                    } 
                    break;
                case MidiInterop.MidiInMessage.Close:
                    // message Parameter 1 & 2 are not used
                    break;
                case MidiInterop.MidiInMessage.LongData:
                    // parameter 1 is pointer to MIDI header
                    // parameter 2 is milliseconds since MidiInStart
                    if (SysexMessageReceived != null)
                    {
                        MidiInterop.MIDIHDR hdr = (MidiInterop.MIDIHDR)Marshal.PtrToStructure(messageParameter1, typeof(MidiInterop.MIDIHDR));

                        //  Copy the bytes received into an array so that the buffer is immediately available for re-use
                        var sysexBytes = new byte[hdr.dwBytesRecorded];
                        Marshal.Copy(hdr.lpData, sysexBytes, 0, hdr.dwBytesRecorded);

                        SysexMessageReceived(this, new MidiInSysexMessageEventArgs(sysexBytes, messageParameter2.ToInt32()));
                        //  Re-use the buffer - but not if we have no event handler registered as we are closing
                        MidiInterop.midiInAddBuffer(hMidiIn, messageParameter1, Marshal.SizeOf(typeof(MidiInterop.MIDIHDR)));
                    }
                    break;
                case MidiInterop.MidiInMessage.LongError:
                    // parameter 1 is pointer to MIDI header
                    // parameter 2 is milliseconds since MidiInStart
                    break;
                case MidiInterop.MidiInMessage.MoreData:
                    // parameter 1 is packed MIDI message
                    // parameter 2 is milliseconds since MidiInStart
                    break;
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static MidiInCapabilities DeviceInfo(int midiInDeviceNumber)
        {
            MidiInCapabilities caps = new MidiInCapabilities();
            int structSize = Marshal.SizeOf(caps);
            MmException.Try(MidiInterop.midiInGetDevCaps((IntPtr)midiInDeviceNumber,out caps,structSize),"midiInGetDevCaps");
            return caps;
        }

        /// <summary>
        /// Closes the MIDI out device
        /// </summary>
        /// <param name="disposing">True if called from Dispose</param>
        protected virtual void Dispose(bool disposing) 
        {
            if(!this.disposed) 
            {
                //if(disposing) Components.Dispose();

                if (SysexBufferHeaders.Length > 0)
                {
                    //  Reset in order to release any Sysex buffers
                    //  We can't Unprepare and free them until they are flushed out. Neither can we close the handle.
                    MmException.Try(MidiInterop.midiInReset(hMidiIn), "midiInReset");

                    //  Free up all created and allocated buffers for incoming Sysex messages
                    foreach (var lpHeader in SysexBufferHeaders)
                    {
                        MidiInterop.MIDIHDR hdr = (MidiInterop.MIDIHDR)Marshal.PtrToStructure(lpHeader, typeof(MidiInterop.MIDIHDR));
                        MmException.Try(MidiInterop.midiInUnprepareHeader(hMidiIn, lpHeader, Marshal.SizeOf(typeof(MidiInterop.MIDIHDR))), "midiInPrepareHeader");
                        Marshal.FreeHGlobal(hdr.lpData);
                        Marshal.FreeHGlobal(lpHeader);
                    }

                    //  Defensive protection against double disposal
                    SysexBufferHeaders = new IntPtr[0];
                }
                MidiInterop.midiInClose(hMidiIn);
            }
            disposed = true;
        }

        /// <summary>
        /// Cleanup
        /// </summary>
        ~MidiIn()
        {
            System.Diagnostics.Debug.Assert(false,"MIDI In was not finalised");
            Dispose(false);
        }
    }
}