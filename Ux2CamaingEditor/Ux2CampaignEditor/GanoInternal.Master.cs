using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using GanoExcel.Ux2CampaignEditor ;

namespace GanoExcel.InternalWebMasterPage
{
    public partial class GanoInternal : System.Web.UI.MasterPage
    {
        private const string LOGOUT_PAGE = "~/Logout.aspx";

        protected void Page_Load(object sender, EventArgs e)
        {
            GanoExcel.Ux2CampaignEditor.Authentication.User user = null;

            //this.backLinkButton.Attributes.Add("onClick", "javascript:history.back(); return false;");
            //this.Page.ClientScript.RegisterClientScriptInclude("sesson-timeoutJS", "sesson-timeout.js");

            this.footerLabel.Text = "© 2008-" + DateTime.Now.Year.ToString() + " Gano Excel North America";
            this.versionLabel.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            user = (GanoExcel.Ux2CampaignEditor.Authentication.User)this.Session["UserInfo"];

            //if (this.Session["TimeoutEnabled"] != null && (bool)this.Session["TimeoutEnabled"])
            //    this.timeoutControlField.Value = "1";
            //else
            //    this.timeoutControlField.Value = "0";

            if (this.Session["SiteTitle"] != null)
                this.siteTitleLabel.Text = this.Session["SiteTitle"].ToString();
            else
#if DEBUG
                this.siteTitleLabel.Text = "[Site Title]";
#else
                this.siteTitleLabel.Text = "";
#endif

            if (this.Session["PageTitle"] != null)
                this.pageTitleLabel.Text = this.Session["PageTitle"].ToString();
            else
#if DEBUG
                this.pageTitleLabel.Text = "[Page Title]";
#else
                this.pageTitleLabel.Text = "";
#endif

            if (this.Session["PageDescription"] != null)
                this.pageDescriptionLabel.Text = this.Session["PageDescription"].ToString();
            else
#if DEBUG
                this.pageDescriptionLabel.Text = "[Page Description] This is the page description. The page description is made to be expanded across the page and then down, wrapping around the Page Title. It would be nice to have a background image that also expanded along with this description.";
#else
                this.pageDescriptionLabel.Text = "";
#endif

            if (user != null)
            {
                this.logOutButton.Visible = true;
                this.userLabel.Text = user.FirstName + " " + user.LastName;
            }
            else
            {
#if DEBUG
                this.logOutButton.Text = "[Log Out]";
                this.logOutButton.Visible = true;
#else
                this.logOutButton.Text = "Log Out";
                this.logOutButton.Visible = false;
#endif

                this.userLabel.Text = "Not Logged In";
            }
        }

        protected void logOutButton_Click(object sender, EventArgs e)
        {
            this.userLabel.Text = "";
            Response.Redirect(LOGOUT_PAGE);
        }

        public bool TimeoutEnabled
        {
            get { return (bool)this.Session["TimeoutEnabled"]; }
            set { this.Session["TimeoutEnabled"] = value; }
        }

        public string SiteTitle
        {
            get
            {
                if (this.Session["SiteTitle"] != null)
                    return this.Session["SiteTitle"].ToString();
                else
                    return "";
            }
            set
            {
                this.Session["SiteTitle"] = value;
            }
        }

        public string PageTitle
        {
            get
            {
                if (this.Session["PageTitle"] != null)
                    return this.Session["PageTitle"].ToString();
                else
                    return "";
            }
            set
            {
                this.Session["PageTitle"] = value;
            }
        }

        public string PageDescription
        {
            get
            {
                if (this.Session["PageDescription"] != null)
                    return this.Session["PageDescription"].ToString();
                else
                    return "";
            }
            set
            {
                this.Session["PageDescription"] = value;
            }
        }

        public string BackLinkURL
        {
            set { this.backLinkButton.PostBackUrl = value; }
        }

        public string BackLinkText
        {
            set { this.backLinkButton.Text = value; }
        }

        public bool BackLinkVisible
        {
            set { this.backLinkButton.Visible = value; }
        }
    }
}
