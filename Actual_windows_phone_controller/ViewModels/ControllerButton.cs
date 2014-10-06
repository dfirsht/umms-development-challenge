using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actual_windows_phone_controller.ViewModels
{
    public class ControllerButton : INotifyPropertyChanged
    {
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
