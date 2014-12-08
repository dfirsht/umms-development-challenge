using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Reflection;
using System.ComponentModel;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Actual_windows_phone_controller.Resources;
using Actual_windows_phone_controller.ViewModels;

namespace Actual_windows_phone_controller
{

    public partial class MainPage : PhoneApplicationPage
    {
        //private bool optionsShown = false;
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

            item_selected = -1;

            
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
            if (App.ViewModel.Items.IndexOf((MainLongListSelector.SelectedItem as ControllerViewModel)) == item_selected)
            {
                return;
            }
            // Navigate to the new page
            NavigationService.Navigate(new Uri("/ControllerPage.xaml?selectedItem=" + (MainLongListSelector.SelectedItem as ControllerViewModel).ID, UriKind.Relative));

            // Reset selected item to null (no selection)
            MainLongListSelector.SelectedItem = null;
        }
        private void ConnectionPageClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ConnectionPage.xaml", UriKind.Relative));
        }
        private void CreatePage_Click(object sender, RoutedEventArgs e) 
        { 
            NavigationService.Navigate(new Uri("/CreationPage.xaml", UriKind.RelativeOrAbsolute)); 
        } 

        private void add_button_pressed(object sender, RoutedEventArgs e)
        {
            App.ViewModel.addItem("New Controller");
        }
        private int item_selected;
        private void item_held(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if(item_selected >= 0)
            {
                hideOptions(item_selected);
            }
            ControllerViewModel itemViewModel = (sender as StackPanel).DataContext as ControllerViewModel;
            if (App.ViewModel.Items.IndexOf(itemViewModel) != item_selected)
            {
                foreach (UIElement element in (sender as StackPanel).Children)
                {
                    if (element.GetType().Name == "Canvas")
                    {
                        element.Visibility = Visibility.Visible;
                    }
                    //else if (element.GetType().Name == "TextBox")
                    //{
                    //    (element as TextBox).Focus();
                    //}
                }
                (sender as StackPanel).Margin = new Thickness(0, 0, 0, 80);

                item_selected = App.ViewModel.Items.IndexOf(itemViewModel);
            }
            else
            {
                item_selected = -1;
                MainLongListSelector.Focus();
            }
        }

        //private void randomClick(object sender, RoutedEventArgs e)
        //{
        //    if (optionsShown)
        //        hideOptions(item_selected);
        //}

        private void moveElementUp(object sender, RoutedEventArgs e)
        {
            if (item_selected != 0)
            {
                ControllerViewModel southernItem = App.ViewModel.Items.ElementAt<ControllerViewModel>(item_selected);
                ControllerViewModel NorthernItem = App.ViewModel.Items.ElementAt<ControllerViewModel>(item_selected - 1);
                string temp = southernItem.Title;
                southernItem.Title = NorthernItem.Title;
                NorthernItem.Title = temp;
                hideOptions(item_selected);
                showOptions(item_selected - 1);

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
                hideOptions(item_selected);
                showOptions(item_selected + 1);

                item_selected++;
            }
        }

        private void deleteElement(object sender, RoutedEventArgs e)
        {
            ControllerViewModel NorthernItem = App.ViewModel.Items.ElementAt<ControllerViewModel>(item_selected);
            App.ViewModel.removeItem(NorthernItem);
            item_selected = -1;
        }

        private void showOptions(int indexNumber)
        {
            //optionsShown = true;
            ListBoxItem newListBoxItem =
      (ListBoxItem)(MainLongListSelector.ItemContainerGenerator.ContainerFromIndex(indexNumber));

            StackPanel newStackPanel = (StackPanel)GetVisualChild<StackPanel>(newListBoxItem);

            foreach (UIElement element in newStackPanel.Children)
            {
                if (element.GetType().Name == "Canvas")
                {
                    element.Visibility = Visibility.Visible;
                }
                else if (element.GetType().Name == "TextBox")
                {
                    (element as TextBox).Focus();
                }
            }
            newStackPanel.Margin = new Thickness(0, 0, 0, 80);
        }
        private void hideOptions(int indexNumber)
        {
            //optionsShown = false;
            ListBoxItem currentListBoxItem =
    (ListBoxItem)(MainLongListSelector.ItemContainerGenerator.ContainerFromIndex(indexNumber));

            StackPanel currentStackPanel = (StackPanel)GetVisualChild<StackPanel>(currentListBoxItem);

            foreach (UIElement element in currentStackPanel.Children)
            {
                if (element.GetType().Name == "Canvas")
                {
                    element.Visibility = Visibility.Collapsed;
                }
            }
            currentStackPanel.Margin = new Thickness(0, 0, 0, 20);
        }

        private void controllerTitleTextLostFocus(object sender, RoutedEventArgs e)
        {
            SolidColorBrush whiteBrush = new SolidColorBrush(Colors.White);
            (sender as TextBox).Foreground = whiteBrush;
        }

        private void controllerTitleTextGotFocus(object sender, RoutedEventArgs e)
        {
            SolidColorBrush blackBrush = new SolidColorBrush(Colors.Black);
            (sender as TextBox).Foreground = blackBrush;
        }

        private void controllerTitleTextChanged(object sender, TextChangedEventArgs e)
        {
            ControllerViewModel itemViewModel = (sender as TextBox).DataContext as ControllerViewModel;
            itemViewModel.Title = (sender as TextBox).Text;
            itemViewModel.Save();
        }

        private void controllerTitleTextTapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ControllerViewModel itemViewModel = (sender as TextBox).DataContext as ControllerViewModel;
            int item_tapped = App.ViewModel.Items.IndexOf(itemViewModel);
            if (item_tapped != item_selected)
            {
                MainLongListSelector.SelectedIndex = item_tapped;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void item_tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ControllerViewModel itemViewModel = (sender as StackPanel).DataContext as ControllerViewModel;
            int item_tapped = App.ViewModel.Items.IndexOf(itemViewModel);
            if (item_tapped == item_selected)
            {
                MainLongListSelector.Focus();
                e.Handled = true;
            }
        }

        private void navagateToEditPage(object sender, RoutedEventArgs e)
        {
            // Navigate to the new page
            NavigationService.Navigate(new Uri("/CreationPage.xaml?selectedItem=" + item_selected, UriKind.Relative));

            // Reset selected item to null (no selection)
            MainLongListSelector.SelectedItem = null;
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