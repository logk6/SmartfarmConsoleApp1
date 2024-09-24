using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using static System.Net.Sockets.Socket;
using System.Text.Json;
//using Newtonsoft.Json;
using System.Collections;
using Smartfarm1;



namespace Smartfarm1
{

    public class Req
    {
        public string mess { get; set; }
        public int rgb1 { get; set; }
        public int rgb2 { get; set; }
        public int rgb3 { get; set; }
    }

    public class Person
    {
        public string name { get; set; }
        public int age { get; set; }
        public string city { get; set; }

    }

    internal class Program
    {
        static void Main(string[] args)
        {
            /*
                            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=mvcTest1;Integrated Security=True;";
                            SqlConnection connection = new SqlConnection(connectionString);
                            connection.Open();

                            string query = "SELECT * FROM FarmStatus";
                            SqlCommand command = new SqlCommand(query, connection);
                            SqlDataReader reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                Console.WriteLine(reader["CO2"].ToString());
                            }
                            */

            /*
             string query = "INSERT INTO FarmStatus (DateTime, CO2, SoilMoisture, Light_0x5C) VALUES (@value1, @value2, @value3, @value4)";
             SqlCommand command = new SqlCommand(query, connection);
             command.Parameters.AddWithValue("@value1", DateTime.Now);
             command.Parameters.AddWithValue("@value2", 1320);
             command.Parameters.AddWithValue("@value3", 70);
             command.Parameters.AddWithValue("@value4", 63);
             command.ExecuteNonQuery();
             */

            /*
             string query = "DELETE FROM myTable WHERE Id = @id";
             SqlCommand command = new SqlCommand(query, connection);
             command.Parameters.AddWithValue("@id", 1);
             command.ExecuteNonQuery();*/

            //connection.Close();
            //Console.WriteLine("ahihi");
            //string str = { "humiin": 51.27, "co2val": 1198, "tempin": 28.21, "light_inval": 12, "soilval": 2503};
            /**/

            ExecuteClient();
        }


        static void ExecuteClient()
        {//     172.31.98.6             172.31.99.116   
            ArrayList farmlist = new ArrayList();
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=mvcTest1;Integrated Security=True;";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = "SELECT * FROM Smartfarm";
            SqlCommand command1 = new SqlCommand(query, connection);
            SqlDataReader reader = command1.ExecuteReader();
            while (reader.Read())
            {
                Smartfarm fm = new Smartfarm();
                //Console.WriteLine(reader["IPAddress"].ToString());
                fm.Id = Convert.ToInt32(reader["Id"]);
                fm.Name = reader["Name"].ToString();
                fm.IP = reader["IPAddress"].ToString();
                farmlist.Add(fm);
            }
            connection.Close();


            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            foreach (Smartfarm fm in farmlist)
            {
                //"172.31.98.24"
                //IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                string ip = fm.IP;
                IPAddress ipAddr = IPAddress.Parse(ip);//ipHost.AddressList[3];
                IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 40674);

                Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                sender.ReceiveTimeout = 40000;
                sender.Connect(localEndPoint);

                Req req = new Req { mess = "measre" };
                string reqstrg = JsonSerializer.Serialize<Req>(req);
                byte[] messSend = Encoding.ASCII.GetBytes(reqstrg);
                sender.Send(messSend);

                byte[] messageReceived = new byte[2048];
                int byteRecv = sender.Receive(messageReceived);
                string strg = Encoding.ASCII.GetString(messageReceived).Replace("\0", string.Empty);

                Console.WriteLine(strg);
                var cate = JsonSerializer.Deserialize<FarmStatus>(strg); cate.SFID = 1;

                sender.Close();

                string query1 = "INSERT INTO FarmStatus (DateTime, SFID, CO2, SoilMoisture, Light_0x5C, Humidity, Temperature) VALUES (@value1, @value2, @value3, @value4, @value5, @value6, @value7)";
                SqlCommand command = new SqlCommand(query1, connection1);

                command.Parameters.AddWithValue("@value1", DateTime.Now);
                command.Parameters.AddWithValue("@value2", fm.Id);
                command.Parameters.AddWithValue("@value3", cate.CO2);
                command.Parameters.AddWithValue("@value4", cate.SoilMoisture);
                command.Parameters.AddWithValue("@value5", cate.Light_0x5C);
                command.Parameters.AddWithValue("@value6", cate.Humidity);
                command.Parameters.AddWithValue("@value7", cate.Temperature);
                command.ExecuteNonQuery();

            }
            connection1.Close();

        }
    }
}
