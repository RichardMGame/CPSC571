using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using System.Threading;

namespace contentbased
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(150, 43);
           
            Creator creator = new Creator();
            List<User> users = new List<User>();
            List<Book> books = new List<Book>();
            String input = "0";
            String filename = null;
            int numSuggestions = 10;
            int userId;
            double authorWeight = 5;
            double yearWeight = 1;
            double publisherWeight = 0.5;


            Console.WriteLine("####################################");
            Console.WriteLine("# DEFAULT SETTINGS ARE AS FOLLOWS  #");
            Console.WriteLine("# Author Weight    = 5             #");
            Console.WriteLine("# Year Weight      = 1             #");
            Console.WriteLine("# Publisher Weight = 0.5           #");
            Console.WriteLine("# Results are saved to results.csv #");
            Console.WriteLine("# in the local folder              #");
            Console.WriteLine("# Results are APPENDED             #");
            Console.WriteLine("####################################");

            SqlConnection myConnection = new SqlConnection("Server=localhost;Database=books;Integrated Security=true");
            try
            {
                myConnection.Open();
            }
            catch (SqlException e)
            {
                Console.WriteLine("Database could not be reached. Is it named books? The program will exit and need to be restarted.");
                Thread.Sleep(5000);
                Environment.Exit(1);
            }
            myConnection.Close();

            Console.WriteLine("Retrieving books and creating book profiles...");
            books = creator.createBookProfiles(myConnection);
            Console.WriteLine("Book profiles created.");
            
            while (true)
            {
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine("1. Edit Weighting of Values");
                Console.WriteLine("2. Edit Number of Suggestions to Retrieve");
                Console.WriteLine("3. Retrieve Suggestions for a Specific User");
                Console.WriteLine("4. Retrieve Suggestions for a List of Users from a File");
                Console.WriteLine("5. Delete Results File");
                Console.WriteLine("6. Quit");
                Console.WriteLine("-------------------------------------------------------");
                Console.Write("Please enter the number of your choice: ");
                input = Console.ReadLine();
                if (input.Equals("1"))
                {
                    Console.Write("Enter weight for author   : ");
                    authorWeight = Double.Parse(Console.ReadLine());
                    Console.Write("Enter weight for year     : ");
                    yearWeight = Double.Parse(Console.ReadLine());
                    Console.Write("Enter weight for publisher: ");
                    publisherWeight = Double.Parse(Console.ReadLine());
                }
                else if (input.Equals("2"))
                {
                    Console.Write("Enter number of suggestions to retrieve: ");
                    numSuggestions = Int32.Parse(Console.ReadLine());
                }
                else if (input.Equals("3"))
                {
                    Console.Write("Enter id of user to retrieve suggestions for: ");
                    userId = Int32.Parse(Console.ReadLine());
                    creator.findUserBookMatches(myConnection, books, userId, numSuggestions, authorWeight, yearWeight, publisherWeight);
                }
                else if (input.Equals("4"))
                {
                    Console.Write("Enter name of file to retrieve user ID list from: ");
                    filename = Console.ReadLine();
                    String line = null;
                    System.IO.StreamReader file = new System.IO.StreamReader(filename);
                    while ((line = file.ReadLine()) != null)
                    {
                        creator.findUserBookMatches(myConnection, books, Int32.Parse(line), numSuggestions, authorWeight, yearWeight, publisherWeight);
                        Console.WriteLine("#" + Int32.Parse(line));
                    }
                    file.Close();
                }
                else if (input.Equals("5"))
                {
                    File.Delete("results.csv");
                }
                else if (input.Equals("6"))
                {
                    break;
                }             
            }
        }
    }
}
