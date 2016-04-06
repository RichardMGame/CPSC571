using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPSC571
{
    class Books
    {
        public long isbn { get; set; }
        public int rating { get; set; }

        public Books(long isbn, int rating)
        {
            this.isbn = isbn;
            this.rating = rating;
        }

    }
}
