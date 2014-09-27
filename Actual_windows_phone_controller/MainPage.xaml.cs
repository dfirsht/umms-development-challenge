using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Actual_windows_phone_controller.Resources;
using Actual_windows_phone_controller.ViewModels;

namespace Actual_windows_phone_controller
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the LongListSelector control to the sample data
            DataContext = App.ViewModel;

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
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
            NavigationService.Navigate(new Uri("/DetailsPage.xaml?selectedItem=" + (MainLongListSelector.SelectedItem as ItemViewModel).ID, UriKind.Relative));

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
        private bool item_held_down = false;
        private int item_held_selected;
        private UIElementCollection UIElement_selected;
        private int item_hovered_over;
        private void item_held(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ItemViewModel itemViewModel = (sender as StackPanel).DataContext as ItemViewModel;
            item_held_selected = App.ViewModel.Items.IndexOf(itemViewModel);
            UIElement_selected = (sender as StackPanel).Children;
            // MessageBox.Show(item_held_selected.ToString());
            item_held_down = true;
        }
        private void item_released(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (item_held_down)
            {
                item_held_down = false;
                MessageBox.Show("done");
            }
        }

        private void item_moved(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (item_held_down)
            {
                ItemViewModel itemViewModel = (sender as StackPanel).DataContext as ItemViewModel;
                item_hovered_over = App.ViewModel.Items.IndexOf(itemViewModel);
                if (item_hovered_over != item_held_selected)
                {
                    App.ViewModel.moveItem(item_held_selected, item_hovered_over);
                }
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