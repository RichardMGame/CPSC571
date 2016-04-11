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


        public static void ClearLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop - 1);
        }

        static void Main(string[] args)
        {
            SqlConnection myConnection = new SqlConnection("Server=localhost;Database=CPSC571;Integrated Security=true");
            //list of users
            var list = new List<string>();
            //list of users and there books
            var list3 = new List<UserBook>();

            // gets user id from ratings
            string queryString = "SELECT DISTINCT r.[User-Id] FROM dbo.Ratings as r";
            SqlDataReader myReader = null;
            SqlCommand myCommand = myConnection.CreateCommand();
            myCommand.CommandText = queryString;
            myConnection.Open();
            myReader = myCommand.ExecuteReader();
            Console.WriteLine("Retrieving all users");
            bool first = true;
            while (myReader.Read())
            {
                list.Add(myReader.GetString(0));
            }
            Console.WriteLine("All users retrieved");
            myConnection.Close();
            myReader.Close();
            myReader = null;
            myCommand = null;
            string uid = null;
            // gets a list of isbn for each user
            Console.WriteLine("Retrieving user's books");
            Console.WriteLine("0");
            double old_current = 0;
            double current = 0;
            for (int i = 0; i < list.Count * 0.01; i++)
            {

                current = (i / (list.Count * 0.01)) * 100;
                //Console.WriteLine(current);
                if (old_current < current)
                {
                    ClearLine();
                    Console.WriteLine("{0:N2}" + "%", current);
                    old_current = current;
                }
                uid = list[i];
                //Console.WriteLine("Books for " + uid);
                var list2 = new List<Books>();
                string queryRatings = "SELECT b.ISBN, r.[Book-Rating] FROM dbo.Books as b, dbo.Ratings as r WHERE r.[User-ID] = @UID AND b.ISBN = r.ISBN";
                myCommand = myConnection.CreateCommand();
                myCommand.CommandText = queryRatings;
                SqlParameter p = new SqlParameter();
                p.ParameterName = "@UID";
                p.Value = uid;
                myCommand.Parameters.Add(p);
                myConnection.Open();
                myReader = myCommand.ExecuteReader();
                while (myReader.Read())
                {
                    char[] charsToTrim = { 'X', 'x' };
                    long result;
                    if (Int64.TryParse(myReader.GetString(0).TrimEnd(charsToTrim), out result))
                    {
                        Books temp = new Books(result, Int32.Parse(myReader.GetString(1)));
                        list2.Add(temp);
                    }
                }

                UserBook t = new UserBook(uid, list2);
                list3.Add(t);

                list2 = null;
                myConnection.Close();
                myReader.Close();
                myCommand = null;
                myReader = null;
                myCommand = null;

            }
            ClearLine();
            Console.WriteLine("100" + "%", current);
            Console.WriteLine("All user's books retrieved");

            Console.WriteLine("Building CF algorithm table");

            SlopeOne so = new SlopeOne();
            //so.Test();
            //int count = 0;
            var list4 = new List<Dictionary<long, float>>();
            foreach (var users in list3)
            {
                UserBook ub = users;
                Dictionary<long, float> userRating = new Dictionary<long, float>();
                foreach (Books book in ub.books)
                {
                    try
                    {
                        userRating.Add(book.Isbn, ((float)book.Rating));
                    }
                    catch (ArgumentException)
                    {
                        // Console.WriteLine("Unable to enter in rating");
                    }
                }
                list4.Add(userRating);
                so.AddUserRatings(userRating);

            }

            Console.WriteLine("CF algorithm table built");


            for (int i = 0; i < list4.Count; i++)
            {

                Dictionary<long, float> user = list4[i];

                IDictionary<long, float> Predictions = so.Predict(user);


                int count = 0;
                foreach (var rating in Predictions)
                {

                    if (!float.IsNaN(rating.Value))
                    {
                        Console.WriteLine("User: " + list3[i].uid + " Book: " + rating.Key + " Rating: " + rating.Value);
                        Console.WriteLine("#############################################################################");
                        count++;
                        if (count > 10) break;
                    }

                    
                }


            }
            Console.WriteLine("Press any key to terminate program");
            Console.ReadKey();

        }
    }
}
