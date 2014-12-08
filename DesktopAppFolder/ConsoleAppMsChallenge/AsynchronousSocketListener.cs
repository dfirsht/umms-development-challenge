using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

// State object for reading client data asynchronously
public class StateObject
{
    // Client  socket.
    public Socket workSocket = null;
    // Size of receive buffer.
    public const int BufferSize = 1024;
    // Receive buffer.
    public byte[] buffer = new byte[BufferSize];
    // Received data string.
    public StringBuilder sb = new StringBuilder();
}

public class AsynchronousSocketListener
{
    
    private const char keyTag = 'k';
    private const char consoleTag = 'c';
    private const char mouseTag = 'm';
    private const char clickTag = 't';
    private const char controllerTag = 'x';

    private Socket listener = null;


    public static char[] tags = { keyTag, consoleTag, mouseTag, clickTag , controllerTag};
    // Thread signal.
    public static ManualResetEvent allDone = new ManualResetEvent(false);

    public AsynchronousSocketListener()
    {
    }

    public void StartListening()
    {
        IPAddress ipAddress = null;
        int portNum = 7777;
        
        try
        {
            // Create a TCP/IP socket.
            
                ipAddress = IPAddress.Parse("192.168.173.1");

                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, portNum);


                listener = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                listener.ExclusiveAddressUse = false;
                // Bind to my address
                listener.Bind(localEndPoint);

                listener.Listen(100);
            while (true)
            {
                var worker = new AsynchronousSocketListener();

                Console.WriteLine("listening on: " + ipAddress + " on port: " + portNum);

                Console.WriteLine("Waiting for a connection...");

                // Start new connection on the current thread, this is used for new client

                worker.listener = listener.Accept();

                Thread receiveConnection = new Thread(worker.AcceptMessage);

                receiveConnection.Start();

                while (!receiveConnection.IsAlive) ;
            }
        }
        catch (Exception e)
        {
            if (listener != null)
            {
                listener.Close();
            }
            Console.WriteLine(e.ToString());
        }

        Console.WriteLine("\nPress ENTER to continue...");
        Console.Read();

    }

    public void AcceptMessage()
    {
        Console.WriteLine("connection attempt");

        // this is my loop to continue accepting messages from the mobile
        // socket which i connected to.
        StateObject state = new StateObject();
        state.workSocket = this.listener;
        const int buf_size = 200;

        // this function should loop forever, so that I can asynchrenously pole the socket
        try
        {
            listener.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                         new AsyncCallback(ReadCallback), state);
        }
        catch(Exception e)
        {
            this.listener.Disconnect(true);
        }
        while(listener.Connected)
        {
            //do nothing
        }
        listener.Dispose();
       // Thread.CurrentThread.a
        // thread dies 
    }

    public void ReadCallback(IAsyncResult ar)
    {
        String content = String.Empty;

        // Retrieve the state object and the handler socket
        // from the asynchronous state object.
        StateObject state = (StateObject)ar.AsyncState;

        // Read data from the client socket.
        int bytesRead;

        try
        {
            bytesRead = this.listener.EndReceive(ar);
        }
        catch (Exception e)
        {

            this.listener.Close();
            return;
        }

        if (bytesRead > 0)
        {
            // There  might be more data, so store the data received so far.
            state.sb.Append(Encoding.ASCII.GetString(
                state.buffer, 0, bytesRead));

            // Check for end-of-file tag. If it is not there, read 
            // more data.
            content = state.sb.ToString();

            // find first instance of eof
            int i = 0;
            bool eof = false;
            for (i = 0; i < content.Length; i++)
            {
                if (content[i] == '<' && content[i + 4] == '>')
                {
                     eof = true;
                     break;
                }
            }
            if (eof)
            {
                //Console.WriteLine("everything is done?");
                // All the data has been read from the 
                // client. Display it on the console.
                int cut = 4;
                string msg = content.Substring(cut, (i-cut));
                content = content.Substring(i);
               /* Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                    content.Length, msg);*/

                lock (CMD.Program.keyLock)
                {
                    // this sets the string so if last command is taking a
                    // bit this will halt, ensures that last command is finshed before new
                    // one is started (however there are draw backs to this method)

                    // reverse checkcall 
                    while (CMD.Program.CheckCallStrings(true))
                    {
                        // to reduce buisy waiting
                        Monitor.Wait(CMD.Program.keyLock);
                    }
                    string fill = msg.Substring(1, msg.Length-1);
                    bool invalidTag = true;

                    for (int j = 0; j < tags.Length; j++)
                    {
                        if(msg[0] == tags[j])
                        {
                            invalidTag = false;
                            CMD.Program.callStrings[j] = fill;
                            break;
                        }
                    }
                    if (invalidTag)
                    {
                        // this should never happen, it means that the there is no first tag
                        // Exception e = new Exception("invalid network tag");
                        // throw e;
                    }
                    Monitor.Pulse(CMD.Program.keyLock);
                }
            }
            else
            {
                listener.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReadCallback), state);
            }

            
        }
        // non blocking continues to call itself
        state = new StateObject();
        state.workSocket = listener;
        listener.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
    }

    private static void Send(Socket handler, String data)
    {
        // Convert the string data to byte data using ASCII encoding.
        byte[] byteData = Encoding.ASCII.GetBytes(data);

        // Begin sending the data to the remote device.
        handler.BeginSend(byteData, 0, byteData.Length, 0,
            new AsyncCallback(SendCallback), handler);
    }

    private static void SendCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.
            Socket handler = (Socket)ar.AsyncState;

            // Complete sending the data to the remote device.
            int bytesSent = handler.EndSend(ar);
            Console.WriteLine("Sent {0} bytes to client.", bytesSent);

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private static bool pollSocket(Socket s)
    {
        bool part1 = s.Poll(1000, SelectMode.SelectRead);
        bool part2 = (s.Available == 0);
        if (part1 && part2)
        {
            Console.WriteLine("meow I am a cat, please fear me, or you shall die!!!");
            return false;
        }
        else
        {
            return true;
        }
    }
}