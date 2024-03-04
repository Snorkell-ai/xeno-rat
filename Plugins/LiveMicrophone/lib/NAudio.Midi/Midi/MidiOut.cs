using System;
using System.Runtime.InteropServices;
using NAudio.Wave;

namespace NAudio.Midi 
{
    /// <summary>
    /// Represents a MIDI out device
    /// </summary>
    public class MidiOut : IDisposable 
    {
        private IntPtr hMidiOut = IntPtr.Zero;
        private bool disposed = false;
        MidiInterop.MidiOutCallback callback;

        /// <summary>
        /// Gets the number of MIDI devices available in the system
        /// </summary>
        public static int NumberOfDevices 
        {
            get 
            {
                return MidiInterop.midiOutGetNumDevs();
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static MidiOutCapabilities DeviceInfo(int midiOutDeviceNumber)
        {
            MidiOutCapabilities caps = new MidiOutCapabilities();
            int structSize = Marshal.SizeOf(caps);
            MmException.Try(MidiInterop.midiOutGetDevCaps((IntPtr)midiOutDeviceNumber, out caps, structSize), "midiOutGetDevCaps");
            return caps;
        }

        
        /// <summary>
        /// Opens a specified MIDI out device
        /// </summary>
        /// <param name="deviceNo">The device number</param>
        public MidiOut(int deviceNo) 
        {
            this.callback = new MidiInterop.MidiOutCallback(Callback);
            MmException.Try(MidiInterop.midiOutOpen(out hMidiOut, (IntPtr)deviceNo, callback, IntPtr.Zero, MidiInterop.CALLBACK_FUNCTION), "midiOutOpen");
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
        /// Gets or sets the volume for this MIDI out device
        /// </summary>
        public int Volume 
        {
            // TODO: Volume can be accessed by device ID
            get 
            {
                int volume = 0;
                MmException.Try(MidiInterop.midiOutGetVolume(hMidiOut,ref volume),"midiOutGetVolume");
                return volume;
            }
            set 
            {
                MmException.Try(MidiInterop.midiOutSetVolume(hMidiOut,value),"midiOutSetVolume");
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void Reset() 
        {
            MmException.Try(MidiInterop.midiOutReset(hMidiOut),"midiOutReset");
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void SendDriverMessage(int message, int param1, int param2) 
        {
            MmException.Try(MidiInterop.midiOutMessage(hMidiOut,message,(IntPtr)param1,(IntPtr)param2),"midiOutMessage");
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void Send(int message) 
        {
            MmException.Try(MidiInterop.midiOutShortMsg(hMidiOut,message),"midiOutShortMsg");
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
                MidiInterop.midiOutClose(hMidiOut);
            }
            disposed = true;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        private void Callback(IntPtr midiInHandle, MidiInterop.MidiOutMessage message, IntPtr userData, IntPtr messageParameter1, IntPtr messageParameter2)
        {
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void SendBuffer(byte[] byteBuffer)
        {
            var header = new MidiInterop.MIDIHDR();
            header.lpData = Marshal.AllocHGlobal(byteBuffer.Length);
            Marshal.Copy(byteBuffer, 0, header.lpData, byteBuffer.Length);

            header.dwBufferLength = byteBuffer.Length;
            header.dwBytesRecorded = byteBuffer.Length;
            int size = Marshal.SizeOf(header);
            MidiInterop.midiOutPrepareHeader(this.hMidiOut, ref header, size);
            var errcode = MidiInterop.midiOutLongMsg(this.hMidiOut, ref header, size);
            if (errcode != MmResult.NoError)
            {
                MidiInterop.midiOutUnprepareHeader(this.hMidiOut, ref header, size);
            }
            Marshal.FreeHGlobal(header.lpData);
        }

        /// <summary>
        /// Cleanup
        /// </summary>
        ~MidiOut()
        {
            System.Diagnostics.Debug.Assert(false);
            Dispose(false);
        }
    }
}
