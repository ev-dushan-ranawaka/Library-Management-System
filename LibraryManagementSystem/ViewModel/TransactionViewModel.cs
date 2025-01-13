using LibraryManagementSystem.Commands;
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
using System.Diagnostics;

namespace LibraryManagementSystem.ViewModel
{
    public class TransactionViewModel : INotifyPropertyChanged
    {
        #region PropertyChanged Event
        public event PropertyChangedEventHandler? PropertyChanged; //It is triggered whenever a property value changes in the view model.
        #endregion


        #region onPropertyChanged Method 
        private void onPropertyChanged(string propertyName) //Parameter is the name of the property that changed.
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        #region Private Fields
        private ObservableCollection<TransactionModel> transactionListInDisplay; 
        private TransactionEntity currentTransactionInDisplay; 
        private TransactionService transactionService; 
        private ICollectionView transactionCollectionView;

        private MemberService memebrService;
        private MemberEntity currentMemberInDisplay;
        private ICollectionView memberCollectionView;

        private BookService bookService;
        private BookEntity currentBookInDisplay;
        private ICollectionView bookCollectionView;
        #endregion


        #region Constructor
        public TransactionViewModel()
        {
            currentTransactionInDisplay = new TransactionEntity();
            transactionService = new TransactionService(); 
            LoadData(); 

            saveCommand = new RelayCommand(SaveTransaction);
            searchCommand = new RelayCommand(SearchTransaction);
            updateCommand = new RelayCommand(UpdateTransaction);
            deleteCommand = new RelayCommand(DeleteTransaction);

            searchMemberCommand = new RelayCommand(SearchMember);
            currentMemberInDisplay = new MemberEntity(); 
            memebrService = new MemberService();

            searchBookCommand = new RelayCommand(SearchBook);
            currentBookInDisplay = new BookEntity();
            bookService = new BookService();

            ClickAddTransactionComand = new ClickEventCommand(ClickAddTransaction);
            ReturnTransactionCommand = new ClickEventCommand(ClickReturnTransaction);
            MouseUpCloseCommand = new ClickEventCommand(OnMouseUpClose);
        }
        #endregion


        #region Properties
        public TransactionEntity CurrentTransactionInDisplay 
        {
            get { return currentTransactionInDisplay; }
            set 
            { 
                currentTransactionInDisplay = value;
                onPropertyChanged(nameof(CurrentTransactionInDisplay));
                //onPropertyChanged("CurrentTransactionInDisplay"); 
            }
        }

        public ObservableCollection<TransactionModel> TransactionListInDisplay
        {
            get
            {
                // Filter the list to only include transactions with a null Return Date
                return new ObservableCollection<TransactionModel>(
                    transactionListInDisplay.Where(t => t.RDate != null)
                );
            }
            set
            {
                transactionListInDisplay = value;
                //onPropertyChanged("TransactionListInDisplay");
                onPropertyChanged(nameof(TransactionListInDisplay)); 
            }
        }

        public MemberEntity CurrentMemberInDisplay //Exposes the currently displayed member entity. When set, it raises the PropertyChanged event to update the UI.
        {
            get { return currentMemberInDisplay; }
            set 
            { 
                currentMemberInDisplay = value;
                onPropertyChanged(nameof(CurrentMemberInDisplay));
                //onPropertyChanged("CurrentMemberInDisplay"); 
            }
        }
        public BookEntity CurrentBookInDisplay //Exposes the currently displayed member entity. When set, it raises the PropertyChanged event to update the UI.
        {
            get { return currentBookInDisplay; }
            set 
            { 
                currentBookInDisplay = value;
                onPropertyChanged(nameof(CurrentBookInDisplay));
                //onPropertyChanged("CurrentBookInDisplay"); 
            }
        }
        #endregion


        #region Commands for handle event
        public ICommand ClickAddTransactionComand { get; set; }

        public ICommand ReturnTransactionCommand { get; set; }

        public ICommand MouseUpCloseCommand { get; }
        #endregion


        #region Command Methods
        private void ClickAddTransaction(object parameter)
        {
            NewBookTransaction newBookTransaction = new NewBookTransaction();
            newBookTransaction.ShowDialog();
        }
        private void ClickReturnTransaction(object parameter)
        {
            BookReturn bookReturn = new BookReturn();
            bookReturn.ShowDialog();
        }
        private void OnMouseUpClose(object parameter)
        {
            if (parameter is Window window)
            {
                window.Close();
            }
        }
        #endregion


