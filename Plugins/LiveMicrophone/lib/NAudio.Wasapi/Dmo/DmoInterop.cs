using System;
using System.Text;
using System.Runtime.InteropServices;

namespace NAudio.Dmo
{
    static class DmoInterop
    {

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("msdmo.dll")]
        public static extern int DMOEnum(
            [In] ref Guid guidCategory,
            DmoEnumFlags flags,
            int inTypes,
            [In] DmoPartialMediaType[] inTypesArray,
            int outTypes,
            [In] DmoPartialMediaType[] outTypesArray,
            out IEnumDmo enumDmo);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("msdmo.dll")]
        public static extern int MoFreeMediaType(
            [In] ref DmoMediaType mediaType);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("msdmo.dll")]
        public static extern int MoInitMediaType(
            [In,Out] ref DmoMediaType mediaType, int formatBlockBytes);

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("msdmo.dll")]
        public static extern int DMOGetName([In] ref Guid clsidDMO,
            // preallocate 80 characters
            [Out] StringBuilder name);
    }
}
