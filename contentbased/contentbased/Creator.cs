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
        private List<User> users;
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
            string genre = null;
            string year = null;
            List<User> userlist = new List<User>();
            int userID = 0;
            int prevID = -1;
            User newUser = null;

            cmd.CommandText = "select users.[User-ID], ratings.isbn, ratings.[book-rating], [book-author], [year-of-publication], genre FROM users join ratings on users.[User-ID] = ratings.[User-ID] join books on ratings.[ISBN] = books.ISBN ORDER BY cast(users.[user-id] as int) asc";
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
                    genre = reader.GetString(5);
                    Console.WriteLine(author + " " + year + " " + genre + " " + userID);
                    
                    newUser.buildAuthorList(author);
                    newUser.buildGenreList(genre);
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
    }
}
