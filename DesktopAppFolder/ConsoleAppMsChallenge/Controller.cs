using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
//using iTunesLib;

namespace CMD
{
        public class Controller
        {
            public static void act(String command)
            {
                try
                {
                    switch (command)
                    {
                        case "PC Control Panel":          //Settings
                            SettingsControl.openControlPanel();
                            break;
                        case "PC Task Manager":
                            SettingsControl.openTaskManager();
                            break;
                        case "PC Shutdown":
                            SettingsControl.pcShutdown();
                            break;
                        case "PC Restart":
                            SettingsControl.pcRestart();
                            break;
                        case "PC Logoff":
                            SettingsControl.pcLogOff();
                            break;
                        case "PC Lock":
                            SettingsControl.pcLock();
                            break;
                        case "PC Hibernate":
                            SettingsControl.pcHibernate();
                            break;
                        case "PC Sleep":
                            SettingsControl.pcStandby();
                            break;
                        case "Open Notepad":                //Applications
                            ApplicationControl.openProgram(ApplicationControl.appNotepad);
                            break;
                        case "Open iTunes":
                            ApplicationControl.openProgram(ApplicationControl.appItunes);
                            break;
                        case "Open Steam":
                            ApplicationControl.openProgram(ApplicationControl.appSteam);
                            break;
                        case "Open Internet Explorer":
                            ApplicationControl.openProgram(ApplicationControl.appInternetExplorer);
                            break;
                        case "Open File Explorer":
                            ApplicationControl.openProgram(ApplicationControl.appFileExplorer);
                            break;
                        case "Open Microsoft Word":
                            ApplicationControl.openProgram(ApplicationControl.appMSWord);
                            break;
                        case "Open Microsoft PowerPoint":
                            ApplicationControl.openProgram(ApplicationControl.appMSPowerPoint);
                            break;
                        case "Open Microsoft Excel":
                            ApplicationControl.openProgram(ApplicationControl.appMSExcel);
                            break;
                        case "View Web Browser":            //Web Control
                            WebControl.openBrowser();
                            break;
                        case "View Google":
                            WebControl.openBrowser(WebControl.appWebGoogle);
                            break;
                        case "View YouTube":
                            WebControl.openBrowser(WebControl.appWebYouTube);
                            break;
                        case "View Facebook":
                            WebControl.openBrowser(WebControl.appWebFacebook);
                            break;
                        case "View Twitter":
                            WebControl.openBrowser(WebControl.appWebTwitter);
                            break;
                        case "Browse Desktop":                //Directory Control
                            DirectoryControl.openDirectory(DirectoryControl.dirDesktop);
                            break;
                        case "Browse Computer":
                            DirectoryControl.openDirectory(DirectoryControl.dirComputer);
                            break;
                        case "Browse Documents":
                            DirectoryControl.openDirectory(DirectoryControl.dirDocuments);
                            break;
                        case "Browse Music":
                            DirectoryControl.openDirectory(DirectoryControl.dirMusic);
                            break;
                        case "Browse Pictures":
                            DirectoryControl.openDirectory(DirectoryControl.dirPictures);
                            break;
                        case "Browse Videos":
                            DirectoryControl.openDirectory(DirectoryControl.dirVideos);
                            break;
                        case "Browse Program Files":
                            DirectoryControl.openDirectory(DirectoryControl.dirProgramFiles);
                            break;
                        case "Browse Program Files (x86)":
                            DirectoryControl.openDirectory(DirectoryControl.dirProgramFilesX86);
                            break;
                        case "Play Song":                   //iTunes Control
                            //iTunesControl.play();
                            break;
                        case "Pause Song":
                            //iTunesControl.pause();
                            break;
                        case "Stop Song":
                            //iTunesControl.stop();
                            break;
                        case "Next Song":
                            //iTunesControl.next();
                            break;
                        case "Previous Song":
                            //iTunesControl.previous();
                            break;
                    }
                }
                catch (Exception e)
                {
                    return;
                }
            }
        }
        
