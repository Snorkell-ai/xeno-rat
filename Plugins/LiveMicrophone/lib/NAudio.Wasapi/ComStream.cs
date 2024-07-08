using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace NAudio.Wave
{
    /// <summary>
    /// Implementation of Com IStream
    /// </summary>
    class ComStream : Stream, IStream
    {
        private Stream stream;

        public override bool CanRead => stream.CanRead;

        public override bool CanSeek => stream.CanSeek;

        public override bool CanWrite => stream.CanWrite;

        public override long Length => stream.Length;

        public override long Position
        {
            get { return stream.Position; }
            set { stream.Position = value; }
        }

        public ComStream(Stream stream)
            : this(stream, true)
        {
        }

        internal ComStream(Stream stream, bool synchronizeStream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (synchronizeStream)
                stream = Synchronized(stream);
            this.stream = stream;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        void IStream.Clone(out IStream ppstm)
        {
            ppstm = null;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        void IStream.Commit(int grfCommitFlags)
        {
            stream.Flush();
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        void IStream.CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten)
        {
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        void IStream.LockRegion(long libOffset, long cb, int dwLockType)
        {
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        void IStream.Read(byte[] pv, int cb, IntPtr pcbRead)
        {
            if (!CanRead)
                throw new InvalidOperationException("Stream is not readable.");
            int val = Read(pv, 0, cb);
            if (pcbRead != IntPtr.Zero)
                Marshal.WriteInt32(pcbRead, val);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        void IStream.Revert()
        {
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        void IStream.Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition)
        {
            SeekOrigin origin = (SeekOrigin) dwOrigin;
            long val = Seek(dlibMove, origin);
            if (plibNewPosition != IntPtr.Zero)
                Marshal.WriteInt64(plibNewPosition, val);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        void IStream.SetSize(long libNewSize)
        {
            SetLength(libNewSize);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        void IStream.Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag)
        {
            const int STGM_READ = 0x00000000;
            const int STGM_WRITE = 0x00000001;
            const int STGM_READWRITE = 0x00000002;

            var tmp = new System.Runtime.InteropServices.ComTypes.STATSTG { type = 2, cbSize = Length, grfMode = 0 };

            if (CanWrite && CanRead)
                tmp.grfMode |= STGM_READWRITE;
            else if (CanRead)
                tmp.grfMode |= STGM_READ;
            else if (CanWrite)
                tmp.grfMode |= STGM_WRITE;
            else
                throw new ObjectDisposedException("Stream");

            pstatstg = tmp;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        void IStream.UnlockRegion(long libOffset, long cb, int dwLockType)
        {
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        void IStream.Write(byte[] pv, int cb, IntPtr pcbWritten)
        {
            if (!CanWrite)
                throw new InvalidOperationException("Stream is not writeable.");
            Write(pv, 0, cb);
            if (pcbWritten != IntPtr.Zero)
                Marshal.WriteInt32(pcbWritten, cb);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public override void Flush()
        {
            stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return stream.Seek(offset, origin);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public override void SetLength(long value)
        {
            stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            stream.Write(buffer, offset, count);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (stream == null)
                return;
            stream.Dispose();
            stream = null;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public override void Close()
        {
            base.Close();
            if (stream == null)
                return;
            stream.Close();
            stream = null;
        }
    }
}
