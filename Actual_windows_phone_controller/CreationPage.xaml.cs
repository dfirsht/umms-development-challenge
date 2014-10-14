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
        private TranslateTransform move = new TranslateTransform();
        private ScaleTransform resize = new ScaleTransform();
        private TransformGroup rectangleTransforms = new TransformGroup();

        public CreationPage()
        {
            InitializeComponent();
            // Combine the moving and resizing tranforms into one TransformGroup.
            // The rectangle's RenderTransform can only contain a single transform or TransformGroup.
            rectangleTransforms.Children.Add(move);
            rectangleTransforms.Children.Add(resize);
            rectangle.RenderTransform = rectangleTransforms;

            // Handle manipulation events.
            rectangle.ManipulationDelta +=
                new EventHandler<ManipulationDeltaEventArgs>(Rectangle_ManipulationDelta);
        }

        void Rectangle_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            // Move the rectangle.
            move.X += e.DeltaManipulation.Translation.X;
            move.Y += e.DeltaManipulation.Translation.Y;

            // Resize the rectangle.
            if (e.DeltaManipulation.Scale.X > 0 && e.DeltaManipulation.Scale.Y > 0)
            {
                // Scale the rectangle.
                resize.ScaleX *= e.DeltaManipulation.Scale.X;
                resize.ScaleY *= e.DeltaManipulation.Scale.Y;
            }
        }
    }
}
