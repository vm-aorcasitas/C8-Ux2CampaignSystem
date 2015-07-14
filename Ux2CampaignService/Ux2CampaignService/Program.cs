using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;
using ServiceDebuggerHelper;

namespace GanoExcel.Ux2
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0].ToLower().Equals("/debug"))
            {
                Application.Run(new ServiceRunner(new Ux2CampaignService()));
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
			    { 
				    new Ux2CampaignService() 
			    };
                ServiceBase.Run(ServicesToRun);
            }



        }
    }
}
