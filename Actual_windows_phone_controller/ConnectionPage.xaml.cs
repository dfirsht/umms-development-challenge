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
using Actual_windows_phone_controller.ViewModels;

namespace Actual_windows_phone_controller
{
    public partial class Page1 : PhoneApplicationPage
    {
        // this may be needed depending on implementation
        //NetworkAdapter adapter = null;

        Network network;
        

        public Page1()
        {
            network = Network.GetInstance();
            InitializeComponent();
        }


        private void OnKeyDownHandler(object sender, TextChangedEventArgs e)
        {
            if (SendKeyBox.Text == "")
            {
                return;
            }

            char index = SendKeyBox.Text[SendKeyBox.Text.Length - 1];


            string stringToSend = SendKeyBox.Text;

            if (network.convertKey.ContainsKey(stringToSend))
            {
                stringToSend = network.convertKey[stringToSend];
            }
            SendKeyBox.Text = "";
            NotifyUser.Text = "You Entered: " + stringToSend;
            stringToSend = Network.keyTag + stringToSend;
            network.SendString(stringToSend);
        }


        private void EnterDel(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter && e.Key != Key.Back)
            {
                return;
            }
            string stringToSend;
            if (e.Key == Key.Enter)
            {
                stringToSend = Network.keyTag + "~";
            }
            else
            {
                stringToSend = Network.keyTag + "{BS}";
            }

            //NotifyUser.Text = "You Entered: " + stringToSend;
            network.SendString(stringToSend);
        }
        private void ConnectionClick(object sender, RoutedEventArgs e)
        {
            network.CreateConnection(IpText.Text);
        } 

    }
}