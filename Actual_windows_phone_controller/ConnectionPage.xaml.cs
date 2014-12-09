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
                network.CreateConnection(IpText.Text);
            }
            Thread.Sleep(500);
            isConnected();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            string name = "nircmd.exe setsysvolume";
            double percent = (Math.Floor(Slide.Value));
            // fill percent with zeroes to give unified format
            percent = percent / Slide.Maximum;

            if (name == "nircmd.exe setsysvolume")
            {
                percent = (Math.Floor(Network.maxVolume * percent));
            }

            NotifyUser.Text = percent.ToString();
            string stringToSend = Network.cmdTag + name + ' ' + percent.ToString();
            network.SendString(stringToSend);
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


    }
}