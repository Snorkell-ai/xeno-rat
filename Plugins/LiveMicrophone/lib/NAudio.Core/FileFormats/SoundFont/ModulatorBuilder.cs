using System.IO;

namespace NAudio.SoundFont
{
    class ModulatorBuilder : StructureBuilder<Modulator>
    {

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public override Modulator Read(BinaryReader br)
        {
            Modulator m = new Modulator();
            m.SourceModulationData = new ModulatorType(br.ReadUInt16());
            m.DestinationGenerator = (GeneratorEnum)br.ReadUInt16();
            m.Amount = br.ReadInt16();
            m.SourceModulationAmount = new ModulatorType(br.ReadUInt16());
            m.SourceTransform = (TransformEnum)br.ReadUInt16();
            data.Add(m);
            return m;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public override void Write(BinaryWriter bw, Modulator o)
        {
            //Zone z = (Zone) o;
            //bw.Write(p.---);
        }

        public override int Length => 10;

        public Modulator[] Modulators => data.ToArray();
    }
}