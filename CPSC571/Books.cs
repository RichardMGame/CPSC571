using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPSC571
{
    class Books
    {
        private long _isbn;
        public long Isbn
        {
            get
            {
                return this._isbn;
            }
        }
        private int _rating;
        public int Rating
        {
            get
            {
                return this._rating;
            }
        }

            public Books(long isbn, int rating)
        {
            this._isbn = isbn;
            this._rating = rating;
        }

}
}
