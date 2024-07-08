using System;
using System.IO;
using System.Text;

namespace NAudio.Midi 
{
    /// <summary>
    /// Represents a MIDI tempo event
    /// </summary>
    public class TempoEvent : MetaEvent 
    {
        private int microsecondsPerQuarterNote;
        
        /// <summary>
        /// Reads a new tempo event from a MIDI stream
        /// </summary>
        /// <param name="br">The MIDI stream</param>
        /// <param name="length">the data length</param>
        public TempoEvent(BinaryReader br,int length) 
        {
            if(length != 3) 
            {
                throw new FormatException("Invalid tempo length");
            }
            microsecondsPerQuarterNote = (br.ReadByte() << 16) + (br.ReadByte() << 8) + br.ReadByte();
        }

        /// <summary>
        /// Creates a new tempo event with specified settings
        /// </summary>
        /// <param name="microsecondsPerQuarterNote">Microseconds per quarter note</param>
        /// <param name="absoluteTime">Absolute time</param>
        public TempoEvent(int microsecondsPerQuarterNote, long absoluteTime)
            : base(MetaEventType.SetTempo,3,absoluteTime)
        {
            this.microsecondsPerQuarterNote = microsecondsPerQuarterNote;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public override MidiEvent Clone() => (TempoEvent)MemberwiseClone();

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public override string ToString() 
        {
            return String.Format("{0} {2}bpm ({1})",
                base.ToString(),
                microsecondsPerQuarterNote,
                (60000000 / microsecondsPerQuarterNote));
        }

        /// <summary>
        /// Microseconds per quarter note
        /// </summary>
        public int MicrosecondsPerQuarterNote
        {
            get { return microsecondsPerQuarterNote; }
            set { microsecondsPerQuarterNote = value; }
        }

        /// <summary>
        /// Tempo
        /// </summary>
        public double Tempo
        {
            get { return (60000000.0/microsecondsPerQuarterNote); }
            set { microsecondsPerQuarterNote = (int) (60000000.0/value); }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public override void Export(ref long absoluteTime, BinaryWriter writer)
        {
            base.Export(ref absoluteTime, writer);
            writer.Write((byte) ((microsecondsPerQuarterNote >> 16) & 0xFF));
            writer.Write((byte) ((microsecondsPerQuarterNote >> 8) & 0xFF));
            writer.Write((byte) (microsecondsPerQuarterNote & 0xFF));
        }
    }
}