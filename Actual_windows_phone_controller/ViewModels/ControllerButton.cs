using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using System.IO;

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
        public void Save(StreamWriter writer)
        {
            writer.WriteLine(DisplayTitle);
            writer.WriteLine(x);
            writer.WriteLine(y);
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
    }
}
