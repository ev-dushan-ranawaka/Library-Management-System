using LibraryManagementSystem.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Data
{
    public class LibraryDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=DUSHAN\\MSSQLSERVER03;Initial Catalog=Library_Management;Integrated Security=True;Trust Server Certificate=True");
            //Configures the context to use SQL Server as the database provider
        }


        //Tables in the database.
        public DbSet<LoginEntity> Logins { get; set; }
        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<BookEntity> Books { get; set; }
        public DbSet<MemberEntity> Members { get; set; }
        public DbSet<TransactionEntity> Transactions { get; set; }


        //Configure the relationships between the entities in the model. 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // One-to-many relationship between Category and Book
            modelBuilder.Entity<BookEntity>()
                .HasOne(b => b.Category) // Each Book has one Category
                .WithMany(c => c.Books) // Each Category can have many Books
                .HasForeignKey(b => b.CategoryID); // Foreign key in BookEntity refers to CategoryID

            // One-to-many relationship between Member and Transaction
            modelBuilder.Entity<TransactionEntity>()
                .HasOne(t => t.Member)
                .WithMany(m => m.Transactions)
                .HasForeignKey(t => t.MemberID);

            // One-to-one relationship between Book and Transaction
            modelBuilder.Entity<TransactionEntity>()
                .HasOne(t => t.Book)
                .WithOne(b => b.Transaction)
                .HasForeignKey<TransactionEntity>(t => t.BookID);
        }
    }
}
