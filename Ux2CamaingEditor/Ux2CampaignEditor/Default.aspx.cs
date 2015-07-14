using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GanoExcel.InternalWebMasterPage;

namespace GanoExcel.Ux2CampaignEditor
{
    public partial class Default : System.Web.UI.Page
    {
        private const string NEXT_PAGE = "~/Login.aspx";
        private const string THIS_PAGE = "~/Default.aspx";
        private const string NEXT_PAGE_2 = "~/EditorMain.aspx";
        private const string APP_TITLE = "Ux2 Campaign Editor";
        private const string APP_DESC = "- Create and Edit Ux2 Campaigns.";
        
        protected void Page_Load(object sender, EventArgs e)
        {

            ((GanoInternal)this.Master).BackLinkVisible = false;
            ((GanoInternal)this.Master).SiteTitle = APP_TITLE;
            ((GanoInternal)this.Master).PageTitle = APP_TITLE;
            ((GanoInternal)this.Master).PageDescription = APP_DESC;

            if (this.Session["UserInfo"] != null)
            {
                Response.Redirect(NEXT_PAGE_2);
            }
            else
            {
                Response.Redirect(NEXT_PAGE);
            }

        }
    }
}
