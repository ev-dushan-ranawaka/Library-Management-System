using LibraryManagementSystem.Commands;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Entity;
using LibraryManagementSystem.MessageBox;
using LibraryManagementSystem.Model;
using LibraryManagementSystem.Service;
using LibraryManagementSystem.View.Popups;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LibraryManagementSystem.ViewModel
{
    public class CategoryViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {

        #region PropertyChanged Event
        public event PropertyChangedEventHandler? PropertyChanged; //It is triggered whenever a property value changes in the view model.
        #endregion


        #region onPropertyChanged Method 
        private void onPropertyChanged(string propertyName) //Parameter is the name of the property that changed.
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        #region Private Fields
        private ObservableCollection<CategoryModel> categoryListInDisplay;
        private CategoryEntity currentCategoryInDisplay;
        private CategoryService categoryService;
        private ICollectionView categoryCollectionView;
        #endregion


        #region Constructor
        public CategoryViewModel()
        {
            currentCategoryInDisplay = new CategoryEntity();
            categoryService = new CategoryService();
            LoadData();

            saveCommand = new RelayCommand(SaveCategory);
            searchCommand = new RelayCommand(SearchCategory);
            updateCommand = new RelayCommand(UpdateCategory);
            deleteCommand = new RelayCommand(DeleteCategory);

            ClickAddCategoryComand = new ClickEventCommand(ClickAddCategory);
            ClickUpdateCategoryCommand = new ClickEventCommand(ClickUpdateCategory);
            MouseUpCloseCommand = new ClickEventCommand(OnMouseUpClose);

            SubmitCommand = new ActionCommand(Submit, CanSubmit);
        }
        #endregion


        #region Properties
        public CategoryEntity CurrentCategoryInDisplay
        {
            get { return currentCategoryInDisplay; }
            set { currentCategoryInDisplay = value; onPropertyChanged("CurrentCategoryInDisplay"); }
        }

        public ObservableCollection<CategoryModel> CategoryListInDisplay
        {
            get { return categoryListInDisplay; }
            set 
            { 
                categoryListInDisplay = value; 
                onPropertyChanged("CategoryListInDisplay");
                onPropertyChanged(nameof(categoryListInDisplay));
            }
        }
        #endregion


        #region Commands for handle event
        public ICommand ClickAddCategoryComand { get; set; }

        public ICommand ClickUpdateCategoryCommand { get; set; }

        public ICommand MouseUpCloseCommand { get; }
        public ActionCommand SubmitCommand { get; set; }
        #endregion


        #region Command Methods
        private void ClickAddCategory(object parameter)
        {
            AddCategoryView addCategoryView = new AddCategoryView();
            addCategoryView.ShowDialog();
        }
        private void ClickUpdateCategory(object parameter)
        {
            UpdateCategoryView updateCategoryView = new UpdateCategoryView();
            updateCategoryView.ShowDialog();
        }
        private void OnMouseUpClose(object parameter)
        {
            if (parameter is Window window)
            {
                window.Close();
            }
        }
        #endregion


        #region ConvertToDisplayInCategoryList Method
        private List<CategoryModel> ConvertToDisplayInCategoryList(List<CategoryEntity> categoryList)
        {
            return categoryList.Select(category => new CategoryModel
            {
                id = category.CategoryID,
                name = category.CategoryName,
                description = category.CategoryDescription,
            }).ToList();
        }
        #endregion


        #region 
        private void LoadData()
        {
            try
            {
                var categories = ConvertToDisplayInCategoryList(categoryService.GetAll());
                categoryListInDisplay = new ObservableCollection<CategoryModel>(categories);
            }
            catch (Exception ex)
            {
                MessageBoxOK msg = new MessageBoxOK();
                msg.megboxmsg.Text = $"Error loading categories: {ex.Message}";
                msg.ShowDialog();
            }

            /*var categories = ConvertToDisplayInCategoryList(categoryService.GetAll());
            Application.Current.Dispatcher.Invoke(() =>
            {
                CategoryListInDisplay = new ObservableCollection<CategoryModel>(categories);
                onPropertyChanged(nameof(CategoryListInDisplay));
            });*/
        }
        #endregion


        #region Relay Commands
        private RelayCommand saveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                return saveCommand ?? (saveCommand = new RelayCommand(param => SaveCategory()));
            }
        }

        private RelayCommand updateCommand;
        public RelayCommand UpdateCommand
        {
            get
            {
                return updateCommand ?? (updateCommand = new RelayCommand(param => UpdateCategory()));
            }
        }

        private RelayCommand deleteCommand;
        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ?? (deleteCommand = new RelayCommand(DeleteCategory));
            }
        }

        private RelayCommand searchCommand;
        public RelayCommand SearchCommand
        {
            get
            {
                return searchCommand;
            }
        }
        #endregion


        #region Command Methods
        public void SaveCategory()
        {
            try
            {
                CurrentCategoryInDisplay.CategoryName = Name;
                CurrentCategoryInDisplay.CategoryDescription = Description;

                var IsSaved = categoryService.AddCategory(CurrentCategoryInDisplay);
                if (IsSaved)
                {
                    LoadData();
                    CurrentCategoryInDisplay = new CategoryEntity(); // Clear the form
                    MessageBoxOK msg = new MessageBoxOK();
                    msg.megboxmsg.Text = "Category Saved Successfully!";
                    msg.ShowDialog();
                }
                else
                {
                    MessageBoxOK msg = new MessageBoxOK();
                    msg.megboxmsg.Text = "Category Save Operation failed";
                    msg.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBoxOK msg = new MessageBoxOK();
                msg.megboxmsg.Text = $"Error saving Category: {ex.Message}";
                msg.ShowDialog();
            }
        }



        public void UpdateCategory()
        {
            try
            {
                MessageBoxError msgerror = new MessageBoxError();

                if (CurrentCategoryInDisplay.CategoryID <= 0)
                {
                    msgerror.megboxmsg.Text = "Invalid CategoryID";
                    msgerror.ShowDialog();
                }
                else if (string.IsNullOrWhiteSpace(CurrentCategoryInDisplay.CategoryName))
                {
                    msgerror.megboxmsg.Text = "Category name is required";
                    msgerror.ShowDialog();
                }
                else if (string.IsNullOrWhiteSpace(CurrentCategoryInDisplay.CategoryDescription))
                {
                    msgerror.megboxmsg.Text = "Description is required";
                    msgerror.ShowDialog();
                }

                else
                {
                    var isUpdated = categoryService.UpdateCategory(CurrentCategoryInDisplay);
                    if (isUpdated)
                    {
                        LoadData();
                        CurrentCategoryInDisplay = new CategoryEntity();
                        MessageBoxOK msg = new MessageBoxOK();
                        msg.megboxmsg.Text = "Category updated Successfully !";
                        msg.ShowDialog();
                    }
                    else
                    {
                        MessageBoxOK msg = new MessageBoxOK();
                        msg.megboxmsg.Text = "Category update failed";
                        msg.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxOK msg = new MessageBoxOK();
                msg.megboxmsg.Text = $"Error updating category: {ex.Message}";
                msg.ShowDialog();
            }
        }

        private void DeleteCategory(object parameter)
        {
            if (parameter is int categoryId)
            {
                try
                {
                    var isDeleted = categoryService.DeleteCategory(categoryId);
                    if (isDeleted)
                    {
                        MessageBoxOK msg = new MessageBoxOK();
                        msg.megboxmsg.Text = "Category Deleted Successfully !";
                        msg.ShowDialog();
                        LoadData();
                    }
                    else
                    {
                        MessageBoxOK msg = new MessageBoxOK();
                        msg.megboxmsg.Text = "Category Deletion Faild";
                        msg.ShowDialog();
                    }
                }
                catch (Exception e)
                {
                    MessageBoxOK msg = new MessageBoxOK();
                    msg.megboxmsg.Text = $"Error deleting category: {e.Message}";
                    msg.ShowDialog();
                }
            }
        }

        public void SearchCategory()
        {
            try
            {
                var category = categoryService.SearchCategory(CurrentCategoryInDisplay.CategoryID);
                if (category != null)
                {
                    CurrentCategoryInDisplay = new CategoryEntity
                    {
                        CategoryID = category.CategoryID,
                        CategoryName = category.CategoryName,
                        CategoryDescription = category.CategoryDescription,
                    };
                }
                else
                {
                    MessageBoxOK msg = new MessageBoxOK();
                    msg.megboxmsg.Text = "Category not found";
                    msg.ShowDialog();
                    CurrentCategoryInDisplay = new CategoryEntity();
                }
                onPropertyChanged(nameof(CurrentCategoryInDisplay));
            }
            catch (Exception e)
            {
                MessageBoxOK msg = new MessageBoxOK();
                msg.megboxmsg.Text = $"Error searching category: {e.Message}";
                msg.ShowDialog();
            }
        }
        #endregion


        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        public bool HasErrors => _errors.Any();

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !_errors.ContainsKey(propertyName))
            {
                return Enumerable.Empty<string>();
            }

            return _errors[propertyName];
        }

        public void Validate(string propertyName, object propertyValue)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(this) { MemberName = propertyName };
            Validator.TryValidateProperty(propertyValue, context, results);

            if (results.Any())
            {
                _errors[propertyName] = results.Select(r => r.ErrorMessage).ToList();
            }
            else
            {
                _errors.Remove(propertyName);
            }

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            SubmitCommand.RaiseCanExecuteChanged();
        }

        private string _name;

        [Required(ErrorMessage = "Name is Required")]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                Validate(nameof(Name), value);
            }
        }

        private string _description;

        [Required(ErrorMessage = "Description is Required")]
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                Validate(nameof(Description), value);
            }
        }

        private bool CanSubmit(object obj)
        {
            var context = new ValidationContext(this);
            var results = new List<ValidationResult>();
            return Validator.TryValidateObject(this, context, results, true);
        }

        private void Submit(object obj)
        {
            SaveCategory();
        }
    }
}
