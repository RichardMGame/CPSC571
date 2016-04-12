using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPSC571
{
    class UserBook
    {
        public string uid { get;}
        public List<Books> books { get; }

        public UserBook(string uid, List<Books> books)
        {
            this.uid = uid;
            this.books = books;
        }
    }
}
