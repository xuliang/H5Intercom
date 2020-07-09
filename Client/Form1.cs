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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        NamedPipeServerStream pipeServer = new NamedPipeServerStream("testpipe", PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(delegate 
            {
                pipeServer.BeginWaitForConnection((o) =>
                {
                    NamedPipeServerStream pServer = (NamedPipeServerStream)o.AsyncState;
                    pServer.EndWaitForConnection(o);
                    StreamReader sr = new StreamReader(pServer);
                    while (true)
                    {
                        this.Invoke((MethodInvoker)delegate { listView1.Items.Add(sr.ReadLine()); });
                    }
                }, pipeServer);
            });

        }

    }
}
