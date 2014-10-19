using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

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

}
