using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace LZW
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public static List<int> Compress(string uncompressed)
        {
            // build the dictionary
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            for (int i = 0; i < 256; i++)
                dictionary.Add(((char)i).ToString(), i);

            string w = string.Empty;
            List<int> compressed = new List<int>();

            foreach (char c in uncompressed)
            {
                string wc = w + c;
                if (dictionary.ContainsKey(wc))
                {
                    w = wc;
                }
                else
                {
                    // write w to output
                        compressed.Add(dictionary[w]);
                    // wc is a new sequence; add it to the dictionary
                    dictionary.Add(wc, dictionary.Count);
                    w = c.ToString();
                }
            }

            // write remaining output if necessary
            if (!string.IsNullOrEmpty(w))
                compressed.Add(dictionary[w]);
            return compressed;
        }

        public static string Decompress(List<int> compressed)
        {
            // build the dictionary
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            for (int i = 0; i < 256; i++)
                dictionary.Add(i, ((char)i).ToString());

            string w = dictionary[compressed[0]];
            compressed.RemoveAt(0);
            StringBuilder decompressed = new StringBuilder(w);

            foreach (int k in compressed)
            {
                string entry = null;
                if (dictionary.ContainsKey(k))
                    entry = dictionary[k];
                else if (k == dictionary.Count)
                    entry = w + w[0];

                decompressed.Append(entry);

                // new sequence; add it to the dictionary
                dictionary.Add(dictionary.Count, w + entry[0]);

                w = entry;
            }

            return decompressed.ToString();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        bool down;

        private Point lastLocation;
        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if(down)
            {
                this.Location = new Point((this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
            }   
        }
        private void Panel2_MouseUp(object sender, MouseEventArgs e) => down = false;
        private void label5_Click(object sender, EventArgs e)
        {
            if (inputfield.Text == "") return;
            List<int> lists = Compress(inputfield.Text);
            float K = lists.Count;
            var result = string.Join(" ", lists);
            textBox1.Text = result;
            textBox2.Text = Decompress(lists);
            label9.Text = "Length: " + inputfield.Text.Length;
            label10.Text = "Size: " + K;
            label8.Text = "Compression ratio: " + K/inputfield.Text.Length;
        }
        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            down = true;
            lastLocation = e.Location;
        }

        private void label6_Click(object sender, EventArgs e)
        {
            openFD.InitialDirectory = "D:";
            openFD.Title = "Select text file";
            openFD.FileName = ".txt";
            openFD.Filter = "Text file|*.txt";
            if (openFD.ShowDialog() == DialogResult.Cancel)
            {
                MessageBox.Show("Operation cancelled!", "Warning");
            }
            else
            {
                inputfield.Clear();
                label8.Text = "Compression ratio: #";
                label9.Text = "Length: 0";
                label10.Text = "Size: 0";
                textBox1.Clear();
                textBox2.Clear();
                StreamReader sr = new StreamReader(openFD.FileName);
                string line;
                int count = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    if (count == 0)
                    {
                        inputfield.Text += line;
                        count++;
                    }
                    else
                    {
                        inputfield.Text += "\r\n" + line;
                        count++;
                    }
                }
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {
            inputfield.Clear();
            textBox1.Clear();
            textBox2.Clear();
            label8.Text = "Compression ratio: #";
            label9.Text = "Length: 0";
            label10.Text = "Size: 0";
        }

        private void label11_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
