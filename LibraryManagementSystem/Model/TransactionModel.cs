using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Model
{
    public class TransactionModel
    {
        private int TransactionID;
        public int Tid
        {
            get { return TransactionID; }
            set { TransactionID = value; }
        }

        private int BookID;
        public int Bid
        {
            get { return BookID; }
            set { BookID = value; }
        }

        private int MemberID;
        public int Mid
        {
            get { return MemberID; }
            set { MemberID = value; }
        }

        private DateTime BorrowDate;
        public DateTime BDate
        {
            get { return BorrowDate; }
            set { BorrowDate = value; }
        }

        private DateTime DueDate;
        public DateTime DDate
        {
            get { return DueDate; }
            set { DueDate = value; }
        }

        private DateTime ReturnDate;
        public DateTime RDate
        {
            get { return ReturnDate; }
            set { ReturnDate = value; }
        }
    }
}
