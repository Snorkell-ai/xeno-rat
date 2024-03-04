// AForge Video for Windows Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
//

namespace AForge.Video.DirectShow.Internals
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;

    /// <summary>
    /// Some Win32 API used internally.
    /// </summary>
    /// 
    internal static class Win32
    {

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport( "ole32.dll" )]
        public static extern
        int CreateBindCtx( int reserved, out IBindCtx ppbc );

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport( "ole32.dll", CharSet = CharSet.Unicode )]
        public static extern
        int MkParseDisplayName( IBindCtx pbc, string szUserName,
            ref int pchEaten, out IMoniker ppmk );

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport( "ntdll.dll", CallingConvention = CallingConvention.Cdecl )]
        public static unsafe extern int memcpy(
            byte* dst,
            byte* src,
            int count );

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport( "oleaut32.dll" )]
        public static extern int OleCreatePropertyFrame(
            IntPtr hwndOwner,
            int x,
            int y,
            [MarshalAs( UnmanagedType.LPWStr )] string caption,
            int cObjects,
            [MarshalAs( UnmanagedType.Interface, ArraySubType = UnmanagedType.IUnknown )] 
            ref object ppUnk,
            int cPages,
            IntPtr lpPageClsID,
            int lcid,
            int dwReserved,
            IntPtr lpvReserved );
    }
}
