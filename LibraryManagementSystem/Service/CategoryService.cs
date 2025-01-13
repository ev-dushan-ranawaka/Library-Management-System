using LibraryManagementSystem.Data;
using LibraryManagementSystem.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Service
{
    public class CategoryService
    {
        #region Private Field
        private readonly LibraryDbContext CategoryDbContext;
        //readonly - The field can only be assigned once, either at declaration or in the constructor.
        #endregion
        

        #region Constructor
        public CategoryService()
        {
            CategoryDbContext = new LibraryDbContext(); //Initializes the CategoryDbContext field.
        }
        #endregion


        #region Get All Categories List
        public List<CategoryEntity> GetAll() //Retrieve all Categories from the database.
        {
            List<CategoryEntity> categorylist = new List<CategoryEntity>(); //A new empty list of CategoryEntity objects is initialized.
            try
            {
                categorylist = CategoryDbContext.Categories.ToList();
                //Retrieves all Category records from the Categories table in the database and converts them to a list.

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                //If an exception occurs catch and display in msgbox.
            }
            return categorylist;
        }
        #endregion


        #region Add Category Method
        public bool AddCategory(CategoryEntity currentCategory) //Set new Category values from currentCategory
        {
            try
            {
                var category = new CategoryEntity(); //Created new object Category.

                category.CategoryName = currentCategory.CategoryName;
                category.CategoryDescription = currentCategory.CategoryDescription;

                CategoryDbContext.Categories.Add(category); //New Category is added to the Categories collection of CategoryDbContext.
                CategoryDbContext.SaveChanges(); //New Category to the database.

                return true; // Operation succeeded
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message); //If an exception occurs catch and display in msgbox.
                return false; // Operation failed
            }
        }
        #endregion


        #region Update Category Method
        public bool UpdateCategory(CategoryEntity categoryupdate) //Get updated details of the Category.
        {
            try
            {
                var existingCategory = CategoryDbContext.Categories.Find(categoryupdate.CategoryID); //Finds the existing Category in the Categories collection using the CategoryID.
                if (existingCategory != null) //If the Category is found, it updates the properties with the new values.
                {
                    existingCategory.CategoryID= categoryupdate.CategoryID;
                    existingCategory.CategoryName = categoryupdate.CategoryName;
                    existingCategory.CategoryDescription = categoryupdate.CategoryDescription;
                    CategoryDbContext.SaveChanges(); //Update Category to the database.
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


        #region Delete Category Method
        public bool DeleteCategory(int categoryid) //Get which is the ID of the Category to be deleted.
        {
            try
            {
                var category = CategoryDbContext.Categories.Find(categoryid); //Finds the Category in the Categories collection using the CategoryID.
                if (category != null) //If the Category is found, it is removed from the Categories table.
                {
                    CategoryDbContext.Categories.Remove(category); //Delete Operation.
                    CategoryDbContext.SaveChanges(); //Delete Category from database.
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


        #region Search Category Method
        public CategoryEntity SearchCategory(int categoryid) //Get which is the ID of the Category to search for.
        {
            try
            {
                var existingCategory = CategoryDbContext.Categories.Find(categoryid); //Finds the Category in the Categories collection using the CategoryID.
                if (existingCategory != null) //If the Category is found,
                {
                    CategoryEntity category = new CategoryEntity
                    {
                        CategoryID = existingCategory.CategoryID,
                        CategoryName = existingCategory.CategoryName,
                        CategoryDescription = existingCategory.CategoryDescription,
                    };
                    return category;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            return null;
        }
        #endregion
    }
}
