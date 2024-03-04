using System;
using System.Runtime.InteropServices;

namespace NAudio.Midi
{
    internal class MidiInterop
    {

        public enum MidiInMessage
        {
            /// <summary>
            /// MIM_OPEN
            /// </summary>
            Open = 0x3C1,
            /// <summary>
            /// MIM_CLOSE
            /// </summary>
            Close = 0x3C2,
            /// <summary>
            /// MIM_DATA
            /// </summary>
            Data = 0x3C3,
            /// <summary>
            /// MIM_LONGDATA
            /// </summary>
            LongData = 0x3C4,
            /// <summary>
            /// MIM_ERROR
            /// </summary>
            Error = 0x3C5,
            /// <summary>
            /// MIM_LONGERROR
            /// </summary>
            LongError = 0x3C6,
            /// <summary>
            /// MIM_MOREDATA
            /// </summary>
            MoreData = 0x3CC,
        }



        public enum MidiOutMessage
        {
            /// <summary>
            /// MOM_OPEN
            /// </summary>
            Open = 0x3C7,
            /// <summary>
            /// MOM_CLOSE
            /// </summary>
            Close = 0x3C8,
            /// <summary>
            /// MOM_DONE
            /// </summary>
            Done = 0x3C9
        }

        // http://msdn.microsoft.com/en-us/library/dd798460%28VS.85%29.aspx
        public delegate void MidiInCallback(IntPtr midiInHandle, MidiInMessage message, IntPtr userData, IntPtr messageParameter1, IntPtr messageParameter2);

        // http://msdn.microsoft.com/en-us/library/dd798478%28VS.85%29.aspx
        public delegate void MidiOutCallback(IntPtr midiInHandle, MidiOutMessage message, IntPtr userData, IntPtr messageParameter1, IntPtr messageParameter2);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiConnect(IntPtr hMidiIn, IntPtr hMidiOut, IntPtr pReserved);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiDisconnect(IntPtr hMidiIn, IntPtr hMidiOut, IntPtr pReserved);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiInAddBuffer(IntPtr hMidiIn, IntPtr lpMidiInHdr, int uSize);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiInClose(IntPtr hMidiIn);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll", CharSet = CharSet.Auto)]
        public static extern MmResult midiInGetDevCaps(IntPtr deviceId, out MidiInCapabilities capabilities, int size);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiInGetErrorText(int err, string lpText, int uSize);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiInGetID(IntPtr hMidiIn, out int lpuDeviceId);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern int midiInGetNumDevs();

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiInMessage(IntPtr hMidiIn, int msg, IntPtr dw1, IntPtr dw2);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll", EntryPoint = "midiInOpen")]
        public static extern MmResult midiInOpen(out IntPtr hMidiIn, IntPtr uDeviceID, MidiInCallback callback, IntPtr dwInstance, int dwFlags);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll", EntryPoint = "midiInOpen")]
        public static extern MmResult midiInOpenWindow(out IntPtr hMidiIn, IntPtr uDeviceID, IntPtr callbackWindowHandle, IntPtr dwInstance, int dwFlags);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiInPrepareHeader(IntPtr hMidiIn, IntPtr lpMidiInHdr, int uSize);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiInReset(IntPtr hMidiIn);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiInStart(IntPtr hMidiIn);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiInStop(IntPtr hMidiIn);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiInUnprepareHeader(IntPtr hMidiIn, IntPtr lpMidiInHdr, int uSize);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiOutCacheDrumPatches(IntPtr hMidiOut, int uPatch, IntPtr lpKeyArray, int uFlags);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiOutCachePatches(IntPtr hMidiOut, int uBank, IntPtr lpPatchArray, int uFlags);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiOutClose(IntPtr hMidiOut);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll", CharSet = CharSet.Auto)]
        public static extern MmResult midiOutGetDevCaps(IntPtr deviceNumber, out MidiOutCapabilities caps, int uSize);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiOutGetErrorText(IntPtr err, string lpText, int uSize);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiOutGetID(IntPtr hMidiOut, out int lpuDeviceID);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern int midiOutGetNumDevs();

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiOutGetVolume(IntPtr uDeviceID, ref int lpdwVolume);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiOutLongMsg(IntPtr hMidiOut, ref MIDIHDR lpMidiOutHdr, int uSize);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiOutMessage(IntPtr hMidiOut, int msg, IntPtr dw1, IntPtr dw2);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiOutOpen(out IntPtr lphMidiOut, IntPtr uDeviceID, MidiOutCallback dwCallback, IntPtr dwInstance, int dwFlags);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiOutPrepareHeader(IntPtr hMidiOut, ref MIDIHDR lpMidiOutHdr, int uSize);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiOutReset(IntPtr hMidiOut);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiOutSetVolume(IntPtr hMidiOut, int dwVolume);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiOutShortMsg(IntPtr hMidiOut, int dwMsg);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiOutUnprepareHeader(IntPtr hMidiOut, ref MIDIHDR lpMidiOutHdr, int uSize);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiStreamClose(IntPtr hMidiStream);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiStreamOpen(out IntPtr hMidiStream, IntPtr puDeviceID, int cMidi, IntPtr dwCallback, IntPtr dwInstance, int fdwOpen);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiStreamOut(IntPtr hMidiStream, ref MIDIHDR pmh, int cbmh);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiStreamPause(IntPtr hMidiStream);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiStreamPosition(IntPtr hMidiStream, ref MMTIME lpmmt, int cbmmt);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiStreamProperty(IntPtr hMidiStream, IntPtr lppropdata, int dwProperty);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiStreamRestart(IntPtr hMidiStream);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("winmm.dll")]
        public static extern MmResult midiStreamStop(IntPtr hMidiStream);

        // TODO: this is general MM interop
        public const int CALLBACK_FUNCTION = 0x30000;
        public const int CALLBACK_NULL = 0;

        // http://msdn.microsoft.com/en-us/library/dd757347%28VS.85%29.aspx
        // TODO: not sure this is right
        [StructLayout(LayoutKind.Sequential)]
        public struct MMTIME
        {
            public int wType;
            public int u;
        }

        // TODO: check for ANSI strings in these structs
        // TODO: check for WORD params
        [StructLayout(LayoutKind.Sequential)]
        public struct MIDIEVENT
        {
            public int dwDeltaTime;
            public int dwStreamID;
            public int dwEvent;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public int dwParms;
        }

        // http://msdn.microsoft.com/en-us/library/dd798449%28VS.85%29.aspx
        [StructLayout(LayoutKind.Sequential)]
        public struct MIDIHDR
        {
            public IntPtr lpData; // LPSTR
            public int dwBufferLength; // DWORD
            public int dwBytesRecorded; // DWORD
            public IntPtr dwUser; // DWORD_PTR
            public int dwFlags; // DWORD
            public IntPtr lpNext; // struct mididhdr_tag *
            public IntPtr reserved; // DWORD_PTR
            public int dwOffset; // DWORD
            // n.b. MSDN documentation incorrect, see mmsystem.h
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)] 
            public IntPtr[] dwReserved; // DWORD_PTR dwReserved[8]
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MIDIPROPTEMPO
        {
            public int cbStruct;
            public int dwTempo;
        }
    }
}
