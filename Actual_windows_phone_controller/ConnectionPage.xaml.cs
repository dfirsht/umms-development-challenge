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

        public bool mouseOn = false;
        public double lastMouseX = 0;
        public double lastMouseY = 0;
        public double mouseX = 0;
        public double mouseY = 0;
        Network network;
        

        public Page1()
        {
            network = Network.GetInstance();
            InitializeComponent();
        }


        private void OnKeyDownHandler(object sender, TextChangedEventArgs e)
        {
            if(SendKeyBox.Text == "")
            {
                return;
            }

            char index = SendKeyBox.Text[SendKeyBox.Text.Length-1];


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
            if(e.Key != Key.Enter && e.Key != Key.Back)
            {
                return;
            }
            string stringToSend;
            if(e.Key == Key.Enter)
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mouseOn = !mouseOn;
        }

        private void SendMouseMove(object sender, MouseEventArgs e)
        {
            if(mouseOn)
            {
                string x;
                string y;

                x = RoundToSig(e.GetPosition(null).X - mouseX);
                y = RoundToSig(e.GetPosition(null).Y - mouseY);

                mouseX = e.GetPosition(null).X;
                mouseY = e.GetPosition(null).Y;
               
                NotifyUser.Text = "x = " + x + " y = " + y;
                string stringToSend = Network.mouseTag + x + y;
                network.SendString(stringToSend);
            }
        }

        private string RoundToSig(double x)
        {
            // this function is used to make every x+y an exact block size
            // so that I can more quickly send mouse cordinates
            
            string ret = Math.Round(Math.Abs(x),0).ToString();
            bool neg = false;
            if (x < 0)
            {
                neg = true;
            }
            string foreZero;
            if(neg)
            {
                foreZero = "-000";
            }
            else
            {
                foreZero = "0000";
            }
           
            for (int i = 0; i < ret.Length;  i++)
            {
               foreZero = foreZero.Remove(foreZero.Length - 1);
            }

            ret = foreZero + ret;


            return ret;
        }

        private void LayoutRoot_MouseEnter(object sender, MouseEventArgs e)
        {

            if (mouseOn)
            {
                mouseX = e.GetPosition(null).X;
                mouseY = e.GetPosition(null).Y;
            }
        }

        private void ConnectionClick(object sender, RoutedEventArgs e)
        {
            network.CreateConnection(IpText.Text);
        }


        private void SendRightClick(object sender, GestureEventArgs e)
        {
            if (mouseOn)
            {
                string stringToSend = Network.clickTag + "right";
                network.SendString(stringToSend);
            }
        }

        private void SendLeftClick(object sender, GestureEventArgs e)
        {
            if (mouseOn)
            {
                string stringToSend = Network.clickTag + "left";
                network.SendString(stringToSend);
            }
        }

        private void SendHold(object sender, GestureEventArgs e)
        {
            if (mouseOn)
            {
                string stringToSend = Network.clickTag + "hold";
                network.SendString(stringToSend);
            }
        }

    }
}