using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GanoExcel.InternalWebMasterPage
{
    public partial class Logout : System.Web.UI.Page
    {

        private const string DEFALT_PAGE = "~/Default.aspx";
        
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Session["UserInfo"] = null;

            foreach (Control ctrl in this.Controls)
            {
                Console.WriteLine(ctrl.ID);
            }


            Response.Redirect(DEFALT_PAGE);
        }
    }
}
