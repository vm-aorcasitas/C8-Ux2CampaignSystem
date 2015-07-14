using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;


namespace GanoExcel.Ux2
{
    public class ScheduleProcessor
    {
        private const string SP_GET_CAMPAIGN_PARAMS = "usp_ux2_get_campaign_schedule_params";
        private const string SP_GET_CAMPAIGN_SCHEDULE_EMAILS = "usp_ux2_get_active_schedule_emails";
        private const string SP_GET_CAMPAIGN_SCHEDULE_QUEUE = "usp_ux2_get_active_campaign_schedules";//
        private const string SP_GET_LOGGED_EMAILS = "usp_ux2_get_logged_emails";//

        private const string SP_UPDATE_SCHEDULE_QUEUE_ENTRY = "usp_ux2_update_schedule_queue_entry";//
        private const string SP_ADD_AUTO_EVENT_LOG = "usp_ux2_add_autoeventlog_entry";//
        private const string SP_ADD_EMAIL_LOG_ENTRY = "usp_ux2_add_email_log_entry";//

        private const int CAMPAIGN_FINISHED_STATUS = 2;
        private const int CAMPAIGN_ONGOING_STATUS = 1;

        private const int AUTOEVENLOG_NEW_STATUS = -1;

        private readonly DateTime MIN_SQL_DATETIME = new DateTime(1753, 1, 1);

        private string cConnectionString = string.Empty;
        private bool cCanceled = false;
        private int cMaxAge = 7 * 24;

        private class EmailEntry
        {
            public int cId = 0;
            public int cScheduleId = 0;
            public int cEmailTypeId = 0;
            public int cEmailGroupId = 0;
            public int cOffset = 0;
            public string cQuery = string.Empty;
            public string cParam1Name = string.Empty;
            public string cParam2Name = string.Empty;
            public string cParam3Name = string.Empty;
            public string cEventDataParamName = string.Empty;
            public string cDistIdParamName = string.Empty;
            

        }

        private class ScheduleQueueEntry
        {
            public int cId = 0;
            public int cScheduleId = 0;
            public int cStatus = 0;
            public DateTime cStartedOn = DateTime.Now;
            public DateTime cLastEvaluatedOn = DateTime.Now;
            public List<EmailEntry> cEmails = new List<EmailEntry>();
        }

        private class EmailLogEntry
        {
            public int cId = 0;
            public int cScheduleQueueId = 0;
            public int cEmailId = 0;
            public DateTime cProcessedOn = DateTime.Now;
        }

        private class AutoEventLogEntry
        {
            public int cGroupId = 0;
            public int cTypeId = 0;
            public int cPriSorceId = 0;
            public int cSecSorceId = 0;
            public int cTerSorceId = 0;
            public string cEventData = string.Empty;
            public int cDistId = 0;

            public bool cPriSorceIdIsNull = true;
            public bool cSecSorceIdIsNull = true;
            public bool cTerSorceIdIsNull = true;
            public bool cEventDataIsNull = true;
            public bool cDistIdIsNull = true;

            public int cScheduleEmailId = 0;
            public int cScheduleQueueId = 0;
            
        }

        public ScheduleProcessor(string connectionString, int maxAge)
        {
            this.cCanceled = false;
            this.cConnectionString = connectionString;
            this.cMaxAge = maxAge;
        }

        public void ProcessSchedules()
        {
            List<ScheduleQueueEntry> scheduleQueueEntries = null;
            List<AutoEventLogEntry> autoEventEntries = new List<AutoEventLogEntry>();

            this.cCanceled = false;
            
            scheduleQueueEntries = GetScheduleQueue();

            foreach (ScheduleQueueEntry entry in scheduleQueueEntries)
            {
                if (this.cCanceled)
                    return;
                autoEventEntries.AddRange(EvaluateSchedule(entry));                
            }
           
            foreach(AutoEventLogEntry entry in autoEventEntries)
            {
                if (this.cCanceled)
                    return;
                ProcessAutoEventEntry(entry);
            }

            foreach (ScheduleQueueEntry entry in scheduleQueueEntries)
            {

                if (this.cCanceled)
                    return;
                UpdateScheduleQueueEntry(entry);

            }
        }

        public void Cancel()
        {
            this.cCanceled = true;
        }

        private List<ScheduleQueueEntry> GetScheduleQueue()
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataAdapter da = null;
            DataSet ds = null;
            List<ScheduleQueueEntry> entries = new List<ScheduleQueueEntry>();
            ScheduleQueueEntry entry = null;

