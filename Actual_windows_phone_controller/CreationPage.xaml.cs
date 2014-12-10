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
using System.Windows.Media.Imaging;
using System.Windows.Controls.Primitives;

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
            Network.isEditing = true;
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
                    controllerTitle.Text = (DataContext as ControllerViewModel).Title;
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
                //BitmapImage bitmapImage = new BitmapImage();
                //bitmapImage.UriSource = new Uri("ms-appx:///Assets/Images/trash2.png");
                garbageCan.Fill = new SolidColorBrush(Colors.Red);
                //garbageCan.Source = bitmapImage;
            }
            else 
            {
                //BitmapImage bitmapImage = new BitmapImage();
                //bitmapImage.UriSource = new Uri("ms-appx:///Assets/Images/trash.png");
                garbageCan.Fill = new SolidColorBrush(Colors.Gray);
                //garbageCan.Source = bitmapImage;
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
            //button.x = controllerCanvas.ActualWidth / 4 - button.width / 2;
            //button.y = controllerCanvas.ActualHeight / 4 + button.height;
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

        // Web Browser Buttons
        private void stringControllerSelected(String title, MouseButtonEventArgs e)
        {
            AbstractControllerButton button = new StringControllerButton();
            button.DisplayTitle = title;
            initalizeVisualElement(button, e);
        }
        private void viewBrowserItemSelected(object sender, MouseButtonEventArgs e)
        {
            stringControllerSelected("View Web Browser", e);
        }
        private void viewGoogleItemSelected(object sender, MouseButtonEventArgs e)
        {
            stringControllerSelected("View Google", e);
        }
        private void viewYouTubeItemSelected(object sender, MouseButtonEventArgs e)
        {
            stringControllerSelected("View YouTube", e);
        }
        private void viewFacebookItemSelected(object sender, MouseButtonEventArgs e)
        {
            stringControllerSelected("View Facebook", e);
        }
        private void viewTwitterItemSelected(object sender, MouseButtonEventArgs e)
        {
            stringControllerSelected("View Twitter", e);
        }
        private void pcShutdownItemSelected(object sender, MouseButtonEventArgs e)
        {
            stringControllerSelected("PC Shutdown", e);
        }
        private void pcSleepItemSelected(object sender, MouseButtonEventArgs e)
        {
            stringControllerSelected("PC Sleep", e);
        }
        private void openiTunesItemSelected(object sender, MouseButtonEventArgs e)
        {
            stringControllerSelected("Open iTunes", e);
        }
        private void openSteamItemSelected(object sender, MouseButtonEventArgs e)
        {
            stringControllerSelected("Open Steam", e);
        }
        private void openIEItemSelected(object sender, MouseButtonEventArgs e)
        {
            stringControllerSelected("Open Internet Explorer", e);
        }
        private void openFileExplorerItemSelected(object sender, MouseButtonEventArgs e)
        {
            stringControllerSelected("Open File Explorer", e);
        }
        private void openMSWordItemSelected(object sender, MouseButtonEventArgs e)
        {
            stringControllerSelected("Open Microsoft Word", e);
        }
        private void openMSPPTItemSelected(object sender, MouseButtonEventArgs e)
        {
            stringControllerSelected("Open Microsoft PowerPoint", e);
        }
        private void browseDocumentsItemSelected(object sender, MouseButtonEventArgs e)
        {
            stringControllerSelected("Browse Documents", e);
        }
        private void browseMusicItemSelected(object sender, MouseButtonEventArgs e)
        {
            stringControllerSelected("Browse Music", e);
        }
        private void browsePicturesItemSelected(object sender, MouseButtonEventArgs e)
        {
            stringControllerSelected("Browse Pictures", e);
        }

        private void macroItemSelected(object sender, MouseButtonEventArgs e)
        {

            Popup popup = new Popup();
            popup.Height = this.Height;
            popup.Width = this.Width;
            popup.VerticalOffset = 0;
            PopupBox control = new PopupBox();
            control.Height = Application.Current.Host.Content.ActualHeight;
            control.Width = Application.Current.Host.Content.ActualWidth;
            popup.Child = control;
            popup.IsOpen = true;

            control.btnOK.Click += (s, args) =>
            {
                popup.IsOpen = false;

                AbstractControllerButton button = new MacroControllerButton();
                button.DisplayTitle = control.tbx.Text;
                initalizeVisualElement(button, e);
            };

            control.btnCancel.Click += (s, args) =>
            {
                popup.IsOpen = false;
            };
        }

        // Handler Selector Sliding
        private void ButtonSelectorSelected(object sender, MouseButtonEventArgs e)
        {
            previousSelectorSelectionPosition = e.GetPosition(null);
        }
        private void ButtonSelectorMoved(object sender, MouseEventArgs e)
        {
            ScrollViewer selectorScroller = (ScrollViewer)sender;
            Double newXposition = Canvas.GetLeft(selectorScroller) + e.GetPosition(null).X - previousSelectorSelectionPosition.X;
            Double minX = contentControl.ActualWidth / 2 - selectorScroller.ActualWidth;
            Double maxX = contentControl.ActualWidth/2 - 30;
            
            if (newXposition < minX)
            {
                Canvas.SetLeft(selectorScroller, minX);
            }
            else if (newXposition > maxX)
            {
                Canvas.SetLeft(selectorScroller, maxX);
            }
            else
            {
                Canvas.SetLeft(selectorScroller, newXposition);
            }
            previousSelectorSelectionPosition = e.GetPosition(null);
        }

        private void viewVolumeItemSelected(object sender, MouseButtonEventArgs e)
        {
            // Inialize new data object
            AbstractControllerButton button = AbstractControllerButton.ButtonFactory(ButtonType.Volume);
            initalizeVisualElement(button, e);
        }

        private void TitleGotFocus(object sender, RoutedEventArgs e)
        {
            SolidColorBrush blackBrush = new SolidColorBrush(Colors.Black);
            (sender as TextBox).Foreground = blackBrush;
        }

        private void TitleLostFocus(object sender, RoutedEventArgs e)
        {
            SolidColorBrush whiteBrush = new SolidColorBrush(Colors.White);
            (sender as TextBox).Foreground = whiteBrush;
        }

        double buttonHoldTime = .25;

        DateTime toolbarStarted;
        private void startToolbar(object sender, MouseButtonEventArgs e)
        {
            toolbarStarted = DateTime.Now;
        }

        private void endToolbar(object sender, MouseButtonEventArgs e)
        {
            if (DateTime.Now - toolbarStarted > TimeSpan.FromSeconds(buttonHoldTime))
            {
                AbstractControllerButton button = AbstractControllerButton.ButtonFactory(ButtonType.Button);
                initalizeVisualElement(button, e);
            }
        }

        DateTime mouseStarted;
        private void startMouse(object sender, MouseButtonEventArgs e)
        {
            mouseStarted = DateTime.Now;
        }

        private void endMouse(object sender, MouseButtonEventArgs e)
        {
            if (DateTime.Now - mouseStarted > TimeSpan.FromSeconds(buttonHoldTime))
            {
                //MouseItemSelected(sender, e);
                AbstractControllerButton button = new MouseControllerButton();
                initalizeVisualElement(button, e);
            }
        }

        DateTime keyboardStarted;
        private void startKB(object sender, MouseButtonEventArgs e)
        {
            keyboardStarted = DateTime.Now;
        }

        private void endKB(object sender, MouseButtonEventArgs e)
        {
            if (DateTime.Now - keyboardStarted > TimeSpan.FromSeconds(buttonHoldTime))
            {
                AbstractControllerButton button = new KeyboardControllerButton();
                initalizeVisualElement(button, e);
            }
        }
        
        DateTime browserStarted;
        private void startBrowser(object sender, MouseButtonEventArgs e)
        {
            browserStarted = DateTime.Now;
        }

        private void endBrowser(object sender, MouseButtonEventArgs e)
        {
            if (DateTime.Now - browserStarted > TimeSpan.FromSeconds(buttonHoldTime))
            {
                stringControllerSelected("View Web Browser", e);
            }
        }

        DateTime ytStarted;
        private void startYT(object sender, MouseButtonEventArgs e)
        {
            ytStarted = DateTime.Now;
        }

        private void endYT(object sender, MouseButtonEventArgs e)
        {
            if (DateTime.Now - ytStarted > TimeSpan.FromSeconds(buttonHoldTime))
            {
                stringControllerSelected("View YouTube", e);
            }
        }

        DateTime googleStarted;
        private void startGoogle(object sender, MouseButtonEventArgs e)
        {
            googleStarted = DateTime.Now;
        }

        private void endGoogle(object sender, MouseButtonEventArgs e)
        {
            if (DateTime.Now - googleStarted > TimeSpan.FromSeconds(buttonHoldTime))
            {
                stringControllerSelected("View Google", e);
            }
        }

        DateTime fbStarted;
        private void startFB(object sender, MouseButtonEventArgs e)
        {
            fbStarted = DateTime.Now;
        }

        private void endFB(object sender, MouseButtonEventArgs e)
        {
            if (DateTime.Now - fbStarted > TimeSpan.FromSeconds(buttonHoldTime))
            {
                stringControllerSelected("View Facebook", e);
            }
        }
        
        DateTime twitStarted;
        private void startTwit(object sender, MouseButtonEventArgs e)
        {
            twitStarted = DateTime.Now;
        }

        private void endTwit(object sender, MouseButtonEventArgs e)
        {
            if (DateTime.Now - twitStarted > TimeSpan.FromSeconds(buttonHoldTime))
            {
                stringControllerSelected("View Twitter", e);
            }
        }
        
        DateTime pcStarted;
        private void startPC(object sender, MouseButtonEventArgs e)
        {
            pcStarted = DateTime.Now;
        }

        private void endPC(object sender, MouseButtonEventArgs e)
        {
            if (DateTime.Now - pcStarted > TimeSpan.FromSeconds(buttonHoldTime))
            {
                stringControllerSelected("PC Shutdown", e);
            }
        }
        
        DateTime sleepStarted;
        private void startSleep(object sender, MouseButtonEventArgs e)
        {
            sleepStarted = DateTime.Now;
        }

        private void endSleep(object sender, MouseButtonEventArgs e)
        {
            if (DateTime.Now - sleepStarted > TimeSpan.FromSeconds(buttonHoldTime))
            {
                stringControllerSelected("PC Sleep", e);
            }
        }
        
        DateTime itunesStarted;
        private void startItunes(object sender, MouseButtonEventArgs e)
        {
            itunesStarted = DateTime.Now;
        }

        private void endItunes(object sender, MouseButtonEventArgs e)
        {
            if (DateTime.Now - itunesStarted > TimeSpan.FromSeconds(buttonHoldTime))
            {
                stringControllerSelected("Open iTunes", e);
            }
        }

        DateTime steamStarted;
        private void startSteam(object sender, MouseButtonEventArgs e)
        {
            steamStarted = DateTime.Now;
        }

        private void endSteam(object sender, MouseButtonEventArgs e)
        {
            if (DateTime.Now - steamStarted > TimeSpan.FromSeconds(buttonHoldTime))
            {
                stringControllerSelected("Open Steam", e);
            }
        }

        DateTime ieStarted;
        private void startIE(object sender, MouseButtonEventArgs e)
        {
            ieStarted = DateTime.Now;
        }

        private void endIE(object sender, MouseButtonEventArgs e)
        {
            if (DateTime.Now - ieStarted > TimeSpan.FromSeconds(buttonHoldTime))
            {
                stringControllerSelected("Open Internet Explorer", e);
            }
        }

        DateTime feStarted;
        private void startFE(object sender, MouseButtonEventArgs e)
        {
            feStarted = DateTime.Now;
        }

        private void endFE(object sender, MouseButtonEventArgs e)
        {
            if (DateTime.Now - feStarted > TimeSpan.FromSeconds(buttonHoldTime))
            {
                stringControllerSelected("Open File Explorer", e);
            }
        }
        
        DateTime wordStarted;
        private void startWord(object sender, MouseButtonEventArgs e)
        {
            wordStarted = DateTime.Now;
        }

        private void endWord(object sender, MouseButtonEventArgs e)
        {
            if (DateTime.Now - wordStarted > TimeSpan.FromSeconds(buttonHoldTime))
            {
                stringControllerSelected("Open Microsoft Word", e);
            }
        }
        
        DateTime ppStarted;
        private void startPP(object sender, MouseButtonEventArgs e)
        {
            ppStarted = DateTime.Now;
        }

        private void endPP(object sender, MouseButtonEventArgs e)
        {
            if (DateTime.Now - ppStarted > TimeSpan.FromSeconds(buttonHoldTime))
            {
                stringControllerSelected("Open Microsoft PowerPoint", e);
            }
        }

        DateTime docsStarted;
        private void startDocs(object sender, MouseButtonEventArgs e)
        {
            docsStarted = DateTime.Now;
        }

        private void endDocs(object sender, MouseButtonEventArgs e)
        {
            if (DateTime.Now - docsStarted > TimeSpan.FromSeconds(buttonHoldTime))
            {
                stringControllerSelected("Browse Documents", e);
            }
        }

        DateTime musicStarted;
        private void startMusic(object sender, MouseButtonEventArgs e)
        {
            musicStarted = DateTime.Now;
        }

        private void endMusic(object sender, MouseButtonEventArgs e)
        {
            if (DateTime.Now - musicStarted > TimeSpan.FromSeconds(buttonHoldTime))
            {
                stringControllerSelected("Browse Music", e);
            }
        }

        DateTime picsStarted;
        private void startPics(object sender, MouseButtonEventArgs e)
        {
            picsStarted = DateTime.Now;
        }

        private void endPics(object sender, MouseButtonEventArgs e)
        {
            if (DateTime.Now - picsStarted > TimeSpan.FromSeconds(buttonHoldTime))
            {
                stringControllerSelected("Browse Pictures", e);
            }
        }

        DateTime volumeStarted;
        private void startVol(object sender, MouseButtonEventArgs e)
        {
            volumeStarted = DateTime.Now;
        }

        private void endVol(object sender, MouseButtonEventArgs e)
        {
            if (DateTime.Now - volumeStarted > TimeSpan.FromSeconds(buttonHoldTime))
            {
                AbstractControllerButton button = AbstractControllerButton.ButtonFactory(ButtonType.Volume);
                initalizeVisualElement(button, e);
            }
        }

        DateTime macroStarted;
        private void startMacro(object sender, MouseButtonEventArgs e)
        {
            macroStarted = DateTime.Now;
        }

        private void endMacro(object sender, MouseButtonEventArgs e)
        {
            if (DateTime.Now - macroStarted > TimeSpan.FromSeconds(buttonHoldTime))
            {
                Popup popup = new Popup();
                popup.Height = this.Height;
                popup.Width = this.Width;
                popup.VerticalOffset = 0;
                PopupBox control = new PopupBox();
                control.Height = Application.Current.Host.Content.ActualHeight;
                control.Width = Application.Current.Host.Content.ActualWidth;
                popup.Child = control;
                popup.IsOpen = true;

                control.btnOK.Click += (s, args) =>
                {
                    popup.IsOpen = false;

                    AbstractControllerButton button = new MacroControllerButton();
                    button.DisplayTitle = control.tbx.Text;
                    initalizeVisualElement(button, e);
                };

                control.btnCancel.Click += (s, args) =>
                {
                    popup.IsOpen = false;
                };
            }
        }

        private void TitleTextChanged(object sender, TextChangedEventArgs e)
        {
            ((ControllerViewModel)DataContext).Title = ((TextBox)sender).Text;
            ((ControllerViewModel)DataContext).Save();
        }
    }
}
