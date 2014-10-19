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

namespace Actual_windows_phone_controller
{
    public partial class Page1 : PhoneApplicationPage
    {
        // this may be needed depending on implementation
        //NetworkAdapter adapter = null;
        StreamSocket socket;
        DataWriter writer;
        bool shift = false;
        Dictionary<String, String> convertKey = new Dictionary<String,String>();
        
        

        public Page1()
        {
            InitDict();
            InitializeComponent();
        }


        private async void ConnectionClick(object sender, RoutedEventArgs e)
        {
            // host name references IP address
            HostName hostname;
            string ipString = IpText.Text;
            string portNum = "7777";

            if (String.IsNullOrEmpty(ipString))
            {
                NotifyUser.Text = "please enter in an IP address";
                return;
            }

            // it is posible for the user code to provide me with an invalid hostname, so I must catch
            // TODO: I want to scan for ipadresses so I don't have to trust users anymore
            try
            {
                hostname = new HostName(IpText.Text);
            }
            catch (ArgumentException)
            {
                NotifyUser.Text = "Invalid IP address";
                return;
            }
            socket = new StreamSocket();

            // I need to save the socket so that I can send and recieve
            // messages on the socket. Save the socket, so subsequent steps can use it.
            // CoreApplication.Properties.Add("clientSocket", socket);

            try
            {
                NotifyUser.Text = "Connecting to port: " + "7777" + " with ip: " + ipString + '.';
                await socket.ConnectAsync(hostname, portNum);
                NotifyUser.Text = "Connected";
                writer = new DataWriter(socket.OutputStream);
                HelloButton.Visibility = Visibility.Visible;
                SendKeyBox.Visibility = Visibility.Visible;

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
                NotifyUser.Text = "Connect failed with error: " + exception.Message;
            }
        }

        private void SendMessage(object sender, RoutedEventArgs e)
        {

            //CoreApplication.Properties.Add("clientDataWriter", writer);

            // Write first the length of the string as UINT32 value followed up by the string. 
            // Writing data to the writer will just store data in memory.
            string stringToSend = "nircmd.exe mutesysvolume 1";

            SendString(stringToSend);
        }


        private bool isCaps()
        {
            // all this to get the state of caps lock.....
            //Windows.UI.Core.CoreVirtualKeyStates keyState = Windows.UI.Core.CoreWindow.GetForCurrentThread().GetKeyState((Windows.System.VirtualKey.CapitalLock));
            
            Windows.System.VirtualKey x = Windows.System.VirtualKey.CapitalLock;
            Windows.UI.Core.CoreVirtualKeyStates keyState = Windows.UI.Core.CoreWindow.GetForCurrentThread().GetKeyState(x);
            if(keyState == Windows.UI.Core.CoreVirtualKeyStates.Locked)
            {
                return true;
            }
            return false;
        }

        private async void SendString(string stringToSend)
        {
            stringToSend += "<EOF>";
            writer.WriteUInt32(writer.MeasureString(stringToSend));
            writer.WriteString(stringToSend);

            // Write the locally buffered data to the network.
            try
            {
                await writer.StoreAsync();
                NotifyUser.Text = "\"" + stringToSend + "\" sent successfully.";
            }
            catch (Exception exception)
            {
                // If this is an unknown status it means that the error if fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }
                NotifyUser.Text = "Send failed with error: " + exception.Message;
            }
        }

        private void OnKeyDownHandler(object sender, TextChangedEventArgs e)
        {
            if (SendKeyBox.Text == "" || SendKeyBox.Text == "£" || SendKeyBox.Text == "€" ||
                SendKeyBox.Text == "¥" || SendKeyBox.Text == "•")
            {
                return;
            }

            string stringToSend = SendKeyBox.Text;
           
            if (convertKey.ContainsKey(stringToSend))
            {
                stringToSend = convertKey[stringToSend];
            }
            SendKeyBox.Text = "";
            NotifyUser.Text = "You Entered: " + stringToSend;
            SendString(stringToSend);
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

        private void EnterDel(object sender, KeyEventArgs e)
        {
            if(e.Key != Key.Enter && e.Key != Key.Back)
            {
                return;
            }
            string stringToSend;
            if(e.Key == Key.Enter)
            {
                stringToSend = "~";
            }
            else
            {
                stringToSend = "{BS}";
            }

            NotifyUser.Text = "You Entered: " + stringToSend;
            SendString(stringToSend);
        }

    }
}