using LibraryManagementSystem.Commands;
using LibraryManagementSystem.Entity;
using LibraryManagementSystem.Model;
using LibraryManagementSystem.Service;
using LibraryManagementSystem.MessageBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LibraryManagementSystem.View.Popups;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.Identity.Client;

namespace LibraryManagementSystem.ViewModel
{
    public class MemberViewModel : INotifyPropertyChanged
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
        private ObservableCollection<MemberModel> memberListInDisplay; //This collection is used to store and manage a dynamic list of MemberModel objects.
        private MemberEntity currentMemberInDisplay; //This field holds the currently selected or displayed MemberEntity.
        private MemberService memebrService; //This is an instance of the MemberService class, for CRUD Operations.
        private ICollectionView memberCollectionView; //It allows for filtering, sorting, and grouping of data.
        #endregion


        #region Constructor
        public MemberViewModel()
        {
            currentMemberInDisplay = new MemberEntity(); //Iinitialized to a new MemberEntity.
            memebrService = new MemberService(); //Initialized to a new instance of MemberService.
            LoadData(); //load member data into the view model.

            saveCommand = new RelayCommand(SaveMember);
            searchCommand = new RelayCommand(SearchMember);
            updateCommand = new RelayCommand(UpdateMember);
            deleteCommand = new RelayCommand(DeleteMember);

            ClickAddMemberComand = new ClickEventCommand(ClickAddMember);
            ClickUpdateMemberCommand = new ClickEventCommand(ClickUpdateMember);
            MouseUpCloseCommand = new ClickEventCommand(OnMouseUpClose);
        }
        #endregion


        #region Properties
        public MemberEntity CurrentMemberInDisplay
        {
            get { return currentMemberInDisplay; }
            set { currentMemberInDisplay = value; onPropertyChanged("CurrentMemberInDisplay"); }
        }

        public ObservableCollection<MemberModel> MemberListInDisplay
        {
            get { return memberListInDisplay; }
            set { memberListInDisplay = value; onPropertyChanged("MemberListInDisplay"); }
        }
        #endregion


        #region Commands for handle event
        public ICommand ClickAddMemberComand { get; set; }

        public ICommand ClickUpdateMemberCommand { get; set; }

        public ICommand MouseUpCloseCommand { get; }
        #endregion


        #region Command Methods
        private void ClickAddMember(object parameter)
        {
            AddMemberView addMemberView = new AddMemberView();
            addMemberView.ShowDialog();
        }
        private void ClickUpdateMember(object parameter)
        {
            UpdateMemberView updateMemberView = new UpdateMemberView();
            updateMemberView.ShowDialog();
        }
        private void OnMouseUpClose(object parameter)
        {
            if (parameter is Window window)
            {
                window.Close();
            }
        }
        #endregion


        #region ConvertToDisplayInMemberList Method
        private List<MemberModel> ConvertToDisplayInMemberList(List<MemberEntity> memberList)
        {
            return memberList.Select(member => new MemberModel
            {
                id = member.MemberID,
                name = member.MemberName,
                position = member.MemberPosition,
                email = member.MemberEmail,
                mobile = member.MemberMobile,
                membershipDate = member.MembershipDate
            }).ToList();
        }
        #endregion


