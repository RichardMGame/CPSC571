using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace contentbased
{
    class User
    {
        public int id;
        private List<String> authors;
        private List<String> genres;
        private List<String> years;
        private List<String> publishers;

        // constructor
        public User(int userID)
        {
            this.id = userID;
            this.authors = new List<String>();
            this.genres = new List<String>();
            this.years = new List<String>();
            this.publishers = new List<String>();
        }

        public int getID()
        {
            return this.id;
        }

        public void printUserProfile()
        {
            Console.WriteLine("USERID: " + this.id);
            Console.WriteLine("-------------------");
            foreach (String s in authors)
            {
                Console.WriteLine("Author: " + s);
            }
            foreach (String s in publishers)
            {
                Console.WriteLine("Publisher: " + s);
            }
            foreach (String s in years)
            {
                Console.WriteLine("Year: " + s);
            }
        }

        // builds a list of authors that the user has bought books from
        public void buildAuthorList(String author)
        {
            authors.Add(author);
        }

        // builds a list of genres that the user has bought books from
        public void buildGenreList(String genre)
        {
            genres.Add(genre);
        }

        // builds a list of years that the user has bought books from
        public void buildYearList(String year)
        {
            years.Add(year);
        }

        // builds a list of years that the user has bought books from
        public void buildPublisherList(String publisher)
        {
            publishers.Add(publisher);
        }
    }
}
