using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Model
{
    public class BookModel
    {
        private int BookID;
        public int id
        {
            get { return BookID; }
            set { BookID = value; }
        }

        private String BookTitle;
        public string title
        {
            get { return BookTitle; }
            set { BookTitle = value; }
        }

        private String BookAuthor;
        public string author
        {
            get { return BookAuthor; }
            set { BookAuthor = value; }
        }

        private String BookISBN;
        public string isbn
        {
            get { return BookISBN; }
            set { BookISBN = value; }
        }

        private String BookGenre;
        public string genre
        {
            get { return BookGenre; }
            set { BookGenre = value; }
        }

        private bool BookAvailability;
        public bool availability
        {
            get { return BookAvailability; }
            set { BookAvailability = value; }
        }
    }
}
