using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GanoExcel.InternalWebMasterPage;

namespace GanoExcel.Ux2CampaignEditor
{
    public partial class EditorMain : System.Web.UI.Page
    {
        private const string THIS_PAGE = "~/EditorMain.aspx";
        private const string NEXT_PAGE = "~/EditCampaign.aspx";
        private const string LAST_PAGE = "~/Default.aspx";
        private const string DEFALT_PAGE = "~/Default.aspx";
        private const string LOGOUT_PAGE = "~/Logout.aspx";
        private const string PAGE_TITLE = "Campaign Schedules:";
        private const string PAGE_DESCRIPTION = "Listed below are all of the currently available Campaign Schedules.";
        private const string RESULTS_ROW_CLASS_PREFIX = "alt_row_";

        Ux2CampaingEditorAPI cEditorAPI = null;



        protected void Page_Load(object sender, EventArgs e)
        {

            if (IsPostBack && this.Request.Params.Get("__EVENTTARGET").Contains("logOutButton"))
                return;

            if (this.Session["UserInfo"] == null)
            {
                Response.Redirect(DEFALT_PAGE);
                return;
            }

            if (this.Session["CampaignEditorClass"] == null)
            {
                Response.Redirect(LOGOUT_PAGE);
                return;
            }

            ((GanoInternal)this.Master).PageTitle = PAGE_TITLE;
            ((GanoInternal)this.Master).PageDescription = PAGE_DESCRIPTION;
            ((GanoInternal)this.Master).BackLinkVisible = false;

            ClientScript.RegisterClientScriptInclude("TableSortJS", "TableSort.js");



            cEditorAPI = (Ux2CampaingEditorAPI)this.Session["CampaignEditorClass"];
            BuildCampaignsTable();
            LoadCampaigns();
        }

        private void LoadCampaigns()
        {
            int i = 0;
            TableRow row = null;
            TableCell cell = null;
            LinkButton lnk = null;
            List<Ux2CampaignSchedule> campaigns = cEditorAPI.GetCampaignSchedules();

            foreach (Ux2CampaignSchedule campaign in campaigns)
            {
                row = new TableRow();

                lnk = new LinkButton();
                lnk.Text = campaign.Id.ToString();
                lnk.ToolTip = "Click to edit this Campaign Schedule";
                lnk.Click += new EventHandler(lnk_Click);


                cell = new TableCell();
                cell.Text = "";
                cell.Controls.Add(lnk);
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = campaign.Description;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = campaign.Active ? "Active" : "Inactive";
                row.Cells.Add(cell);

                row.CssClass = RESULTS_ROW_CLASS_PREFIX + (i % 2).ToString();

                this.campaignsTable.Rows.Add(row);

                i++;

            }

            this.countLabel.Text = "Campaign Schedules Found: " + campaigns.Count.ToString();

        }

        void lnk_Click(object sender, EventArgs e)
        {
            this.Session["CampaignId"] = ((LinkButton)sender).Text;
            Response.Redirect(NEXT_PAGE);
        }

        private void BuildCampaignsTable()
        {
            TableHeaderCell hCell = null;
            TableHeaderRow hRow = null;


            this.campaignsTable.Rows.Clear();

            hRow = new TableHeaderRow();

            hCell = new TableHeaderCell();
            hCell.Text = "Id";
            hCell.ID = "campaignIdHeaderTH";
            hCell.CssClass = "EditorTableHeaders";
            hRow.Cells.Add(hCell);

            hCell = new TableHeaderCell();
            hCell.Text = "Description";
            hCell.ID = "campaignDescriptionHeaderTH";
            hCell.CssClass = "EditorTableHeaders";
            hRow.Cells.Add(hCell);

            hCell = new TableHeaderCell();
            hCell.Text = "Status";
            hCell.ID = "statusHeaderTH";
            hCell.CssClass = "EditorTableHeaders";
            hRow.Cells.Add(hCell);

            this.campaignsTable.Rows.Add(hRow);

            this.countLabel.Text = "Campaign Schedules Found: 0";

            this.campaignsTable.Attributes.Add("class", "sortable EditorTables");
        }

        protected void newCampaignButton_Click(object sender, EventArgs e)
        {
            this.Session["CampaignId"] = "0";
            Response.Redirect(NEXT_PAGE);
        }
    }
}
