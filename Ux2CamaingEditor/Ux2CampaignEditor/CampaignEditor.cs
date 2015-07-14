using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace GanoExcel.Ux2CampaignEditor
{
    public class CampaignEditor
    {
        private const string RESULTS_ROW_CLASS_PREFIX = "alt_row_";


        private string cConnectionString = string.Empty;
        private string cResult = string.Empty;

        public CampaignEditor(string connectionString)
        {
            this.cConnectionString = connectionString;
        }

        public DataSet RunQuery(string sql)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataAdapter da = null;
            DataSet ds = null;

            try
            {


                conn = new SqlConnection(this.cConnectionString);
                cmd = new SqlCommand(sql, conn);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();

                cmd.CommandType = CommandType.Text;

                conn.Open();

                da.Fill(ds);

                if (ds.Tables.Count < 1)
                {
                    this.cResult = "No Records Returned";
                    return null;
                }

                if (ds.Tables[0].Rows.Count < 1)
                {
                    this.cResult = "No Records Returned";
                    return null;
                }

                return ds;

            }
            catch (Exception exc)
            {

                this.cResult = "Error: " + exc.Message;
                return null;
            }
            finally
            {
                if (conn != null)
                    conn.Dispose();

                if (cmd != null)
                    cmd.Dispose();

                if (da != null)
                    da.Dispose();

                if (ds != null)
                    ds.Dispose();
            }

        }

        public string BuildTable(DataSet ds)
        {
            string htmlTable = string.Empty;
            int i = 0;

            try
            {


                htmlTable = "<table class=\"QueryResults sortable\">";

                htmlTable += "<tr class=\"HeaderRow\">";

                foreach (DataColumn col in ds.Tables[0].Columns)
                {

                    htmlTable += "<th class=" + i.ToString() + ">" + col.ColumnName + "</th>";
                    i++;
                }

                htmlTable += "</tr>";

                //This is set up so that the first col header is marked with class="FirstCol" and the last is marked with class="LastCol". If there is only one col then
                //that col gets both classes.
                if (i == 1)
                {
                    htmlTable = htmlTable.Replace("class=0", "class=\"FirstCol LastCol\"");
                }
                else
                {
                    i--;
                    htmlTable = htmlTable.Replace("class=0", "class=\"FirstCol\"");
                    htmlTable = htmlTable.Replace("class=" + i.ToString(), "class=\"LastCol\"");
                }

                i = 0;

                foreach (DataRow row in ds.Tables[0].Rows)
                {

                    htmlTable += "<tr class=\"" + RESULTS_ROW_CLASS_PREFIX + (i % 2).ToString() + "\">";

                    foreach (DataColumn col in ds.Tables[0].Columns)
                    {
                        htmlTable += "<td>" + row[col].ToString() + "</td>";
                    }

                    htmlTable += "</tr>";

                    i++;
                }



                if (ds.Tables[0].Rows.Count == 1)
                    this.cResult = "1 Row Returned";
                else
                    this.cResult = ds.Tables[0].Rows.Count.ToString() + " Rows Returned";

                return htmlTable;

            }
            catch (Exception exc)
            {
                this.cResult = "Error: " + exc.Message;
                return string.Empty;
            }
        }

        public string Result
        {
            get { return this.cResult; }
        }

        public static string HoursToDaysAndHours(int hours)
        {
            int days;


            days = hours / 24;
            hours -= days * 24;

            return days.ToString() + ":" + hours.ToString();

        }
    }
}
