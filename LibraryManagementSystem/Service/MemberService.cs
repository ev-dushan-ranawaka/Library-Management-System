using LibraryManagementSystem.Data;
using LibraryManagementSystem.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Service
{
    public class MemberService
    {
        #region Private Field
        private readonly LibraryDbContext MemberDbContext;
        //readonly - The field can only be assigned once, either at declaration or in the constructor.
        #endregion


        #region Constructor
        public MemberService()
        {
            MemberDbContext = new LibraryDbContext(); //Initializes the MemberDbContext field.
        }
        #endregion


        #region Get All Members List
        public List<MemberEntity> GetAll() //Retrieve all members from the database.
        {
            List<MemberEntity> memberlist = new List<MemberEntity>(); //A new empty list of MemberEntity objects is initialized.
            try
            {
                memberlist = MemberDbContext.Members.ToList();
                //Retrieves all member records from the Members table in the database and converts them to a list.
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                //If an exception occurs catch and display in msgbox.
            }
            return memberlist;
        }
        #endregion


        #region Add Member Method
        public bool AddMember(MemberEntity currentMember) //Set new member values from currentMember
        {
            try
            {
                var member = new MemberEntity(); //Created new object member.

                member.MemberName = currentMember.MemberName;
                member.MemberPosition = currentMember.MemberPosition;
                member.MemberEmail = currentMember.MemberEmail;
                member.MemberMobile = currentMember.MemberMobile;
                member.MembershipDate = currentMember.MembershipDate;

                MemberDbContext.Members.Add(member); //New member is added to the Members collection of MemberDbContext.
                MemberDbContext.SaveChanges(); //New member to the database.

                return true; // Operation succeeded
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message); //If an exception occurs catch and display in msgbox.
                return false; // Operation failed
            }
        }
        #endregion


        #region Update Member Method
        public bool UpdateMember(MemberEntity memberupdate) //Get updated details of the member.
        {
            try
            {
                var existingMember = MemberDbContext.Members.Find(memberupdate.MemberID); //Finds the existing member in the Members collection using the MemberID.
                if (existingMember != null) //If the member is found, it updates the properties with the new values.
                {
                    existingMember.MemberID = memberupdate.MemberID;
                    existingMember.MemberName = memberupdate.MemberName;
                    existingMember.MemberPosition = memberupdate.MemberPosition;
                    existingMember.MemberEmail = memberupdate.MemberEmail;
                    existingMember.MemberMobile = memberupdate.MemberMobile;
                    existingMember.MembershipDate = memberupdate.MembershipDate;
                    MemberDbContext.SaveChanges(); //Update member to the database.
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


        #region Delete Member Method
        public bool DeleteMember(int memberid) //Get which is the ID of the member to be deleted.
        {
            try
            {
                var member = MemberDbContext.Members.Find(memberid); //Finds the member in the Members collection using the memberid.
                if (member != null) //If the member is found, it is removed from the Members table.
                {
                    MemberDbContext.Members.Remove(member); //Delete Operation.
                    MemberDbContext.SaveChanges(); //Delete member from database.
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


        #region Search Member Method
        public MemberEntity SearchMember(int memberid) //Get which is the ID of the member to search for.
        {
            try
            {
                var existingMember = MemberDbContext.Members.Find(memberid); //Finds the member in the Members collection using the MemberID.
                if (existingMember != null) //If the member is found,
                {
                    MemberEntity member = new MemberEntity
                    {
                        MemberID = existingMember.MemberID,
                        MemberName = existingMember.MemberName,
                        MemberPosition = existingMember.MemberPosition,
                        MemberEmail = existingMember.MemberEmail,
                        MemberMobile = existingMember.MemberMobile,
                        MembershipDate = existingMember.MembershipDate,
                    };
                    return member;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            return null;
        }
        #endregion

        public bool IsMemberInTransaction(int bookId)
        {
            using (var context = new LibraryDbContext()) //Creates a new instance of LibraryDbContext
            {
                //LINQ Query:
                return context.Transactions.Any(t => t.MemberID == bookId && t.ReturnDate == null); //Checks if there are any records in the Transactions table where:
            }
        }
    }
}
