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
using System.Runtime.InteropServices;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        //для FindWindow, чтобы найти хендл нужного окна
        [DllImportAttribute("User32.dll")]
        private static extern int FindWindow(String ClassName, String WindowName);

        //SetForeground, чтоб активировать окно по хендлу
        [DllImportAttribute("User32.dll")]
        private static extern IntPtr SetForegroundWindow(int hWnd);

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

                    string textColor = "";

                    for(int i = 0; i < count; i++)
                    {
                        Clear_Message += message[i];
                    }

                    int countColor = Clear_Message.IndexOf(":::5");
                    for (int i = countColor + 4; i < Clear_Message.Length; i++)
                        textColor += Clear_Message[i];
                    string Last_Message = "";
                    for (int i = 0; i < countColor; i++)
                        Last_Message += Clear_Message[i];

                    Color TextColor = new Color();
                    
                    if (textColor != "")
                        TextColor = Color.FromName(textColor);
                    else
                        TextColor = Color.Black;
                    
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        buffer[i] = 0;
                    }
                    this.Invoke((MethodInvoker)delegate()
                    {
                        System.Media.SystemSounds.Asterisk.Play();
                        richTextBox1.AppendText(Last_Message);
                        richTextBox1.SelectionStart = richTextBox1.Text.LastIndexOf(Last_Message);
                        richTextBox1.SelectionLength = Clear_Message.Length;
                        richTextBox1.SelectionColor = TextColor;
                        if (!bold)
                        {
                            richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Bold);
                            bold = true;
                        }
                        else
                        {
                            richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Regular);
                            bold = false;
                        }
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
            SendMessage("\n" + nameBox.Text + ": " + messageBox.Text + ":::5" + nameBox.ForeColor.Name + ";;;5");
            messageBox.Clear();
        }

        private void connect_button_Click(object sender, EventArgs e)
        {
            try
            {
                {
                    Process.Start(Application.StartupPath + @"\ConsoleApplication1\Debug\ConsoleApplication1.exe");
                }
                if (nameBox.Text != " " && nameBox.Text != "")
                {
                    Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    if (Ip != null)
                    {
                        Client.Connect(Ip, port);
                        th = new Thread(delegate() { RecvMessage(); });
                        th.Start();
                        messageBox.Focus();
                    }

                    do
                    {
                        Random random = new Random();
                        KnownColor[] names = (KnownColor[])Enum.GetValues(typeof(KnownColor));
                        KnownColor randomColorName = names[random.Next(names.Length)];
                        Color randomColor = Color.FromKnownColor(randomColorName);
                        nameBox.ForeColor = randomColor;
                    } while (nameBox.ForeColor == Color.White);


                }
            }
            catch (Exception ex)
            {
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Process[] procs = Process.GetProcesses();
            foreach (Process p in procs)
            {
                richTextBox1.AppendText(p.MainWindowTitle);
            }
        }
    }
}
