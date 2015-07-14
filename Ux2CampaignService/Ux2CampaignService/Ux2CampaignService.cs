using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using ServiceDebuggerHelper;
using System.Xml;
using System.Windows.Forms;
using System.IO;
using System.Configuration;

namespace GanoExcel.Ux2
{
    public partial class Ux2CampaignService : ServiceBase, IDebuggableService
    {
        private const string EXC_URL = @"http://api.ganocorp.com/webservices/LogUx2ServiceErr.asp";
        //private const string DEFAULT_CONNECTION_STRING = @"Server=db.ganocorp.com,14920;Database=GANO_INTL1;UID=merydion;PWD=$@llyj@n3";
        private const string DEFAULT_CONNECTION_STRING = @"Server=db.ganocorp.com,14920;Database=GANO_INTL1;UID=merydion;PWD=0v3ru$3dPW";
        private const int DEFAULT_INTERVAL = 10000;
        private const string SETTINGS_FILE = "Ux2CampaignService.xml";
        private const int DEFAULT_MAX_AGE = 168; //In Hours (7 days)

        private const string EVENT_LOG_SOURCE = "Ux2 Campaign Service";
        private const string EVENT_LOG = "Application";

        private System.Timers.Timer cScheduleTimer = null;
        private string cErrorFile = "error.txt";
        private double cInterval = DEFAULT_INTERVAL; //60 seconds
        private string cConnectionString = DEFAULT_CONNECTION_STRING;
        private int cMaxAge = DEFAULT_MAX_AGE;
        //private GanoExcel.GanoExceptionHandling cExcHandler = null;

        private ScheduleProcessor cProcessor = null;

        private delegate void CallScheduleProcessor();
        private CallScheduleProcessor cProcessorCaller = null;

        public Ux2CampaignService()
        {
            InitializeComponent();
            //this.cExcHandler = new GanoExceptionHandling(EXC_URL, cErrorFile);

            LoadSettings();
            
            if (!EventLog.SourceExists(EVENT_LOG_SOURCE))
                EventLog.CreateEventSource(EVENT_LOG_SOURCE, EVENT_LOG);

            try
            {
                this.cScheduleTimer = new System.Timers.Timer(this.cInterval);
                this.cScheduleTimer.Elapsed += new ElapsedEventHandler(cScheduleTimer_Elapsed);
                this.cScheduleTimer.Enabled = true;

                EventLog.WriteEntry(EVENT_LOG_SOURCE, "Ux2 Campaign Service Started on " + DateTime.Now.ToString() + "\n" + 
                    "Settings loaded from " + Path.GetDirectoryName(Application.ExecutablePath) + "\\" + SETTINGS_FILE + "\n" + 
                    "Processing Interval: " + this.cInterval + "ms\n" + 
                    "Max Campaign Age: " + this.cMaxAge + "hours");
              


                this.cProcessor = new ScheduleProcessor(this.cConnectionString, cMaxAge);
                this.cProcessorCaller = new CallScheduleProcessor(this.cProcessor.ProcessSchedules);
            }
            catch (Exception exc)
            {
                //this.cExcHandler.LogExceptionQuiet(null, exc);
                EventLog.WriteEntry(EVENT_LOG_SOURCE, exc.Message);
            }
            
        }

        void cScheduleTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                this.cProcessorCaller.BeginInvoke(null, null);
            }
            catch (Exception exc)
            {
                //this.cExcHandler.LogExceptionQuiet(null, exc);
                EventLog.WriteEntry(EVENT_LOG_SOURCE, exc.Message);

            }
        }

        private void LoadSettings()
        {
            XmlDocument doc = new XmlDocument();
            string settingsFile = Path.GetDirectoryName(Application.ExecutablePath) + "\\" + SETTINGS_FILE;

            if (!System.IO.File.Exists(settingsFile))
                BuildDefaultSettingsFile();

            try
            {
                doc.Load(settingsFile);
            }
            catch
            {
                System.IO.File.Delete(settingsFile);
                BuildDefaultSettingsFile();
                doc.Load(SETTINGS_FILE);
            }

            //this.cConnectionString = doc.SelectSingleNode("Settings/ConnectionString").Attributes["value"].Value;
            //this.cInterval = Convert.ToDouble(doc.SelectSingleNode("Settings/Interval").Attributes["value"].Value); //Value expected in milliseconds
            //this.cMaxAge = Convert.ToInt32(doc.SelectSingleNode("Settings/MaxAge").Attributes["value"].Value); //Value expected in hours
    
            this.cConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
            this.cInterval = Convert.ToDouble(ConfigurationManager.AppSettings["TimerInterval"]); //Value expected in milliseconds
            this.cMaxAge = Convert.ToInt32(ConfigurationManager.AppSettings["MaxAge"]); //Value expected in hours
        }

        private void BuildDefaultSettingsFile()
        {
            XmlWriter writer = null;
            string settingsFile = Path.GetDirectoryName(Application.ExecutablePath) + "\\" + SETTINGS_FILE;

            try
            {
                writer = XmlWriter.Create(settingsFile);

                writer.WriteStartDocument();
                writer.WriteStartElement("Settings");

                writer.WriteStartElement("ConnectionString");
                writer.WriteAttributeString("value", DEFAULT_CONNECTION_STRING);
                writer.WriteEndElement();

                writer.WriteStartElement("Interval");
                writer.WriteAttributeString("value", DEFAULT_INTERVAL.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("MaxAge");
                writer.WriteAttributeString("value", DEFAULT_MAX_AGE.ToString());
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        protected override void OnStart(string[] args)
        {
            this.cScheduleTimer.Start();

            try
            {
                this.cProcessorCaller.BeginInvoke(null, null);
            }
            catch (Exception exc)
            {
                //this.cExcHandler.LogExceptionQuiet(null, exc);
                throw exc;
            }

        }

        protected override void OnStop()
        {
            this.cScheduleTimer.Stop();
            this.cProcessor.Cancel();
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnContinue()
        {
            base.OnContinue();
        }
        #region IDebuggableService Members

        public void Start(string[] args)
        {
            OnStart(args);
        }

        public void StopService()
        {
            OnStop();
        }

        public void Pause()
        {
            OnPause();
        }

        public void Continue()
        {
            OnContinue();
        }

        #endregion
    }
}
