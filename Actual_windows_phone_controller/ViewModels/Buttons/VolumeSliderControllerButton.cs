using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Actual_windows_phone_controller.ViewModels.Buttons
{
    class VolumeSliderControllerButton : AbstractControllerButton
    {
        public VolumeSliderControllerButton()
        {
            width = 234;
            height = 84;
        }
        public VolumeSliderControllerButton(StreamReader reader)
            : base(reader)
        {
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
            writer.WriteLine(ButtonType.Volume);
            writer.WriteLine(x);
            writer.WriteLine(y);
            writer.WriteLine(width);
            writer.WriteLine(height);
        }
        override public FrameworkElement getVisualElement()
        {
            Slider uibutton = new Slider();
            uibutton.Width = width;
            uibutton.Height = height;
            uibutton.Maximum = 100;
            Canvas.SetLeft(uibutton, x);
            Canvas.SetTop(uibutton, y);
            uibutton.ValueChanged += uibutton_ValueChanged;

            uibutton.DataContext = this;
            return uibutton;
        }

        void uibutton_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider Slide = (Slider)sender;
            string name = "nircmd.exe setsysvolume";
            double percent = (Math.Floor(Slide.Value));
            // fill percent with zeroes to give unified format
            percent = percent / Slide.Maximum;

            if (name == "nircmd.exe setsysvolume")
            {
                percent = (Math.Floor(Network.maxVolume * percent));
            }

            string stringToSend = Network.cmdTag + name + ' ' + percent.ToString();
            Network network = Network.GetInstance();
            network.SendString(stringToSend);
        }

        override public void updateData(FrameworkElement control)
        {
            Slider uibutton = (Slider)control;
            x = Canvas.GetLeft(uibutton);
            y = Canvas.GetTop(uibutton);
            width = control.Width;
            height = control.Height;
        }
    }
}
