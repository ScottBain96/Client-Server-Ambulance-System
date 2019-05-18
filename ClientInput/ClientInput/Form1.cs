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
        //DATABASE CONNECTION, IT IS NOT SET AS A RELATIVE DATABASE DUE TO TIME CONSTRAINTS.
        //THE CONNECTIONSTRING HAS TO BE MODIFIED ACCORDINGLY, DATABASE FILE PROVIDED ALONG WITH THE PROJECT.
        //CAN ALSO MODIFY THE DATASOURCE INSTEAD, WAS HAVING SOME ISSUES ON MY PC WITH THIS PART OF THE CW

        
        SqlConnection conn = new SqlConnection(Properties.Settings.Default.KwikdataBaseConnectionString2);

        Patient patient = new Patient();
        public Form1()
        {
            InitializeComponent();
            txtStatus.ReadOnly = true;
            txtStatus.BackColor = System.Drawing.SystemColors.Window;

        }

        SimpleTcpServer server;

        //BUTTON TO START SERVER
        
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

                   //RETRIEVING THE MESSAGE AND SETTING UP THE VARIABLES OF THE PATIENT CLASS TO THEN USE IT ON THE DATABASE

                   string text1 = e.MessageString;
                   string[] mytext1 = text1.Split(',');
                   patient.name = mytext1[0];
                   patient.numberNHS = mytext1[1];
                   patient.address = mytext1[2];
                   patient.condition = mytext1[3].Remove(mytext1[3].Length-1);

                   //INSERT VALUES OF TEXTBOXES TO THE DATABASE

                   string query = "INSERT INTO LocalHelp(NAme, NumberNHS, Address, Condition) " +
                     "VALUES('" + patient.name + "', '" + patient.numberNHS + "', '" + patient.address + "', '" + patient.condition + "')";

                   using (SqlCommand command = new SqlCommand(query, conn))
                   {
                       conn.Open();
                       command.ExecuteNonQuery();
                       txtStatus.Text += "Added record for " + patient.name+Environment.NewLine;

                   }


                   //GET THE MEDICAL RECORD FOR THE NHS NUMBER
                  
                   string queryString = "SELECT * FROM MedicalRecords WHERE numberNHS=" + patient.numberNHS + ";";
                   SqlCommand commands =
                       new SqlCommand(queryString, conn);
                 
                   SqlDataReader reader = commands.ExecuteReader();


                   while (reader.Read())
                   {
                       ReadSingleRow((IDataRecord)reader);
                       
                   }


                   reader.Close();


                   if (conn != null)
                   {
                       conn.Close(); 

                   }
  
                
                   e.ReplyLine(string.Format("Record added for: {0}", patient.name+Environment.NewLine));
                   

               });


         


        }

        //STOP THE SERVER

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


        //SET THE RESULTS TEXTBOX TO DISPLAY ALL THE REQUIRED INFORMATION THAT WOULD BE SENT TO THE AMBULANCE.
        //USING THIS AS AN EXAMPLE OF WHAT DATA WOULD BE SENT.

        private void ReadSingleRow(IDataRecord record) //static
        {
         
            string test = (String.Format("NumberNHS: {0} ,Forename: {1} , Surname: {2} , Date of Birth: {3} ,  Address: {4} , Phone Number: {5}, Address of Ambulance Pickup: {6}, Condition: {7}", record[0], record[1], record[2], record[3], record[4], record[5], patient.address, patient.condition));
            txtResults.Text = test;
            

        }

       
        //THEIR FEEDBACK WOULD BE STORED AGAIN IN THE DATABASE AND LINKED WITH THE CALL RECORD
        //AGAIN ANOTHER EXAMPLE, SQL WOULD BE LINKED PROPERLY AS THIS IS JUST A PROTOTYPE
       
        private void btnInsert_Click(object sender, EventArgs e)
        {

            string query = "INSERT INTO ambulanceREports " +
                                "VALUES('" +patient.numberNHS + "', 'Example data of Action Taken', 'Example data of when it was done', 'Example data of where it was','Example data of time elapsed');";

           

            using (SqlCommand command = new SqlCommand(query, conn))
            {
                conn.Open();
                command.ExecuteNonQuery();
                txtStatus.Text += "feedback added from ambulance for " + patient.name + Environment.NewLine;

            }



        }
    }
}
