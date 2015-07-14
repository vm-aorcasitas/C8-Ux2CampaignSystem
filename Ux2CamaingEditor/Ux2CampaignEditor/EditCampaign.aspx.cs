using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GanoExcel.InternalWebMasterPage;

namespace GanoExcel.Ux2CampaignEditor
{
    public partial class EditCampaign : System.Web.UI.Page
    {
        private const string THIS_PAGE = "~/EditCampaign.aspx";
        private const string NEXT_PAGE = "~/EditEmail.aspx";
        private const string LAST_PAGE = "~/EditorMain.aspx";
        private const string DEFALT_PAGE = "~/Default.aspx";
        private const string LOGOUT_PAGE = "~/Logout.aspx";
        private const string PAGE_TITLE = "Edit Campaign Schedule:";
        private const string PAGE_DESCRIPTION = "Here you can edit a Campaing and the associated emails.";
        private const string RESULTS_ROW_CLASS_PREFIX = "alt_row_";

        private Ux2CampaingEditorAPI cEditorAPI = null;

        private int cCampaignId = 0;

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
            ((GanoInternal)this.Master).BackLinkText = "<< Back to Campaigns";
            ((GanoInternal)this.Master).BackLinkURL = LAST_PAGE;
            ((GanoInternal)this.Master).BackLinkVisible = true;

            ClientScript.RegisterClientScriptInclude("TableSortJS", "TableSort.js");
            
            cEditorAPI = (Ux2CampaingEditorAPI)this.Session["CampaignEditorClass"];



            if (this.Session["CampaignId"] == null)
                this.Session["CampaignId"] = 0;

            this.cCampaignId = Convert.ToInt32(this.Session["CampaignId"]);

            BuildEmailsTable();

            if (IsPostBack && (this.Request.Params.Get("__EVENTTARGET").Contains("saveButton") || this.Request.Params.Get("__EVENTTARGET").Contains("applyButton")))
                return;

            

            if (this.cCampaignId == 0)
            {
                this.applyButton.Visible = false;
                this.campaignIdLabel.Text = "New";
            }
            else
            {
                //Existing Campaign
                this.applyButton.Visible = true;
                LoadCampaignData(this.cCampaignId);
            }

        }

        private void LoadCampaignData(int id)
        {
            Ux2CampaignSchedule schedule = this.cEditorAPI.GetCampaignSchedule(id);

            if (schedule != null)
            {
                this.campaignIdLabel.Text = schedule.Id.ToString();
                this.campaignDescriptionTextBox.Text = schedule.Description;
                this.campaignStatusCheckBox.Checked = schedule.Active;

                LoadEmailsTable(schedule.Id);
            }

        }

        private void BuildEmailsTable()
        {
            TableHeaderCell hCell = null;
            TableHeaderRow hRow = null;

            this.emailsTable.Rows.Clear();

            hRow = new TableHeaderRow();
            hRow.CssClass = "TableHeaderRow";

            hCell = new TableHeaderCell();
            hCell.Text = "Id";
            hCell.CssClass = "EmailIdHeader";
            hCell.CssClass = "EditorTableHeaders";
            hRow.Cells.Add(hCell);

            hCell = new TableHeaderCell();
            hCell.Text = "Description";
            hCell.CssClass = "EmailDescriptionHeader";
            hCell.CssClass = "EditorTableHeaders";
            hRow.Cells.Add(hCell);

            hCell = new TableHeaderCell();
            hCell.Text = "Offset(dd:hh)";
            hCell.CssClass = "EmailOffsetHeader";
            hCell.CssClass = "EditorTableHeaders";
            hRow.Cells.Add(hCell);

            hCell = new TableHeaderCell();
            hCell.Text = "Status";
            hCell.CssClass = "EmailStatusHeader";
            hCell.CssClass = "EditorTableHeaders";
            hRow.Cells.Add(hCell);

            this.emailsTable.Rows.Add(hRow);

            this.emailsCountLabel.Text = "Campaign Emails Found: 0";

            this.emailsTable.Attributes.Add("class", "sortable EditorTables");
        }

