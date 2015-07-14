using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GanoExcel.InternalWebMasterPage;

namespace GanoExcel.Ux2CampaignEditor
{
    public partial class EditEmail : System.Web.UI.Page
    {
        private const string THIS_PAGE = "~/EditEmail.aspx";
        private const string NEXT_PAGE = "~/.aspx";
        private const string LAST_PAGE = "~/EditCampaign.aspx";
        private const string DEFALT_PAGE = "~/Default.aspx";
        private const string LOGOUT_PAGE = "~/Logout.aspx";
        private const string PAGE_TITLE = "Edit Campaign Email:";
        private const string PAGE_DESCRIPTION = "Here you can edit a Campaing email.";
        private const string RESULTS_ROW_CLASS_PREFIX = "alt_row_";

        private Ux2CampaingEditorAPI cEditorAPI = null;

        private int cEmailId = 0;
        private List<Ux2EmailTemplate> cTemplates = new List<Ux2EmailTemplate>();

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

            Properties.Settings emailTemplatesURL = new GanoExcel.Ux2CampaignEditor.Properties.Settings();
            this.editEmailTemplatesLink.NavigateUrl =  emailTemplatesURL.EmailTemplateEditorURL;

            this.emailTemplatesDropDown.Attributes.Add("onChange", "return TemplateDropDownChanged();");

            ((GanoInternal)this.Master).PageTitle = PAGE_TITLE;
            ((GanoInternal)this.Master).PageDescription = PAGE_DESCRIPTION;
            ((GanoInternal)this.Master).BackLinkText = "<< Back to Campaign Editor";
            ((GanoInternal)this.Master).BackLinkURL = LAST_PAGE;
            ((GanoInternal)this.Master).BackLinkVisible = true;

            //this.previewSQLLink.Attributes.Add("onClick", "return ShowSQLPopup();");
            this.previewOKButton.Attributes.Add("onClick", "return ClosePopup();");


            ClientScript.RegisterClientScriptInclude("TableSortJS", "TableSort.js");
            cEditorAPI = (Ux2CampaingEditorAPI)this.Session["CampaignEditorClass"];

            this.cTemplates = this.cEditorAPI.GetEmailTemplates();

            this.cEmailId = Convert.ToInt32(this.Session["EmailId"]);

            LoadEmailTemplates();

            if (this.Session["EmailId"] == null)
                this.Session["EmailId"] = 0;

            this.editEmailAccordion.SelectedIndex = 0;

            EnableTabType(this.queryTextBox);

            if (IsPostBack && this.Request.Params.Get("__EVENTTARGET").Contains("previewSQLLink"))
            {
                this.Session["SQLQuery"] = this.queryTextBox.Text;

                string sb = "";
                sb += "<script>";
                sb += "popupWindow = window.open('SQLPreview.aspx', 'Preview', 'toolbar=0,width=600,height=600,scrollbars=1');";
                sb += "</script>";

                Page.RegisterStartupScript("SQLPreview", sb.ToString());

                this.editEmailAccordion.SelectedIndex = 1;

                this.previewMPE.Show();

                return;
            }

            if (IsPostBack)
                return;

            


            if (this.cEmailId == 0)
            {
                //New Campaign;
                this.emailIdLabel.Text = "New";
                this.applyButton.Visible = false;
                this.emailTemplatesDropDown.SelectedIndex = 0;
            }
            else
            {
                //Existing Campaign
                
                LoadEmailData(this.cEmailId);
                this.applyButton.Visible = true;
            }

        }

        public void EnableTabType(TextBox tb)
        {
            tb.Attributes.Add("onkeydown",
            "if(event.which || event.keyCode)" +
            "{" +
            "  if ((event.which == 9) || (event.keyCode == 9))" +
            "  {" +
            "    insertTab(document.getElementById('" + tb.ClientID + "'));" +
            "    return false;" +
            "  }" +
            "}" +
            "else {return true}; ");
        } 

        private void LoadEmailTemplates()
        {
            
            string itemText = string.Empty;
            string htmlSelect = string.Empty;
            ListItem li = null;
            this.emailTemplatesDropDown.Items.Clear();


            foreach (Ux2EmailTemplate template in this.cTemplates)
            {

                //itemText = "(" + template.Id.ToString() + ") " + template.Description + " In Group: (" + template.GroupId.ToString() + ") " + template.GroupDescription;
                itemText = "(" + template.Id.ToString() + ") " + template.Description;
                li = new ListItem(itemText, template.Id.ToString());
                li.Attributes.Add("title", "Group: (" + template.GroupId.ToString() + ") " + template.GroupDescription);
                this.emailTemplatesDropDown.Items.Add(li);

            }

        }

