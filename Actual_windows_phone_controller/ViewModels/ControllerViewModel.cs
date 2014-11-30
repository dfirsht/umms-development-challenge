using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.IO.IsolatedStorage;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;

namespace Actual_windows_phone_controller.ViewModels
{
    public class ControllerViewModel : INotifyPropertyChanged
    {
        public ControllerViewModel()
        {
            this.Buttons = new ObservableCollection<AbstractControllerButton>();
            AbstractControllerButton sampleButton = new StringControllerButton();
            sampleButton.DisplayTitle = "Hello";
            sampleButton.x = 100;
            sampleButton.y = 300;
            sampleButton.width = 200;
            sampleButton.height = 100;
            this.Buttons.Add(sampleButton);
            AbstractControllerButton sampleButton2 = new StringControllerButton();
            sampleButton2.DisplayTitle = "Buttons";
            sampleButton2.x = 50;
            sampleButton2.y = 400;
            sampleButton2.width = 200;
            sampleButton2.height = 100;
            this.Buttons.Add(sampleButton2);
        }
        public ControllerViewModel(IsolatedStorageFileStream file)
        {
            StreamReader reader = new StreamReader(file);
            ID = reader.ReadLine();
            Title = reader.ReadLine();
            Subtitle = reader.ReadLine();

            this.Buttons = new ObservableCollection<AbstractControllerButton>();
            int numButtons = Convert.ToInt16(reader.ReadLine());
            for (int i = 0; i < numButtons; ++i)
            {
                String buttonName = reader.ReadLine();
                ButtonType buttonType = (ButtonType)Enum.Parse(typeof(ButtonType), buttonName, true);
                Buttons.Add(AbstractControllerButton.ButtonFactory(buttonType, reader));
            }
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
        public ObservableCollection<AbstractControllerButton> Buttons { get; private set; }
        public Canvas Draw()
        {
            Canvas newCanvas = new Canvas();
            newCanvas.Margin = new Thickness(12, 0, 12, 0);
            foreach (AbstractControllerButton button in Buttons)
            {
                newCanvas.Children.Add(button.getVisualElement());
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
        public void Save()
        {
            IsolatedStorageFile localFileSystem = IsolatedStorageFile.GetUserStoreForApplication();
            if (localFileSystem.FileExists("Controller" + ID + ".txt"))
            {
                localFileSystem.DeleteFile("Controller" + ID + ".txt");
            }
            IsolatedStorageFileStream controllerFile = new IsolatedStorageFileStream("Controller" + ID + ".txt", FileMode.Create, localFileSystem);
            using (StreamWriter writer = new StreamWriter(controllerFile))
            {
                writer.WriteLine(ID);
                writer.WriteLine(Title);
                writer.WriteLine(Subtitle);
                writer.WriteLine(Buttons.Count);
                foreach (AbstractControllerButton button in Buttons)
                {
                    button.Save(writer);
                }
                writer.Close();
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