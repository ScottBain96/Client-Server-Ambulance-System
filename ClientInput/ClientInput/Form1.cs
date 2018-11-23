using SimpleTCP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace ClientInput
{
    public partial class Form1 : Form
    {
        //DATABASE CONNECTION

        SqlConnection conn = new SqlConnection(Properties.Settings.Default.KwikdataBaseConnectionString2);

        Patient patient = new Patient();
        public Form1()
        {
            InitializeComponent();
            txtStatus.ReadOnly = true;
            txtStatus.BackColor = System.Drawing.SystemColors.Window;

        }

        SimpleTcpServer server;

        //button to start server
        
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!server.IsStarted)
            {
                int port = 8910;
                txtStatus.Text += "Server Online"+Environment.NewLine;
                System.Net.IPAddress ip = System.Net.IPAddress.Parse("127.0.0.1");
                server.Start(ip, port);
                labelStatus.Text = "Connected";

            }
            
        }

     

        private void Form1_Load(object sender, EventArgs e)
        {
            server = new SimpleTcpServer();
            server.Delimiter = 0x13;
            server.StringEncoder = Encoding.UTF8;
            server.DataReceived += Server_DataReceived;
        }

        private void Server_DataReceived(object sender, SimpleTCP.Message e)
        {

            txtStatus.Invoke((MethodInvoker)delegate ()
               {

                  //Retrieving message and setting up the variables of the patient class to then use it on the database

                   string text1 = e.MessageString;
                   //txtStatus.Text = text1;
                   string[] mytext1 = txtStatus.Text.Split(',');
                   patient.name = mytext1[0];
                   patient.numberNHS = mytext1[1];
                   patient.address = mytext1[2];
                   patient.condition = mytext1[3].Remove(mytext1[3].Length-1);



                   //Insert values of textBoxes to the database

                   string query = "INSERT INTO LocalHelp(NAme, NumberNHS, Address, Condition) " +
                     "VALUES('" + patient.name + "', '" + patient.numberNHS + "', '" + patient.address + "', '" + patient.condition + "')";

                   using (SqlCommand command = new SqlCommand(query, conn))
                   {
                       conn.Open();
                       command.ExecuteNonQuery();
                       txtStatus.Text += "Added record for " + patient.name+Environment.NewLine;

                   }

                   if (conn != null)
                   {
                       conn.Close(); 

                   }

                
                   e.ReplyLine(string.Format("Record added for: {0}", patient.name+Environment.NewLine));
               });

          
        }

        //Stop server

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (server.IsStarted)
                server.Stop();
                labelStatus.Text = "Disconnected";
                txtStatus.Text += "Server Offline" + Environment.NewLine;

        }


        //Delete LocalHelp Table (FOR TESTING REASONS)

        private void button1_Click(object sender, EventArgs e)
        {
            var queryString = "DELETE FROM LocalHelp;";
            conn.Open();
            SqlCommand command = new SqlCommand(queryString, conn);
            command.ExecuteNonQuery();
            conn.Close();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string queryString =
               "SELECT Address FROM LocalHelp WHERE Name='becca';";
            SqlCommand command =
                new SqlCommand(queryString, conn);
            conn.Open();

            SqlDataReader reader = command.ExecuteReader();

            
            while (reader.Read())
            {
                ReadSingleRow((IDataRecord)reader);
            }

         
            reader.Close();

        }

        private static void ReadSingleRow(IDataRecord record)
        {
            Console.WriteLine(String.Format("{0}", record[0]));
        }

      
    }
}
