using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GanoExcel.Ux2CampaignEditor
{
    public partial class PreviewGenerator : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.previewHTMLDiv.InnerHtml = this.Session["IFrameHTML"].ToString();
            ClientScript.RegisterClientScriptInclude("TableSortJS", "TableSort.js");
        }
    }
}
