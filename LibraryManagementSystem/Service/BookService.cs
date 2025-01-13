using LibraryManagementSystem.Data;
using LibraryManagementSystem.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Service
{
    public class BookService
    {
        #region Private Field
        private readonly LibraryDbContext BookDbContext;
        //readonly - The field can only be assigned once, either at declaration or in the constructor.
        #endregion


        #region Constructor
        public BookService()
        {
            BookDbContext = new LibraryDbContext(); //Initializes the BookDbContext field.
        }
        #endregion


        #region Get All Books List Method
        public List<BookEntity> GetAll() //Retrieve all Books from the database.
        {
            try
            {
                return BookDbContext.Books.ToList();
                //Retrieves all Book records from the Books table in the database and converts them to a list.
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                //If an exception occurs catch and display in msgbox.
            }
            return new List<BookEntity>();
        }
        #endregion


        #region Add Book Method
        public bool AddBook(BookEntity currentBook) //Set new Book values from currentBook
        {
            try
            {
                var book = new BookEntity(); //Created new object Book.

                book.BookTitle = currentBook.BookTitle;
                book.BookAuthor = currentBook.BookAuthor;
                book.BookISBN = currentBook.BookISBN;
                book.BookGenre = currentBook.BookGenre;
                book.BookAvailability = currentBook.BookAvailability;
                book.CategoryID = currentBook.CategoryID;

                BookDbContext.Books.Add(book); //New Book is added to the Books collection of BookDbContext.
                BookDbContext.SaveChanges(); //New Book to the database.

                return true; // Operation succeeded
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message); //If an exception occurs catch and display in msgbox.
                return false; // Operation failed
            }
        }
        #endregion


        #region Update Book Method
        public bool UpdateBook(BookEntity bookupdate) // Get updated details of the Book.
        {
            try
            {
                var existingBook = BookDbContext.Books.Find(bookupdate.BookID); // Find the existing Book in the Books collection using the BookID.
                if (existingBook != null) // If the Book is found, update only non-null properties.
                {
                    // Only update properties if they are provided (not null or default value).
                    if (!string.IsNullOrEmpty(bookupdate.BookTitle))
                        existingBook.BookTitle = bookupdate.BookTitle;

                    if (!string.IsNullOrEmpty(bookupdate.BookAuthor))
                        existingBook.BookAuthor = bookupdate.BookAuthor;

                    if (!string.IsNullOrEmpty(bookupdate.BookISBN))
                        existingBook.BookISBN = bookupdate.BookISBN;

                    if (!string.IsNullOrEmpty(bookupdate.BookGenre))
                        existingBook.BookGenre = bookupdate.BookGenre;

                    // Directly update BookAvailability as a boolean value.
                    existingBook.BookAvailability = bookupdate.BookAvailability;

                    if (bookupdate.CategoryID > 0) // Assuming CategoryID is greater than 0 when valid.
                        existingBook.CategoryID = bookupdate.CategoryID;

                    BookDbContext.SaveChanges(); // Save changes to the database.
                    return true;
                }
                else
                {
                    return false; // Book not found.
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return false; // Return false if an exception occurs.
            }
        }
        #endregion


        #region Delete Book Method
        public bool DeleteBook(int bookid) //Get which is the ID of the Book to be deleted.
        {
            try
            {
                var book = BookDbContext.Books.Find(bookid); //Finds the Book in the Books collection using the BookID.
                if (book != null) //If the Book is found, it is removed from the Books table.
                {
                    BookDbContext.Books.Remove(book); //Delete Operation.
                    BookDbContext.SaveChanges(); //Delete Book from database.
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return false;
            }
        }
        #endregion


        #region Search Books Method
        public BookEntity SearchBook(int bookid) //Get which is the ID of the Book to search for.
        {
            try
            {
                var existingBook = BookDbContext.Books.Find(bookid); //Finds the Book in the Books collection using the BookID.
                if (existingBook != null) //If the Book is found,
                {
                    BookEntity book = new BookEntity
                    {
                        BookID = existingBook.BookID,
                        BookTitle = existingBook.BookTitle,
                        BookAuthor = existingBook.BookAuthor,
                        BookISBN = existingBook.BookISBN,
                        BookGenre = existingBook.BookGenre,
                        BookAvailability = existingBook.BookAvailability,
                        Category = existingBook.Category,
                    };
                    return book;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            return null;
        }
        #endregion


        #region Check Book part of an active Transaction
        public bool IsBookInTransaction(int bookId)
        {
            using (var context = new LibraryDbContext()) //Creates a new instance of LibraryDbContext
            {
                //LINQ Query:
                return context.Transactions.Any(t => t.BookID == bookId && t.ReturnDate == null); //Checks if there are any records in the Transactions table where:
            }
        }
        #endregion
    }
}