            try
            {
                conn = new SqlConnection(this.cConnectionString);
                cmd = new SqlCommand(SP_GET_CAMPAIGN_SCHEDULE_QUEUE, conn);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();

                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                da.Fill(ds);

                if (ds.Tables.Count > 0)
                {
                    foreach(DataRow row in ds.Tables[0].Rows)
                    {
                        entry = new ScheduleQueueEntry();

                        entry.cId = GetValue<int>(row, "Id", 0);
                        entry.cStatus = GetValue<int>(row, "Status", 0);
                        entry.cScheduleId = GetValue<int>(row, "CampaignScheduleId", 0);
                        entry.cStartedOn = GetValue<DateTime>(row, "StartedOn", DateTime.Now);
                        entry.cLastEvaluatedOn = GetValue<DateTime>(row, "LastEvaluatedOn", MIN_SQL_DATETIME);
                        entry.cEmails = GetScheduleEmails(entry.cScheduleId);
                        entries.Add(entry);
                    }
                }

                return entries;

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

        private List<EmailEntry> GetScheduleEmails(int scheduleId)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataAdapter da = null;
            DataSet ds = null;
            List<EmailEntry> entries = new List<EmailEntry>();
            EmailEntry entry = null;

            try
            {
                conn = new SqlConnection(this.cConnectionString);
                cmd = new SqlCommand(SP_GET_CAMPAIGN_SCHEDULE_EMAILS, conn);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();

                cmd.Parameters.AddWithValue("@campaignScheduleId", scheduleId);

                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                da.Fill(ds);

                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        entry = new EmailEntry();

                        entry.cId = GetValue<int>(row, "Id", 0);
                        entry.cScheduleId = GetValue<int>(row, "CampaignScheduleId", 0);
                        entry.cEmailTypeId = GetValue<int>(row, "EmailTypeId", 0);
                        entry.cEmailGroupId = GetValue<int>(row, "EmailGroupId", 0);
                        entry.cOffset = GetValue<int>(row, "Offset", 0);
                        entry.cQuery = GetValue<string>(row, "Query", string.Empty);
                        entry.cParam1Name = GetValue<string>(row, "Param1Name", string.Empty).ToUpper();
                        entry.cParam2Name = GetValue<string>(row, "Param2Name", string.Empty).ToUpper();
                        entry.cParam3Name = GetValue<string>(row, "Param3Name", string.Empty).ToUpper();
                        entry.cEventDataParamName = GetValue<string>(row, "EventDataParamName", string.Empty).ToUpper();
                        entry.cDistIdParamName = GetValue<string>(row, "DistIdParamName", string.Empty).ToUpper();

                        entries.Add(entry);
                    }
                }

                return entries;

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

        private List<EmailLogEntry> GetEmailLog(int scheduleQueueId)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataAdapter da = null;
            DataSet ds = null;
            List<EmailLogEntry> entries = new List<EmailLogEntry>();
            EmailLogEntry entry = null;

            try
            {
                conn = new SqlConnection(this.cConnectionString);
                cmd = new SqlCommand(SP_GET_LOGGED_EMAILS, conn);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();

                cmd.Parameters.AddWithValue("@scheduleQueueId", scheduleQueueId);

                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                da.Fill(ds);

                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        entry = new EmailLogEntry();

                        entry.cId = GetValue<int>(row, "Id", 0);
                        entry.cScheduleQueueId = GetValue<int>(row, "CampaignScheduleQueueId", 0);
                        entry.cEmailId = GetValue<int>(row, "CampaignEmailId", 0);
                        entry.cProcessedOn = GetValue<DateTime>(row, "ProcessedOn", MIN_SQL_DATETIME);

                        entries.Add(entry);
                    }
                }

                return entries;

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

        private Dictionary<string, string> GetCampaignParams(int scheduleQueueId)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataAdapter da = null;
            DataSet ds = null;
            Dictionary<string, string> entries = new Dictionary<string, string>();
            string paramName = string.Empty;

            try
            {
                conn = new SqlConnection(this.cConnectionString);
                cmd = new SqlCommand(SP_GET_CAMPAIGN_PARAMS, conn);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();

                cmd.Parameters.AddWithValue("@campaignScheduleQueueId", scheduleQueueId);

                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                da.Fill(ds);

                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {

                        paramName = GetValue<string>(row, "ParamName", string.Empty).ToUpper();

                        if (paramName != string.Empty)
                        {
                            if (entries.ContainsKey(paramName))
                                entries[paramName] = GetValue<string>(row, "ParamValue", string.Empty);
                            else
                                entries.Add(paramName, GetValue<string>(row, "ParamValue", string.Empty));
                        }
                    }
                }

                return entries;

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

