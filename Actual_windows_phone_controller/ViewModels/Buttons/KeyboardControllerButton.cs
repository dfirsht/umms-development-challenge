using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Actual_windows_phone_controller.ViewModels.Buttons
{
    class KeyboardControllerButton : AbstractControllerButton
        {
        public Network network;
        public TextBox SendKeyBox;

        public KeyboardControllerButton()
        {
            width = 300;
            height = 80;
            Initalize();
        }
        public KeyboardControllerButton(StreamReader reader) : base(reader)
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
            writer.WriteLine(ButtonType.Keyboard);
            writer.WriteLine(x);
            writer.WriteLine(y);
            writer.WriteLine(width);
            writer.WriteLine(height);
        }
        override public FrameworkElement getVisualElement()
        {
            TextBox uibutton = new TextBox();
            SendKeyBox = uibutton;
            uibutton.Width = width;
            uibutton.Height = height;
           // uibutton.Fill = new SolidColorBrush(Colors.White);
            Canvas.SetLeft(uibutton, x);
            Canvas.SetTop(uibutton, y);
            
            uibutton.DataContext = this;
            uibutton.TextChanged += uibutton_SendKey;
            uibutton.KeyDown += uibutton_KeyDown;
            return uibutton;
        }


        void uibutton_SendKey(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            
        }

        private void uibutton_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            string stringToSend;
            if (e.Key != Key.Enter && e.Key != Key.Back)
            {
                if (SendKeyBox.Text == "")
                {
                    return;
                }

                stringToSend = SendKeyBox.Text[SendKeyBox.Text.Length - 1].ToString();

                if (network.convertKey.ContainsKey(stringToSend))
                {
                    stringToSend = network.convertKey[stringToSend];
                }
                if (String.IsNullOrWhiteSpace(SendKeyBox.Text.ElementAt(SendKeyBox.Text.Length - 1).ToString()))
                {
                    SendKeyBox.Text = "";
                }
                stringToSend = Network.keyTag + stringToSend;
                network.SendString(stringToSend);
                return;
            }
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

        override public void updateData(FrameworkElement control)
        {
            TextBox uibutton = (TextBox)control;
            x = Canvas.GetLeft(uibutton);
            y = Canvas.GetTop(uibutton);
            width = control.Width;
            height = control.Height;
        }
    }
}
