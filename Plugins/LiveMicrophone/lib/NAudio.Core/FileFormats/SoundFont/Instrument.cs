using System;

namespace NAudio.SoundFont
{
    /// <summary>
    /// SoundFont instrument
    /// </summary>
    public class Instrument
    {
        internal ushort startInstrumentZoneIndex;
        internal ushort endInstrumentZoneIndex;

        /// <summary>
        /// instrument name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Zones
        /// </summary>
        public Zone[] Zones { get; set; }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public override string ToString() => Name;
    }
}