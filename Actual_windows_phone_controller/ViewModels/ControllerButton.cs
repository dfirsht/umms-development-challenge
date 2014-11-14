using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Actual_windows_phone_controller.ViewModels
{
    public class ControllerButton : INotifyPropertyChanged
    {
        public ControllerButton()
        {
        }
        public ControllerButton(StreamReader reader)
        {
            DisplayTitle = reader.ReadLine();
            x = Convert.ToDouble(reader.ReadLine());
            y = Convert.ToDouble(reader.ReadLine());
            width = Convert.ToDouble(reader.ReadLine());
            height = Convert.ToDouble(reader.ReadLine());
        }
        private string _displayTitle;
        public string DisplayTitle
        {
            get
            {
                return _displayTitle;
            }
            set
            {
                if (value != _displayTitle)
                {
                    _displayTitle = value;
                    NotifyPropertyChanged("DisplayTitle");
                }
            }
        }
        private double _x;
        public double x
        {
            get { return _x; }
            set
            {
                if (value != _x)
                {
                    _x = value;
                    NotifyPropertyChanged("x");
                }
            }
        }
        private double _y;
        public double y
        {
            get { return _y; }
            set
            {
                if (value != _y)
                {
                    _y = value;
                    NotifyPropertyChanged("y");
                }
            }
        }
        public double width;
        public double height;
        public void Save(StreamWriter writer)
        {
            writer.WriteLine(DisplayTitle);
            writer.WriteLine(x);
            writer.WriteLine(y);
            writer.WriteLine(width);
            writer.WriteLine(height);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public Control getVisualElement()
        {
            Button uibutton = new Button();
            uibutton.Content = DisplayTitle;
            uibutton.Width = width;
            uibutton.Height = height;
            Canvas.SetLeft(uibutton, x);
            Canvas.SetTop(uibutton, y);
            
            uibutton.DataContext = this;
            Binding contentBinder = new Binding {
                Source = DisplayTitle,
                Path = new PropertyPath("Content"),
                Mode = BindingMode.TwoWay,
            };
            return uibutton;
        }
        public void updateData(Control control)
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
