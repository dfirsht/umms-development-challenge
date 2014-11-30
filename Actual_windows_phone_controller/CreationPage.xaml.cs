﻿using System;
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

namespace Actual_windows_phone_controller
{
    public partial class CreationPage : PhoneApplicationPage
    {
        public CreationPage()
        {
            // get instance
            // pass error handler 
            InitializeComponent();
            //Canvas.SetTop(garbageCan, Application.Current.RootVisual.RenderSize.Height - garbageCan.ActualHeight);
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
            control.MouseMove += changePosition;
            control.ManipulationDelta += changeSize;
            control.LostMouseCapture += checkGarbageCollision;
            control.MouseLeave += checkGarbageCollision;
            control.LostMouseCapture += removeReference;
            control.MouseLeave += removeReference;

        }

        void checkGarbageCollision(object sender, MouseEventArgs e)
        {
            if (previousSender != sender)
            {
                return;
            }
            FrameworkElement button = (FrameworkElement)sender;
            double mouseX = mousePreviousPosition.X;
            double mouseY = mousePreviousPosition.Y;
            double garbageCanX = Canvas.GetLeft(garbageCan) + garbageCan.ActualWidth / 2;
            double garbageCanY = Canvas.GetTop(garbageCan) + garbageCan.ActualHeight / 2;
            if (Math.Abs(mouseX - garbageCanX) <= 15 && Math.Abs(mouseY - garbageCanY) <= 15)
            {
                controllerCanvas.Children.Remove(button);
                if (DataContext != null)
                {
                    ((ControllerViewModel)DataContext).Buttons.Remove((AbstractControllerButton)button.DataContext);
                    ((ControllerViewModel)DataContext).Save();
                }
                
            }
        }
        private void toolbarItemSelected(object sender, MouseButtonEventArgs e)
        {
            // Inialize new data object
            AbstractControllerButton button = AbstractControllerButton.ButtonFactory(ButtonType.Button);
            
            //Set object position
            Point mouseCordinates = e.GetPosition(controllerCanvas);
            button.x = mouseCordinates.X - button.width / 2;
            button.y = mouseCordinates.Y - button.height / 2;
            FrameworkElement uibutton = button.getVisualElement();
            controllerCanvas.Children.Add(uibutton);
            mousePreviousPosition = mouseCordinates;
            //Set event handlers
            SetFrameworkElementEventHandlers(uibutton);
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
                rectangle.Height = rectangle.Height * e.DeltaManipulation.Scale.Y;
                Canvas.SetLeft(rectangle, centerX - rectangle.Width  / 2);
                Canvas.SetTop(rectangle, centerY - rectangle.Height / 2);
                ((AbstractControllerButton)(((FrameworkElement)sender).DataContext)).updateData((FrameworkElement)sender);
                if (DataContext != null)
                {
                    ((ControllerViewModel)DataContext).Save();
                }
            }
        }
        Point mousePreviousPosition;
        private void removeReference(object sender, MouseEventArgs e)
        {
            previousSender = null;
        }
        private void MouseItemSelected(object sender, MouseButtonEventArgs e)
        {
            // Inialize new data object
            AbstractControllerButton button = new MouseControllerButton();

            //Set object position
            Point mouseCordinates = e.GetPosition(controllerCanvas);
            button.x = mouseCordinates.X - button.width / 2;
            button.y = mouseCordinates.Y - button.height / 2;
            FrameworkElement uibutton = button.getVisualElement();
            controllerCanvas.Children.Add(uibutton);
            mousePreviousPosition = mouseCordinates;
            //Set event handlers
            SetFrameworkElementEventHandlers(uibutton);
            uibutton.CaptureMouse();
            //Add to Controller
            if (DataContext != null)
            {
                ((ControllerViewModel)DataContext).Buttons.Add(button);
                ((ControllerViewModel)DataContext).Save();
            }
        }

        private void KeyboardItemSelected(object sender, MouseButtonEventArgs e)
        {
            // Inialize new data object
            AbstractControllerButton button = new KeyboardControllerButton();

            //Set object position
            Point mouseCordinates = e.GetPosition(controllerCanvas);
            button.x = mouseCordinates.X - button.width / 2;
            button.y = mouseCordinates.Y - button.height / 2;
            FrameworkElement uibutton = button.getVisualElement();
            controllerCanvas.Children.Add(uibutton);
            mousePreviousPosition = mouseCordinates;
            //Set event handlers
            SetFrameworkElementEventHandlers(uibutton);
            uibutton.CaptureMouse();
            //Add to Controller
            if (DataContext != null)
            {
                ((ControllerViewModel)DataContext).Buttons.Add(button);
                ((ControllerViewModel)DataContext).Save();
            }
        }
    }
}
