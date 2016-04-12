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
            //List<User> SortedUsers = userlist.OrderBy(o => o.id).ToList();
            foreach(User u in userlist)
                Console.WriteLine(u.id);
            return userlist;
            //return SortedUsers;
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
                    //Console.WriteLine(author + " " + year + " " + publisher + " " + userID);
                    
                    newUser.buildAuthorList(author);
                    newUser.buildPublisherList(publisher);
                    newUser.buildYearList(year);
                    prevID = userID;
                }
            }
            else
            {
                Console.WriteLine("No rows found.");
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
                    author = reader.GetString(2);
                    year = reader.GetString(3);
                    publisher = reader.GetString(4);
                    genre = reader.GetString(5);

                    newBook = new Book(isbn, author, genre, year, publisher);
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
                    //Console.WriteLine(author + " " + year + " " + publisher + " " + userID);

                    newUser.buildAuthorList(author);
                    newUser.buildPublisherList(publisher);
                    newUser.buildYearList(year);
                    newUser.buildOwnedBooksList(isbn);
                }
            }
            else
            {
                Console.WriteLine("No rows found.");
            }
            connection.Close();


            return newUser;
        }

        // create a list of books to recommend to the user using the profiles created for the user and the books
        // this approach will be revamped a few times as I develop
        // current version:
        //          authors > years > publishers 
        //          don't list a book if they own it
        //          need to incorporate ratings (assume 0 is a rating of "5" or average, and multiply the filter value by pos/neg depending on rating)
        //             may need to add rating to all profile values and multiply by 0.6/0.7/.../1.3/1.4...could find a better way
        public void findUserBookMatches(SqlConnection connection, List<Book> bookList, int userID)
        {
            User user = profileUser(connection, userID);
            double bookScore = 0;

            foreach (Book book in bookList)
            {           
                foreach (String s in user.Authors)
                {
                    if (s.ToLower().Equals(book.Author))
                    {
                        bookScore += 5;
                    }
                }
                foreach (String s in user.Years)
                {
                    if (s.Equals(book.Year))
                    {
                        bookScore += 1;
                    }
                }
                foreach (String s in user.Publishers) 
                {
                    if (s.ToLower().Equals(book.Publisher))
                    {
                        bookScore += 0.5;
                    }
                }
                // if the book is already owned, we rate it 0 as the user will not want it suggested to him.
                foreach (String s in user.OwnedBooks)
                {
                    if (s.ToLower().Equals(book.ISBN))
                    {
                        bookScore = 0;
                    }
                }
                Console.WriteLine(book.Author);
                user.addSuggestedBook(book, bookScore);
                bookScore = 0;
            }
            user.sortSuggestedBooks();
            user.printTopSuggestions();
        }
    }
}
