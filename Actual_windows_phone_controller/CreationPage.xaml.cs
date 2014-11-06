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

namespace Actual_windows_phone_controller
{
    public partial class CreationPage : PhoneApplicationPage
    {
        public CreationPage()
        {
            InitializeComponent();
        }

        private void toolbarItemSelected(object sender, MouseButtonEventArgs e)
        {
            // Inialize new object
            Rectangle newRectangle = new Rectangle();
            newRectangle.Width = 100;
            newRectangle.Height = 100;
            newRectangle.Fill = new SolidColorBrush(Colors.Blue);
            Point mouseCordinates = e.GetPosition(controllerCanvas);
            //Set object position
            Canvas.SetLeft(newRectangle, mouseCordinates.X - newRectangle.Width / 2);
            Canvas.SetTop(newRectangle, mouseCordinates.Y - newRectangle.Height / 2);
            controllerCanvas.Children.Add(newRectangle);
            //Set event handlers
            mousePreviousPosition = mouseCordinates;
            newRectangle.MouseMove += changePosition;
            newRectangle.ManipulationDelta += changeSize;
            newRectangle.MouseLeftButtonDown += recordPosition;
            newRectangle.CaptureMouse();
        }

        private void changePosition(object sender, MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition(controllerCanvas);
            double originalLeft = Canvas.GetLeft((UIElement)sender);
            double originalTop = Canvas.GetTop((UIElement)sender);
            Canvas.SetLeft((UIElement)sender, originalLeft + mousePosition.X - mousePreviousPosition.X);
            Canvas.SetTop((UIElement)sender, originalTop + mousePosition.Y - mousePreviousPosition.Y);
            mousePreviousPosition = mousePosition;
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
            }
        }
        Point mousePreviousPosition;
        private void recordPosition(object sender, MouseButtonEventArgs e)
        {
            mousePreviousPosition = e.GetPosition(controllerCanvas);
        }
    }
}
