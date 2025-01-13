using LibraryManagementSystem.Commands;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Entity;
using LibraryManagementSystem.Model;
using LibraryManagementSystem.Service;
using LibraryManagementSystem.View.Popups;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using LibraryManagementSystem.MessageBox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Net;

namespace LibraryManagementSystem.ViewModel
{
    public class BookViewModel : INotifyPropertyChanged
    {

        #region PropertyChanged Event
        public event PropertyChangedEventHandler? PropertyChanged; //This is an event that is triggered whenever a property value changes in the view model.
        #endregion


        #region onPropertyChanged Method 
        private void onPropertyChanged(string propertyName) //Parameter is the name of the property that changed.
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        //Store data that will be exposed to the view via public properties.
        #region Private Fields
        private ObservableCollection<BookModel> bookListInDisplay;  //This collection holds a list of BookModel objects that are currently displayed in the UI.
        private BookEntity currentBookInDisplay;
        private BookService bookService; //This is an instance of the BookService class, which likely handle CRUD Operations
        private ICollectionView bookCollectionView; //Collections to support current record management, custom sorting, filtering, and grouping.

        private ObservableCollection<CategoryModel> _categories; // //This collection holds a list of BookModel objects for Category Combobox
        private CategoryModel _selectedCategory; //This field holds the currently selected category in the Combobox
        #endregion


        #region Constructor
        public BookViewModel()
        {
            currentBookInDisplay = new BookEntity();
            bookService = new BookService();
            LoadData();

            saveCommand = new RelayCommand(SaveBook);
            searchCommand = new RelayCommand(SearchBook);
            updateCommand = new RelayCommand(UpdateBook);
            deleteCommand = new RelayCommand(DeleteBook);

            ClickAddBookComand = new ClickEventCommand(ClickAddBook);
            ClickUpdateBookCommand = new ClickEventCommand(ClickUpdateBook);
            MouseUpCloseCommand = new ClickEventCommand(OnMouseUpClose);

            LoadCategoriesCommand = new RelayCommand(async () => await LoadCategories());
            Categories = new ObservableCollection<CategoryModel>();

            // Initialize with default values
            CurrentBookInDisplay = new BookEntity
            {
                BookAvailability = true  // Default to Available
            };
        }
        #endregion


        #region Properties
        public BookEntity CurrentBookInDisplay
        {
            get { return currentBookInDisplay; }
            set 
            { 
                currentBookInDisplay = value; //returns the current value
                onPropertyChanged(nameof(CurrentBookInDisplay)); //any UI elements bound to this property will be updated accordingly.
                //onPropertyChanged("CurrentBookInDisplay"); 
            }
        }

        public ObservableCollection<BookModel> BookListInDisplay
        {
            get { return bookListInDisplay; }
            set 
            { 
                bookListInDisplay = value;
                onPropertyChanged(nameof(BookListInDisplay));
                //onPropertyChanged("BookListInDisplay"); 
            }
        }

        public ObservableCollection<CategoryModel> Categories
        {
            get { return _categories; }
            set 
            { 
                _categories = value; 
                onPropertyChanged(nameof(Categories)); 
            }
        }

        public CategoryModel SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    onPropertyChanged(nameof(SelectedCategory));

