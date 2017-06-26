using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Wander.Utils;

// ReSharper disable InconsistentNaming
namespace Wander
{
    public class WanderHooker : IDisposable
    {
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        private const int WH_MOUSE_LL = 14;

        private enum MouseMessages
        {
            WM_MOUSEMOVE = 0x0200,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        extern static private IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern static private bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        extern static private IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        extern static private IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        extern static private int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        private IntPtr _hookId;
        private readonly LowLevelMouseProc _lowLevelMouseProc;
        private IntPtr _targetHandle;
        
        private readonly List<Orientation> _currentOrientations = new List<Orientation>();
        private Point _cursorOrigin;
        private Point _currentOrigin;
        public int DeadZone { get; set; } = 50;

        public event EventHandler<StartingGestureEventArgs> GestureStarting;
        public event TaskEventHandler<StartGestureEventArgs> GestureStarted;
        public event TaskEventHandler<GesturingEventArgs> Gesturing;
        public event TaskEventHandler<OrientationAddedEventArgs> OrientationAdded;
        public event TaskEventHandler<EndGestureEventArgs> GestureEnded;

        public WanderHooker()
        {
            _lowLevelMouseProc = HookCallback;
        }

        public void Hook()
        {
            if (_hookId != IntPtr.Zero)
                return;

            _hookId = SetWindowsHookEx(WH_MOUSE_LL, _lowLevelMouseProc, GetModuleHandle("user32"), 0);
            if (_hookId == IntPtr.Zero)
                throw new Win32Exception();
        }

        public void Unhook()
        {
            if (_hookId == IntPtr.Zero)
                return;

            UnhookWindowsHookEx(_hookId);
            _hookId = IntPtr.Zero;
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                var mouseMessages = (MouseMessages)wParam;
                switch (mouseMessages)
                {
                    case MouseMessages.WM_RBUTTONDOWN:
                    {
                        _targetHandle = CursorUtils.GetWindowHandleAtCursorPosition();
                        if (_targetHandle == IntPtr.Zero)
                            break;

                        InitGesture();
                        return (IntPtr)1;
                    }
                    case MouseMessages.WM_MOUSEMOVE:
                    {
                        if (_targetHandle == IntPtr.Zero)
                            break;

                        UpdateGesture();
                        break;
                    }
                    case MouseMessages.WM_RBUTTONUP:
                    {
                        if (_targetHandle == IntPtr.Zero)
                            break;

                        StopGesture();
                        return (IntPtr)1;
                    }
                }
            }

            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        private void InitGesture()
        {
            var eventArgs = new StartingGestureEventArgs(_targetHandle);
            GestureStarting?.Invoke(this, eventArgs);

            if (!eventArgs.Handled)
            {
                _targetHandle = IntPtr.Zero;
                return;
            }

            _cursorOrigin = _currentOrigin = CursorUtils.GetPosition();
            GestureStarted?.InvokeAsync(this, new StartGestureEventArgs(_cursorOrigin));
        }

        private void UpdateGesture()
        {
            Gesturing?.InvokeAsync(this, new GesturingEventArgs(CursorUtils.GetPosition()));

            Orientation newOrientation;
            Point cursorPosition = CursorUtils.GetPosition();
            Point move = cursorPosition - _currentOrigin;
            bool isHorizontal = Math.Abs(move.X) >= Math.Abs(move.Y);

            if (isHorizontal)
            {
                if (move.X > -DeadZone && move.X < DeadZone)
                    return;

                newOrientation = move.X >= 0 ? Orientation.Right : Orientation.Left;
            }
            else
            {
                if (move.Y > -DeadZone && move.Y < DeadZone)
                    return;

                newOrientation = move.Y >= 0 ? Orientation.Down : Orientation.Up;
            }

            // If move continue in same direction, test other axis
            if (_currentOrientations.Count > 0 && newOrientation == _currentOrientations[_currentOrientations.Count - 1])
            {
                _currentOrigin = isHorizontal ? new Point(cursorPosition.X, _currentOrigin.Y) : new Point(_currentOrigin.X, cursorPosition.Y);

                move = cursorPosition - _currentOrigin;

                if (isHorizontal)
                {
                    if (move.Y > -DeadZone && move.Y < DeadZone)
                        return;

                    newOrientation = move.Y >= 0 ? Orientation.Down : Orientation.Up;
                }
                else
                {
                    if (move.X > -DeadZone && move.X < DeadZone)
                        return;

                    newOrientation = move.X >= 0 ? Orientation.Right : Orientation.Left;
                }
            }

            _currentOrientations.Add(newOrientation);
            _currentOrigin = CursorUtils.GetPosition();

            OrientationAdded?.InvokeAsync(this, new OrientationAddedEventArgs(newOrientation));
        }

        private void StopGesture()
        {
            if (_currentOrientations.Count == 0)
                SendRightClick();
            else
                GestureEnded?.InvokeAsync(this, new EndGestureEventArgs(_currentOrientations.ToArray(), _cursorOrigin));

            _targetHandle = IntPtr.Zero;
            _currentOrientations.Clear();
        }

        private void SendRightClick()
        {
            Point cursorPosition = CursorUtils.GetPosition();
            var rightClickLParam = (IntPtr)((ushort)cursorPosition.X | ((ushort)cursorPosition.Y << 16));
            SendMessage(_targetHandle, (int)MouseMessages.WM_RBUTTONDOWN, IntPtr.Zero, rightClickLParam);
            SendMessage(_targetHandle, (int)MouseMessages.WM_RBUTTONUP, IntPtr.Zero, rightClickLParam);
        }

        public void Dispose()
        {
            if (_hookId != IntPtr.Zero)
                Unhook();
        }
    }

    public class StartingGestureEventArgs : EventArgs
    {
        public IntPtr WindowHandle { get; }
        public bool Handled { get; private set; }

        public StartingGestureEventArgs(IntPtr windowHandle)
        {
            WindowHandle = windowHandle;
        }

        public void Handle()
        {
            Handled = true;
        }
    }

    public class StartGestureEventArgs : EventArgs
    {
        public Point CursorPosition { get; }

        public StartGestureEventArgs(Point cursorPosition)
        {
            CursorPosition = cursorPosition;
        }
    }

    public class GesturingEventArgs : EventArgs
    {
        public Point CursorPosition { get; }

        public GesturingEventArgs(Point cursorPosition)
        {
            CursorPosition = cursorPosition;
        }
    }

    public class OrientationAddedEventArgs : EventArgs
    {
        public Orientation Orientation { get; }

        public OrientationAddedEventArgs(Orientation orientation)
        {
            Orientation = orientation;
        }
    }

    public class EndGestureEventArgs : EventArgs
    {
        public Orientation[] Orientations { get; }
        public Point CursorPosition { get; }

        public EndGestureEventArgs(Orientation[] orientations, Point cursorPosition)
        {
            Orientations = orientations;
            CursorPosition = cursorPosition;
        }
    }
}
