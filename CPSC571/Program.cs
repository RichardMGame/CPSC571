using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace CPSC571
{
    class Program
    {
            static void Main(string[] args)
            {
                SqlConnection myConnection = new SqlConnection("Server=localhost;Database=CPSC571;Integrated Security=true");

                try
                {
                    Console.WriteLine("Connecting to Database...\n");
                    myConnection.Open();
                    Console.WriteLine("Connected to Database!\n");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Cannot connect to Database.\n");
                    Console.WriteLine(e.ToString());
                }

            

            Console.ReadLine();

                /*string connetionString = null;
                SqlConnection cnn ;
                connetionString = "Data Source=ALANHOANG;Initial Catalog=571;User ID=UserName;Password=Password";
                cnn = new SqlConnection(connetionString);
                try
                {
                    cnn.Open();
                    Console.WriteLine("Connected to Database!\n");
                    cnn.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Cannot connect to Database.\n");
                    Console.WriteLine(e.ToString());
                }*/
            }
        }
    }
