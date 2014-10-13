using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using Actual_windows_phone_controller.Resources;

namespace Actual_windows_phone_controller.ViewModels
{
    public class ControllerCollectionViewModel : INotifyPropertyChanged
    {
        public ControllerCollectionViewModel()
        {
            this.Items = new ObservableCollection<ControllerViewModel>();
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<ControllerViewModel> Items { get; private set; }

        private string _sampleProperty = "Sample Runtime Property Value";
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding
        /// </summary>
        /// <returns></returns>
        public string SampleProperty
        {
            get
            {
                return _sampleProperty;
            }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        /// <summary>
        /// Sample property that returns a localized string
        /// </summary>
        public string LocalizedSampleProperty
        {
            get
            {
                return AppResources.SampleProperty;
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            //IsolatedStorageSettings.ApplicationSettings.Clear();
            if (!IsolatedStorageSettings.ApplicationSettings.Contains("ControllerCollectionCount"))
            {
                //// Sample data; replace with real data
                //this.Items.Add(new ControllerViewModel() { ID = "0", Title = "runtime one", Subtitle = "Maecenas praesent accumsan bibendum" });
                //this.Items.Add(new ControllerViewModel() { ID = "1", Title = "runtime two", Subtitle = "Dictumst eleifend facilisi faucibus" });
                //this.Items.Add(new ControllerViewModel() { ID = "2", Title = "runtime three", Subtitle = "Habitant inceptos interdum lobortis" });
                //this.Items.Add(new ControllerViewModel() { ID = "3", Title = "runtime four", Subtitle = "Nascetur pharetra placerat pulvinar" });
                //this.Items.Add(new ControllerViewModel() { ID = "4", Title = "runtime five", Subtitle = "Maecenas praesent accumsan bibendum" });
                //this.Items.Add(new ControllerViewModel() { ID = "5", Title = "runtime six", Subtitle = "Dictumst eleifend facilisi faucibus" });
                //this.Items.Add(new ControllerViewModel() { ID = "6", Title = "runtime seven", Subtitle = "Habitant inceptos interdum lobortis" });
                //this.Items.Add(new ControllerViewModel() { ID = "7", Title = "runtime eight", Subtitle = "Nascetur pharetra placerat pulvinar" });
                //this.Items.Add(new ControllerViewModel() { ID = "8", Title = "runtime nine", Subtitle = "Maecenas praesent accumsan bibendum" });
                //this.Items.Add(new ControllerViewModel() { ID = "9", Title = "runtime ten", Subtitle = "Dictumst eleifend facilisi faucibus" });
                //this.Items.Add(new ControllerViewModel() { ID = "10", Title = "runtime eleven", Subtitle = "Habitant inceptos interdum lobortis" });
                //this.Items.Add(new ControllerViewModel() { ID = "11", Title = "runtime twelve", Subtitle = "Nascetur pharetra placerat pulvinar" });
                //this.Items.Add(new ControllerViewModel() { ID = "12", Title = "runtime thirteen", Subtitle = "Maecenas praesent accumsan bibendum" });
                //this.Items.Add(new ControllerViewModel() { ID = "13", Title = "runtime fourteen", Subtitle = "Dictumst eleifend facilisi faucibus" });
                //this.Items.Add(new ControllerViewModel() { ID = "14", Title = "runtime fifteen", Subtitle = "Habitant inceptos interdum lobortis" });
                //this.Items.Add(new ControllerViewModel() { ID = "15", Title = "runtime sixteen", Subtitle = "Nascetur pharetra placerat pulvinar" });
            }
            else
            {
                int numControllers = 0;
                numControllers = Convert.ToInt16(IsolatedStorageSettings.ApplicationSettings["ControllerCollectionCount"]); 
                IsolatedStorageFile localFileSystem = IsolatedStorageFile.GetUserStoreForApplication();
                for (int i = 0; i < numControllers; ++i)
                {
                    IsolatedStorageFileStream controllerData = localFileSystem.OpenFile("Controller" + i.ToString() + ".txt",System.IO.FileMode.Open,System.IO.FileAccess.Read);
                    ControllerViewModel newController = new ControllerViewModel(controllerData);
                    newController.ID = i.ToString();

                    this.Items.Add(newController);
                }
                    
            }
            

            this.IsDataLoaded = true;
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

        public void addItem(String title)
        {
            ControllerViewModel newController = new ControllerViewModel() { ID = Items.Count.ToString(), Title = title };
            this.Items.Add(newController);
            if (IsolatedStorageSettings.ApplicationSettings.Contains("ControllerCollectionCount"))
            {
                IsolatedStorageSettings.ApplicationSettings.Remove("ControllerCollectionCount");
            }
            IsolatedStorageSettings.ApplicationSettings.Add("ControllerCollectionCount",Items.Count.ToString());
            newController.Save();
            IsolatedStorageSettings.ApplicationSettings.Save();
        }
    }
}