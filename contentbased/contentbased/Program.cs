using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace contentbased
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection myConnection = new SqlConnection("Server=localhost;Database=books;Integrated Security=true");
            Creator creator = new Creator();
            List<User> users = new List<User>();
            List<Book> books = new List<Book>();

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
            myConnection.Close();

            users = creator.createUserProfiles(myConnection);
            users.ElementAt(1).printUserProfile();
            books = creator.createBookProfiles(myConnection);
            books.ElementAt(1).printBookProfile();
            Console.ReadLine();
        }
    }
}
