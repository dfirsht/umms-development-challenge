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
using Actual_windows_phone_controller.ViewModels.Buttons;
using Windows.UI;

namespace Actual_windows_phone_controller
{
    public partial class CreationPage : PhoneApplicationPage
    {
        object previousSender;
        Point mousePreviousPosition;
        object previousObjectSelected = null;
        Point previousSelectorSelectionPosition;

        public CreationPage()
        {
            // get instance
            // pass error handler 
            InitializeComponent();
            // Set the data context of the LongListSelector control to the sample data
        }
        // When page is navigated to set data context to selected item in list
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (DataContext == null)
            {
                string selectedIndex = "";
                // Attempt to load Controller Buttons from the ControllerViewModel
                if (NavigationContext.QueryString.TryGetValue("selectedItem", out selectedIndex))
                {
                    int index = int.Parse(selectedIndex);
                    DataContext = App.ViewModel.Items[index];
                    foreach (AbstractControllerButton button in ((ControllerViewModel)DataContext).Buttons)
                    {
                        FrameworkElement uibutton = button.getVisualElement();
                        SetFrameworkElementEventHandlers(uibutton);
                        controllerCanvas.Children.Add(uibutton);
                    }
                }
            }
        }
        private void SetFrameworkElementEventHandlers(FrameworkElement control)
        {
            // event handelers to handle moving the object and resizing it
            // In addition handler user stop moving an dragging it to garbage
            control.MouseMove += changePosition;
            control.MouseMove += checkToOpenGarbageCan;
            control.ManipulationDelta += changeSize;
            control.LostMouseCapture += checkInGarbageCan;
            control.LostMouseCapture += removeReference;
            control.MouseLeave += checkInGarbageCan;
            control.MouseLeave += removeReference;
        }
        //GarbageCan Handlers
        void checkToOpenGarbageCan(object sender, MouseEventArgs e)
        {
            if(isOverGarbageCan(sender))
            {
                garbageCan.Fill = new SolidColorBrush(Colors.Red);
            }
            else 
            {
                garbageCan.Fill = new SolidColorBrush(Colors.Gray);
            }
        }
        bool isOverGarbageCan(object sender)
        {
            // check if previous coordinates are valid
            if (previousSender != sender)
            {
                return false;
            }
            double mouseX = mousePreviousPosition.X;
            double mouseY = mousePreviousPosition.Y;
            double garbageCanX = Canvas.GetLeft(garbageCan) + garbageCan.ActualWidth / 2;
            double garbageCanY = Canvas.GetTop(garbageCan) + garbageCan.ActualHeight / 2;
            if (Math.Abs(mouseX - garbageCanX) <= 15 && Math.Abs(mouseY - garbageCanY) <= 15)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        void checkInGarbageCan(object sender, MouseEventArgs e)
        {
            FrameworkElement button = (FrameworkElement)sender;
            if(isOverGarbageCan(sender))
            {
                controllerCanvas.Children.Remove(button);
                garbageCan.Fill = new SolidColorBrush(Colors.Gray);
                if (DataContext != null)
                {
                    ((ControllerViewModel)DataContext).Buttons.Remove((AbstractControllerButton)button.DataContext);
                    ((ControllerViewModel)DataContext).Save();
                }
                
            }

        }
        // To remove mouse capture on garbageCan
        private void LoseMouseCapture(object sender, MouseEventArgs e)
        {
            if (previousObjectSelected != null)
                ((FrameworkElement)previousObjectSelected).CaptureMouse();
        }
        // Button manipulation handlers
        private void changePosition(object sender, MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition(controllerCanvas);
            if (previousSender == sender)
            {
                // Calculate difference in mouse position since last update
                FrameworkElement controlSender = (FrameworkElement)sender;
                double originalLeft = Canvas.GetLeft(controlSender);
                double originalTop = Canvas.GetTop(controlSender);
                Canvas.SetLeft(controlSender, originalLeft + mousePosition.X - mousePreviousPosition.X);
                Canvas.SetTop(controlSender, originalTop + mousePosition.Y - mousePreviousPosition.Y);
                ((AbstractControllerButton)(controlSender.DataContext)).updateData(controlSender);
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
                if (rectangle.Width > this.ActualWidth)
                {
                    rectangle.Width = this.ActualWidth;
                }
                if (rectangle.Width < 50)
                    rectangle.Width = 50;
                rectangle.Height = rectangle.Height * e.DeltaManipulation.Scale.Y;
                if (rectangle.Height > this.ActualHeight)
                {
                    rectangle.Height = this.ActualHeight;
                }
                if (rectangle.Height < 50)
                    rectangle.Height = 50;
                Canvas.SetLeft(rectangle, centerX - rectangle.Width  / 2);
                Canvas.SetTop(rectangle, centerY - rectangle.Height / 2);
                ((AbstractControllerButton)(((FrameworkElement)sender).DataContext)).updateData((FrameworkElement)sender);
                if (DataContext != null)
                {
                    ((ControllerViewModel)DataContext).Save();
                }
            }
        }
        private void removeReference(object sender, MouseEventArgs e)
        {
            previousObjectSelected = previousSender;
            previousSender = null;
        }
        void initalizeVisualElement(AbstractControllerButton button, MouseButtonEventArgs e)
        {
            //Set object position
            Point mouseCordinates = e.GetPosition(controllerCanvas);
            button.x = mouseCordinates.X - button.width / 2;
            button.y = mouseCordinates.Y - button.height / 2;
            FrameworkElement uibutton = button.getVisualElement();
            controllerCanvas.Children.Add(uibutton);
            mousePreviousPosition = mouseCordinates;
            //Set event handlers
            SetFrameworkElementEventHandlers(uibutton);
            //give focus to visual object
            uibutton.CaptureMouse();
            //Add to Controller
            if (DataContext != null)
            {
                ((ControllerViewModel)DataContext).Buttons.Add(button);
                ((ControllerViewModel)DataContext).Save();
            }
        }
        // Button Selector click handlers
        private void toolbarItemSelected(object sender, MouseButtonEventArgs e)
        {
            // Inialize new data object
            AbstractControllerButton button = AbstractControllerButton.ButtonFactory(ButtonType.Button);
            initalizeVisualElement(button, e);
        }
        private void MouseItemSelected(object sender, MouseButtonEventArgs e)
        {
            // Inialize new data object
            AbstractControllerButton button = new MouseControllerButton();
            initalizeVisualElement(button, e);
        }

        private void KeyboardItemSelected(object sender, MouseButtonEventArgs e)
        {
            // Inialize new data object
            AbstractControllerButton button = new KeyboardControllerButton();
            initalizeVisualElement(button, e);
        }
        // Handler Selector Sliding
        private void ButtonSelectorSelected(object sender, MouseButtonEventArgs e)
        {
            previousSelectorSelectionPosition = e.GetPosition(null);
        }
        private void ButtonSelectorMoved(object sender, MouseEventArgs e)
        {
            StackPanel selector = (StackPanel)sender;
            Double newXposition = Canvas.GetLeft(selector) + e.GetPosition(null).X - previousSelectorSelectionPosition.X;
            Double minX = contentControl.ActualWidth/2 - selector.ActualWidth;
            Double maxX = contentControl.ActualWidth/2 - 30;
            
            if (newXposition < minX)
            {
                Canvas.SetLeft(selector,minX);
            }
            else if (newXposition > maxX)
            {
                Canvas.SetLeft(selector, maxX);
            }
            else
            {
                Canvas.SetLeft(selector, newXposition);
            }
            previousSelectorSelectionPosition = e.GetPosition(null);
        }
    }
}
