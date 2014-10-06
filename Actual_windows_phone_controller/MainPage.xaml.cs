using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Actual_windows_phone_controller.Resources;
using Actual_windows_phone_controller.ViewModels;

namespace Actual_windows_phone_controller
{

    public partial class MainPage : PhoneApplicationPage
    {
         public T GetVisualChild<T>(UIElement parent) where T : UIElement
        {
            T child = null; // default(T);

            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                UIElement element = (UIElement)VisualTreeHelper.GetChild(parent, i);
                child = element as T;
                if (child == null)
                    child = GetVisualChild<T>(element);
                if (child != null)
                    break;
            }

            return child;
        }
    

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the LongListSelector control to the sample data
            DataContext = App.ViewModel;

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();

            //MouseLeftButtonUp += item_released;
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        // Handle selection changed on LongListSelector
        private void MainLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected item is null (no selection) do nothing
            if (MainLongListSelector.SelectedItem == null)
                return;

            // Navigate to the new page
            NavigationService.Navigate(new Uri("/ControllerPage.xaml?selectedItem=" + (MainLongListSelector.SelectedItem as ControllerViewModel).ID, UriKind.Relative));

            // Reset selected item to null (no selection)
            MainLongListSelector.SelectedItem = null;
        }
        private void ConnectionPageClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ConnectionPage.xaml", UriKind.Relative));
        }
        private void add_button_pressed(object sender, RoutedEventArgs e)
        {
            App.ViewModel.addItem("New Controller!");
        }
        private int item_selected;
        private void item_held(object sender, System.Windows.Input.GestureEventArgs e)
        {
            foreach(UIElement element in (sender as StackPanel).Children) {
                if (element.GetType().Name == "Canvas")
                {
                    element.Visibility = Visibility.Visible;
                }
            }
            (sender as StackPanel).Margin =  new Thickness(0,0,0,80);
            ControllerViewModel itemViewModel = (sender as StackPanel).DataContext as ControllerViewModel;
            item_selected = App.ViewModel.Items.IndexOf(itemViewModel);

            /*ItemViewModel itemViewModel = (sender as StackPanel).DataContext as ItemViewModel;
            //itemViewModel.Title = "cat";
            item_held_selected = App.ViewModel.Items.IndexOf(itemViewModel);
            // MessageBox.Show(item_held_selected.ToString());
            item_held_down = true;
            ScrollViewer viewer = GetVisualChild<ScrollViewer>(MainLongListSelector);
            ScrollViewer.SetVerticalScrollBarVisibility(viewer, ScrollBarVisibility.Disabled);
             */
        }
        private void item_released(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            /* if (item_held_down)
            {
                item_held_down = false;
                MessageBox.Show("done");
                ScrollViewer viewer = GetVisualChild<ScrollViewer>(MainLongListSelector);
                ScrollViewer.SetVerticalScrollBarVisibility(viewer, ScrollBarVisibility.Auto);
            }
             */
        }
        private void item_moved(object sender, System.Windows.Input.MouseEventArgs e)
        {
            /*
            if (item_held_down)
            {
                ItemViewModel itemViewModel = (sender as StackPanel).DataContext as ItemViewModel;
                item_hovered_over = App.ViewModel.Items.IndexOf(itemViewModel);
                ItemViewModel originalViewModel = App.ViewModel.Items.ElementAt<ItemViewModel>(item_held_selected);
                MainLongListSelector.ScrollIntoView(originalViewModel);
                if (item_hovered_over != item_held_selected)
                {
                    //string temp = itemViewModel.ID;
                    //itemViewModel.ID = originalViewModel.ID;
                    //originalViewModel.ID = temp;
                    string temp = itemViewModel.Title;
                    itemViewModel.Title = originalViewModel.Title;
                    originalViewModel.Title = temp;

                    item_held_selected = App.ViewModel.Items.IndexOf(itemViewModel);

                    (sender as UIElement).CaptureMouse();
            
                    //App.ViewModel.moveItem(item_held_selected, item_hovered_over);
                }
            }
             */
        }

        private void moveElementUp(object sender, RoutedEventArgs e)
        {
            if (item_selected != 0)
            {
                ControllerViewModel southernItem = App.ViewModel.Items.ElementAt<ControllerViewModel>(item_selected);
                ControllerViewModel NorthernItem = App.ViewModel.Items.ElementAt<ControllerViewModel>(item_selected - 1);
                string temp = southernItem.Title;
                southernItem.Title = NorthernItem.Title;
                NorthernItem.Title = temp;
                ListBoxItem currentListBoxItem =
    (ListBoxItem)(MainLongListSelector.ItemContainerGenerator.ContainerFromIndex(item_selected));

                StackPanel currentStackPanel = (StackPanel)GetVisualChild<StackPanel>(currentListBoxItem);

                foreach (UIElement element in currentStackPanel.Children)
                {
                    if (element.GetType().Name == "Canvas")
                    {
                        element.Visibility = Visibility.Collapsed;
                    }
                }
                currentStackPanel.Margin = new Thickness(0, 0, 0, 20);

                ListBoxItem newListBoxItem =
      (ListBoxItem)(MainLongListSelector.ItemContainerGenerator.ContainerFromIndex(item_selected - 1));

                StackPanel newStackPanel = (StackPanel)GetVisualChild<StackPanel>(newListBoxItem);

                foreach (UIElement element in newStackPanel.Children)
                {
                    if (element.GetType().Name == "Canvas")
                    {
                        element.Visibility = Visibility.Visible;
                    }
                }
                newStackPanel.Margin = new Thickness(0, 0, 0, 80);
                item_selected--;
            }
        }

        private void moveElementDown(object sender, RoutedEventArgs e)
        {
            if (item_selected != App.ViewModel.Items.Count - 1)
            {
                ControllerViewModel southernItem = App.ViewModel.Items.ElementAt<ControllerViewModel>(item_selected + 1);
                ControllerViewModel NorthernItem = App.ViewModel.Items.ElementAt<ControllerViewModel>(item_selected);
                string temp = southernItem.Title;
                southernItem.Title = NorthernItem.Title;
                NorthernItem.Title = temp;
                ListBoxItem currentListBoxItem =
    (ListBoxItem)(MainLongListSelector.ItemContainerGenerator.ContainerFromIndex(item_selected));

                StackPanel currentStackPanel = (StackPanel)GetVisualChild<StackPanel>(currentListBoxItem);

                foreach (UIElement element in currentStackPanel.Children)
                {
                    if (element.GetType().Name == "Canvas")
                    {
                        element.Visibility = Visibility.Collapsed;
                    }
                }
                currentStackPanel.Margin = new Thickness(0, 0, 0, 20);

                ListBoxItem newListBoxItem =
      (ListBoxItem)(MainLongListSelector.ItemContainerGenerator.ContainerFromIndex(item_selected + 1));

                StackPanel newStackPanel = (StackPanel)GetVisualChild<StackPanel>(newListBoxItem);

                foreach (UIElement element in newStackPanel.Children)
                {
                    if (element.GetType().Name == "Canvas")
                    {
                        element.Visibility = Visibility.Visible;
                    }
                }
                newStackPanel.Margin = new Thickness(0, 0, 0, 80);
                item_selected++;
            }
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}