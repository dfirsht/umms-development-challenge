﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Actual_windows_phone_controller.ViewModels
{
    class StringControllerButton : AbstractControllerButton
    {
        public StringControllerButton()
        {
            width = 300;
            height = 100;
        }
        public StringControllerButton(StreamReader reader) : base(reader)
        {
        }
        override protected void initalizeFromStream(StreamReader reader)
        {
            DisplayTitle = reader.ReadLine();
            x = Convert.ToDouble(reader.ReadLine());
            y = Convert.ToDouble(reader.ReadLine());
            width = Convert.ToDouble(reader.ReadLine());
            height = Convert.ToDouble(reader.ReadLine());
        }
        override public void Save(StreamWriter writer)
        {
            writer.WriteLine(ButtonType.Button);
            writer.WriteLine(DisplayTitle);
            writer.WriteLine(x);
            writer.WriteLine(y);
            writer.WriteLine(width);
            writer.WriteLine(height);
        }
        override public FrameworkElement getVisualElement()
        {
            Button uibutton = new Button();
            uibutton.Content = DisplayTitle;
            uibutton.Width = width;
            uibutton.Height = height;
            Canvas.SetLeft(uibutton, x);
            Canvas.SetTop(uibutton, y);
            uibutton.Click += SendCommand;

            uibutton.DataContext = this;
            return uibutton;
        }

        void SendCommand(object sender, RoutedEventArgs e)
        {
            Network network = Network.GetInstance();
            network.SendString(DisplayTitle);
        }
        override public void updateData(FrameworkElement control)
        {
            Button uibutton = (Button)control;
            DisplayTitle = Convert.ToString(uibutton.Content);
            x = Canvas.GetLeft(uibutton);
            y = Canvas.GetTop(uibutton);
            width = control.Width;
            height = control.Height;
        }
    }
}

