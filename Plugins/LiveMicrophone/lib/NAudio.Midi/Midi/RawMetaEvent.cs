using System.IO;
using System.Text;

namespace NAudio.Midi
{
    /// <summary>
    /// Represents a MIDI meta event with raw data
    /// </summary>
    public class RawMetaEvent : MetaEvent
    {
        /// <summary>
        /// Raw data contained in the meta event
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        ///  Creates a meta event with raw data
        /// </summary>
        public RawMetaEvent(MetaEventType metaEventType, long absoluteTime, byte[] data) : base(metaEventType, data?.Length ?? 0, absoluteTime)
        {
            Data = data;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public override MidiEvent Clone() => new RawMetaEvent(MetaEventType, AbsoluteTime, (byte[])Data?.Clone());

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public override string ToString()
        {
            var sb = new StringBuilder().Append(base.ToString());
            foreach (var b in Data)
                sb.AppendFormat(" {0:X2}", b);
            return sb.ToString();
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public override void Export(ref long absoluteTime, BinaryWriter writer)
        {
            base.Export(ref absoluteTime, writer);
            if (Data == null) return;
            writer.Write(Data, 0, Data.Length);
        }
    }
}
