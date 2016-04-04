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
        private string genre;
        private string year;
        private string publisher;

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
