using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LibraryManagementSystem.View.Popups
{
    /// <summary>
    /// Interaction logic for NewBookTransaction.xaml
    /// </summary>
    public partial class NewBookTransaction : Window
    {
        public NewBookTransaction()
        {
            InitializeComponent();
            SetDatePickerDate();
        }

        private void SetDatePickerDate()
        {
            dpBorrowDate.SelectedDate = DateTime.Now;
            dpDueDae.SelectedDate = DateTime.Today.AddDays(14);
        }
    }
}