        /**
         * Utility for controlling the volume.
         * 
         * Does REQUIRE a form handle.
         */
        public class VolumeControl
        {
            private const int APPCOMMAND_VOLUME_MUTE = 0x80000;
            private const int APPCOMMAND_VOLUME_UP = 0xA0000;
            private const int APPCOMMAND_VOLUME_DOWN = 0x90000;
            private const int WM_APPCOMMAND = 0x319;
            private const int WPARAM = 0xffff;


            /**
             * Imported function to control volume
             * @param hWnd: a handle to the window whose window procedure will receive the message
             * @param Msg: message to be sent
             * @param wParam: additional message-specific information
             * @param lParam: additional message-specific information
             * 
             * Reference:
             * http://msdn.microsoft.com/en-us/library/ms644950(VS.85).aspx
             */
            [DllImport("user32.dll")]
            private static extern IntPtr SendMessageW(
                IntPtr hWnd,
                int Msg,
                IntPtr wParam,
                IntPtr lParam
            );

            /*static extern short VkKeyScan(char ch);

            static public System.Windows.Input.Key ResolveKey(char charToResolve)
            {
                return System.Windows.Input.KeyInterop.KeyFromVirtualKey(VkKeyScan(charToResolve));
            }*/

            /**
             * Increases the volume (by 2 units).
             * @oaram handle: handle to the window whose window will receive the message
             * 
             * ex:  
             * Form x;
             * increase_volume(x.Handle);
             */
            public static void increase_volume(IntPtr handle)
            {
                SendMessageW(handle, WM_APPCOMMAND, (IntPtr)WPARAM, (IntPtr)APPCOMMAND_VOLUME_UP);
            }

            
            /*
             * Decreases the volume (by 2 units).
             * @parm handle: handle to the window whose window will receive the message
             * 
             * ex:  
             * Form x;
             * decrease_volume(x.Handle);
             */
            public static void decrease_volume(IntPtr handle)
            {
                SendMessageW(handle, WM_APPCOMMAND, (IntPtr)WPARAM, (IntPtr)APPCOMMAND_VOLUME_DOWN);
            }


            /*
             * Mute the volume.
             * @param handle: handle to the window whose window will receive the message
             * 
             * ex:  
             * Form x;
             * mute_volume(x.Handle);
             */
            public static void mute_volume(IntPtr handle)
            {
                SendMessageW(handle, WM_APPCOMMAND, (IntPtr)WPARAM, (IntPtr)APPCOMMAND_VOLUME_MUTE);
            }
        }

        /**
         * Utility for controlling the mouse.
         * 
         * The top left of the screen is (0,0). All units are in pixels.
         * 
         * Does NOT require a form handle.
         */
        public class MouseControl
        {

            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

            private const int MOUSEEVENTF_LEFTDOWN = 0x02;
            private const int MOUSEEVENTF_LEFTUP = 0x04;
            private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
            private const int MOUSEEVENTF_RIGHTUP = 0x10;

            /**
             * Imported function to set the position of the mouse cursor
             * @param X: x position of the mouse
             * @param Y: y position of the mouse
             */
            [DllImport("User32.dll")]
            private static extern bool SetCursorPos(int X, int Y);

            /**
             * Returns the x position of the mouse (pixels).
             */
            public static int getMouseX()
            {
                return Cursor.Position.X;
            }


            /**
             * Returns the x position of the mouse (pixels).
             */
            public static int getMouseY()
            {
                return Cursor.Position.Y;
            }


            /**
             * Sets the position of the mouse.
             * @param x, the x position of the mouse (pixels)
             * @param y, the y position of the mouse (pixels)
             */
            public static void setMouse(int x, int y)
            {
                SetCursorPos(x, y);
            }


            /**
             * Moves the mouse from the current position by the given units.
             * @param x, the offset from the current position in the x-direction (pixels)
             * @param y, the offset from the current position in the y-direction (pixels)
             */
            public static void moveMouse(int x, int y)
            {
                setMouse(getMouseX() + x, getMouseY() + y);
            }

