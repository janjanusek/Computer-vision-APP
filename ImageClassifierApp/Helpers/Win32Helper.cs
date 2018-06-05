using System;
using System.Runtime.InteropServices;

namespace ImageClassifierApp.Helpers
{
    /// <summary>
    /// Code of this class was originally presented at https://stackoverflow.com/questions/15852769/i-need-to-activate-the-main-window-of-my-only-one-instance-wpf-app-when-another
    /// </summary>
    public static class Win32Helper
    {
        //Import the FindWindow API to find our window
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindowNative(string className, string windowName);
        //Import the SetForeground API to activate it
        [DllImport("User32.dll", EntryPoint = "SetForegroundWindow")]
        private static extern IntPtr SetForegroundWindowNative(IntPtr hWnd);

        public static IntPtr FindWindow(string className, string windowName)
        {
            try
            {
                return FindWindowNative(className, windowName);
            }
            catch (Exception)
            {
                return IntPtr.Zero;
                // ignored
            }
        }

        public static IntPtr SetForegroundWindow(IntPtr hWnd)
        {
            try
            {
                return SetForegroundWindowNative(hWnd);
            }
            catch (Exception)
            {
                return IntPtr.Zero;
                // ignored
            }
        }

        public static void Activate(string paTitle)
        {
            try
            {
                //Find the window, using the Window Title
                IntPtr hWnd = Win32Helper.FindWindow(null, paTitle);
                if (hWnd.ToInt32() > 0) //If found
                {
                    Win32Helper.SetForegroundWindow(hWnd); //Activate it
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

    }
}