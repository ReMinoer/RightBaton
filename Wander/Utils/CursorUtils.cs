using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Wander.Utils
{
    static public class CursorUtils
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern static private bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern static private bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        extern static private IntPtr WindowFromPoint(POINT point);

        static public Point GetPosition()
        {
            if (!GetCursorPos(out POINT cusorPoint))
                throw new Win32Exception();

            return new Point(cusorPoint.x, cusorPoint.y);
        }

        static public void SetPosition(Point position)
        {
            if (!SetCursorPos(position.X, position.Y))
                throw new Win32Exception();
        }

        static public IntPtr GetWindowHandleAtCursorPosition()
        {
            if (!GetCursorPos(out POINT cusorPoint))
                throw new Win32Exception();

            return WindowFromPoint(cusorPoint);
        }
    }
}