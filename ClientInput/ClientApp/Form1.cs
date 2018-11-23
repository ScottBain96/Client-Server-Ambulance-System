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
namespace ClientApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            btnSend.Enabled = false;
            txtStatus.ReadOnly = true;
            txtStatus.BackColor = System.Drawing.SystemColors.Window;
        }

        SimpleTcpClient client;

        private void Form1_Load(object sender, EventArgs e)
        {
            client = new SimpleTcpClient();
            client.StringEncoder = Encoding.UTF8;
            client.DataReceived += Client_DataReceived;
        }

        private void Client_DataReceived(object sender, SimpleTCP.Message e)
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
            client.Connect("127.0.0.1", port);
            labelStatus.Text = "Connected";
            btnSend.Enabled = true;


        }

        //Sending all the data as a single message separated by "," to then split it on the server application

        private void btnSend_Click(object sender, EventArgs e)
        {

           

            if (txtAddress.Text == "" || txtName.Text == "" || txtCondition.Text == "" || txtNHSnumber.Text == "")
            {
                MessageBox.Show("Fill all details before sending!");

            }
           else
            {
                string Message = txtName.Text + ", " + txtNHSnumber.Text + ", " + txtAddress.Text + ", " + txtCondition.Text;
                client.WriteLineAndGetReply(Message, TimeSpan.FromSeconds(2));
               
            }
            
        }

    }
}
