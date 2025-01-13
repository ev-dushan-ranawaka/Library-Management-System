using LibraryManagementSystem.Data;
using LibraryManagementSystem.Entity;
using LibraryManagementSystem.MessageBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Service
{
    public class TransactionService
    {
        #region Private Field
        private readonly LibraryDbContext TransactionDbContext;
        //readonly - The field can only be assigned once, either at declaration or in the constructor.
        #endregion


        #region Constructor
        public TransactionService()
        {
            TransactionDbContext = new LibraryDbContext(); //Initializes the TransactionDbContext field.
        }
        #endregion


        #region Get All Transactions List
        public List<TransactionEntity> GetAll() //Retrieve all Transactions from the database.
        {
            List<TransactionEntity> Transactionlist = new List<TransactionEntity>(); //A new empty list of TransactionEntity objects is initialized.
            try
            {
                Transactionlist = TransactionDbContext.Transactions.ToList();
                //Retrieves all Transactions records from the Transaction table in the database and converts them to a list.
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                //If an exception occurs catch and display in msgbox.
            }
            return Transactionlist;
        }
        #endregion


        #region Add Transaction Method
        public bool AddTransaction(TransactionEntity currentTransaction) //Set new Transaction values from currentTransaction
        {
            try
            {   
                var Transaction = new TransactionEntity(); //Created new object Transaction.

                Transaction.BookID = currentTransaction.BookID;
                Transaction.MemberID = currentTransaction.MemberID;
                Transaction.BorrowDate = currentTransaction.BorrowDate;
                Transaction.DueDate = currentTransaction.DueDate;
                Transaction.ReturnDate = currentTransaction.ReturnDate;

                TransactionDbContext.Transactions.Add(Transaction); //New Transaction is added to the Transactions collection of TransactionDbContext.
                TransactionDbContext.SaveChanges(); //New Transaction to the database.

                return true; // Operation succeeded

            }
            catch (Exception)
            {
                MessageBoxError msgerror = new MessageBoxError();

                msgerror.megboxmsg.Text = "This Book already borrowd";
                msgerror.ShowDialog();

                //System.Windows.MessageBox.Show(ex.Message); //If an exception occurs catch and display in msgbox.
                return false; // Operation failed
            }
        }
        #endregion


        #region Update Transaction Method
        public bool UpdateTransaction(TransactionEntity Transaction) //Get updated details of the Transaction.
        {
            try
            {
                var existingTransaction  = TransactionDbContext.Transactions.Find(Transaction.TransactionID); //Finds the existing Transaction in the Transactions collection using the TransactionID.
                if (existingTransaction != null) //If the Transaction is found, it updates the properties with the new values.
                {
                    //existingTransaction.TransactionID = Transaction.TransactionID;
                    //existingTransaction.MemberID = Transaction.MemberID;
                    //existingTransaction.BookID = Transaction.BookID;
                    //existingTransaction.BorrowDate = Transaction.BorrowDate;
                    //existingTransaction.DueDate = Transaction.DueDate;
                    existingTransaction.ReturnDate = Transaction.ReturnDate;
                    TransactionDbContext.SaveChanges(); //Update Transaction to the database.
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


        #region Delete Transaction Method
        public bool DeleteTransaction(int Transtctionid) //Get which is the ID of the Transaction to be deleted.
        {
            try
            {
                var transaction = TransactionDbContext.Transactions.Find(Transtctionid); //Finds the Transaction in the Transactions collection using the Transactionid.
                if (transaction != null) //If the Transaction is found, it is removed from the Transactions table.
                {
                    TransactionDbContext.Transactions.Remove(transaction); //Delete Operation.
                    TransactionDbContext.SaveChanges(); //Delete Transaction from database.
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


        #region Search Transaction Method
        public TransactionEntity SearchTransaction(int Transtctionid) //Get which is the ID of the Transaction to search for.
        {
            try
            {
                var existingTransaction = TransactionDbContext.Transactions.Find(Transtctionid); //Finds the Transaction in the Transactions collection using the MemberID.
                if (existingTransaction != null) //If the Transaction is found,
                {
                    TransactionEntity member = new TransactionEntity
                    {
                        TransactionID = existingTransaction.TransactionID,
                        MemberID = existingTransaction.MemberID,
                        BookID = existingTransaction.BookID,
                        BorrowDate = existingTransaction.BorrowDate,
                        DueDate = existingTransaction.DueDate,
                        ReturnDate = existingTransaction.ReturnDate,
                    };
                    return member;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            return null;
        }
        #endregion


        #region Check Book already borrowd
        public bool IsBookBorrowd(int bookId)
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
