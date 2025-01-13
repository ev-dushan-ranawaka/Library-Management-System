using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Entity
{
    public class BookEntity //Entity Framework model class that represents a book
    {
        [Key] //Marks a property as the primary key in a database table
        public int BookID { get; set; } //primary key for the book table (Unique identifier for each book)

        [Required]  //Property must have a value (Not Null)
        [MaxLength(30)] //Maximum length allowed for a Book title
        public string BookTitle { get; set; }

        [Required]
        [MaxLength(30)]
        public string BookAuthor { get; set; }

        [Required]
        [MaxLength(50)]
        public string BookISBN { get; set; }

        [Required]
        [MaxLength(20)]
        public string BookGenre { get; set; }

        [Required]
        public bool BookAvailability { get; set; }

        public int CategoryID { get; set; } // Foreign key to the CategoryEntity (one-to-many relationship)

        public virtual CategoryEntity Category { get; set; } // Navigation property for the relationship

        public virtual TransactionEntity Transaction { get; set; } // Navigation property for the one-to-one relationship

        /*  virtual- Data is loaded from the database only when it's actually accessed,
            Helps improve performance by avoiding unnecessary data loading  */

        /*  get: Reads the value
            set: Writes the value   */
    }
}
