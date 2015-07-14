using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace GanoExcel.Ux2CampaignEditor 
{
    public class Ux2CampaingEditorAPI
    {
        private const string SP_GET_SCHEDULES = "usp_ux2_editor_get_campaign_schedules";
        private const string SP_GET_EMAILS = "usp_ux2_editor_get_campaign_emails";
        private const string SP_GET_SCHEDULE_BY_ID = "usp_ux2_editor_get_campaign_schedule_by_id";
        private const string SP_GET_EMAIL_BY_ID = "usp_ux2_editor_get_campaign_email_by_id";
        private const string SP_ADD_SCHEDULE = "usp_ux2_editor_add_campaign_schedule";
        private const string SP_ADD_EMAIL = "usp_ux2_editor_add_campaign_email";
        private const string SP_UPDATE_SCHEDULE = "usp_ux2_editor_update_campaign_schedule";
        private const string SP_UPDATE_EMAIL = "usp_ux2_editor_update_campaign_email";
        private const string SP_GET_EMAIL_TEMPLATES = "usp_ux2_editor_get_ae_email_templates";

        private string cConnectionString = string.Empty;


        public Ux2CampaingEditorAPI(string connectionString)
        {
            this.cConnectionString = connectionString;
        }

        public int AddCampaign(string description, bool active, int userId)
        {

            SqlConnection conn = null;
            SqlCommand cmd = null;

            try
            {
                conn = new SqlConnection(this.cConnectionString);
                cmd = new SqlCommand(SP_ADD_SCHEDULE, conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@createdBy", userId);
                cmd.Parameters.AddWithValue("@createdOn", DateTime.Now);
                cmd.Parameters.AddWithValue("@active", active);
                cmd.Parameters.Add("@id", SqlDbType.Int).Direction = ParameterDirection.Output;

                conn.Open();
                cmd.ExecuteNonQuery();

                if (cmd.Parameters["@id"].Value == DBNull.Value)
                    return -1;
                else
                    return Convert.ToInt32(cmd.Parameters["@id"].Value);


            }
            finally
            {
                if (conn != null)
                    conn.Dispose();

                if (cmd != null)
                    cmd.Dispose();
            }

        }

        public void UpdateCampaign(int id, string description, bool active)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;

            try
            {
                conn = new SqlConnection(this.cConnectionString);
                cmd = new SqlCommand(SP_UPDATE_SCHEDULE, conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@active", active);

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

        public List<Ux2CampaignSchedule> GetCampaignSchedules()
        {
            List<Ux2CampaignSchedule> schedules = new List<Ux2CampaignSchedule>();
            Ux2CampaignSchedule schedule = null;
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataAdapter da = null;
            DataSet ds = null;

            try
            {
                conn = new SqlConnection(this.cConnectionString);
                cmd = new SqlCommand(SP_GET_SCHEDULES, conn);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();

                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                da.Fill(ds);

                if (ds.Tables.Count > 0 )
                {

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        schedule = new Ux2CampaignSchedule();
                        schedule.Id = GetValue<int>(row, "Id", 0);
                        schedule.Description = GetValue<string>(row, "Description", string.Empty);
                        schedule.CreatedBy = GetValue<int>(row, "CreatedBy", 0);
                        schedule.CreatedOn = GetValue<DateTime>(row, "CreatedOn", DateTime.Now);
                        schedule.Active = GetValue<bool>(row, "Active", false);

                        schedules.Add(schedule);
                    }
                
                }

                return schedules;

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

        public Ux2CampaignSchedule GetCampaignSchedule(int id)
        {
            Ux2CampaignSchedule schedule = null;
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataAdapter da = null;
            DataSet ds = null;
            
            try
            {
                conn = new SqlConnection(this.cConnectionString);
                cmd = new SqlCommand(SP_GET_SCHEDULE_BY_ID, conn);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                da.Fill(ds);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {

                    schedule = new Ux2CampaignSchedule();
                    schedule.Id = GetValue<int>(ds.Tables[0].Rows[0], "Id", 0);
                    schedule.Description = GetValue<string>(ds.Tables[0].Rows[0], "Description", string.Empty);
                    schedule.CreatedBy = GetValue<int>(ds.Tables[0].Rows[0], "CreatedBy", 0);
                    schedule.CreatedOn = GetValue<DateTime>(ds.Tables[0].Rows[0], "CreatedOn", DateTime.Now);
                    schedule.Active = GetValue<bool>(ds.Tables[0].Rows[0], "Active", false);

                }

                return schedule;

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


        public int SaveCampaignEmail(Ux2CampaignEmail campaignEmail)
        {

            SqlConnection conn = null;
            SqlCommand cmd = null;

            try
            {
                conn = new SqlConnection(this.cConnectionString);

                if (campaignEmail.Id < 1)
                { //Assume this is a new email
                    cmd = new SqlCommand(SP_ADD_EMAIL, conn);
                    cmd.Parameters.Add("@id", SqlDbType.Int).Direction = ParameterDirection.Output;
                }
                else
                {
                    cmd = new SqlCommand(SP_UPDATE_EMAIL, conn);
                    cmd.Parameters.AddWithValue("@id", campaignEmail.Id);
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@description", campaignEmail.Description);
                cmd.Parameters.AddWithValue("@scheduleId", campaignEmail.CampaignId);
                cmd.Parameters.AddWithValue("@emailTypeId", campaignEmail.EmailTypeId);
                cmd.Parameters.AddWithValue("@emailGroupId", campaignEmail.EmailGroupId);
                cmd.Parameters.AddWithValue("@offset", campaignEmail.Offset);
                cmd.Parameters.AddWithValue("@query", campaignEmail.Query);
                cmd.Parameters.AddWithValue("@param1Name", campaignEmail.Param1Name);
                cmd.Parameters.AddWithValue("@param2Name", campaignEmail.Param2Name);
                cmd.Parameters.AddWithValue("@param3Name", campaignEmail.Param3Name);
                cmd.Parameters.AddWithValue("@eventDataParamName", campaignEmail.EventDataParamName);
                cmd.Parameters.AddWithValue("@distIdParamName", campaignEmail.DistIdParamName);
                cmd.Parameters.AddWithValue("@active", campaignEmail.Active);

                conn.Open();

                cmd.ExecuteNonQuery();

                if (campaignEmail.Id < 1)
                {
                    if (cmd.Parameters["@id"].Value == DBNull.Value)
                        return -1;
                    else
                        return Convert.ToInt32(cmd.Parameters["@id"].Value);
                }

                return campaignEmail.Id;

            }
            finally
            {
                if (conn != null)
                    conn.Dispose();

                if (cmd != null)
                    cmd.Dispose();
            }

        }

        public List<Ux2CampaignEmail> GetCampaignEmails(int campaignId)
        {

            List<Ux2CampaignEmail> emails = new List<Ux2CampaignEmail>();
            Ux2CampaignEmail email = null;
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataAdapter da = null;
            DataSet ds = null;

            try
            {
                conn = new SqlConnection(this.cConnectionString);
                cmd = new SqlCommand(SP_GET_EMAILS, conn);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@scheduleId", campaignId);

                conn.Open();
                da.Fill(ds);

                if (ds.Tables.Count > 0)
                {

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        email = new Ux2CampaignEmail();
                        email.Id = GetValue<int>(row, "Id", 0);
                        email.CampaignId = GetValue<int>(row, "CampaignScheduleId", 0);
                        email.Description = GetValue<string>(row, "Description", string.Empty);
                        email.EmailTypeId = GetValue<int>(row, "EmailTypeId", 0);
                        email.EmailGroupId = GetValue<int>(row, "EmailGroupId", 0);
                        email.Offset = GetValue<int>(row, "Offset", 0);
                        email.Param1Name = GetValue<string>(row, "Param1Name", string.Empty);
                        email.Param2Name = GetValue<string>(row, "Param2Name", string.Empty);
                        email.Param3Name = GetValue<string>(row, "Param3Name", string.Empty);
                        email.EventDataParamName = GetValue<string>(row, "EventDataParamName", string.Empty);
                        email.DistIdParamName = GetValue<string>(row, "DistIdParamName", string.Empty);
                        email.Active = GetValue<bool>(row, "Active", false);

                        emails.Add(email);
                    }

                }

                return emails;

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

        public Ux2CampaignEmail GetCampaignEmail(int id)
        {

            Ux2CampaignEmail email = null;
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataAdapter da = null;
            DataSet ds = null;

            try
            {
                conn = new SqlConnection(this.cConnectionString);
                cmd = new SqlCommand(SP_GET_EMAIL_BY_ID, conn);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                da.Fill(ds);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {

                        email = new Ux2CampaignEmail();
                        email.Id = GetValue<int>(ds.Tables[0].Rows[0], "Id", 0);
                        email.CampaignId = GetValue<int>(ds.Tables[0].Rows[0], "CampaignScheduleId", 0);
                        email.Description = GetValue<string>(ds.Tables[0].Rows[0], "Description", string.Empty);
                        email.EmailTypeId = GetValue<int>(ds.Tables[0].Rows[0], "EmailTypeId", 0);
                        email.EmailGroupId = GetValue<int>(ds.Tables[0].Rows[0], "EmailGroupId", 0);
                        email.Offset = GetValue<int>(ds.Tables[0].Rows[0], "Offset", 0);
                        email.Query = GetValue<string>(ds.Tables[0].Rows[0], "Query", "");
                        email.Param1Name = GetValue<string>(ds.Tables[0].Rows[0], "Param1Name", string.Empty);
                        email.Param2Name = GetValue<string>(ds.Tables[0].Rows[0], "Param2Name", string.Empty);
                        email.Param3Name = GetValue<string>(ds.Tables[0].Rows[0], "Param3Name", string.Empty);
                        email.EventDataParamName = GetValue<string>(ds.Tables[0].Rows[0], "EventDataParamName", string.Empty);
                        email.DistIdParamName = GetValue<string>(ds.Tables[0].Rows[0], "DistIdParamName", string.Empty);
                        email.Active = GetValue<bool>(ds.Tables[0].Rows[0], "Active", false);

                }

                return email;

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

        public List<Ux2EmailTemplate> GetEmailTemplates()
        {

            List<Ux2EmailTemplate> templates = new List<Ux2EmailTemplate>();
            Ux2EmailTemplate template = null;
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataAdapter da = null;
            DataSet ds = null;

            try
            {
                conn = new SqlConnection(this.cConnectionString);
                cmd = new SqlCommand(SP_GET_EMAIL_TEMPLATES, conn);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();

                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                da.Fill(ds);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        template = new Ux2EmailTemplate();
                        
                        template.Id = GetValue<int>(row, "Id", 0);
                        template.Description = GetValue<string>(row, "Description", string.Empty);
                        template.GroupId = GetValue<int>(row, "GroupId", 0);
                        template.GroupDescription = GetValue<string>(row, "GroupDescription", string.Empty);

                        templates.Add(template);
                    }

                }

                return templates;

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

        private T GetValue<T>(DataRow row, string col, T defaultVal)
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

    public class Ux2CampaignSchedule
    {
        private int cId = 0;
        private string cDescription = string.Empty;
        private int cCreadedBy = 0;
        private DateTime cCreatedOn = DateTime.Now;
        private bool cActive = false;

        public Ux2CampaignSchedule()
        {
        }

        public Ux2CampaignSchedule(int id, string description, int createdBy, DateTime createdOn, bool active)
        {
            this.cId = id;
            this.cDescription = description;
            this.cCreadedBy = createdBy;
            this.cCreatedOn = createdOn;
            this.cActive = active;
        }

        public int Id
        {
            get { return this.cId; }
            set { this.cId = value; }
        }

        public string Description
        {
            get { return this.cDescription; }
            set { this.cDescription = value; }
        }

        public int CreatedBy
        {
            get { return this.cCreadedBy; }
            set { this.cCreadedBy = value; }
        }

        public DateTime CreatedOn
        {
            get { return this.cCreatedOn; }
            set { this.cCreatedOn = value; }
        }

        public bool Active
        {
            get { return this.cActive; }
            set { this.cActive = value; }
        }

    }

    public class Ux2CampaignEmail
    {
        private int cId = 0;
        private int cCampaignId = 0;
        private string cDescription = string.Empty;
        private int cEmailTypeId = 0;
        private int cEmailGroupId = 0;
        private int cOffset = 0;
        private string cQuery = string.Empty;
        private string cParam1Name = string.Empty;
        private string cParam2Name = string.Empty;
        private string cParam3Name = string.Empty;
        private string cEventDataParamName = string.Empty;
        private string cDistIdParamName = string.Empty;
        private bool cActive = false;

        public Ux2CampaignEmail()
        {
        }

        public Ux2CampaignEmail(int id, int campaignId, string description, int emailTypeId, int emailGroupId, int offset, string query, string param1Name,
            string param2Name, string param3Name, string eventDataParamName, string distIdParamName, bool active)
        {
            this.cId = id;
            this.cCampaignId = campaignId;
            this.cDescription = description;
            this.cEmailTypeId = emailTypeId;
            this.cEmailGroupId = emailGroupId;
            this.cOffset = offset;
            this.cQuery = query;
            this.cParam1Name = param1Name;
            this.cParam2Name = param2Name;
            this.cParam3Name = param3Name;
            this.cEventDataParamName = eventDataParamName;
            this.cDistIdParamName = distIdParamName;
            this.cActive = active;
        }


        public int Id
        {
            get { return this.cId; }
            set { this.cId = value; }
        }

        public int CampaignId
        {
            get { return this.cCampaignId; }
            set { this.cCampaignId = value; }
        }

        public string Description
        {
            get { return this.cDescription; }
            set { this.cDescription = value; }
        }

        public int EmailTypeId
        {
            get { return this.cEmailTypeId; }
            set { this.cEmailTypeId = value; }
        }

        public int EmailGroupId
        {
            get { return this.cEmailGroupId; }
            set { this.cEmailGroupId = value; }
        }

        public int Offset
        {
            get { return this.cOffset; }
            set { this.cOffset = value; }
        }

        public string Query
        {
            get { return this.cQuery; }
            set { this.cQuery = value; }
        }

        public string Param1Name
        {
            get { return this.cParam1Name; }
            set { this.cParam1Name = value; }
        }

        public string Param2Name
        {
            get { return this.cParam2Name; }
            set { this.cParam2Name = value; }
        }

        public string Param3Name
        {
            get { return this.cParam3Name; }
            set { this.cParam3Name = value; }
        }

        public string EventDataParamName
        {
            get { return this.cEventDataParamName; }
            set { this.cEventDataParamName = value; }
        }

        public string DistIdParamName
        {
            get { return this.cDistIdParamName; }
            set { this.cDistIdParamName = value; }
        }

        public bool Active
        {
            get { return this.cActive; }
            set { this.cActive = value; }
        }

    }

    public class Ux2EmailTemplate
    {
        private int cId = 0;
        private string cDescription = string.Empty;
        private int cGroupId = 0;
        private string cGroupDescription = string.Empty;

        public Ux2EmailTemplate()
        {
        }

        public Ux2EmailTemplate(int id, string description, int groupId, string groupDescription)
        {
            this.cId = id;
            this.cDescription = description;
            this.cGroupId = groupId;
            this.cGroupDescription = groupDescription;
        }

        public int Id
        {
            get { return this.cId; }
            set { this.cId = value; }
        }

        public string Description
        {
            get { return this.cDescription; }
            set { this.cDescription = value; }
        }

        public int GroupId
        {
            get { return this.cGroupId; }
            set { this.cGroupId = value; }
        }

        public string GroupDescription
        {
            get { return this.cGroupDescription; }
            set { this.cGroupDescription = value; }
        }


    }

}
