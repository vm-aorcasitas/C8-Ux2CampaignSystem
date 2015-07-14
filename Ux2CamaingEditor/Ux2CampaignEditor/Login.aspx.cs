using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GanoExcel.InternalWebMasterPage;
using System.Configuration;

namespace GanoExcel.Ux2CampaignEditor
{
    public partial class Login : System.Web.UI.Page
    {
        private const string THIS_PAGE = "~/Login.aspx";
        private const string NEXT_PAGE = "~/AETypes.aspx";
        private const string LAST_PAGE = "~/Default.aspx";
        private const string DEFALT_PAGE = "~/Default.aspx";
        private const int SEC_CODE = 10000;
        private const string PAGE_TITLE = "Login:";
        private const string PAGE_DESCRIPTION = "Enter your User Name and Password.";

        protected void Page_Load(object sender, EventArgs e)
        {

            ((GanoInternal)this.Master).BackLinkVisible = false;
            ((GanoInternal)this.Master).PageTitle = PAGE_TITLE;
            ((GanoInternal)this.Master).PageDescription = PAGE_DESCRIPTION;
        
            

        }

        protected void loginControl_Authenticate(object sender, AuthenticateEventArgs e)
        {
            
            Authentication.Authentication auth = new Authentication.Authentication();

            //
            auth.Url = ConfigurationManager.AppSettings["AuthenticationServiceUrl"].ToString();
            Authentication.User user = auth.Authenticate(this.loginControl.UserName, this.loginControl.Password);
            string jScript = string.Empty;

            this.Session["UserInfo"] = user;

            if (user == null)
                e.Authenticated = false;
            else
            {
                if (auth.IsUserAuthorized(user.Id, SEC_CODE))
                {
                    e.Authenticated = true;
                    //((AETypeEditor)this.Master).TimeoutEnabled = true;
                    
                    //Properties.Settings setting = new Properties.Settings();
                    //this.Session["CampaignEditorClass"] = new Ux2CampaingEditorAPI(setting.ConnectionString);
                    
                    //DefaultConnectionString
                    Properties.Settings setting = new Properties.Settings();
                    this.Session["CampaignEditorClass"] = new Ux2CampaingEditorAPI(ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString);
                }
                else
                {
                    e.Authenticated = false;
                    this.loginControl.FailureText = "You do not have the necessary rights to access this application.";
                }
            }

        }
    }
}
