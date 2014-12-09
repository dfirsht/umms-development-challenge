using CMD;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class HeartBeat
{
    private static string[] batchLines = new string[3];
    private static StreamReader reader;
    private static StreamWriter writer;

    private bool read = true;

    public void beat()
    {
        batchLines[1] = "ping -n 6 127.0.0.1 >nul";
        batchLines[2] = "netsh wlan set hostednetwork mode=disallow";

        

        string path = Directory.GetCurrentDirectory() +  "\\heartBeat.bat";
       
        string stringToWrite = "";
        for(int i = 0 ; i < batchLines.Length; i++)
        {
            stringToWrite += batchLines[i];
            stringToWrite += '\n';
        }
        stringToWrite.Remove(stringToWrite.Length - 1);

        using (FileStream fs = File.Create(path))
        {
            Byte[] info = new UTF8Encoding(true).GetBytes(stringToWrite);
            // Add some information to the file.
            fs.Write(info, 0, info.Length);
        }

        Process p = Program.CreateCmdWindow(true, ref reader, ref writer, "");
        writer.WriteLine("cd " + Directory.GetCurrentDirectory());
        writer.WriteLine("heartBeat.bat");
        while(true)
        {
            Thread.Sleep(TimeSpan.FromSeconds(2));
            Process old = p;
            p = Program.CreateCmdWindow(true, ref reader, ref writer, "");
            writer.WriteLine("cd " + Directory.GetCurrentDirectory());
            writer.WriteLine("heartBeat.bat");
            old.Kill();
        }
    }
}
