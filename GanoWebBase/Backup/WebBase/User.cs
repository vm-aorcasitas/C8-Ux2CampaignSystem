using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GanoExcel.Web.Base
{
    [Serializable]
    public class User
    {
        private int cId = 0;
        private string cLoginName = string.Empty;
        private string cFirstName = string.Empty;
        private string cLastName = string.Empty;
        private string cMiddleInitial = string.Empty;
        private int cDefaultLanguage = 0;
        private int cDefaultWarehouse = 0;
        private int cDefaultCountry = 0;
        private DateTime cLastLogin;
        private bool cChangePassword = false;
        private bool cDeleted = false;

        public User()
        {
        }

        public User(int id, string loginName, string firstName, string lastName, string middleInitial, int defaultLanguage, int defaultWarehouse,
            int defaultCountry, DateTime lastLogin, bool changePassword, bool deleted)
        {
            this.cId = id;
            this.cLoginName = loginName;
            this.cFirstName = firstName;
            this.cLastName = lastName;
            this.cMiddleInitial = middleInitial;
            this.cDefaultLanguage = defaultLanguage;
            this.cDefaultWarehouse = defaultWarehouse;
            this.cDefaultCountry = defaultCountry;
            this.cLastLogin = lastLogin;
            this.cChangePassword = changePassword;
            this.cDeleted = deleted;
        }


        public int Id
        {
            get { return this.cId; }
            set { this.cId = value; }
        }

        public string LoginName
        {
            get { return this.cLoginName; }
            set { this.cLoginName = value; }
        }

        public string FirstName
        {
            get { return this.cFirstName; }
            set { this.cFirstName = value; }
        }

        public string LastName
        {
            get { return this.cLastName; }
            set { this.cLastName = value; }
        }

        public string MiddleInitial
        {
            get { return this.cMiddleInitial; }
            set { this.cMiddleInitial = value; }
        }

        public int DefaultLanguage
        {
            get { return this.cDefaultLanguage; }
            set { this.cDefaultLanguage = value; }
        }

        public int DefaultWarehouse
        {
            get { return this.cDefaultWarehouse; }
            set { this.cDefaultWarehouse = value; }
        }

        public int DefaultCountry
        {
            get { return this.cDefaultCountry; }
            set { this.cDefaultCountry = value; }
        }

        public DateTime LastLogin
        {
            get { return this.cLastLogin; }
            set { this.cLastLogin = value; }
        }

        public bool PasswordChangeRequired
        {
            get { return this.cChangePassword; }
            set { this.cChangePassword = value; }
        }

        public bool Deleted
        {
            get { return this.cDeleted; }
            set { this.cDeleted = value; }
        }

    }
}
