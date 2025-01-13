using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Model
{
    public class CategoryModel
    {
        private int CategoryID;
        public int id
        {
            get { return CategoryID; }
            set { CategoryID = value; }
        }

        private String CategoryName;
        public string name
        {
            get { return CategoryName; }
            set { CategoryName = value; }
        }

        private String CategoryDescription;
        public string description
        {
            get { return CategoryDescription; }
            set { CategoryDescription = value; }
        }
    }
}
