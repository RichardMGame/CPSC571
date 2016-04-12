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

        public List<String> Authors
        {
            get { return authors; }
            set { authors = value; }
        }
        private List<String> genres;

        public List<String> Genres
        {
            get { return genres; }
            set { genres = value; }
        }
        private List<String> years;

        public List<String> Years
        {
            get { return years; }
            set { years = value; }
        }
        private List<String> publishers;

        public List<String> Publishers
        {
            get { return publishers; }
            set { publishers = value; }
        }
        private List<KeyValuePair<Book, double>> suggestedBooks;

        private List<String> ownedBooks;

        public List<String> OwnedBooks
        {
            get { return ownedBooks; }
            set { ownedBooks = value; }
        }
        // constructor
        public User(int userID)
        {
            this.id = userID;
            this.authors = new List<String>();
            this.genres = new List<String>();
            this.years = new List<String>();
            this.publishers = new List<String>();
            this.suggestedBooks = new List<KeyValuePair<Book, double>>();
            this.ownedBooks = new List<String>();
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

        public void buildOwnedBooksList(String ISBN)
        {
            ownedBooks.Add(ISBN);
        }

        public void addSuggestedBook(Book book, double value)
        {
            KeyValuePair<Book, double> newPair = new KeyValuePair<Book, double>(book,value);
            suggestedBooks.Add(newPair);
        }

        public void sortSuggestedBooks()
        {
            suggestedBooks.Sort((x, y) => y.Value.CompareTo(x.Value));
        }

        public void printTopSuggestions()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(suggestedBooks.ElementAt(i).Key.ISBN + " " + suggestedBooks.ElementAt(i).Value);
            }
        }
    }
}
