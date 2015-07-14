using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GanoExcel.Ux2CampaignEditor
{
    public partial class SQLPreview : System.Web.UI.Page
    {

        private const string NEXT_PAGE = "Preview.aspx";

        protected void Page_Load(object sender, EventArgs e)
            
        {
            List<string> parameters = null;

            if (this.Session["SQLQuery"] == null)
                return;

            parameters = BuildParamList(this.Session["SQLQuery"].ToString());
            
            if (parameters.Count < 1)
                Response.Redirect(NEXT_PAGE);
            else
                AddParamFields(parameters);
        }

        private List<string> BuildParamList(string sqlQuery)
        {
            List<string> parameters = new List<string>();
            
            int pos = 0;
            int lastPos = 0;

            while (true)
            {
                pos = sqlQuery.IndexOf("[{", lastPos);

                if (pos < 0)
                    break;

                pos += 2;

                lastPos = sqlQuery.IndexOf("}]", pos);

                if (lastPos < 0)
                {
                    parameters.Add(sqlQuery.Substring(pos));
                    break;
                }

                parameters.Add(sqlQuery.Substring(pos, lastPos - pos));


            }

            return parameters;


        }

        private void AddParamFields(List<string> parameters)
        {

            Label paramLabel = null;
            TextBox paramTextBox = null;
            int i = 0;

            foreach (string param in parameters)
            {
                paramTextBox = new TextBox();
                paramTextBox.CssClass = "ParamValue";
                paramTextBox.ID = "param" + i.ToString() + "TextBox";
                paramLabel = new Label();
                paramLabel.CssClass = "ParamName";
                paramLabel.AssociatedControlID = "param" + i.ToString() + "TextBox";
                paramLabel.Text = param;

                this.Form.Controls.Add(paramLabel);
                this.Form.Controls.Add(paramTextBox);

                i++;
            }

        }


        private void BuildSQLQuery()
        {
            string sqlQuery = string.Empty;
            List<string> parameters = null; ; 
            int i = 0;
            TextBox paramTextBox = null;

            this.Session["SQLQueryPreview"] = "";

            if (this.Session["SQLQuery"] == null)
                return;

            sqlQuery = this.Session["SQLQuery"].ToString();

            parameters = BuildParamList(this.Session["SQLQuery"].ToString());

            foreach (string param in parameters)
            {
                paramTextBox = (TextBox)this.Form.FindControl("param" + i.ToString() + "TextBox");

                if (paramTextBox != null)
                {
                    sqlQuery = sqlQuery.Replace("[{" + param + "}]", paramTextBox.Text);
                }

                i++;

            }

            this.Session["SQLQueryPreview"] = sqlQuery;
        }

        protected void runButton_Click(object sender, EventArgs e)
        {

            BuildSQLQuery();
            this.Response.Redirect("Preview.aspx");


        }


    }
}
