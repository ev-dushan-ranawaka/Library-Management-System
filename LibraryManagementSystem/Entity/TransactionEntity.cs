using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Entity
{
    public class TransactionEntity //Entity Framework model class that represents a book transactions
    {
        [Key] //Marks a property as the primary key in a database table
        public int TransactionID { get; set; } //primary key for the book transactions table (Unique identifier for each transactions)

        [Required] //Property must have a value (Not Null)
        public int BookID { get; set; } // Foreign key to the Book
        public virtual BookEntity Book { get; set; }

        [Required]
        public int MemberID { get; set; } // Foreign key to the Member 
        public virtual MemberEntity Member { get; set; }

        [Required]
        public DateTime BorrowDate { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
