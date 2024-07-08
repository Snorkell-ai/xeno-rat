using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace NAudio.Midi 
{
    /// <summary>
    /// Represents a MIDI sysex message
    /// </summary>
    public class SysexEvent : MidiEvent 
    {
        private byte[] data;

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static SysexEvent ReadSysexEvent(BinaryReader br) 
        {
            SysexEvent se = new SysexEvent();
            //se.length = ReadVarInt(br);
            //se.data = br.ReadBytes(se.length);

            List<byte> sysexData = new List<byte>();
            bool loop = true;
            while(loop) 
            {
                byte b = br.ReadByte();
                if(b == 0xF7) 
                {
                    loop = false;
                }
                else 
                {
                    sysexData.Add(b);
                }
            }
            
            se.data = sysexData.ToArray();

            return se;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public override MidiEvent Clone() => new SysexEvent { data = (byte[])data?.Clone() };

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public override string ToString() 
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in data)
            {
                sb.AppendFormat("{0:X2} ", b);
            }
            return String.Format("{0} Sysex: {1} bytes\r\n{2}",this.AbsoluteTime,data.Length,sb.ToString());
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public override void Export(ref long absoluteTime, BinaryWriter writer)
        {
            base.Export(ref absoluteTime, writer);
            //WriteVarInt(writer,length);
            //writer.Write(data, 0, data.Length);
            writer.Write(data, 0, data.Length);
            writer.Write((byte)0xF7);
        }
    }
}