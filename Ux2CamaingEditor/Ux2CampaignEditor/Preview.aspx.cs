using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace GanoExcel.Ux2CampaignEditor
{
    public partial class ExecQuery : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            string sqlQuery = string.Empty;
            //string param1Name = string.Empty;
            //string param1Val = string.Empty;
            //string param2Name = string.Empty;
            //string param2Val = string.Empty;
            //string param3Name = string.Empty;
            //string param3Val = string.Empty;
            string htmlTable = string.Empty;
            //string eMailText = string.Empty;
            //string eMail = string.Empty;

            Properties.Settings setting = new Properties.Settings();
            CampaignEditor editor = new CampaignEditor(setting.ConnectionString);
            DataSet ds = null;

            this.backLabel.PostBackUrl = Request.UrlReferrer.AbsoluteUri;

            if (this.Session["SQLQueryPreview"] != null)
                sqlQuery = this.Session["SQLQueryPreview"].ToString();


            if (sqlQuery == "")
            {
                this.resultsLabel.Text = "No Query to Preview";
                this.backLabel.Visible = false;
                return;
            }

            ds = editor.RunQuery(sqlQuery);
            this.resultsLabel.Text = editor.Result;

            if (ds == null)
                return;

            //if ((bool)this.Session["DoEmail"])
            //{
            //    eMail = emu.BuildEmail(eMailText, ds.Tables[0]);
            //    this.resultsLabel.Text = emu.Result;

            //    if (eMail == "")
            //        return;

            //    this.Session["IFrameHTML"] = eMail;
                
            //    //this.resultsPanel.InnerHtml = eMail;
                

            //    //((LiteralControl)this.resultsPanel.Controls[0]).Text = eMail;
            //}
            //else
            //{
            htmlTable = editor.BuildTable(ds);
            this.resultsLabel.Text = editor.Result;

            if (htmlTable == "")
                return;
            this.Session["IFrameHTML"] = htmlTable;

               
                //((LiteralControl)this.resultsPanel.Controls[0]).Text = htmlTable;
            //}

            this.resultsPanel.Attributes.Add("src", "PreviewGenerator.aspx");


        }





    }
}
