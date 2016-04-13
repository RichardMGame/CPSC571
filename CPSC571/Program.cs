using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;

namespace CPSC571
{

    class Temp
    {
        public string isbn;
        public string rating;

        public Temp(string s, string r)
        {
            this.isbn = s;
            this.rating = r;
        }
    }

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

            string path = System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName;
            Console.WriteLine(path);

             SqlConnection myConnection = new SqlConnection("Server=localhost;Database=CPSC571;Integrated Security=true");
            //list of users
            var userList = new List<string>();
            //list of users and there books
            var userBookRatingList = new List<UserBook>();

            // gets user id from ratings
            string queryString = "SELECT DISTINCT r.[User-Id] FROM dbo.Ratings as r";
            SqlDataReader myReader = null;
            SqlCommand myCommand = myConnection.CreateCommand();
            myCommand.CommandText = queryString;
            myConnection.Open();
            myReader = myCommand.ExecuteReader();
            Console.WriteLine("Retrieving 1000 users");
            int count = 0;
            while (myReader.Read() && count < 1000)
            {
                userList.Add(myReader.GetString(0));
                count++;
            }

            Console.WriteLine("1000 users retrieved");
            myConnection.Close();
            myReader.Close();
            myReader = null;
            myCommand = null;

            // gets a list of isbn for each user
            Console.WriteLine("Retrieving user's books (100)");

            queryString = "SELECT DISTINCT r.ISBN FROM dbo.Ratings as r";
            myCommand = myConnection.CreateCommand();
            myCommand.CommandText = queryString;
            myConnection.Open();
            myReader = myCommand.ExecuteReader();
            count = 0;
            var bookList = new List<string>();
            while (myReader.Read())
            {
                bookList.Add(myReader.GetString(0));
                count++;
            }

            myConnection.Close();
            myReader.Close();
            myReader = null;
            myCommand = null;

            Console.WriteLine("Retrieved user's books (100)");
            Console.WriteLine("Retrieving user ratings for those books");

            string uid = null;
            string bookISBN = null;
            double old_current = 0;
            double current = 0;
            Console.WriteLine("0%");
            for (int i = 0; i < userList.Count; i++)
            {
                var temp = new List<Temp>();
                var userBookList = new List<Books>();

                current = ((double)i / userList.Count) * 100;

                //Console.WriteLine(current);

                if (old_current < current)
                {

                    ClearLine();
                    Console.WriteLine("{0:N2}" + "%", current);
                    old_current = current;
                }

                uid = userList[i];

                //Console.WriteLine("Book: " + bookISBN);
                string queryRatings = "SELECT r.ISBN, r.[Book-Rating] FROM dbo.Ratings as r WHERE r.[User-ID] = @UID";
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
                    temp.Add(new Temp(myReader.GetString(0), myReader.GetString(1)));
                }

                myConnection.Close();
                myReader.Close();
                myCommand = null;
                myReader = null;
                myCommand = null;

                for (int a = 0; a < temp.Count; a++)
                {
                    char[] charsToTrim = { 'X', 'x' };
                    long result;

                    for (int b = 0; b < bookList.Count; b++)
                    {
                        if (bookList[b] == temp[a].isbn)
                        {
                            if (Int64.TryParse(temp[a].isbn.TrimEnd(charsToTrim), out result))
                            {
                                Books gh = new Books(result, Int32.Parse(temp[a].rating));
                                userBookList.Add(gh);
                            }
                        }
                    }
                }

                UserBook t = new UserBook(uid, userBookList);
                userBookRatingList.Add(t);

            }

            Console.WriteLine("100%");

            Console.WriteLine("All user's books retrieved");

            Console.WriteLine("Building CF algorithm table");

            SlopeOne so = new SlopeOne();
            //so.Test();
            //int count = 0;
            var list4 = new List<Dictionary<long, float>>();
            foreach (var users in userBookRatingList)
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

            string delimeter = ",";
            StringBuilder sb = new StringBuilder();
            string filePath =@""+path+"\\result.cvs";
            sb.AppendLine("UID" + delimeter + "ISBN" + delimeter + "Rating");

            Console.WriteLine("Generating and Printing Results to file");
            for (int i = 0; i < list4.Count; i++)
            {

                Dictionary<long, float> user = list4[i];

                IDictionary<long, float> predictions = so.Predict(user);

                var items = from pair in predictions
                            orderby pair.Value descending
                            select pair;

                count = 0;

                sb.AppendLine(userBookRatingList[i].uid);
                foreach (var rating in items)
                {
                    if (!float.IsNaN(rating.Value))
                    {
                        //Console.WriteLine(" Book: " + rating.Key + " Rating: " + rating.Value);
                        sb.AppendLine(" " + delimeter + rating.Key + delimeter + rating.Value + delimeter);
                        count++;
                        if (count > 5) break;
                    }
                }
            }
            File.WriteAllText(filePath, sb.ToString());

            Console.WriteLine("Press any key to terminate program");
            Console.ReadKey();

        }
    }
}
