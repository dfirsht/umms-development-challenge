﻿using System;
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

        private void ConnectionClick(object sender, RoutedEventArgs e)
        {
            network.CreateConnection(IpText.Text);
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            string name = "nircmd.exe setsysvolume";
            Slide.Maximum = 100;
            Slide.Minimum = 0;
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

    }
}