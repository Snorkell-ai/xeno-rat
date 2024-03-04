using System;
using System.IO;
using System.Text;

namespace NAudio.Midi 
{
    /// <summary>
    /// Represents a MIDI text event
    /// </summary>
    public class TextEvent : MetaEvent 
    {
        private byte[] data;
        
        /// <summary>
        /// Reads a new text event from a MIDI stream
        /// </summary>
        /// <param name="br">The MIDI stream</param>
        /// <param name="length">The data length</param>
        public TextEvent(BinaryReader br,int length) 
        {
            data = br.ReadBytes(length);
        }

        /// <summary>
        /// Creates a new TextEvent
        /// </summary>
        /// <param name="text">The text in this type</param>
        /// <param name="metaEventType">MetaEvent type (must be one that is
        /// associated with text data)</param>
        /// <param name="absoluteTime">Absolute time of this event</param>
        public TextEvent(string text, MetaEventType metaEventType, long absoluteTime)
            : base(metaEventType, text.Length, absoluteTime)
        {
            Text = text;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public override MidiEvent Clone() => (TextEvent)MemberwiseClone();

        /// <summary>
        /// The contents of this text event
        /// </summary>
        public string Text
        {
            get 
            { 
                Encoding byteEncoding = NAudio.Utils.ByteEncoding.Instance;
                return byteEncoding.GetString(data); 
            }
            set
            {
                Encoding byteEncoding = NAudio.Utils.ByteEncoding.Instance;
                data = byteEncoding.GetBytes(value);
                metaDataLength = data.Length;
            }
        }
        
        /// <summary>
        /// The raw contents of this text event
        /// </summary>
        public byte[] Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
                metaDataLength = data.Length;
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public override string ToString() 
        {
            return String.Format("{0} {1}",base.ToString(),Text);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public override void Export(ref long absoluteTime, BinaryWriter writer)
        {
            base.Export(ref absoluteTime, writer);
            writer.Write(data);
        }
    }
}
