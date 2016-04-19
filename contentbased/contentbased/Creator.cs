using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Threading;

namespace contentbased
{
    class Creator
    {
        public List<User> createUsers(SqlConnection connection)
        {
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            List<User> userlist = new List<User>();
            int userId;

            cmd.CommandText = "SELECT [User-ID] from Users ORDER BY cast([User-ID] as INT) asc";
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Connection = connection;

            connection.Open();

            reader = cmd.ExecuteReader();



            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Int32.TryParse(reader.GetString(0), out userId);
                    User newUser = new User(userId);
                    userlist.Add(newUser);
                }
            }
            else
            {
                Console.WriteLine("No rows found.");
            }

            connection.Close();
            foreach(User u in userlist)
                Console.WriteLine(u.id);
            return userlist;
        }

        public List<User> createUserProfiles(SqlConnection connection)
        {
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            string author = null;
            string publisher = null;
            string year = null;
            List<User> userlist = new List<User>();
            int userID = 0;
            int prevID = -1;
            User newUser = null;
            double rating = 5;
            cmd.CommandText = "select users.[User-ID], ratings.isbn, ratings.[book-rating], [book-author], [year-of-publication], [publisher] FROM users join ratings on users.[User-ID] = ratings.[User-ID] join books on ratings.[ISBN] = books.ISBN ORDER BY cast(users.[user-id] as int) asc";
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Connection = connection;

            connection.Open();
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Int32.TryParse(reader.GetString(0), out userID);
                    if (userID != prevID)
                    {
                        newUser = new User(userID);
                        userlist.Add(newUser);
                    }
                    author = reader.GetString(3);
                    year = reader.GetString(4);
                    publisher = reader.GetString(5);
                    Double.TryParse(reader.GetString(2), out rating);
                    rating = getRatingFactor(rating);
                    Tuple<String, double> authorRating = new Tuple<String, double>(author, rating);
                    newUser.buildAuthorList(authorRating);
                    newUser.buildPublisherList(publisher);
                    newUser.buildYearList(year);
                    prevID = userID;
                }
            }
            else
            {
                Console.WriteLine("User has bought no books and cannot be recommended anything.");
            }
                connection.Close();
           

            return userlist;
        }

        public List<Book> createBookProfiles(SqlConnection connection)
        {
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            string isbn = null;
            string genre = null;
            string author = null;
            string publisher = null;
            string year = null;
            string title = null;
            List<Book> bookList = new List<Book>();
            Book newBook = null;

            cmd.CommandText = "select * from Books";
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Connection = connection;

            connection.Open();
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    isbn = reader.GetString(0);
                    title = reader.GetString(1);
                    author = reader.GetString(2);
                    year = reader.GetString(3);
                    publisher = reader.GetString(4);
                    genre = reader.GetString(5);

                    newBook = new Book(isbn, author, genre, year, publisher, title);
                    bookList.Add(newBook);
                }
            }
            else
            {
                Console.WriteLine("No rows found.");
            }
            connection.Close();


            return bookList;
        }

        public User profileUser(SqlConnection connection, int userID)
        {
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            string author = null;
            string publisher = null;
            string year = null;
            string isbn = null;
            double rating = 5;
            User newUser = new User(userID);

            cmd.CommandText = "select users.[User-ID], ratings.isbn, ratings.[book-rating], [book-author], [year-of-publication], [publisher], books.[isbn] FROM users join ratings on users.[User-ID] = ratings.[User-ID] join books on ratings.[ISBN] = books.ISBN WHERE users.[USER-ID] = '" + userID + "' ORDER BY cast(users.[user-id] as int) asc";
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Connection = connection;

            connection.Open();
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Int32.TryParse(reader.GetString(0), out userID);
                    author = reader.GetString(3);
                    year = reader.GetString(4);
                    publisher = reader.GetString(5);
                    isbn = reader.GetString(6);
                    Double.TryParse(reader.GetString(2), out rating);
                    rating = getRatingFactor(rating);
                    Tuple<String,double> authorRating = new Tuple<String,double>(author,rating);

                    newUser.buildAuthorList(authorRating);
                    newUser.buildPublisherList(publisher);
                    newUser.buildYearList(year);
                    newUser.buildOwnedBooksList(isbn);
                    rating = 5;
                }
            }
            else
            {
                Console.WriteLine("User has bought no books and cannot be recommended anything.");
                connection.Close();
                return null;
            }
            connection.Close();


            return newUser;
        }

        // create a list of books to recommend to the user using the profiles created for the user and the books
        // ratings can be adjusted.
        // As for now, author > year > publisher
        //      author is affected by book ratings by a factor (low and high extremities are bigger factors)
        public void findUserBookMatches(SqlConnection connection, List<Book> bookList, int userID, int numSuggestions, double authorWeight, double yearWeight, double publisherWeight)
        {
            User user = profileUser(connection, userID);
            double bookScore = 0;
            if (user == null)
            {
                return;
            }
            foreach (Book book in bookList)
            {           
                foreach (Tuple<String,double> s in user.Authors)
                {
                    // because the author is the most important weighted part the recommendation, we apply
                    // ratings the user has given the author to the bookScore
                    if (s.Item1.ToLower().Equals(book.Author.ToLower()))
                    {
                        bookScore += (authorWeight*s.Item2);
                    }
                }
                foreach (String s in user.Years)
                {
                    if (s.Equals(book.Year))
                    {
                        bookScore += yearWeight;
                    }
                }
                foreach (String s in user.Publishers) 
                {
                    if (s.ToLower().Equals(book.Publisher.ToLower()))
                    {
                        bookScore += publisherWeight;
                    }
                }
                // if the book is already owned, we rate it 0 as the user will not want it suggested to him.
                // this is because the dataset we are using is not properly formed
                // we must assume the user doesn't have a strong opinion about this book either way
                foreach (String s in user.OwnedBooks)
                {
                    if (s.ToLower().Equals(book.ISBN.ToLower()))
                    {
                        bookScore = 0;
                    }
                }
                if (bookScore != 0)
                {
                    user.addSuggestedBook(book, bookScore);
                }
                bookScore = 0;
            }
            user.sortSuggestedBooks();
            user.printToTextFile(numSuggestions);
            user.printTopSuggestions(numSuggestions);
        }

        // we assume a rating of 0 is UNRATED and give it a rating factor equal to a rating of 5 (no strong opinion of the book).
        private double getRatingFactor(double rating)
        {
            double ratingFactor = 0;
            if (rating == 0)
            {
                ratingFactor = 1.0;
            }
            else if (rating == 1)
            {
                ratingFactor = 0.1;
            }
            else if (rating == 2)
            {
                ratingFactor = 0.2;
            }
            else if (rating == 3)
            {
                ratingFactor = 0.4;
            }
            else if (rating == 4)
            {
                ratingFactor = 0.8;
            }
            else if (rating == 5)
            {
                ratingFactor = 1.0;
            }
            else if (rating == 6)
            {
                ratingFactor = 1.25;
            }
            else if (rating == 7)
            {
                ratingFactor = 2.0;
            }
            else if (rating == 8)
            {
                ratingFactor = 2.5;
            }
            else if (rating == 9)
            {
                ratingFactor = 5.0;
            }
            else if (rating == 10)
            {
                ratingFactor = 10;
            }
            return ratingFactor;
        }
    }
}