            //sends a click event to the mouse at its current position
             public static void clickEvent(bool hold, bool right)
            {
                int X = Cursor.Position.X;
                int Y = Cursor.Position.Y;

                //assume left click
                int mouseClickEvent = MOUSEEVENTF_LEFTDOWN;
                int mouseReleaseEvent = MOUSEEVENTF_LEFTUP;

                if(right)
                {
                    mouseClickEvent = MOUSEEVENTF_RIGHTDOWN;
                    mouseReleaseEvent = MOUSEEVENTF_RIGHTUP;
                }

                mouse_event(mouseClickEvent, X, Y, 0, 0);
                
                if(!hold)
                {
                    mouse_event(mouseReleaseEvent, X, Y, 0, 0);
                }

            }
        }

        /**
         * Utility for emulating key presses.
         * 
         * Does NOT require a form handle.
         */
        public class KeyControl
        {
            /**
             * Sample constant to send the <Enter> Key
             */   
            public static String ENTER = "{ENTER}";


            /**
             * Acts as a virtual keyboard, as if the 'key' was pressed.
             * @param key: the key to emulate being pressed
             */
            public static void send(String key)
            {
                SendKeys.Send(key);
            }


            /**
             * Acts as a virtual keyboard, as if the 'key' was pressed and waits.
             * @param key: the key to emulate being pressed
             */
            public static void sendWait(String key)
            {
                SendKeys.SendWait(key);
            }

           
       }

        /**
         * Utility for settings management of the computer.
         * 
         * For example, opening the control panel or shutting down the PC.
         * 
         * Does NOT require a form handle.
         */
        public class SettingsControl
        {
            /**
             * Imported function to suspend (sleep or hibernate) the computer.
             */
            [DllImport("user32")]
            public static extern bool ExitWindowsEx(uint uFlags, uint dwReason);

            /**
             * Imported function to lock the computer.
             */
            [DllImport("user32")]
            public static extern void LockWorkStation();

            /**
             * Opens the control panel as a new process.
             */
            public static void openControlPanel()
            {
                Process.Start("control");
            }

            /**
             * Opens the task manager as a new process.
             */
            public static void openTaskManager()
            {
                Process.Start("taskmgr");
            }

            /**
             * Shuts off the computer.
             */
            public static void pcShutdown()
            {
                Process.Start("shutdown", "/s /t 0");
            }

            /**
             * Restarts the computer.
             */
            public static void pcRestart()
            {
                Process.Start("shutdown", "/r /t 0");
            }

            /**
             * Logs off the current user of the computer.
             */
            public static void pcLogOff()
            {
                ExitWindowsEx(0, 0);
            }

            /**
             * Locks the computer.
             */
            public static void pcLock()
            {
                LockWorkStation();
            }

            /**
             * Hibernates the computer.
             */
            public static void pcHibernate()
            {
                Application.SetSuspendState(PowerState.Hibernate, true, true);
            }

            /**
             * Puts the computer in standby (sleep).
             */
            public static void pcStandby()
            {
                Application.SetSuspendState(PowerState.Suspend, true, true);
            }
        }

        /**
         * Utility for interacting with applications.
         * 
         * Does NOT require a form handle.
         */
        public class ApplicationControl
        {
            //Common Programs
            public static String appNotepad = "notepad.exe";
            public static String appItunes = "itunes.exe";
            public static String appSteam = "steam.exe";

            //Standard Windows Programs
            public static String appInternetExplorer = "iexplore.exe";
            public static String appFileExplorer = "explorer.exe";

            //Microsoft Office
            public static String appMSWord = "winword.exe";
            public static String appMSPowerPoint = "powerpnt.exe";
            public static String appMSExcel = "excel.exe";

            /**
             * Opens the program from the specified path.
             * Will return true if the open was successful,
             * false otherwise.
             * 
             * @param path: file location or file name
             */
            public static bool openProgram(String path)
            {
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = path;
                return openProcess(info);
            }

            /**
             * Opens the program from the specified path with the given command
             * line arguments.
             * Will return true if the open was successful,
             * false otherwise.
             * 
             * @param path: file location or file name
             * @param args: command line arguments of the program
             */
            public static bool openProgram(String path, String args)
            {
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = path;
                info.Arguments = args;
                return openProcess(info);
            }

