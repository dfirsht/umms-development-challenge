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
    public enum ButtonType
    {
        Button
    }
    abstract public class AbstractControllerButton : INotifyPropertyChanged
    {
        public AbstractControllerButton()
        {
        }
        public AbstractControllerButton(StreamReader reader)
        {
            initalizeFromStream(reader);
        }
        static public AbstractControllerButton ButtonFactory(ButtonType type, StreamReader reader = null)
        {
            switch(type)
            {
                case ButtonType.Button:
                    AbstractControllerButton newButton;
                    if(reader != null)
                    {
                        newButton = new ControllerButton(reader);
                    }
                    else
                    {
                        newButton = new ControllerButton();
                    }
                    
                    return newButton;
                default:
                    throw new InvalidDataException();
            }
        }
        abstract protected void initalizeFromStream(StreamReader reader);
        abstract public void Save(StreamWriter writer); // First line must be type
        abstract public Control getVisualElement();
        abstract public void updateData(Control control);

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        // Fat interface properties
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
    }
}
