using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Collections.ObjectModel;

namespace Actual_windows_phone_controller.ViewModels
{
    public class ControllerViewModel : INotifyPropertyChanged
    {
        public ControllerViewModel()
        {
            this.Buttons = new ObservableCollection<ControllerButton>();
            ControllerButton sampleButton = new ControllerButton();
            sampleButton.DisplayTitle = "Hello";
            sampleButton.x = 100;
            sampleButton.y = 300;
            this.Buttons.Add(sampleButton);
            ControllerButton sampleButton2 = new ControllerButton();
            sampleButton2.DisplayTitle = "Buttons";
            sampleButton2.x = 50;
            sampleButton2.y = 400;
            this.Buttons.Add(sampleButton2);
        }
        private Canvas _UICanvas;
        public Canvas UICanvas
        {
            get
            {
                _UICanvas = Draw();
                return _UICanvas;
            }
            set
            {
                // never used

            }
        }
        public ObservableCollection<ControllerButton> Buttons { get; private set; }
        public Canvas Draw()
        {
            Canvas newCanvas = new Canvas();
            newCanvas.Margin = new Thickness(12, 0, 12, 0);
            foreach (ControllerButton button in Buttons)
            {
                Button uibutton = new Button();
                uibutton.Content = button.DisplayTitle;
                Canvas.SetLeft(uibutton, button.x);
                Canvas.SetTop(uibutton, button.y);
                newCanvas.Children.Add(uibutton);
            }
            return newCanvas;
        }
        private string _id;
        /// <summary>
        /// Sample ViewModel property; this property is used to identify the object.
        /// </summary>
        /// <returns></returns>
        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                if (value != _id)
                {
                    _id = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        private string _title;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (value != _title)
                {
                    _title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        private string _subtitle;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public string Subtitle
        {
            get
            {
                return _subtitle;
            }
            set
            {
                if (value != _subtitle)
                {
                    _subtitle = value;
                    NotifyPropertyChanged("Subtitle");
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