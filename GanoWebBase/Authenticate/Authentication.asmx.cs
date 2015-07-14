using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using GanoExcel.Web.Base;
using System.Xml.Serialization;

namespace GanoExcel.Web
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://www.ganoexcel.us/", Name = "Authentication")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Authentication : System.Web.Services.WebService
    {

        [WebMethod]
        public User Authenticate(string userName, string password)
        {
            User user = Security.AuthenticateUser(userName, password, Utilities.GetConnectionString());
            return user;
        }

        [WebMethod]
        public bool IsUserAuthorized(int userId, int securityCode)
        {
            return Security.IsUserAuthorized(userId, securityCode, Utilities.GetConnectionString()); 
        }

    }
}

