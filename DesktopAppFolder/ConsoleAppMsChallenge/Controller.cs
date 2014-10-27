﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CMD
{
    
        /**
         * Utility for controlling the volume.
         * 
         * Does REQUIRE a form handle.
         */
        public class Controller
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
            //Applications that can be opened
            //---------------------------------------------------------------------
            //Common Programs
            public static String appNotepad = "notepad.exe";
            public static String appItunes = "itunes.exe";
            public static String appSteam = "steam.exe";

            //Standard Windows Programs
            public static String appInternetExplorer = "iexplore.exe";
            public static String appFileExplorer = "explorer.exe";
            public static String appWebBrowser = "http://www.microsoft.com";
            public static String appWebGoogle = "http://www.google.com";

            //Microsoft Office
            public static String appMSWord = "winword.exe";
            public static String appMSPowerPoint = "powerpnt.exe";
            public static String appMSExcel = "excel.exe";
            //---------------------------------------------------------------------

            //Useful Keywords
            //---------------------------------------------------------------------
            public static String appWebSearch = "http://google.com/serach?q=";
            //---------------------------------------------------------------------

            //Command line arguments associated with applications
            //---------------------------------------------------------------------
            public static String appSteam_argBigPicture = "-bigpicture";
            //---------------------------------------------------------------------

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
                catch (Exception e)
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
                    Process.Start(appWebSearch + query);
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }

}
