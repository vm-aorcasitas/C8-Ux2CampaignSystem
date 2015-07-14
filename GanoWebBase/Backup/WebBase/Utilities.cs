using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace GanoExcel.Web.Base
{
    public static class Utilities
    {
        
        private const string CONN_STR_PUB_KEY = "gano";
        private const string INI_FILE = "dbconnect.ini";
        private const string TEST_INI_FILE = "dbtestconnect.ini";
        private const string CONNECTION_STRING_ARG = "/C";
        
        public static string GetConnectionStringValue(string connectionString, string key)
        {
            string[] pairs = connectionString.Split(new char[] { ';' });
            string[] keyValue;
            char[] delim = new char[] {'='};

            if (connectionString == null)
                return "";

            foreach (string pair in pairs)
            {

                keyValue = pair.Split(delim);
                if (keyValue[0].Trim() == key)
                {
                    if (keyValue.Length > 1)
                        return keyValue[1].Trim();
                    else
                        return "";
                }
            }

            return "";

        }

        public static string AssemblyVersion
        {
            get
            {
                return Assembly.GetEntryAssembly().GetName().Version.ToString();
            }
        }

        public static String GetGanoDataSource(string connectionString)
        {
            string source;

            if (connectionString == null)
                return "";

            source = GetConnectionStringValue(connectionString, "Data Source");
            source += "; " + GetConnectionStringValue(connectionString, "Initial Catalog");

            return source;
        }

        public static string GetCommandArg(string[] args, string key)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].ToUpper() == key && i + 1 < args.Length)
                    return args[i + 1];
            }

            return "";
        }

        public static void CheckDatabaseConnection(string connectionString)
        {
            CheckDatabaseConnection(connectionString, 15);
        }

        public static void CheckDatabaseConnection(string connectionString, int timeOut)
        {
            SqlConnection conn = null;

            try
            {
                if (!connectionString.EndsWith(";"))
                    connectionString += ";";


                connectionString += "Connection Timeout =" + timeOut.ToString() + ";";

                conn = new SqlConnection(connectionString);
                conn.Open();
            }
            finally
            {
                if (conn != null)
                {
                    if (conn.State != System.Data.ConnectionState.Closed)
                        conn.Close();

                    conn.Dispose();
                }
            }
        }

        public static string GetAppSetting(string key)
        {
            string value;
            
            try
            {
                value = ConfigurationSettings.AppSettings[key];

                if (value == null)
                    value = "";

                return value;
            }
            catch 
            {
                return "";
            }            

        }

        public static string GetConnectionString()
        {
            Properties.Settings settings = new GanoExcel.Web.Base.Properties.Settings();
            
            return settings["DefaultString"].ToString(); 
        }

        public static string GetConnectionString(int countryId, bool devServer)
        {
            if (devServer)
                return ConfigurationSettings.AppSettings["ConnectionString_Dev_" + countryId.ToString()];
            else
                return ConfigurationSettings.AppSettings["ConnectionString_" + countryId.ToString()];
        }

    }
}
