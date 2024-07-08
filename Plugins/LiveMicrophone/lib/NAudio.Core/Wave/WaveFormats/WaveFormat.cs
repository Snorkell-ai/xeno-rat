using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using NAudio.Utils;

namespace NAudio.Wave
{
    /// <summary>
    /// Represents a Wave file format
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi, Pack=2)]
    public class WaveFormat
    {
        /// <summary>format type</summary>
        protected WaveFormatEncoding waveFormatTag;
        /// <summary>number of channels</summary>
        protected short channels;
        /// <summary>sample rate</summary>
        protected int sampleRate;
        /// <summary>for buffer estimation</summary>
        protected int averageBytesPerSecond;
        /// <summary>block size of data</summary>
        protected short blockAlign;
        /// <summary>number of bits per sample of mono data</summary>
        protected short bitsPerSample;
        /// <summary>number of following bytes</summary>
        protected short extraSize;

        /// <summary>
        /// Creates a new PCM 44.1Khz stereo 16 bit format
        /// </summary>
        public WaveFormat() : this(44100,16,2)
        {

        }
        
        /// <summary>
        /// Creates a new 16 bit wave format with the specified sample
        /// rate and channel count
        /// </summary>
        /// <param name="sampleRate">Sample Rate</param>
        /// <param name="channels">Number of channels</param>
        public WaveFormat(int sampleRate, int channels)
            : this(sampleRate, 16, channels)
        {
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public int ConvertLatencyToByteSize(int milliseconds)
        {
            int bytes = (int) ((AverageBytesPerSecond/1000.0)*milliseconds);
            if ((bytes%BlockAlign) != 0)
            {
                // Return the upper BlockAligned
                bytes = bytes + BlockAlign - (bytes % BlockAlign);
            }
            return bytes;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static WaveFormat CreateCustomFormat(WaveFormatEncoding tag, int sampleRate, int channels, int averageBytesPerSecond, int blockAlign, int bitsPerSample)
        {
            WaveFormat waveFormat = new WaveFormat();
            waveFormat.waveFormatTag = tag;
            waveFormat.channels = (short)channels;
            waveFormat.sampleRate = sampleRate;
            waveFormat.averageBytesPerSecond = averageBytesPerSecond;
            waveFormat.blockAlign = (short)blockAlign;
            waveFormat.bitsPerSample = (short)bitsPerSample;
            waveFormat.extraSize = 0;
            return waveFormat;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static WaveFormat CreateALawFormat(int sampleRate, int channels)
        {
            return CreateCustomFormat(WaveFormatEncoding.ALaw, sampleRate, channels, sampleRate * channels, channels, 8);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static WaveFormat CreateMuLawFormat(int sampleRate, int channels)
        {
            return CreateCustomFormat(WaveFormatEncoding.MuLaw, sampleRate, channels, sampleRate * channels, channels, 8);
        }

        /// <summary>
        /// Creates a new PCM format with the specified sample rate, bit depth and channels
        /// </summary>
        public WaveFormat(int rate, int bits, int channels)
        {
            if (channels < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(channels), "Channels must be 1 or greater");
            }
            // minimum 16 bytes, sometimes 18 for PCM
            waveFormatTag = WaveFormatEncoding.Pcm;
            this.channels = (short)channels;
            sampleRate = rate;
            bitsPerSample = (short)bits;
            extraSize = 0;

            blockAlign = (short)(channels * (bits / 8));
            averageBytesPerSecond = this.sampleRate * this.blockAlign;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static WaveFormat CreateIeeeFloatWaveFormat(int sampleRate, int channels)
        {
            var wf = new WaveFormat();
            wf.waveFormatTag = WaveFormatEncoding.IeeeFloat;
            wf.channels = (short)channels;
            wf.bitsPerSample = 32;
            wf.sampleRate = sampleRate;
            wf.blockAlign = (short) (4*channels);
            wf.averageBytesPerSecond = sampleRate * wf.blockAlign;
            wf.extraSize = 0;
            return wf;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static WaveFormat MarshalFromPtr(IntPtr pointer)
        {
            var waveFormat = Marshal.PtrToStructure<WaveFormat>(pointer);
            switch (waveFormat.Encoding)
            {
                case WaveFormatEncoding.Pcm:
                    // can't rely on extra size even being there for PCM so blank it to avoid reading
                    // corrupt data
                    waveFormat.extraSize = 0;
                    break;
                case WaveFormatEncoding.Extensible:
                    waveFormat = Marshal.PtrToStructure<WaveFormatExtensible>(pointer);
                    break;
                case WaveFormatEncoding.Adpcm:
                    waveFormat = Marshal.PtrToStructure<AdpcmWaveFormat>(pointer);
                    break;
                case WaveFormatEncoding.Gsm610:
                    waveFormat = Marshal.PtrToStructure<Gsm610WaveFormat>(pointer);
                    break;
                default:
                    if (waveFormat.ExtraSize > 0)
                    {
                        waveFormat = Marshal.PtrToStructure<WaveFormatExtraData>(pointer);
                    }
                    break;
            }
            return waveFormat;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static IntPtr MarshalToPtr(WaveFormat format)
        {
            int formatSize = Marshal.SizeOf(format);
            IntPtr formatPointer = Marshal.AllocHGlobal(formatSize);
            Marshal.StructureToPtr(format, formatPointer, false);
            return formatPointer;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static WaveFormat FromFormatChunk(BinaryReader br, int formatChunkLength)
        {
            var waveFormat = new WaveFormatExtraData();
            waveFormat.ReadWaveFormat(br, formatChunkLength);
            waveFormat.ReadExtraData(br);
            return waveFormat;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        private void ReadWaveFormat(BinaryReader br, int formatChunkLength)
        {
            if (formatChunkLength < 16)
                throw new InvalidDataException("Invalid WaveFormat Structure");
            waveFormatTag = (WaveFormatEncoding)br.ReadUInt16();
            channels = br.ReadInt16();
            sampleRate = br.ReadInt32();
            averageBytesPerSecond = br.ReadInt32();
            blockAlign = br.ReadInt16();
            bitsPerSample = br.ReadInt16();
            if (formatChunkLength > 16)
            {
                extraSize = br.ReadInt16();
                if (extraSize != formatChunkLength - 18)
                {
                    Debug.WriteLine("Format chunk mismatch");
                    extraSize = (short)(formatChunkLength - 18);
                }
            }
        }

        /// <summary>
        /// Reads a new WaveFormat object from a stream
        /// </summary>
        /// <param name="br">A binary reader that wraps the stream</param>
        public WaveFormat(BinaryReader br)
        {
            int formatChunkLength = br.ReadInt32();
            ReadWaveFormat(br, formatChunkLength);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public override string ToString()
        {
            switch (waveFormatTag)
            {
                case WaveFormatEncoding.Pcm:
                case WaveFormatEncoding.Extensible:
                    // extensible just has some extra bits after the PCM header
                    return $"{bitsPerSample} bit PCM: {sampleRate}Hz {channels} channels";
                case WaveFormatEncoding.IeeeFloat:
                    return $"{bitsPerSample} bit IEEFloat: {sampleRate}Hz {channels} channels";
                default:
                    return waveFormatTag.ToString();
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public override bool Equals(object obj)
        {
            var other = obj as WaveFormat;
            if(other != null)
            {
                return waveFormatTag == other.waveFormatTag &&
                    channels == other.channels &&
                    sampleRate == other.sampleRate &&
                    averageBytesPerSecond == other.averageBytesPerSecond &&
                    blockAlign == other.blockAlign &&
                    bitsPerSample == other.bitsPerSample;
            }
            return false;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public override int GetHashCode()
        {
            return (int) waveFormatTag ^ 
                (int) channels ^ 
                sampleRate ^ 
                averageBytesPerSecond ^ 
                (int) blockAlign ^ 
                (int) bitsPerSample;
        }

        /// <summary>
        /// Returns the encoding type used
        /// </summary>
        public WaveFormatEncoding Encoding => waveFormatTag;

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public virtual void Serialize(BinaryWriter writer)
        {
            writer.Write((int)(18 + extraSize)); // wave format length
            writer.Write((short)Encoding);
            writer.Write((short)Channels);
            writer.Write((int)SampleRate);
            writer.Write((int)AverageBytesPerSecond);
            writer.Write((short)BlockAlign);
            writer.Write((short)BitsPerSample);
            writer.Write((short)extraSize);
        }

        /// <summary>
        /// Returns the number of channels (1=mono,2=stereo etc)
        /// </summary>
        public int Channels => channels;

        /// <summary>
        /// Returns the sample rate (samples per second)
        /// </summary>
        public int SampleRate => sampleRate;

        /// <summary>
        /// Returns the average number of bytes used per second
        /// </summary>
        public int AverageBytesPerSecond => averageBytesPerSecond;

        /// <summary>
        /// Returns the block alignment
        /// </summary>
        public virtual int BlockAlign => blockAlign;

        /// <summary>
        /// Returns the number of bits per sample (usually 16 or 32, sometimes 24 or 8)
        /// Can be 0 for some codecs
        /// </summary>
        public int BitsPerSample => bitsPerSample;

        /// <summary>
        /// Returns the number of extra bytes used by this waveformat. Often 0,
        /// except for compressed formats which store extra data after the WAVEFORMATEX header
        /// </summary>
        public int ExtraSize => extraSize;
    }
}
