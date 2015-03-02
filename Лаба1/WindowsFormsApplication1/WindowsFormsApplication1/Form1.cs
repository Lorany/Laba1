using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        static private Socket Client;
        private IPAddress Ip = null;
        private int port = 0;
        private Thread th;
        public Form1()
        {
            InitializeComponent();

            try
            {
                var sr = new StreamReader(@"Client_info/datainfo.txt");
                string buffer = sr.ReadToEnd();
                sr.Close();
                string[] connect_info = buffer.Split(':');
                Ip = IPAddress.Parse(connect_info[0]);
                port = int.Parse(connect_info[1]);
                label4.ForeColor = Color.Green;
                label4.Text = "Настройки: \nIP сурвера: " + connect_info[0] + "\nПорт сервера: " + connect_info[1];
            }
            catch (Exception e)
            {
                label4.ForeColor = Color.Red;
                label4.Text = "fdvdfkvjkdvd";
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (th != null)
                th.Abort();
        }

        void SendMessage( string message)
        {
            if (message != " " && message !="")
            {
                byte[] buffer = new byte[1024];
                buffer = Encoding.UTF8.GetBytes(message);
                Client.Send(buffer);
            }
        }

        bool bold = false;
        void RecvMessage()
        {
            byte[] buffer = new byte[1024];
            for(int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = 0;
            }
            for (; ; )
            {
                try
                {
                    Client.Receive(buffer);
                    string message = Encoding.UTF8.GetString(buffer);
                    int count = message.IndexOf(";;;5");
                    if (count == -1)
                    {
                        continue;
                    }
                    string Clear_Message = "";

                    for(int i = 0; i < count; i++)
                    {
                        Clear_Message += message[i];
                    }
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        buffer[i] = 0;
                    }
                    this.Invoke((MethodInvoker)delegate()
                    {
                        System.Media.SystemSounds.Asterisk.Play();
                        richTextBox1.AppendText(Clear_Message);
                        richTextBox1.SelectionStart = richTextBox1.Text.LastIndexOf(Clear_Message);
                        richTextBox1.SelectionLength = Clear_Message.Length;
                        if (!bold) 
                            richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Bold);
                        else
                            richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Regular);
                        richTextBox1.SelectionLength = 0;
                        richTextBox1.SelectionStart = richTextBox1.Text.Length;
                    });
                }
                catch (Exception e)
                { }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SendMessage("\n" + textBox1.Text + ": " + richTextBox2.Text + ";;;5");
            richTextBox2.Clear();
        }

        private void connect_button_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(Application.StartupPath + @"\ConsoleApplication1\Debug\ConsoleApplication1.exe");
                if (textBox1.Text != " " && textBox1.Text != "")
                {
                    Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    if (Ip != null)
                    {
                        Client.Connect(Ip, port);
                        th = new Thread(delegate() { RecvMessage(); });
                        th.Start();
                        richTextBox2.Focus();
                    }

                }
            }
            catch (Exception ex)
            { }
        }
    }
}
