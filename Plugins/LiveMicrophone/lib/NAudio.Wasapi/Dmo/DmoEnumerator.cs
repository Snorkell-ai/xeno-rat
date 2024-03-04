using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NAudio.Dmo
{
    /// <summary>
    /// DirectX Media Object Enumerator
    /// </summary>
    public class DmoEnumerator
    {

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static IEnumerable<DmoDescriptor> GetAudioEffectNames()
        {
            return GetDmos(DmoGuids.DMOCATEGORY_AUDIO_EFFECT);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static IEnumerable<DmoDescriptor> GetAudioEncoderNames()
        {
            return GetDmos(DmoGuids.DMOCATEGORY_AUDIO_ENCODER);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static IEnumerable<DmoDescriptor> GetAudioDecoderNames()
        {
            return GetDmos(DmoGuids.DMOCATEGORY_AUDIO_DECODER);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        private static IEnumerable<DmoDescriptor> GetDmos(Guid category)
        {
            IEnumDmo enumDmo;
            var hresult = DmoInterop.DMOEnum(ref category, DmoEnumFlags.None, 0, null, 0, null, out enumDmo);
            Marshal.ThrowExceptionForHR(hresult);
            int itemsFetched;
            do
            {
                Guid guid;
                IntPtr namePointer;
                enumDmo.Next(1, out guid, out namePointer, out itemsFetched);

                if (itemsFetched == 1)
                {
                    string name = Marshal.PtrToStringUni(namePointer);
                    Marshal.FreeCoTaskMem(namePointer);
                    yield return new DmoDescriptor(name, guid);
                }
            } while (itemsFetched > 0);
        }
    }
}