            /**
             * Runs the process, and returns true if successful, otherwise false.
             * The process could be opening an application (Microsoft Word) or
             * opening a file (bankStatement.docx).
             * 
             * @param info: start information for the process, such as
             *              the file path or command line arguments
             */
            public static bool openProcess(ProcessStartInfo info)
            {
                try
                {
                    Process.Start(info);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /**
         * Utility for interacting with the web.
         * 
         * Does NOT require a form handle.
         */
        public class WebControl
        {
            //Websites
            //---------------------------------------------------------------------
            public static String appWebBrowser = "http://www.microsoft.com";
            public static String appWebGoogle = "http://www.google.com";
            public static String appWebGoogleSearch = "http://google.com/search?q=";

            public static String appWebYouTube = "http://www.youtube.com";
            public static String appWebFacebook = "http://www.facebook.com";
            public static String appWebTwitter = "http://www.twitter.com";
            //---------------------------------------------------------------------


            //Browser
            //---------------------------------------------------------------------
            /**
             * Opens the default browser.
             */
            public static bool openBrowser()
            {
                return openBrowser(appWebGoogle);
            }

            /**
             * Opens the url in the default browser.
             * 
             * @param url: website url to open
             */
            public static bool openBrowser(string url)
            {
                try
                {
                    Process.Start(url);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            /**
             * Searches google with the given query.
             * Returns true if successful, otherwise false.
             * 
             * @param query: google query phrase to search for 
             *               example: "Why is the sky blue?"
             */
            public static bool searchGoogle(string query)
            {
                try
                {
                    Process.Start(appWebGoogleSearch + query);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            //---------------------------------------------------------------------
        }

        /**
         * Utility for interacting with file directories.
         * 
         * Does NOT require a form handle.
         */
        public class DirectoryControl
        {
            //Default Locations
            //---------------------------------------------------------------------
            public static string dirDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            public static string dirComputer = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            public static string dirDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            public static string dirMusic = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            public static string dirPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            public static string dirVideos = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            public static string dirProgramFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            public static string dirProgramFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            //---------------------------------------------------------------------

            //Directory Control
            //---------------------------------------------------------------------
            /**
             * Opens the file location in a file explorer browser.
             * Returns true if operation successful, otherwise false.
             * 
             * @param path: file location
             */
            public static bool openDirectory(string path)
            {
                try
                {
                    string windir = Environment.GetEnvironmentVariable("WINDIR");
                    Process prc = new Process();
                    prc.StartInfo.FileName = windir + @"/explorer.exe";
                    prc.StartInfo.Arguments = path;
                    prc.Start();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            //---------------------------------------------------------------------
        }

        /**
         * Utility for interacting with iTunes.
         * 
         * Does NOT require a form handle.
        // */
        //public class iTunesControl
        //{
        //    private delegate void Router(object arg);

        //    private static iTunesApp app = null;

        //    /**
        //     * Ensures iTunes is opened.
        //     */
        //    public static void itunesOpen()
        //    {
        //        app = new iTunesApp();
        //    }

        //    /**
        //     * Skips to the next song in iTunes.
        //     */
        //    public static void next()
        //    {
        //        if (app == null) itunesOpen();
        //        app.NextTrack();
        //    }

        //    /**
        //     * Skips to the previous song in iTunes.
        //     */
        //    public static void previous()
        //    {
        //        if (app == null) itunesOpen();
        //        app.PreviousTrack();
        //    }

        //    /**
        //     * Plays a song in iTunes.
        //     */
        //    public static void play()
        //    {
        //        if (app == null) itunesOpen();
        //        app.Play();
        //    }

        //    /**
        //     * Pauses a playing song in iTunes.
        //     */
        //    public static void pause()
        //    {
        //        if (app == null) itunesOpen();
        //        app.Pause();
        //    }

        //    /**
        //     * Stops a playing song in iTunes.
        //     */
        //    public static void stop()
        //    {
        //        if (app == null) itunesOpen();
        //        app.Stop();
        //    }
        //}

        /**
         * TODO: Goals
         * 
         * Useful Nircmd Commands:	
         *  speak				(reads text-file outloud)
         *  setprimarydisplay	(switch monitors)
         */
    }