        private void LoadEmailsTable(int campaignId)
        {
            int i = 0;
            TableRow row = null;
            TableCell cell = null;
            LinkButton lnk = null;
            List<Ux2CampaignEmail> emails = cEditorAPI.GetCampaignEmails(campaignId);

            foreach (Ux2CampaignEmail email in emails)
            {
                row = new TableRow();

                lnk = new LinkButton();
                lnk.Text = email.Id.ToString();
                lnk.ToolTip = "Click to edit this Campaign Email";
                lnk.Click += new EventHandler(lnk_Click);


                cell = new TableCell();
                cell.Text = "";
                cell.Controls.Add(lnk);
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = email.Description;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = CampaignEditor.HoursToDaysAndHours(email.Offset);
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = email.Active ? "Active" : "Inactive";
                row.Cells.Add(cell);

                row.CssClass = RESULTS_ROW_CLASS_PREFIX + (i % 2).ToString();

                this.emailsTable.Rows.Add(row);

                i++;

            }

            this.emailsCountLabel.Text = "Campaign Emails Found: " + emails.Count.ToString();

        }

        private void SaveCampaign(bool redirect)
        {
            Ux2CampaignSchedule campaign = new Ux2CampaignSchedule();
            Authentication.User user = null;

            try
            {
                
                user = (Authentication.User)this.Session["UserInfo"];


                this.Session["CampaignEditRedirect"] = redirect;

                //Show Popup

                if (this.cCampaignId == 0)
                {
                    campaign.Id = this.cEditorAPI.AddCampaign(this.campaignDescriptionTextBox.Text, this.campaignStatusCheckBox.Checked, user.Id);
                    this.Session["CampaignId"] = campaign.Id;
                    this.cCampaignId = campaign.Id;
                    this.savedLabel.Text = "The new Campaign has been added. It's ID is " + campaign.Id.ToString() + ".";
                }
                else
                {
                    this.cEditorAPI.UpdateCampaign(this.cCampaignId, this.campaignDescriptionTextBox.Text, this.campaignStatusCheckBox.Checked);
                    this.savedLabel.Text = "The Campaign has been updated.";
                }

                this.savedMPE.Show();
            }
            catch (Exception exc)
            {
                //Show popup

                this.Session["CampaignEditRedirect"] = false;
                this.savedLabel.Text = "Error Saving: " + exc.Message;
                this.savedMPE.Show();
            }
        }

        void lnk_Click(object sender, EventArgs e)
        {
           

            this.Session["EmailId"] = ((LinkButton)sender).Text;
            Response.Redirect(NEXT_PAGE);
        }

        protected void newEmailButton_Click(object sender, EventArgs e)
        {
            this.Session["EmailId"] = "0";

            if (this.cCampaignId == 0)
            {
                this.saveFirstMPE.Show();
            }
            else
            {
                Response.Redirect(NEXT_PAGE);
            }
        }

        protected void saveButton_Click(object sender, EventArgs e)
        {
            this.Session["CampaignEditRedirectPage"] = LAST_PAGE;
            SaveCampaign(true);
        }

        protected void applyButton_Click(object sender, EventArgs e)
        {
            SaveCampaign(false);
        }

        protected void savedOKButton_Click(object sender, EventArgs e)
        {
            this.savedMPE.Hide();

            if ((bool)this.Session["CampaignEditRedirect"])
                Response.Redirect(this.Session["CampaignEditRedirectPage"].ToString());
        }

        
        protected void yesButton_Click(object sender, EventArgs e)
        {
            this.Session["CampaignEditRedirectPage"] = NEXT_PAGE;
            this.saveFirstMPE.Hide();
            SaveCampaign(true);

        }

        //protected void noButton_Click(object sender, EventArgs e)
        //{
        //    this.saveFirstMPE.Hide();
        //}
    }
}