        private void LoadEmailData(int id)
        {
            Ux2CampaignEmail email = this.cEditorAPI.GetCampaignEmail(id);

            if (email != null)
            {
                this.emailIdLabel.Text = email.Id.ToString();
                this.descriptionTextBox.Text = email.Description;
                this.emailTemplatesDropDown.SelectedValue = email.EmailTypeId.ToString();
                this.offsetTextBox.Text = CampaignEditor.HoursToDaysAndHours(email.Offset);
                this.statusCheckBox.Checked = email.Active;
                this.queryTextBox.Text = email.Query;
                this.param1NameTextBox.Text = email.Param1Name;
                this.param2NameTextBox.Text = email.Param2Name;
                this.param3NameTextBox.Text = email.Param3Name;
                this.eventDataParamNameTextBox.Text = email.EventDataParamName;
                this.distIdParamNameTextBox.Text = email.DistIdParamName;

                if (this.emailTemplatesDropDown.SelectedIndex < 0)
                {
                    this.templateGroupLabel.Text = "Email Template Group: ".Replace("Group: ", "");
                }
                else
                {
                    this.templateGroupLabel.Text = "Email Template Group: " + this.emailTemplatesDropDown.SelectedItem.Attributes["title"].Replace("Group: ", "");
                }
                
            }

        }

        private void SaveEmail(bool redirect)
        {
            Ux2CampaignEmail email = new Ux2CampaignEmail();

            try
            {

                email.Id = this.cEmailId;
                email.CampaignId = Convert.ToInt32(this.Session["CampaignId"]);
                email.Description = this.descriptionTextBox.Text;                
                email.EmailTypeId = Convert.ToInt32(this.emailTemplatesDropDown.SelectedValue);
                email.EmailGroupId = GetTemplateGroupId(Convert.ToInt32(this.emailTemplatesDropDown.SelectedValue), -1);
                email.Offset = GetOffsetHours();
                email.Query = this.queryTextBox.Text;
                email.Param1Name = this.param1NameTextBox.Text;
                email.Param2Name = this.param2NameTextBox.Text;
                email.Param3Name = this.param3NameTextBox.Text;
                email.EventDataParamName = this.eventDataParamNameTextBox.Text;
                email.DistIdParamName = this.distIdParamNameTextBox.Text;
                email.Active = this.statusCheckBox.Checked;

                email.Id = this.cEditorAPI.SaveCampaignEmail(email);

                this.Session["EmailEditRedirect"] = redirect;

                //Show Popup

                if (this.cEmailId == 0)
                    this.savedLabel.Text = "The new Campaign Email has been added. It's ID is " + email.Id.ToString() + ".";
                else
                    this.savedLabel.Text = "The Campaign Email has been updated.";
               
                this.savedMPE.Show();
            }
            catch (Exception exc)
            {
                //Show popup

                this.Session["EmailEditRedirect"] = false;
                this.savedLabel.Text = "Error Saving: " + exc.Message;
                this.savedMPE.Show();
            }


        }

        private int GetOffsetHours()
        {
            string offset = this.offsetTextBox.Text.Trim();
            string days = string.Empty;
            string hours = string.Empty;
            int total = 0;

            try
            {
                if (offset.Contains(":"))
                {
                    days = offset.Split(new char[] { ':' })[0];

                    hours = offset.Split(new char[] { ':' })[1];

                    if (days.Trim() == "")
                        days = "0";

                    if (hours.Trim() == "")
                        hours = "0";

                    total = (Convert.ToInt32(days) * 24) + Convert.ToInt32(hours);
                }
                else
                {
                    total = Convert.ToInt32(offset);
                }

                return total;
            }
            catch
            {
                throw new Exception("The Offset must be in a day:hours format or hours only.");
            }

            
        }

        private int GetTemplateGroupId(int id, int defaultId)
        {
            foreach (Ux2EmailTemplate template in this.cTemplates)
            {
                if (template.Id == id)
                    return template.GroupId;
            }

            return defaultId;

        }

        protected void saveButton_Click(object sender, EventArgs e)
        {
            SaveEmail(true);
        }

        protected void applyButton_Click(object sender, EventArgs e)
        {
            SaveEmail(false);
        }

        protected void savedOKButton_Click(object sender, EventArgs e)
        {
            this.savedMPE.Hide();

            if ((bool)this.Session["EmailEditRedirect"])
                Response.Redirect(LAST_PAGE);
        }


    }
}
