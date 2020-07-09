using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class Form1 : Form
    {
        NamedPipeClientStream pipeClient = new NamedPipeClientStream("localhost", "testpipe", PipeDirection.InOut, PipeOptions.Asynchronous, TokenImpersonationLevel.None);
        StreamWriter sw = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                pipeClient.Connect(5000);
                sw = new StreamWriter(pipeClient);
                sw.AutoFlush = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("连接建立失败，请确保服务端程序已经被打开。");
                this.Close();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (sw != null)
            {
                sw.WriteLine(this.textBox1.Text);
            }
            else
            {
                MessageBox.Show("未建立连接，不能发送消息。");
            }

        }
    }
}