        private List<AutoEventLogEntry> EvaluateSchedule(ScheduleQueueEntry entry)
        {
            List<EmailLogEntry> loggedEmails = new List<EmailLogEntry>();
            Dictionary<string, string> parameters = null;
            List<AutoEventLogEntry> aeEntries = new List<AutoEventLogEntry>();
            

            loggedEmails = GetEmailLog(entry.cId);

            foreach (EmailEntry email in entry.cEmails)
            {
                if (entry.cStartedOn.AddHours(email.cOffset) < DateTime.Now 
                    &&  !EmailEntryLogExists(loggedEmails, email.cId))
                {
                    if ((DateTime.Now.Subtract(entry.cStartedOn.AddHours(email.cOffset))).TotalHours > this.cMaxAge)
                    {
                        AddEmailLogEntry(email.cId, entry.cId, true);
                    }
                    else
                    {
                        parameters = GetCampaignParams(entry.cId);
                        aeEntries.AddRange(CreateAutoEventLogEntries(email, entry.cId, parameters));
                    }
                    
                }

            }

            return aeEntries;

        }
        
        private List<AutoEventLogEntry> CreateAutoEventLogEntries(EmailEntry email, int scheduleQueueId, Dictionary<string, string> parameters)
        {
            List<AutoEventLogEntry> entries = new List<AutoEventLogEntry>();
            AutoEventLogEntry aeEntry = null;
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataAdapter da = null;
            DataSet ds = null;
            string queryString = string.Empty;

            try
            {
                
                

                if (email.cQuery == string.Empty)
                {
                    aeEntry = CreateAutoEventEntry(email, scheduleQueueId, parameters);

                    if (aeEntry != null)
                        entries.Add(aeEntry);
                }
                else
                {
                    conn = new SqlConnection(this.cConnectionString);
                    conn.Open();

                    foreach (string paramName in parameters.Keys)
                    {
                        email.cQuery = Regex.Replace(email.cQuery, "\\[\\{" + paramName + "\\}\\]", parameters[paramName].ToString(), RegexOptions.IgnoreCase);
                        Console.WriteLine(email.cQuery);
                    }

                    cmd = new SqlCommand(email.cQuery, conn);
                    da = new SqlDataAdapter(cmd);
                    ds = new DataSet();

                    cmd.CommandType = CommandType.Text;

                    da.Fill(ds);
                    
                    if (ds.Tables.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {

                            parameters = new Dictionary<string, string>();

                            foreach (DataColumn col in ds.Tables[0].Columns)
                            {
                                if (!parameters.ContainsKey(col.ColumnName.ToUpper()))
                                    parameters.Add(col.ColumnName.ToUpper(), row[col].ToString());
                            }
                            
                            aeEntry = CreateAutoEventEntry(email, scheduleQueueId, parameters);

                            if (aeEntry != null)
                                entries.Add(aeEntry);
                        }
                    }

                    da.Dispose();
                    ds.Dispose();
                    cmd.Dispose();

                }

                return entries;

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

        private AutoEventLogEntry CreateAutoEventEntry(EmailEntry email, int scheduleQueueId, Dictionary<string, string> parameters)
        {
            AutoEventLogEntry entry = new AutoEventLogEntry();
            try
            {
                entry.cGroupId = email.cEmailGroupId;
                entry.cTypeId = email.cEmailTypeId;
                entry.cScheduleEmailId = email.cId;
                entry.cScheduleQueueId = scheduleQueueId;

                if (email.cParam1Name != string.Empty && parameters.ContainsKey(email.cParam1Name) && parameters[email.cParam1Name] != string.Empty)
                {
                    entry.cPriSorceId = Convert.ToInt32(parameters[email.cParam1Name]);
                    entry.cPriSorceIdIsNull = false;
                }

                if (email.cParam2Name != string.Empty && parameters.ContainsKey(email.cParam2Name) && parameters[email.cParam2Name] != string.Empty) 
                {
                    entry.cSecSorceId = Convert.ToInt32(parameters[email.cParam2Name]);
                    entry.cSecSorceIdIsNull = false;
                }

                if (email.cParam3Name != string.Empty && parameters.ContainsKey(email.cParam3Name) && parameters[email.cParam3Name] != string.Empty) 
                {
                    entry.cTerSorceId = Convert.ToInt32(parameters[email.cParam3Name]);
                    entry.cTerSorceIdIsNull = false;
                }

                if (email.cDistIdParamName != string.Empty && parameters.ContainsKey(email.cDistIdParamName) && parameters[email.cDistIdParamName] != string.Empty) 
                {
                    entry.cDistId = Convert.ToInt32(parameters[email.cDistIdParamName]);
                    entry.cDistIdIsNull = false;
                }


                if (email.cEventDataParamName != string.Empty && parameters.ContainsKey(email.cEventDataParamName))
                {
                    entry.cEventData = parameters[email.cEventDataParamName];
                    entry.cEventDataIsNull = false;
                }

                return entry;
            }
            catch 
            {
                return null;
            }
        }

        private void ProcessAutoEventEntry(AutoEventLogEntry entry)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;

            try
            {
                conn = new SqlConnection(this.cConnectionString); 
                cmd = new SqlCommand(SP_ADD_AUTO_EVENT_LOG, conn);

                cmd.Parameters.AddWithValue("@groupId", entry.cGroupId);
                cmd.Parameters.AddWithValue("@typeId", entry.cTypeId);
                
                if (entry.cPriSorceIdIsNull)
                    cmd.Parameters.AddWithValue("@priSource", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@priSource", entry.cPriSorceId);
                                
                if (entry.cSecSorceIdIsNull)
                    cmd.Parameters.AddWithValue("@secSource", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@secSource", entry.cSecSorceId);

                if (entry.cTerSorceIdIsNull)
                    cmd.Parameters.AddWithValue("@terSource", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@terSource", entry.cTerSorceId);

                if (entry.cEventDataIsNull)
                    cmd.Parameters.AddWithValue("@eventData", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@eventData", entry.cEventData); 
                
                if (entry.cDistIdIsNull)
                    cmd.Parameters.AddWithValue("@distId", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@distId", entry.cDistId);

                cmd.Parameters.AddWithValue("@status", AUTOEVENLOG_NEW_STATUS);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                cmd.ExecuteNonQuery();

                AddEmailLogEntry(entry.cScheduleEmailId, entry.cScheduleQueueId, false);


            }
            finally
            {
                if (conn != null)
                    conn.Dispose();

                if (cmd != null)
                    cmd.Dispose();
            }
        }

        private void AddEmailLogEntry(int scheduleEmailId, int scheduleQueueId, bool skipped)
        {

            SqlConnection conn = null;
            SqlCommand cmd = null;

            try
            {

                conn = new SqlConnection(this.cConnectionString);
                cmd = new SqlCommand(SP_ADD_EMAIL_LOG_ENTRY, conn);

                cmd.Parameters.AddWithValue("@scheduleEmailId", scheduleEmailId);
                cmd.Parameters.AddWithValue("@scheduleQueueId", scheduleQueueId);
                cmd.Parameters.AddWithValue("@processedOn", DateTime.Now);
                cmd.Parameters.AddWithValue("@skipped", skipped);

                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();

                cmd.ExecuteNonQuery();
                
            }
            finally
            {
                if (conn != null)
                    conn.Dispose();

                if (cmd != null)
                    cmd.Dispose();
            }
        }

        private void UpdateScheduleQueueEntry(ScheduleQueueEntry entry)
        {

            SqlConnection conn = null;
            SqlCommand cmd = null;
            List<EmailLogEntry> logEntries = null;
            int status = CAMPAIGN_FINISHED_STATUS;


            try
            {

                logEntries = GetEmailLog(entry.cId);

                foreach (EmailEntry email in entry.cEmails)
                {
                    if (!EmailEntryLogExists(logEntries, email.cId))
                    {
                        status = CAMPAIGN_ONGOING_STATUS;
                        break;
                    }
                }

                conn = new SqlConnection(this.cConnectionString);
                cmd = new SqlCommand(SP_UPDATE_SCHEDULE_QUEUE_ENTRY, conn);

                cmd.Parameters.AddWithValue("@id", entry.cId);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@startedOn", entry.cStartedOn);
                cmd.Parameters.AddWithValue("@lastEvaluatedOn", DateTime.Now);

                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                cmd.ExecuteNonQuery();

            }
            finally
            {
                if (conn != null)
                    conn.Dispose();

                if (cmd != null)
                    cmd.Dispose();
            }

        }

        private bool EmailEntryLogExists(List<EmailLogEntry> loggedEmails, int emailId)
        {
            foreach (EmailLogEntry entry in loggedEmails)
            {
                if (entry.cEmailId == emailId)
                    return true;
            }

            return false;
        }

        private static T GetValue<T>(DataRow row, string col, T defaultVal)
        {
            try
            {
                if (row[col] == DBNull.Value)
                    return (T)defaultVal;
                else
                    return (T)Convert.ChangeType(row[col], typeof(T));
            }
            catch
            {
                return (T)defaultVal;
            }
        }


    }
}
