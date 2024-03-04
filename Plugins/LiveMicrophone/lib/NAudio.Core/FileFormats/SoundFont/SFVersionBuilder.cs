using System.IO;

namespace NAudio.SoundFont
{
    /// <summary>
    /// Builds a SoundFont version
    /// </summary>
    class SFVersionBuilder : StructureBuilder<SFVersion>
    {

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public override SFVersion Read(BinaryReader br)
        {
            SFVersion v = new SFVersion();
            v.Major = br.ReadInt16();
            v.Minor = br.ReadInt16();
            data.Add(v);
            return v;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public override void Write(BinaryWriter bw, SFVersion v)
        {
            bw.Write(v.Major);
            bw.Write(v.Minor);
        }

        /// <summary>
        /// Gets the length of this structure
        /// </summary>
        public override int Length => 4;
    }
}