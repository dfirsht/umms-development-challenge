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
using Actual_windows_phone_controller.ViewModels.Buttons;

namespace Actual_windows_phone_controller.ViewModels
{
    public enum ButtonType
    {
        Button, Mouse, Keyboard, Volume
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
            AbstractControllerButton newButton;
            switch(type)
            {
                case ButtonType.Button: 
                    if (reader != null)
                    {
                        newButton = new StringControllerButton(reader);
                    }
                    else
                    {
                        newButton = new StringControllerButton();
                    }
                    
                    return newButton;
                case ButtonType.Mouse:
                    if(reader != null)
                    {
                        newButton = new MouseControllerButton(reader);
                    }
                    else
                    {
                        newButton = new MouseControllerButton();
                    }
                    
                    return newButton;
                case ButtonType.Keyboard:
                    if (reader != null)
                    {
                        newButton = new KeyboardControllerButton(reader);
                    }
                    else
                    {
                        newButton = new KeyboardControllerButton();
                    }

                    return newButton;
                case ButtonType.Volume:
                    if (reader != null)
                    {
                        newButton = new VolumeSliderControllerButton(reader);
                    }
                    else
                    {
                        newButton = new VolumeSliderControllerButton();
                    }

                    return newButton;
                default:
                    throw new InvalidDataException();
            }
        }
        abstract protected void initalizeFromStream(StreamReader reader);
        abstract public void Save(StreamWriter writer); // First line must be type
        abstract public FrameworkElement getVisualElement();
        abstract public void updateData(FrameworkElement control);

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
        public string DisplayTitle;
        public double x;
        public double y;
        public double width;
        public double height;
    }
}
