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

        public void LoadData()
        {
            //IsolatedStorageSettings.ApplicationSettings.Clear(); // uncomment to clear saves
            if (!IsolatedStorageSettings.ApplicationSettings.Contains("ControllerCollectionCount"))
            {
                //// Sample data; replace with real data
                //this.Items.Add(new ControllerViewModel() { ID = "0", Title = "runtime one", Subtitle = "Maecenas praesent accumsan bibendum" });
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

        public void removeItem(ControllerViewModel toDelete)
        {
            for(int i = Convert.ToInt16(toDelete.ID); i < Items.Count; ++i)
            {
                int id = Convert.ToInt16(Items[i].ID);
                --id;
                Items[i].ID = id.ToString();
            }
            this.Items.Remove(toDelete);
            foreach(ControllerViewModel controller in Items)
            {
                controller.Save();
            }
            if (IsolatedStorageSettings.ApplicationSettings.Contains("ControllerCollectionCount"))
            {
                IsolatedStorageSettings.ApplicationSettings.Remove("ControllerCollectionCount");
            }
            IsolatedStorageSettings.ApplicationSettings.Add("ControllerCollectionCount", Items.Count.ToString());
            IsolatedStorageSettings.ApplicationSettings.Save();
        }
    }
}