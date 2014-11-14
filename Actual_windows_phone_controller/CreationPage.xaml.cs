using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Input;
using Actual_windows_phone_controller.ViewModels;

namespace Actual_windows_phone_controller
{
    public partial class CreationPage : PhoneApplicationPage
    {
        public CreationPage()
        {
            InitializeComponent();
        }
        // When page is navigated to set data context to selected item in list
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (DataContext == null)
            {
                string selectedIndex = "";
                if (NavigationContext.QueryString.TryGetValue("selectedItem", out selectedIndex))
                {
                    int index = int.Parse(selectedIndex);
                    DataContext = App.ViewModel.Items[index];
                    foreach (ControllerButton button in ((ControllerViewModel)DataContext).Buttons)
                    {
                        Control uibutton = button.getVisualElement();
                        controllerCanvas.Children.Add(uibutton);
                        //Set event handlers
                        uibutton.MouseMove += changePosition;
                        uibutton.ManipulationDelta += changeSize;
                    }
                }
            }
        }
        private void toolbarItemSelected(object sender, MouseButtonEventArgs e)
        {
            // Inialize new data object
            ControllerButton button = new ControllerButton();
            button.width = 100;
            button.height = 100;
            
            //Set object position
            Point mouseCordinates = e.GetPosition(controllerCanvas);
            button.x = mouseCordinates.X - button.width / 2;
            button.y = mouseCordinates.Y - button.height / 2;
            Control uibutton = button.getVisualElement();
            controllerCanvas.Children.Add(uibutton);
            //Set event handlers
            mousePreviousPosition = mouseCordinates;
            uibutton.MouseMove += changePosition;
            uibutton.ManipulationDelta += changeSize;
            uibutton.CaptureMouse();
            //Add to Controller
            if (DataContext != null)
            {
                ((ControllerViewModel)DataContext).Buttons.Add(button);
                ((ControllerViewModel)DataContext).Save();
            }
        }
        object previousSender;
        private void changePosition(object sender, MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition(controllerCanvas);
            if (previousSender == sender)
            {
                Control controlSender = (Control)sender;
                double originalLeft = Canvas.GetLeft(controlSender);
                double originalTop = Canvas.GetTop(controlSender);
                Canvas.SetLeft(controlSender, originalLeft + mousePosition.X - mousePreviousPosition.X);
                Canvas.SetTop(controlSender, originalTop + mousePosition.Y - mousePreviousPosition.Y);
                ((ControllerButton)(controlSender.DataContext)).updateData(controlSender);
                if (DataContext != null)
                {
                    ((ControllerViewModel)DataContext).Save();
                }
            }
            mousePreviousPosition = mousePosition;
            previousSender = sender;
        }

        private void changeSize(object sender, ManipulationDeltaEventArgs e)
        {
            if (e.DeltaManipulation.Scale.X > 0 && e.DeltaManipulation.Scale.Y > 0)
            {
                FrameworkElement rectangle = (FrameworkElement)sender;
                double centerX = Canvas.GetLeft(rectangle) + rectangle.Width / 2;
                double centerY = Canvas.GetTop(rectangle) + rectangle.Height / 2;
                rectangle.Width = rectangle.Width * e.DeltaManipulation.Scale.X;
                rectangle.Height = rectangle.Height * e.DeltaManipulation.Scale.Y;
                Canvas.SetLeft(rectangle, centerX - rectangle.Width  / 2);
                Canvas.SetTop(rectangle, centerY - rectangle.Height / 2);
                ((ControllerButton)(((Control)sender).DataContext)).updateData((Control)sender);
                if (DataContext != null)
                {
                    ((ControllerViewModel)DataContext).Save();
                }
            }
        }
        Point mousePreviousPosition;
    }
}
