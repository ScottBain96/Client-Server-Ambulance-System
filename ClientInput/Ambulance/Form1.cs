using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimpleTCP;
namespace Ambulance
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SimpleTcpClient client2;

        private void Form1_Load(object sender, EventArgs e)
        {
            client2 = new SimpleTcpClient();
            client2.StringEncoder = Encoding.UTF8;
            client2.DataReceived += Client2_DataReceived;
        }

        private void Client2_DataReceived(object sender, SimpleTCP.Message e)
        {
            txtStatus.Invoke((MethodInvoker)delegate ()
            {
                txtStatus.Text += e.MessageString;


            });

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {

            int port = 8910;
            btnConnect.Enabled = false;
            client2.Connect("127.0.0.1", port);
          
           
        }
    }
}
