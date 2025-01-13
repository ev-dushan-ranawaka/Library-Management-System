using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Model
{
    public class MemberModel
    {
        private int MemberID;
        public int id
        {
            get { return MemberID; }
            set { MemberID = value; }
        }

        private String MemberName;
        public string name
        {
            get { return MemberName; }
            set { MemberName = value; }
        }

        private String MemberPosition;
        public string position
        {
            get { return MemberPosition; }
            set { MemberPosition = value; }
        }

        private String MemberEmail;
        public string email
        {
            get { return MemberEmail; }
            set { MemberEmail = value; }
        }

        private int MemberMobile;
        public int mobile
        {
            get { return MemberMobile; }
            set { MemberMobile = value; }
        }

        private DateTime MembershipDate;
        public DateTime membershipDate
        {
            get { return MembershipDate; }
            set { MembershipDate = value; }
        }
    }
}
