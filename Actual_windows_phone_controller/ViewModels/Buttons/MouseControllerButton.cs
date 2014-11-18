using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Actual_windows_phone_controller.ViewModels.Buttons
{
    class MouseControllerButton : AbstractControllerButton
    {
        public double mouseX = 0;
        public double mouseY = 0;
        public Network network;

        public MouseControllerButton()
        {
            width = 300;
            height = 300;
            Initalize();
        }
        public MouseControllerButton(StreamReader reader) : base(reader)
        {
            Initalize();
        }
        private void Initalize()
        {
            network = Network.GetInstance();
        }
        override protected void initalizeFromStream(StreamReader reader)
        {
            x = Convert.ToDouble(reader.ReadLine());
            y = Convert.ToDouble(reader.ReadLine());
            width = Convert.ToDouble(reader.ReadLine());
            height = Convert.ToDouble(reader.ReadLine());
        }
        override public void Save(StreamWriter writer)
        {
            writer.WriteLine(ButtonType.Mouse);
            writer.WriteLine(x);
            writer.WriteLine(y);
            writer.WriteLine(width);
            writer.WriteLine(height);
        }
        override public FrameworkElement getVisualElement()
        {
            Rectangle uibutton = new Rectangle();
            uibutton.Width = width;
            uibutton.Height = height;
            uibutton.Fill = new SolidColorBrush(Colors.White);
            Canvas.SetLeft(uibutton, x);
            Canvas.SetTop(uibutton, y);
            
            uibutton.DataContext = this;
            uibutton.Tap += uibutton_Tap;
            uibutton.DoubleTap += uibutton_DoubleTap;
            uibutton.MouseEnter += uibutton_MouseEnter;
            uibutton.MouseMove += uibutton_MouseMove;
            uibutton.Hold += uibutton_Hold;
            return uibutton;
        }

        void uibutton_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string stringToSend = Network.clickTag + "hold";
            network.SendString(stringToSend);
        }

        void uibutton_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            string x;
            string y;

            x = RoundToSig(e.GetPosition(null).X - mouseX);
            y = RoundToSig(e.GetPosition(null).Y - mouseY);

            mouseX = e.GetPosition(null).X;
            mouseY = e.GetPosition(null).Y;

            //NotifyUser.Text = "x = " + x + " y = " + y;
            string stringToSend = Network.mouseTag + x + y;
            network.SendString(stringToSend);
        }
        private string RoundToSig(double x)
        {
            // this function is used to make every x+y an exact block size
            // so that I can more quickly send mouse cordinates

            string ret = Math.Round(Math.Abs(x), 0).ToString();
            bool neg = false;
            if (x < 0)
            {
                neg = true;
            }
            string foreZero;
            if (neg)
            {
                foreZero = "-000";
            }
            else
            {
                foreZero = "0000";
            }

            for (int i = 0; i < ret.Length; i++)
            {
                foreZero = foreZero.Remove(foreZero.Length - 1);
            }

            ret = foreZero + ret;


            return ret;
        }
        void uibutton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            mouseX = e.GetPosition(null).X;
            mouseY = e.GetPosition(null).Y;
        }

        void uibutton_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string stringToSend = Network.clickTag + "right";
            network.SendString(stringToSend);
        }

        void uibutton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string stringToSend = Network.clickTag + "left";
            network.SendString(stringToSend);
        }
        override public void updateData(FrameworkElement control)
        {
            Rectangle uibutton = (Rectangle)control;
            x = Canvas.GetLeft(uibutton);
            y = Canvas.GetTop(uibutton);
            width = control.Width;
            height = control.Height;
        }
    }
}