                    // Update the current book's genre and category ID
                    if (CurrentBookInDisplay != null && _selectedCategory != null)
                    {
                        CurrentBookInDisplay.BookGenre = _selectedCategory.name;
                        CurrentBookInDisplay.CategoryID = _selectedCategory.id;
                    }
                }
            }
        }
        #endregion


        //handle events and trigger methods in the ViewModel.
        #region Commands for handle event
        public ICommand ClickAddBookComand { get; set; }

        public ICommand ClickUpdateBookCommand { get; set; }

        public ICommand MouseUpCloseCommand { get; }

        // Command for fetching categories from the database
        public ICommand LoadCategoriesCommand { get; }
        #endregion


        #region Command Methods
        private void ClickAddBook(object parameter)
        {
            AddBookView addBookView = new AddBookView();
            addBookView.ShowDialog();
        }
        private void ClickUpdateBook(object parameter)
        {
            UpdateBookView updateBookView = new UpdateBookView();
            updateBookView.ShowDialog();
        }
        private void OnMouseUpClose(object parameter)
        {
            if (parameter is Window window)
            {
                window.Close();
                LoadData();
            }
        }
        #endregion


        //converts a list of BookEntity objects to a list of BookModel objects.
        #region ConvertToDisplayInBookList Method
        private List<BookModel> ConvertToDisplayInBookList(List<BookEntity> bookList)
        {
            return bookList.Select(book => new BookModel
            {
                //Maps properties from BookEntity
                id = book.BookID,
                title = book.BookTitle,
                author = book.BookAuthor,
                isbn = book.BookISBN,
                genre = book.BookGenre,
                availability = book.BookAvailability
            }).ToList();
            //Returns the list of BookModel objects as a List<BookModel>
        }
        #endregion


        #region LoadData Method
        //loads data into the BookListInDisplay collection
        private void LoadData()
        {
            try
            {
                var books = ConvertToDisplayInBookList(bookService.GetAll());
                Application.Current.Dispatcher.Invoke(() =>
                {
                    BookListInDisplay = new ObservableCollection<BookModel>(books);
                    onPropertyChanged(nameof(BookListInDisplay));
                });
            }
            catch (Exception ex)
            {
                MessageBoxOK msg = new MessageBoxOK();
                msg.megboxmsg.Text = $"Error loading members: {ex.Message}";
                msg.ShowDialog();
            }
        }


        private async Task LoadCategories() // Load Categories from Database
        {
            using (var context = new LibraryDbContext()) 
            {
                var categories = await context.Categories
                                              .Select(c => new CategoryModel //Projects each category entity (c) from the database into a new CategoryModel object.
                                              {
                                                  id = c.CategoryID,
                                                  name = c.CategoryName
                                              })
                                              .ToListAsync(); 

                Categories.Clear(); //Clears any existing items in the Categories observable collection.
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }

                // Set the default selected item to the first category
                if (Categories.Any())
                {
                    SelectedCategory = Categories.First();
                }
            }
        }

        #endregion


        #region Relay Commands
        private RelayCommand saveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                return saveCommand ?? (saveCommand = new RelayCommand(param => SaveBook()));
            }
        }

        private RelayCommand updateCommand;
        public RelayCommand UpdateCommand
        {
            get
            {
                return updateCommand ?? (updateCommand = new RelayCommand(param => UpdateBook()));
            }
        }

        private RelayCommand deleteCommand;
        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ?? (deleteCommand = new RelayCommand(DeleteBook));
            }
        }

        private RelayCommand searchCommand;
        public RelayCommand SearchCommand
        {
            get
            {
                return searchCommand;
            }
        }
        #endregion


        #region Command Methods
        private void SaveBook()
        {
            try
            {
                MessageBoxError msgerror = new MessageBoxError();

                if (string.IsNullOrWhiteSpace(CurrentBookInDisplay.BookTitle))
                {
                    msgerror.megboxmsg.Text = "Title is required";
                    msgerror.ShowDialog();
                }
                else if (string.IsNullOrWhiteSpace(CurrentBookInDisplay.BookAuthor))
                {
                    msgerror.megboxmsg.Text = "Author is required";
                    msgerror.ShowDialog();
                }
                else if (string.IsNullOrWhiteSpace(CurrentBookInDisplay.BookISBN))
                {
                    msgerror.megboxmsg.Text = "ISBN is required";
                    msgerror.ShowDialog();
                }
                else if (!IsValidISBN(CurrentBookInDisplay.BookISBN))
                {
                    msgerror.megboxmsg.Text = "Invalid ISBN";
                    msgerror.ShowDialog();
                }

                else
                {
                    var IsSaved = bookService.AddBook(CurrentBookInDisplay);
                    LoadData();

                    if (IsSaved)
                    {
                        CurrentBookInDisplay = new BookEntity(); // Clear the form
                        MessageBoxOK msg = new MessageBoxOK();
                        msg.megboxmsg.Text = "Book saved successfully !";
                        msg.ShowDialog();
                    }
                    else
                    {
                        MessageBoxOK msg = new MessageBoxOK();
                        msg.megboxmsg.Text = "Book Save operation failed";
                        msg.ShowDialog();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBoxOK msg = new MessageBoxOK();
                msg.megboxmsg.Text = $"Error saving book: {ex.Message}";
                msg.ShowDialog();
            }
        }


        private void UpdateBook()
        {
            try
            {
                MessageBoxError msgerror = new MessageBoxError();

                // Numeric and format validations
                if (CurrentBookInDisplay.BookID <= 0)
                {
                    msgerror.megboxmsg.Text = "Invalid BookID";
                    msgerror.ShowDialog();
                }
                else if (string.IsNullOrWhiteSpace(CurrentBookInDisplay.BookTitle))
                {
                    msgerror.megboxmsg.Text = "Title is required";
                    msgerror.ShowDialog();
                }
                else if (string.IsNullOrWhiteSpace(CurrentBookInDisplay.BookAuthor))
                {
                    msgerror.megboxmsg.Text = "Author is required";
                    msgerror.ShowDialog();
                }
                else if (string.IsNullOrWhiteSpace(CurrentBookInDisplay.BookISBN))
                {
                    msgerror.megboxmsg.Text = "ISBN is required";
                    msgerror.ShowDialog();
                }
                else if (!IsValidISBN(CurrentBookInDisplay.BookISBN))
                {
                    msgerror.megboxmsg.Text = "Invalid ISBN";
                    msgerror.ShowDialog();
                }

                else
                {
                    var isUpdated = bookService.UpdateBook(CurrentBookInDisplay);
                    if (isUpdated)
                    {
                        LoadData();
                        CurrentBookInDisplay = new BookEntity();
                        MessageBoxOK msg = new MessageBoxOK();
                        msg.megboxmsg.Text = "Book updated successfully !";
                        msg.ShowDialog();
                    }
                    else
                    {
                        MessageBoxOK msg = new MessageBoxOK();
                        msg.megboxmsg.Text = "Book updated failed";
                        msg.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxOK msg = new MessageBoxOK();
                msg.megboxmsg.Text = $"Error updating book: {ex.Message}";
                msg.ShowDialog();
            }
        }

        

        private void DeleteBook(object parameter)
        {
            if (parameter is int bookId)
            {
                try
                {
                    MessageBoxError msgerror = new MessageBoxError();

                    // Check if the book exists in any active transactions before deletion
                    bool isInTransaction = bookService.IsBookInTransaction(bookId);
                    if (isInTransaction)
                    {
                        msgerror.megboxmsg.Text = "This Book part of an active transaction";
                        msgerror.ShowDialog();
                        return; // Stop the deletion process
                    }

                    // Proceed with the deletion if the book is not in any active transaction
                    var isDeleted = bookService.DeleteBook(bookId);
                    if (isDeleted)
                    {
                        MessageBoxOK msg = new MessageBoxOK();
                        msg.megboxmsg.Text = "Book Deleted Successfully!";
                        msg.ShowDialog();
                        LoadData();
                    }
                    else
                    {
                        MessageBoxOK msg = new MessageBoxOK();
                        msg.megboxmsg.Text = "Book Deletion Failed";
                        msg.ShowDialog();
                    }
                }
                catch (Exception e)
                {
                    MessageBoxOK msg = new MessageBoxOK();
                    msg.megboxmsg.Text = $"Error deleting book: {e.Message}";
                    msg.ShowDialog();
                }
            }
        }



        public void SearchBook()
        {
            try
            {
                var book = bookService.SearchBook(CurrentBookInDisplay.BookID);
                if (book != null)
                {
                    CurrentBookInDisplay = new BookEntity
                    {
                        BookID = book.BookID,
                        BookTitle = book.BookTitle,
                        BookAuthor = book.BookAuthor,
                        BookISBN = book.BookISBN,
                        BookGenre = book.BookGenre,
                        BookAvailability = book.BookAvailability,
                        CategoryID = book.CategoryID,
                    };
                    // Find the category that matches the CategoryID
                    SelectedCategory = Categories.FirstOrDefault(c => c.name == book.BookGenre);
                }
                else
                {
                    MessageBoxOK msg = new MessageBoxOK();
                    msg.megboxmsg.Text = "Book not found";
                    msg.ShowDialog();
                    CurrentBookInDisplay = new BookEntity();
                }
                onPropertyChanged(nameof(CurrentBookInDisplay));
            }
            catch (Exception e)
            {
                MessageBoxOK msg = new MessageBoxOK();
                msg.megboxmsg.Text = $"Error searching book: {e.Message}";
                msg.ShowDialog();
            }
        }
        #endregion


        private bool IsValidISBN(string isbn)
        {
            // Remove any hyphens
            isbn = isbn.Replace("-", "");

            if (isbn.Length == 10)
            {
                return IsValidISBN10(isbn);
            }
            else if (isbn.Length == 13)
            {
                return IsValidISBN13(isbn);
            }
            else
            {
                return false;
            }
        }

        private bool IsValidISBN10(string isbn10)
        {
            if (isbn10.Length != 10)
            {
                return false;
            }

            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                if (!char.IsDigit(isbn10[i]))
                {
                    return false;
                }
                sum += (isbn10[i] - '0') * (10 - i);
            }

            char check = isbn10[9];
            if (check != 'X' && !char.IsDigit(check))
            {
                return false;
            }
            sum += (check == 'X') ? 10 : (check - '0');

            return (sum % 11 == 0);
        }

        private bool IsValidISBN13(string isbn13)
        {
            if (isbn13.Length != 13 || !isbn13.All(char.IsDigit))
            {
                return false;
            }

            int sum = 0;
            for (int i = 0; i < 12; i++)
            {
                int digit = isbn13[i] - '0';
                sum += (i % 2 == 0) ? digit : digit * 3;
            }

            int checkDigit = (10 - (sum % 10)) % 10;
            return checkDigit == (isbn13[12] - '0');
        }
    }
}


