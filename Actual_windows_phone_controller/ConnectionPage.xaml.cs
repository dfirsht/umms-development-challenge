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
using System.Windows.Media;

namespace Actual_windows_phone_controller
{
    public partial class Page1 : PhoneApplicationPage
    {
        // this may be needed depending on implementation
        //NetworkAdapter adapter = null;

        Network network;
        Page1 page;
        System.Threading.Thread t;

        public Page1()
        {
            network = Network.GetInstance();
            
            InitializeComponent();

        }


        private void ConnectionClick(object sender, RoutedEventArgs e)
        {
            if (!network.connected)
            {
                network.CreateConnection("192.168.173.1");
                Thread.Sleep(700);
            }
            
            isConnected();
        }

       
        public void isConnected()
        {
            if(network.connected)
            {
                connectBox.Text = "Connected";
                connectBox.Background = new SolidColorBrush(Colors.Green);
            }
            else
            {
                connectBox.Text = "Disconnected";
                connectBox.Background = new SolidColorBrush(Colors.Red);
            }
        }

        private void doNothing(object sender, ManipulationStartedEventArgs e)
        {
            // do nothi
        }

        private void looseFocus(object sender, RoutedEventArgs e)
        {
            connectButton.Focus();
        }


    }
}