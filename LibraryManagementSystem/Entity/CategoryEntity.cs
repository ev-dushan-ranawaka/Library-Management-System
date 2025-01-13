using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Entity
{
    public class CategoryEntity //Entity Framework model class that represents a category
    {
        [Key] //Marks a property as the primary key in a database table
        public int CategoryID { get; set; } //primary key for the Category table (Unique identifier for each Category)

        [Required] //Property must have a value (Not Null)
        [MaxLength(50)] //Maximum length allowed for a Category Name
        public string CategoryName { get; set; }

        [Required]
        [MaxLength(255)]
        public string CategoryDescription { get; set; }


        // Navigation property for the one-to-many relationship
        public virtual ICollection<BookEntity> Books { get; set; } = new List<BookEntity>(); // Books is an empty list, never null

        /*  virtual- Data is loaded from the database only when it's actually accessed,
            Helps improve performance by avoiding unnecessary data loading  */

        /*  get: Reads the value
            set: Writes the value   */
    }
}
