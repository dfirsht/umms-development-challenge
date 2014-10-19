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
    // Thread signal.
    public static ManualResetEvent allDone = new ManualResetEvent(false);

    public AsynchronousSocketListener()
    {
    }

    public void StartListening()
    {
        // Data buffer for incoming data.
        byte[] bytes = new Byte[1024];

        // Establish the local endpoint for the socket.
        // The DNS name of the computer
        // running the listener is "host.contoso.com".
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());

        // get all available network devices
        NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

        IPAddress ipAddress = null;
        // iterate through each adapter with "wireless"
        // I may need to change this code if some one has ipv6, or 
        foreach (NetworkInterface adapter in interfaces)
        {
            var ipProps = adapter.GetIPProperties();

            foreach (var ip in ipProps.UnicastAddresses)
            {
                // looking for ipv4 internal network
                if ((adapter.OperationalStatus == OperationalStatus.Up)
                    && (ip.Address.AddressFamily == AddressFamily.InterNetwork))
                {
                    string addType = adapter.NetworkInterfaceType.ToString();
                    // attempting to find the network address of wireless adapter
                    addType = addType.ToLower();
                    if(addType.Contains("wireless") || addType.Contains("wifi"))
                    {
                        ipAddress = ip.Address;
                        break;
                    }
                }
            }
        }

        //Console.WriteLine(ipAddress);
        if(ipAddress == null)
        {
            Console.WriteLine("socket error");
            throw new SocketException();
        }

        int portNum = 7777;
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, portNum);

        //NetworkStream(Socket)
        // Create a TCP/IP socket.
        Socket listener = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);

        //NetworkStream nStream;

       // nStream = new NetworkStream(listener, true);

        // Bind the socket to the local endpoint and listen for incoming connections.
        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(1);
            Console.WriteLine("listening on: " + ipAddress + " on port: " + portNum);

            while (true)
            {
                // Set the event to nonsignaled state.
                allDone.Reset();

                // Start an asynchronous socket to listen for connections.
                Console.WriteLine("Waiting for a connection...");
                listener.BeginAccept(
                    new AsyncCallback(AcceptCallback),
                    listener);

                // Wait until a connection is made before continuing.
                allDone.WaitOne();
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        Console.WriteLine("\nPress ENTER to continue...");
        Console.Read();

    }

    public static void AcceptCallback(IAsyncResult ar)
    {
        Console.WriteLine("connection attempt");
        // Signal the main thread to continue.
        allDone.Set();

        // Get the socket that handles the client request.
        Socket listener = (Socket)ar.AsyncState;
        Socket handler = listener.EndAccept(ar);

        // this is my loop to continue accepting messages from the mobile
        // socket which i connected to.
        while (pollSocket(listener))
        {
            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;

            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }
    }

    public static void ReadCallback(IAsyncResult ar)
    {
        String content = String.Empty;

        // Retrieve the state object and the handler socket
        // from the asynchronous state object.
        StateObject state = (StateObject)ar.AsyncState;
        Socket handler = state.workSocket;

        // Read data from the client socket. 
        int bytesRead = handler.EndReceive(ar);

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
                Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                    content.Length, msg);

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

                    if(msg[0] == 'k')
                    {
                        CMD.Program.callStrings[0] = fill;
                    }
                    else if(msg[0] == 'c')
                    {
                        CMD.Program.callStrings[1] = fill;
                    }
                    else
                    {
                        // this should never happen, it means that the there is no first tag
                        Exception e = new Exception("invalid network tag");
                        throw e;
                    }
                    Monitor.Pulse(CMD.Program.keyLock);
                }
            }
            else
            {
                // Not all data received. Get more.
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
            }
        }
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