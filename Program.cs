using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using static System.Net.Sockets.Socket;
using System.Text.Json;




namespace ConsoleApp1
{
    public class Person
    {
        public string name { get; set; }
        public int age { get; set; }
        public string city { get; set; }

        internal class Program
        {
            static void Main(string[] args)
            {

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

                /*
                 * string query = "INSERT INTO myTable (column1, column2) VALUES (@value1, @value2)";
                   SqlCommand command = new SqlCommand(query, connection);
                   command.Parameters.AddWithValue("@value1", "data1");
                   command.Parameters.AddWithValue("@value2", "data2");
                   command.ExecuteNonQuery();
                */

                /*
                 
                 string query = "DELETE FROM myTable WHERE Id = @id";
                 SqlCommand command = new SqlCommand(query, connection);
                 command.Parameters.AddWithValue("@id", 1);
                 command.ExecuteNonQuery();*/

                connection.Close();
                //Console.WriteLine("ahihi");
            }
        }

        static void ExecuteClient()
        {
            string ip = "172.31.98.6";
            //IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = IPAddress.Parse(ip);//ipHost.AddressList[3];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 40674);

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            sender.Connect(localEndPoint);


            byte[] messageReceived = new byte[1024];
            int byteRecv = sender.Receive(messageReceived);
            Console.WriteLine("Message from Server -> {0}", Encoding.ASCII.GetString(messageReceived, 0, byteRecv));

            var per = new Person
            {
                name = "ChangAnh",
                age = 25,
                city = "Hanoi"
            };

            string jsonString = JsonSerializer.Serialize(per);
            byte[] messageSent = Encoding.ASCII.GetBytes(jsonString);
            int byteSent = sender.Send(messageSent);
            sender.Close();



        }
    }
}
