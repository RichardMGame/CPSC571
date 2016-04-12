using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace contentbased
{
    class Book
    {
        public string ISBN;
        private string author;

        public string Author
        {
            get { return author; }
            set { author = value; }
        }
        private string genre;

        public string Genre
        {
            get { return genre; }
            set { genre = value; }
        }
        private string year;

        public string Year
        {
            get { return year; }
            set { year = value; }
        }
        private string publisher;

        public string Publisher
        {
            get { return publisher; }
            set { publisher = value; }
        }

        public Book(string ISBN, string author, string genre, string year, string publisher)
        {
            this.ISBN = ISBN;
            this.author = author;
            this.genre = genre;
            this.year = year;
            this.publisher = publisher;
        }

        public void printBookProfile()
        {
            Console.WriteLine("ISBN: " + this.ISBN);
            Console.WriteLine("-------------------");
            Console.WriteLine("Author: " + this.author);
            Console.WriteLine("Genre: " + this.genre);
            Console.WriteLine("Year: " + this.year);
            Console.WriteLine("Publisher: " + this.publisher);
        }



    }
}
