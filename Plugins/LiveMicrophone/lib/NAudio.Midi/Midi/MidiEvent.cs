using System;
using System.IO;

namespace NAudio.Midi 
{
    /// <summary>
    /// Represents an individual MIDI event
    /// </summary>
    public class MidiEvent
        : ICloneable
    {
        /// <summary>The MIDI command code</summary>
        private MidiCommandCode commandCode;
        private int channel;
        private int deltaTime;
        private long absoluteTime;

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static MidiEvent FromRawMessage(int rawMessage)
        {
            long absoluteTime = 0;
            int b = rawMessage & 0xFF;
            int data1 = (rawMessage >> 8) & 0xFF;
            int data2 = (rawMessage >> 16) & 0xFF;
            MidiCommandCode commandCode;
            int channel = 1;

            if ((b & 0xF0) == 0xF0)
            {
                // both bytes are used for command code in this case
                commandCode = (MidiCommandCode)b;
            }
            else
            {
                commandCode = (MidiCommandCode)(b & 0xF0);
                channel = (b & 0x0F) + 1;
            }

            MidiEvent me;
            switch (commandCode)
            {
                case MidiCommandCode.NoteOn:
                case MidiCommandCode.NoteOff:
                case MidiCommandCode.KeyAfterTouch:
                    if (data2 > 0 && commandCode == MidiCommandCode.NoteOn)
                    {
                        me = new NoteOnEvent(absoluteTime, channel, data1, data2, 0);
                    }
                    else
                    {
                        me = new NoteEvent(absoluteTime, channel, commandCode, data1, data2);
                    }
                    break;
                case MidiCommandCode.ControlChange:
                    me = new ControlChangeEvent(absoluteTime,channel,(MidiController)data1,data2);
                    break;
                case MidiCommandCode.PatchChange:
                    me = new PatchChangeEvent(absoluteTime,channel,data1);
                    break;
                case MidiCommandCode.ChannelAfterTouch:
                    me = new ChannelAfterTouchEvent(absoluteTime,channel,data1);
                    break;
                case MidiCommandCode.PitchWheelChange:
                    me = new PitchWheelChangeEvent(absoluteTime, channel, data1 + (data2 << 7));
                    break;
                case MidiCommandCode.TimingClock:
                case MidiCommandCode.StartSequence:
                case MidiCommandCode.ContinueSequence:
                case MidiCommandCode.StopSequence:
                case MidiCommandCode.AutoSensing:
                    me = new MidiEvent(absoluteTime,channel,commandCode);
                    break;
                //case MidiCommandCode.MetaEvent:
                //case MidiCommandCode.Sysex:
                default:
                    throw new FormatException(String.Format("Unsupported MIDI Command Code for Raw Message {0}", commandCode));
            }
            return me;

        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static MidiEvent ReadNextEvent(BinaryReader br, MidiEvent previous) 
        {
            int deltaTime = ReadVarInt(br);
            MidiCommandCode commandCode;
            int channel = 1;
            byte b = br.ReadByte();
            if((b & 0x80) == 0) 
            {
                // a running command - command & channel are same as previous
                commandCode = previous.CommandCode;
                channel = previous.Channel;
                br.BaseStream.Position--; // need to push this back
            }
            else 
            {
                if((b & 0xF0) == 0xF0) 
                {
                    // both bytes are used for command code in this case
                    commandCode = (MidiCommandCode) b;
                }
                else 
                {
                    commandCode = (MidiCommandCode) (b & 0xF0);
                    channel = (b & 0x0F) + 1;
                }
            }
            
            MidiEvent me;
            switch(commandCode) 
            {
            case MidiCommandCode.NoteOn:
                me = new NoteOnEvent(br);
                break;
            case MidiCommandCode.NoteOff:
            case MidiCommandCode.KeyAfterTouch:
                me = new NoteEvent(br);
                break;
            case MidiCommandCode.ControlChange:
                me = new ControlChangeEvent(br);
                break;
            case MidiCommandCode.PatchChange:
                me = new PatchChangeEvent(br);
                break;
            case MidiCommandCode.ChannelAfterTouch:
                me = new ChannelAfterTouchEvent(br);
                break;
            case MidiCommandCode.PitchWheelChange:
                me = new PitchWheelChangeEvent(br);
                break;
            case MidiCommandCode.TimingClock:
            case MidiCommandCode.StartSequence:
            case MidiCommandCode.ContinueSequence:
            case MidiCommandCode.StopSequence:
                me = new MidiEvent();
                break;
            case MidiCommandCode.Sysex:
                me = SysexEvent.ReadSysexEvent(br);
                break;
            case MidiCommandCode.MetaEvent:
                me = MetaEvent.ReadMetaEvent(br);
                break;
            default:
                throw new FormatException(String.Format("Unsupported MIDI Command Code {0:X2}",(byte) commandCode));
            }
            me.channel = channel;
            me.deltaTime = deltaTime;
            me.commandCode = commandCode;
            return me;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public virtual int GetAsShortMessage()
        {
            return (channel - 1) + (int)commandCode;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        protected MidiEvent()
        {
        }

        /// <summary>
        /// Creates a MIDI event with specified parameters
        /// </summary>
        /// <param name="absoluteTime">Absolute time of this event</param>
        /// <param name="channel">MIDI channel number</param>
        /// <param name="commandCode">MIDI command code</param>
        public MidiEvent(long absoluteTime, int channel, MidiCommandCode commandCode)
        {
            this.absoluteTime = absoluteTime;
            Channel = channel;
            this.commandCode = commandCode;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public virtual MidiEvent Clone() => (MidiEvent)MemberwiseClone();

        object ICloneable.Clone() => Clone();

        /// <summary>
        /// The MIDI Channel Number for this event (1-16)
        /// </summary>
        public virtual int Channel 
        {
            get => channel;
            set
            {
                if ((value < 1) || (value > 16))
                {
                    throw new ArgumentOutOfRangeException("value", value,
                        String.Format("Channel must be 1-16 (Got {0})",value));
                }
                channel = value;
            }
        }
        
        /// <summary>
        /// The Delta time for this event
        /// </summary>
        public int DeltaTime 
        {
            get 
            {
                return deltaTime;
            }
        }
        
        /// <summary>
        /// The absolute time for this event
        /// </summary>
        public long AbsoluteTime 
        {
            get 
            {
                return absoluteTime;
            }
            set 
            {
                absoluteTime = value;
            }
        }
        
        /// <summary>
        /// The command code for this event
        /// </summary>
        public MidiCommandCode CommandCode 
        {
            get 
            {
                return commandCode;
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static bool IsNoteOff(MidiEvent midiEvent)
        {
            if (midiEvent != null)
            {
                if (midiEvent.CommandCode == MidiCommandCode.NoteOn)
                {
                    NoteEvent ne = (NoteEvent)midiEvent;
                    return (ne.Velocity == 0);
                }
                return (midiEvent.CommandCode == MidiCommandCode.NoteOff);
            }
            return false;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static bool IsNoteOn(MidiEvent midiEvent)
        {
            if (midiEvent != null)
            {
                if (midiEvent.CommandCode == MidiCommandCode.NoteOn)
                {
                    NoteEvent ne = (NoteEvent)midiEvent;
                    return (ne.Velocity > 0);
                }
            }
            return false;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static bool IsEndTrack(MidiEvent midiEvent)
        {
            if (midiEvent != null)
            {
                MetaEvent me = midiEvent as MetaEvent;
                if (me != null)
                {
                    return me.MetaEventType == MetaEventType.EndTrack;
                }
            }
            return false;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public override string ToString() 
        {
            if(commandCode >= MidiCommandCode.Sysex)
                return String.Format("{0} {1}",absoluteTime,commandCode);
            else
                return String.Format("{0} {1} Ch: {2}", absoluteTime, commandCode, channel);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static int ReadVarInt(BinaryReader br) 
        {
            int value = 0;
            byte b;
            for(int n = 0; n < 4; n++) 
            {
                b = br.ReadByte();
                value <<= 7;
                value += (b & 0x7F);
                if((b & 0x80) == 0) 
                {
                    return value;
                }
            }
            throw new FormatException("Invalid Var Int");
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static void WriteVarInt(BinaryWriter writer, int value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException("value", value, "Cannot write a negative Var Int");
            }
            if (value > 0x0FFFFFFF)
            {
                throw new ArgumentOutOfRangeException("value", value, "Maximum allowed Var Int is 0x0FFFFFFF");
            }

            int n = 0;
            byte[] buffer = new byte[4];
            do
            {
                buffer[n++] = (byte)(value & 0x7F);
                value >>= 7;
            } while (value > 0);
            
            while (n > 0)
            {
                n--;
                if(n > 0)
                    writer.Write((byte) (buffer[n] | 0x80));
                else 
                    writer.Write(buffer[n]);
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public virtual void Export(ref long absoluteTime, BinaryWriter writer)
        {
            if (this.absoluteTime < absoluteTime)
            {
                throw new FormatException("Can't export unsorted MIDI events");
            }
            WriteVarInt(writer,(int) (this.absoluteTime - absoluteTime));
            absoluteTime = this.absoluteTime;
            int output = (int) commandCode;
            if (commandCode != MidiCommandCode.MetaEvent)
            {
                output += (channel - 1);
            }
            writer.Write((byte)output);
        }
    }
}