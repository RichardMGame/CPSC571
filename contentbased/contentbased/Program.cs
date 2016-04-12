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
            Console.SetWindowSize(150, 43);
            SqlConnection myConnection = new SqlConnection("Server=localhost;Database=books;Integrated Security=true");
            Creator creator = new Creator();
            List<User> users = new List<User>();
            List<Book> books = new List<Book>();
            String input = "0";
            String numSuggestions = "10";
            Console.WriteLine("Retrieving books and creating book profiles...");
            books = creator.createBookProfiles(myConnection);
            Console.WriteLine("Book profiles created.");

            while (true)
            {
                Console.Write("Enter USERID to find suggestions for or type q to quit: ");
                input = Console.ReadLine();
                Console.Write("Enter # of top suggestions to display to screen: ");
                numSuggestions = Console.ReadLine();
                if (input.Equals("q"))
                {
                    break;
                }
                Console.WriteLine("===============");
                creator.findUserBookMatches(myConnection, books, Int32.Parse(input), Int32.Parse(numSuggestions));
                Console.WriteLine("===============");
            }

        }
    }
}
