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
using System.Security.Principal;
using System.Diagnostics;
using System.Windows.Forms;

namespace CMD
{
    public class Program
    {
    private static StreamWriter cmd_writer;
    private static StreamReader cmd_read;

    public static readonly object cmdLock = new object();
    public static readonly object keyLock = new object();

    private const string cmdString = "";
    private const string keyString = "";
    private const string mouseString = "";
    private const string clickString = "";
    private const String controllerString = "";

    // these variables corrispond with how many functions we can call
    // each string is filled by the network loop
    public static Action<int>[] functions = { SendKeyStrokes, ConsoleCall, SendMousePos, SendClickEvent, SendControlEvent};
    public static string[] callStrings = { keyString, cmdString, mouseString, clickString, controllerString }; 

    private static string format = "";

    static void Main(string[] args)
    {
        if(!IsAdministrator())
        {
            Console.WriteLine("App must be run as Administrator");
            Console.WriteLine("Press any character to exit");
            Console.Read();
            Environment.Exit(0);
        }
             
             
        Process cmdProcess = CreateCmdWindow(true, ref cmd_read, ref cmd_writer, "");

        // user and pass
        string networkName = "";
        string password = "";

        string prompt = "the name of your network";
        string promptValue = "Name";
        UserPass(ref networkName,UserLen, prompt, promptValue);

        prompt = "a password over 8 characters long";
        promptValue = "Password";
        UserPass(ref password, PassLen, prompt, promptValue);

        // make sure the lan netowrk is killed when the app exits
        AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

        // creat heart beat
        HeartBeat h = new HeartBeat();
        Thread heartThread = new Thread(new ThreadStart(h.beat));
        heartThread.Start();
        // spin
        while (!heartThread.IsAlive) { };

        // creating lan network
        callStrings[1] = "netsh wlan set hostednetwork mode=disallow";
        ConsoleCall(1);
        format = String.Format("netsh wlan set hostednetwork mode=allow ssid={0} key={1}", networkName, password);
        callStrings[1] = format;
        ConsoleCall(1);
        callStrings[1] = "netsh wlan start hostednetwork";
        ConsoleCall(1);
        callStrings[1] = "";

        // wait until the lan network is up before binding to it
        string x = "";
        while (x != "The hosted network started. ")
        {
            x = cmd_read.ReadLine();
        }
       // cmdProcess.Close();
        cmdProcess = CreateCmdWindow(false, ref cmd_read, ref cmd_writer, "");
        
        // creating a seprate thread to run the sockets
        AsynchronousSocketListener network = new AsynchronousSocketListener();

        Thread networkThread = new Thread(new ThreadStart(network.StartListening));

        // Start the thread may need to spin for a bit: while (!networkThread.IsAlive) ;
        networkThread.Start();

        ReadInput();
    }

        static void OnProcessExit(object sender, EventArgs e)
        {
            callStrings[1] = format;
            ConsoleCall(1);
        }

        public static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private static bool PassLen(int pass) { return (pass < 8 || pass > 20); }
        private static bool UserLen(int name) { return (name <= 0 || name > 20); }

        private static void UserPass(ref string input, Func<int,bool> function, string prompt, string promptValue)
        {
            while (function(input.Length))
            {
                Console.WriteLine("Please enter " + prompt);
                Console.Write(promptValue + " :");
                input = Console.ReadLine();
            }
        }

        public static Process CreateCmdWindow(bool redirectOutput, ref StreamReader read, 
                                            ref StreamWriter write, string arguments)
        {
            //creating a new process
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

            // makes command line window hidden
            startInfo.CreateNoWindow = false;
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

            //calling process name
            startInfo.FileName = "cmd.exe";

            // file path
            if(arguments.Length > 0)
            {
                startInfo.Arguments = arguments;
                startInfo.WorkingDirectory = Application.StartupPath;
            }

            //establish that I want to use the console as an output stream (receives input from my output)s
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = redirectOutput;
            startInfo.UseShellExecute = false;

            process.StartInfo = startInfo;
            process.Start();

            write = process.StandardInput;
            if(redirectOutput)
            {
                read = process.StandardOutput;
            }
            return process;
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

        //control mouse
        private static void SendMousePos(int i)
        {
            int x = int.Parse(callStrings[i].Substring(0, 4),NumberStyles.AllowLeadingSign);
            int y = int.Parse(callStrings[i].Substring(4, 4), NumberStyles.AllowLeadingSign);
                    
            //if (y == 0) { y = 1; }

            MouseControl.moveMouse(x, y);
        }

        // controll mouse clicks
        private static void SendClickEvent(int i)
        {
            if (callStrings[i] == "left")
            {
                MouseControl.clickEvent(false, false);
            }
            else if (callStrings[i] == "right")
            {
                MouseControl.clickEvent(false, true);
            }
            else if (callStrings[i] == "hold")
            {
                MouseControl.clickEvent(true, false);
            }
        }

        //general event for simple PC / Application / Web control
        private static void SendControlEvent(int i)
        {
            Controller.act(callStrings[i]);
        }
      
    };
}