        #region ConvertToDisplayInMemberList Method
        private List<TransactionModel> ConvertToDisplayInTransactionList(List<TransactionEntity> transactionList)
        {
            return transactionList.Select(transaction => new TransactionModel
            {
                Tid = transaction.TransactionID,
                Mid = transaction.MemberID,
                Bid = transaction.BookID,
                BDate = transaction.BorrowDate,
                DDate = transaction.DueDate,
            }).ToList();
        }
        #endregion

        #region
        private void LoadData()
        {
            try
            {
                var transaction = ConvertToDisplayInTransactionList(transactionService.GetAll());
                // Create new ObservableCollection on UI thread
                TransactionListInDisplay = new ObservableCollection<TransactionModel>(transaction);
            }
            catch (Exception ex)
            {
                MessageBoxOK msg = new MessageBoxOK();
                msg.megboxmsg.Text = $"Error loading members: {ex.Message}";
                msg.ShowDialog();
            }
        }
        #endregion


        #region Relay Commands
        private RelayCommand saveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                return saveCommand ?? (saveCommand = new RelayCommand(param => SaveTransaction()));
            }
        }

        private RelayCommand updateCommand;
        public RelayCommand UpdateCommand
        {
            get
            {
                return updateCommand ?? (updateCommand = new RelayCommand(param => UpdateTransaction()));
            }
        }

        private RelayCommand deleteCommand;
        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ?? (deleteCommand = new RelayCommand(DeleteTransaction));
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
        private RelayCommand searchMemberCommand;
        public RelayCommand SearchMemberCommand
        {
            get
            {
                return searchMemberCommand;
            }
        }
        private RelayCommand searchBookCommand;
        public RelayCommand SearchBookCommand
        {
            get
            {
                return searchBookCommand;
            }
        }
        #endregion


        #region Command Methods
        private void SaveTransaction()
        {
            try
            {
                MessageBoxError msgerror = new MessageBoxError();

                if (CurrentBookInDisplay.BookID <= 0)
                {
                    msgerror.megboxmsg.Text = "Invalid BookID";
                    msgerror.ShowDialog();
                }
                else if (CurrentMemberInDisplay.MemberID <= 0)
                {
                    msgerror.megboxmsg.Text = "Invalid MemberID";
                    msgerror.ShowDialog();
                }
                else if (CurrentTransactionInDisplay.BorrowDate > DateTime.Now)
                {
                    msgerror.megboxmsg.Text = "Check again Borrow date";
                    msgerror.ShowDialog();
                }

                else
                {
                    // Proceed with saving the transaction only if validation is successful
                    // Assign MemberID and BookID to the current transaction
                    CurrentTransactionInDisplay.MemberID = CurrentMemberInDisplay.MemberID;
                    CurrentTransactionInDisplay.BookID = CurrentBookInDisplay.BookID;

                    // Add the transaction using the transaction service
                    var isSaved = transactionService.AddTransaction(CurrentTransactionInDisplay);

                    if (isSaved)
                    {
                        // Update book availability to false after successful transaction
                        CurrentBookInDisplay.BookAvailability = false;
                        var isBookUpdated = bookService.UpdateBook(CurrentBookInDisplay);

                        if (isBookUpdated)
                        {
                            // Reload data and clear the form after successful save
                            LoadData();
                            CurrentTransactionInDisplay = new TransactionEntity(); // Clear the transaction form
                            MessageBoxOK msg = new MessageBoxOK();
                            msg.megboxmsg.Text = "Transaction saved successfully";
                            msg.ShowDialog();
                        }
                        else
                        {
                            CurrentTransactionInDisplay = new TransactionEntity(); // Clear the transaction form
                            // Show message if book update failed but transaction saved
                            MessageBoxOK msg = new MessageBoxOK();
                            msg.megboxmsg.Text = "Transaction saved, but book update failed.";
                            msg.ShowDialog();
                        }
                    }
                    else
                    {
                        // Show message if transaction save failed
                        MessageBoxOK msg = new MessageBoxOK();
                        msg.megboxmsg.Text = "Transaction failed.";
                        msg.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any unexpected errors and show the exception message
                MessageBoxOK msg = new MessageBoxOK();
                msg.megboxmsg.Text = $"Error saving transaction: {ex.Message}";
                msg.ShowDialog();
            }
        }




        private void UpdateTransaction()
        {
            try
            {
                MessageBoxError msgerror = new MessageBoxError();

                if (CurrentTransactionInDisplay.TransactionID <= 0)
                {
                    msgerror.megboxmsg.Text = "Invalid TransactionID";
                    msgerror.ShowDialog();
                }
                else if (CurrentTransactionInDisplay.ReturnDate > DateTime.Now)
                {
                    msgerror.megboxmsg.Text = "Check again Return date";
                    msgerror.ShowDialog();
                }
                else
                {
                    // Assign BookID to the current transaction
                    CurrentTransactionInDisplay.BookID = CurrentBookInDisplay.BookID;

                    // Update the transaction in the database
                    var isUpdated = transactionService.UpdateTransaction(CurrentTransactionInDisplay);

                    if (isUpdated)
                    {
                        // Update the book availability to True (since it's returned)
                        CurrentBookInDisplay.BookAvailability = true;

                        // Update the book availability in the database
                        var isBookUpdated = bookService.UpdateBook(CurrentBookInDisplay);

                        if (isBookUpdated)
                        {
                            // Book availability successfully updated
                            LoadData();
                            CurrentTransactionInDisplay = new TransactionEntity(); // Clear the form
                            MessageBoxOK msg = new MessageBoxOK();
                            msg.megboxmsg.Text = "Book returned successfully!";
                            msg.ShowDialog();
                        }
                        else
                        {
                            // Log or show more detailed error information to help diagnose the problem
                            MessageBoxOK msg = new MessageBoxOK();
                            msg.megboxmsg.Text = "Book returned successfully!";
                            msg.ShowDialog();
                        }

                    }
                    else
                    {
                        // Show message if transaction update failed
                        MessageBoxOK msg = new MessageBoxOK();
                        msg.megboxmsg.Text = "Book return operation failed.";
                        msg.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any unexpected errors and show the exception message
                MessageBoxOK msg = new MessageBoxOK();
                msg.megboxmsg.Text = $"Error returning book: {ex.Message}";
                msg.ShowDialog();
            }
        }



        private void DeleteTransaction(object parameter)
        {
            if (parameter is int transactionId)
            {
                try
                {
                    var isDeleted = transactionService.DeleteTransaction(transactionId);
                    if (isDeleted)
                    {
                        LoadData();
                        MessageBoxOK msg = new MessageBoxOK();
                        msg.megboxmsg.Text = "Transaction Deleted Successfully!";
                        msg.ShowDialog();
                    }
                    else
                    {
                        MessageBoxOK msg = new MessageBoxOK();
                        msg.megboxmsg.Text = "Transaction Deletion Failed";
                        msg.ShowDialog();
                    }
                }
                catch (Exception e)
                {
                    MessageBoxOK msg = new MessageBoxOK();
                    msg.megboxmsg.Text = $"Error deleting transaction: {e.Message}";
                    msg.ShowDialog();
                }
            }
        }


        private void SearchTransaction()
        {
            try
            {
                var transaction = transactionService.SearchTransaction(CurrentTransactionInDisplay.TransactionID);
                if (transaction != null)
                {
                    CurrentTransactionInDisplay = new TransactionEntity
                    {
                        TransactionID = transaction.TransactionID,
                        MemberID = transaction.MemberID,
                        BookID = transaction.BookID,
                        DueDate = transaction.DueDate,
                        BorrowDate = transaction.BorrowDate
                    };
                }
                else
                {
                    MessageBoxOK msg = new MessageBoxOK();
                    msg.megboxmsg.Text = "Transaction not found";
                    msg.ShowDialog();
                    CurrentTransactionInDisplay = new TransactionEntity();
                }
                onPropertyChanged(nameof(CurrentTransactionInDisplay));
            }
            catch (Exception e)
            {
                MessageBoxOK msg = new MessageBoxOK();
                msg.megboxmsg.Text = $"Error searching transaction: {e.Message}";
                msg.ShowDialog();
            }
        }


        private void SearchMember()
        {
            try
            {
                var member = memebrService.SearchMember(CurrentMemberInDisplay.MemberID);
                if (member != null)
                {
                    CurrentMemberInDisplay = new MemberEntity
                    {
                        MemberID = member.MemberID,
                        MemberName = member.MemberName,
                    };
                }
                else
                {
                    MessageBoxOK msg = new MessageBoxOK();
                    msg.megboxmsg.Text = "Member not found";
                    msg.ShowDialog();
                    CurrentMemberInDisplay = new MemberEntity();
                }
                onPropertyChanged(nameof(CurrentMemberInDisplay));
            }
            catch (Exception e)
            {
                MessageBoxOK msg = new MessageBoxOK();
                msg.megboxmsg.Text = $"Error searching member: {e.Message}";
                msg.ShowDialog();
            }
        }
        private void SearchBook()
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
                    };
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

    }
}
