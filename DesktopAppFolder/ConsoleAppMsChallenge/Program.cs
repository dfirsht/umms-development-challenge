using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Globalization;

namespace CMD
{
    public class Program
    {
        public static StreamWriter cmd_writer;
        public static readonly object cmdLock = new object();
        public static readonly object keyLock = new object();

        private  const string cmdString = "";
        private  const string keyString = "";
        private const string mouseString = "";

        // these variables corrispond with how many functions we can call
        // each string is filled by the network loop
        public static Action<int>[] functions = { SendKeyStrokes, ConsoleCall, SendMousePos};
        public static string[] callStrings = { keyString, cmdString, mouseString }; 


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
             
             Console.WriteLine(MouseControl.getMouseX());

             ReadInput();
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

        private static void ReadInput()
        {
            while (true)
            {

                lock (keyLock)
                {
                    while (CheckCallStrings(false))
                    {
                        // to reduce buisy waiting
                        Monitor.Wait(keyLock);
                    }
                    for (int i = 0; i < callStrings.Length; i++)
                    {
                        if (callStrings[i] != "")
                        {
                            functions[i](i);
                            callStrings[i] = "";
                        }
                    }
                    Monitor.PulseAll(keyLock);
                }
            }
        }

        public static bool CheckCallStrings(bool ret)
        {
            for(int i =0; i < callStrings.Length ;i++)
            {
                if (callStrings[i] != "")
                {
                    return ret;
                }
            }
            return !ret;
        }


        // ******these are the functions we call to do stuff****
        
        // keystroke call
        private static void SendKeyStrokes(int i)
        {
            KeyControl.sendWait(callStrings[i]);
        }

        // windows terminal call
        public static void ConsoleCall(int i)
        {
            cmd_writer.WriteLine(callStrings[i]); 
        }

        private static void SendMousePos(int i)
        {
            int x = int.Parse(callStrings[i].Substring(0, 4),NumberStyles.AllowLeadingSign);
            int y = int.Parse(callStrings[i].Substring(4, 4), NumberStyles.AllowLeadingSign);
                    
            //if (y == 0) { y = 1; }

            MouseControl.moveMouse(x, y);
        }
    };
}