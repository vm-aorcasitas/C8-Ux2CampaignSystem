using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

/*
 *By Branden Boucher
 *02MAR2011
 *
 * Where as the GanoBase classes were made for more statful use such as in desktop applications, the GanoExcel.Web.Base classes are intented to
 * be used in a much more stateless way. The two namespaces are very similar but their intended use is what separates them.
 */
 
namespace GanoExcel.Web.Base
{

    public class Security
    {
 
        private const string SP_AUTHENTICATE = "[dbo].[usp_geweb_get_user_by_username]";
        private const string SP_AUTHORIZE = "[dbo].[uspAuthorized]";

        public static Boolean IsUserAuthorized(int userId, int securtyCode, string connectionString)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataAdapter adapter = null;
            DataSet data = null;

            try
            {
                conn = new SqlConnection(connectionString);
                cmd = new SqlCommand(SP_AUTHORIZE);
                adapter = new SqlDataAdapter(cmd);
                data = new DataSet();




                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@SecurityCode", securtyCode);

                if (conn.State != ConnectionState.Closed)
                    conn.Close();

                conn.Open();

                adapter.Fill(data);

                if (data.Tables.Count < 1)
                    return false;

                if (data.Tables[0].Rows.Count < 1)
                    return false;
                
                return (bool)data.Tables[0].Rows[0]["Permission"];
            }
            finally
            {
                if (conn != null)
                    conn.Dispose();

                if (cmd != null)
                    cmd.Dispose();

                if (adapter != null)
                    adapter.Dispose();

                if (data != null)
                    data.Dispose();
            }

        }

        public static User AuthenticateUser(string loginName, string password, string connectionString)
        {
            User user = null;
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataAdapter adapter = null;
            DataSet data = null;
            DataRow row = null;

            try
            {

                conn = new SqlConnection(connectionString);
                cmd = new SqlCommand(SP_AUTHENTICATE);
                adapter = new SqlDataAdapter(cmd);
                data = new DataSet();




                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserName", loginName);

                if (conn.State != ConnectionState.Closed)
                    conn.Close();

                conn.Open();

                adapter.Fill(data);

                if (data.Tables.Count < 1)
                    return null;

                if (data.Tables[0].Rows.Count < 1)
                    return null;

                row = data.Tables[0].Rows[0];

                

                data.Tables[0].Columns[0].DefaultValue = 0;
                data.Tables[0].Columns[1].DefaultValue = "";
                data.Tables[0].Columns[2].DefaultValue = "";
                data.Tables[0].Columns[3].DefaultValue = "";
                data.Tables[0].Columns[4].DefaultValue = "";
                data.Tables[0].Columns[5].DefaultValue = 0;
                data.Tables[0].Columns[6].DefaultValue = 0;
                data.Tables[0].Columns[7].DefaultValue = 0;
                data.Tables[0].Columns[8].DefaultValue = DateTime.MinValue;
                data.Tables[0].Columns[9].DefaultValue = false;
                data.Tables[0].Columns[10].DefaultValue = false;

                user = new User((int)row["UserKey"], row["UserId"].ToString(), row["FirstName"].ToString(), row["LastName"].ToString(), row["MI"].ToString(),
                    (int)row["DefaultLanguage"], (int)row["DefaultWarehouse"], (int)row["UserScope"], (DateTime)row["LastLogin"], 
                    (bool)row["ForcePasswordChange"], (bool)row["Deleted"]);

                if (Cryptography.Decrypt(row["Password"].ToString(), user.Id.ToString()) != password)
                    return null;

                return user;

            }
            finally
            {
                if (conn != null)
                    conn.Dispose();

                if (cmd != null)
                    cmd.Dispose();

                if (adapter != null) 
                    adapter.Dispose();

                if (data != null) 
                    data.Dispose();



            }


        }

        [Serializable]
        public class SecurityException : BaseException
        {
            public SecurityException() :
                base()
            {
            }

            public SecurityException(string message) :
                base(message)
            {
            }

            public SecurityException(string message, Exception innerException) :
                base(message, innerException)
            {
            }
        }
    }
}
