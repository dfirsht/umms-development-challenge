using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MyNewService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new MyNewService() 
            };
            ServiceBase.Run(ServicesToRun);
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

            // makes command line window hidden
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;

            //calling process name
            startInfo.FileName = "cmd.exe";

            //establish that I want to use the console as an output stream (receives input from my output)s
            startInfo.RedirectStandardInput = true;
            startInfo.UseShellExecute = false;

            process.StartInfo = startInfo;
            process.Start();

            StreamWriter cmd_writer = process.StandardInput;

            // command to mute the volume
            string cmd_output = "nircmd.exe mutesysvolume 1";


            cmd_writer.WriteLine(cmd_output);
        }
    }
}