        #region
        private void LoadData()
        {
            try
            {
                var members = ConvertToDisplayInMemberList(memebrService.GetAll());
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MemberListInDisplay = new ObservableCollection<MemberModel>(members);
                    onPropertyChanged(nameof(MemberListInDisplay));
                });
            }
            catch (Exception ex)
            {
                MessageBoxOK msg = new MessageBoxOK();
                msg.megboxmsg.Text = $"Error loading members: {ex.Message}";
                msg.ShowDialog();
            }
        }
        #endregion


        #region Relay Commands
        private RelayCommand saveCommand;
        public RelayCommand SaveCommand
        {
            get { return saveCommand; }
        }

        private RelayCommand updateCommand;
        public RelayCommand UpdateCommand
        {
            get
            {
                return updateCommand ?? (updateCommand = new RelayCommand(param => UpdateMember()));
            }
        }

        private RelayCommand deleteCommand;
        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ?? (deleteCommand = new RelayCommand(DeleteMember));
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
        private void SaveMember()
        {
            try
            {
                MessageBoxError msgerror = new MessageBoxError();
                string mobileNumberString = CurrentMemberInDisplay.MemberMobile.ToString();

                if (string.IsNullOrWhiteSpace(CurrentMemberInDisplay.MemberName))
                {
                    msgerror.megboxmsg.Text = "Name is required";
                    msgerror.ShowDialog();
                }
                else if (CurrentMemberInDisplay.MemberName.Any(char.IsDigit) ||
                CurrentMemberInDisplay.MemberName.Contains("\\") ||
                CurrentMemberInDisplay.MemberName.Contains("/"))
                {
                    msgerror.megboxmsg.Text = "Invalid name";
                    msgerror.ShowDialog();
                }

                else if (string.IsNullOrWhiteSpace(CurrentMemberInDisplay.MemberPosition))
                {
                    msgerror.megboxmsg.Text = "Position is required";
                    msgerror.ShowDialog();
                }

                else if (string.IsNullOrWhiteSpace(CurrentMemberInDisplay.MemberEmail))
                {
                    msgerror.megboxmsg.Text = "Email is required";
                    msgerror.ShowDialog();
                }
                else if (!IsValidEmail(CurrentMemberInDisplay.MemberEmail))
                {
                    msgerror.megboxmsg.Text = "Invalid email format";
                    msgerror.ShowDialog();
                }
                else if (!IsValidMobileNumber(mobileNumberString))
                {
                    msgerror.megboxmsg.Text = "Invalid mobile number";
                    msgerror.ShowDialog();
                }
                else if (CurrentMemberInDisplay.MembershipDate > DateTime.Now)
                {
                    msgerror.megboxmsg.Text = "Invalid Memebrship date";
                    msgerror.ShowDialog();
                }

                else
                {
                    var isSaved = memebrService.AddMember(CurrentMemberInDisplay);
                    LoadData();

                    if (isSaved)
                    {
                        CurrentMemberInDisplay = new MemberEntity(); // Clear the form
                        MessageBoxOK msg = new MessageBoxOK();
                        msg.megboxmsg.Text = "Member saved successfully!";
                        msg.ShowDialog();
                    }
                    else
                    {
                        MessageBoxOK msg = new MessageBoxOK();
                        msg.megboxmsg.Text = "Member Save operation failed";
                        msg.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxOK msg = new MessageBoxOK();
                msg.megboxmsg.Text = $"Error saving member: {ex.Message}";
                msg.ShowDialog();
            }
        }


        private void UpdateMember()
        {
            try
            {
                MessageBoxError msgerror = new MessageBoxError();
                string mobileNumberString = CurrentMemberInDisplay.MemberMobile.ToString();

                // Numeric and format validations
                string memberIDString = CurrentMemberInDisplay.MemberID.ToString();
                if (CurrentMemberInDisplay.MemberID <= 0 || !IsNumeric(memberIDString))
                {
                    msgerror.megboxmsg.Text = "Invalid MemberID";
                    msgerror.ShowDialog();
                }
                else if(string.IsNullOrWhiteSpace(CurrentMemberInDisplay.MemberName))
                {
                    msgerror.megboxmsg.Text = "Name is required";
                    msgerror.ShowDialog();
                }
                else if (CurrentMemberInDisplay.MemberName.Any(char.IsDigit) ||
                CurrentMemberInDisplay.MemberName.Contains("\\") ||
                CurrentMemberInDisplay.MemberName.Contains("/"))
                {
                    msgerror.megboxmsg.Text = "Invalid name";
                    msgerror.ShowDialog();
                }
                else if (string.IsNullOrWhiteSpace(CurrentMemberInDisplay.MemberPosition))
                {
                    msgerror.megboxmsg.Text = "Position is required";
                    msgerror.ShowDialog();
                }
                else if (string.IsNullOrWhiteSpace(CurrentMemberInDisplay.MemberEmail))
                {
                    msgerror.megboxmsg.Text = "Email is required";
                    msgerror.ShowDialog();
                }
                else if (!IsValidEmail(CurrentMemberInDisplay.MemberEmail))
                {
                    msgerror.megboxmsg.Text = "Invalid email format";
                    msgerror.ShowDialog();
                }
                else if (!IsValidMobileNumber(mobileNumberString))
                {
                    msgerror.megboxmsg.Text = "Invalid mobile number";
                    msgerror.ShowDialog();
                }

                else
                {
                    var isUpdated = memebrService.UpdateMember(CurrentMemberInDisplay);
                    if (isUpdated)
                    {
                        LoadData();
                        MessageBoxOK msg = new MessageBoxOK();
                        msg.megboxmsg.Text = "Member updated successfully!";
                        msg.ShowDialog();
                    }
                    else
                    {
                        MessageBoxOK msg = new MessageBoxOK();
                        msg.megboxmsg.Text = "Member update failed";
                        msg.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxOK msg = new MessageBoxOK();
                msg.megboxmsg.Text = $"Error updating member: {ex.Message}";
                msg.ShowDialog();
            }
        }


        private void DeleteMember(object parameter)
        {
            if (parameter is int memberId)
            {
                try
                {
                    MessageBoxError msgerror = new MessageBoxError();

                    // Check if the member exists in any active transactions before deletion
                    bool isInTransaction = memebrService.IsMemberInTransaction(memberId);
                    if (isInTransaction)
                    {
                        msgerror.megboxmsg.Text = "This Member part of an active transaction";
                        msgerror.ShowDialog();
                        return; // Stop the deletion process
                    }

                    // Proceed with the deletion if the member is not in any active transaction
                    var isDeleted = memebrService.DeleteMember(memberId);
                    if (isDeleted)
                    {
                        MessageBoxOK msg = new MessageBoxOK();
                        msg.megboxmsg.Text = "Member Deleted Successfully!";
                        msg.ShowDialog();
                    }
                    else
                    {
                        MessageBoxOK msg = new MessageBoxOK();
                        msg.megboxmsg.Text = "Member Deletion Failed";
                        msg.ShowDialog();
                    }
                }
                catch (Exception e)
                {
                    MessageBoxOK msg = new MessageBoxOK();
                    msg.megboxmsg.Text = $"Error deleting member: {e.Message}";
                    msg.ShowDialog();
                }
            }
        }



        private void SearchMember()
        {
            try
            {
                var member = memebrService.SearchMember(CurrentMemberInDisplay.MemberID);
                if (member != null)
                {
                    CurrentMemberInDisplay = new MemberEntity
                    {
                        MemberID = member.MemberID,
                        MemberName = member.MemberName,
                        MemberPosition = member.MemberPosition,
                        MemberEmail = member.MemberEmail,
                        MemberMobile = member.MemberMobile
                    };
                }
                else
                {
                    MessageBoxOK msg = new MessageBoxOK();
                    msg.megboxmsg.Text = "Member not found";
                    msg.ShowDialog();
                    CurrentMemberInDisplay = new MemberEntity();
                }
                onPropertyChanged(nameof(CurrentMemberInDisplay));
            }
            catch (Exception e)
            {
                MessageBoxOK msg = new MessageBoxOK();
                msg.megboxmsg.Text = $"Error searching member: {e.Message}";
                msg.ShowDialog();
            }
        }
        #endregion


        private bool IsNumeric(string value)
        {
            return value.All(char.IsDigit);
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Define a regular expression for email validation
                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
                return emailRegex.IsMatch(email);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool IsValidMobileNumber(string mobileNumber)
        {
            if (string.IsNullOrWhiteSpace(mobileNumber))
                return false;

            try
            {
                // Define a regular expression for Sri Lankan mobile number validation
                var mobileNumberRegex = new Regex(@"^7[0-9]{8}$", RegexOptions.Compiled);
                return mobileNumberRegex.IsMatch(mobileNumber);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
