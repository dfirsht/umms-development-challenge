using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using System.Threading;
using Windows.UI;

namespace Actual_windows_phone_controller.ViewModels
{
    public class Network
    {
        public bool connected = false;
        public Dictionary<String, String> convertKey;
        public const char keyTag = 'k';
        public const char cmdTag = 'c';
        public const char mouseTag = 'm';
        public const char clickTag = 't';
        public const char percentTag = 'p';
        public const char systemTag = 's';
        public const char controllerTag = 'x';

        public const int maxVolume = 65535;

        private StreamSocket socket;
        private DataWriter writer;
        static Network singleton = null;


        private Network()
        {
            convertKey = new Dictionary<String, String>();
            InitDict();
        }
        

        static public Network GetInstance()
        {
            if(singleton == null)
            {
                singleton = new Network();
            }
            return singleton;
        }

        public void CreateConnection(string ipText)
        {
            Thread networkThread = new Thread (Network.GetInstance().StartConnection);
            networkThread.Start(ipText);
            // Start the thread may need to spin for a bit: 
            while (!networkThread.IsAlive);

        }

        private async void StartConnection(object ipObject)
        {
            // host name references IP address
            HostName hostname;
            string ipString = (string) ipObject;
            string portNum = "7777";

            if (String.IsNullOrEmpty(ipString))
            {
                //NotifyUser.Text = "please enter in an IP address";
                return;
            }

            // it is posible for the user code to provide me with an invalid hostname, so I must catch
            // TODO: I want to scan for ipadresses so I don't have to trust users anymore
            try
            {
                hostname = new HostName(ipString);
            }
            catch (ArgumentException)
            {
               // NotifyUser.Text = "Invalid IP address";
                return;
            }
            socket = new StreamSocket();
            

            // I need to save the socket so that I can send and recieve
            // messages on the socket. Save the socket, so subsequent steps can use it.
            // CoreApplication.Properties.Add("clientSocket", socket);

            try
            {
                //NotifyUser.Text = "Connecting to port: " + "7777" + " with ip: " + ipString + '.';
                await socket.ConnectAsync(hostname, portNum);
                //NotifyUser.Text = "Connected";
                writer = new DataWriter(socket.OutputStream);
                connected = true;
               // HelloButton.Visibility = Visibility.Visible;
               // SendKeyBox.Visibility = Visibility.Visible;

                // Mark the socket as connected. Set the value to null, as we care only about the fact that the 
                // property is set.
                // CoreApplication.Properties.Add("connected", null);
            }
            catch (Exception exception)
            {
                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }
                //NotifyUser.Text = "Connect failed with error: " + exception.Message;
            }
        }

        public async void SendString(string stringToSend)
        {
            if (!connected)
            {
                return;
            }
            stringToSend += "<EOF>";
            writer.WriteUInt32(writer.MeasureString(stringToSend));
            writer.WriteString(stringToSend);

            // Write the locally buffered data to the network.
            try
            {
                await writer.StoreAsync();
                // NotifyUser.Text = "\"" + stringToSend + "\" sent successfully.";
            }
            catch (Exception exception)
            {
                // If this is an unknown status it means that the error if fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }
                connected = false;
               // NotifyUser.Text = "Send failed with error: " + exception.Message;
            }
        }

        private void InitDict()
        {
            convertKey.Add("?", "{?}");
            convertKey.Add("^", "{^}");
            convertKey.Add("+", "{+}");
            convertKey.Add("%", "{%}");
            convertKey.Add("~", "{~}");
            convertKey.Add("{", "{{}");
            convertKey.Add("}", "{}}");
            convertKey.Add("(", "{(}");
            convertKey.Add(")", "{)}");
            convertKey.Add("[", "{[}");
            convertKey.Add("]", "{]}");
        }
    }
}
