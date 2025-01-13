using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Entity
{
    public class MemberEntity //Entity Framework model class that represents a member
    {
        [Key] //Marks a property as the primary key in a database table
        public int MemberID { get; set; } //primary key for the member table (Unique identifier for each member)

        [Required] //Property must have a value (Not Null)
        [MaxLength(50)] //Maximum length allowed for a Member Name
        public string MemberName { get; set; }

        [Required]
        [MaxLength(50)]
        public string MemberPosition { get; set; }

        [Required]
        [MaxLength(30)]
        public string MemberEmail { get; set; }

        [Required]
        public int MemberMobile { get; set; }

        [Required]
        public DateTime MembershipDate { get; set; }


        public virtual ICollection<BookEntity> BorrowedBooks { get; set; } = new List<BookEntity>();

        public virtual ICollection<TransactionEntity> Transactions { get; set; } = new List<TransactionEntity>(); // Transactions is an empty list, never null

        /*  virtual- Data is loaded from the database only when it's actually accessed,
            Helps improve performance by avoiding unnecessary data loading  */

        /*  get: Reads the value
            set: Writes the value   */
    }
}
