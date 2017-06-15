using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using WindowsInput;
using WindowsInput.Native;

namespace Wander
{
    public class WanderEngine : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern static private bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        extern static private uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern static private bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        extern static private IntPtr WindowFromPoint(POINT point);

        private WanderHooker _hooker;
        private IntPtr _targetHandle;
        private IInputSimulator _inputSimulator;

        public IEnumerable<string> ProcessNames
        {
            get
            {
                yield return "firefox";
            }
        }

        public void Start()
        {
            _inputSimulator = new InputSimulator();

            _hooker = new WanderHooker();
            _hooker.StartGesture += HookerOnStartGesture;
            _hooker.EndGesture += HookerOnEndGesture;
            _hooker.Hook();
        }

        public void Stop()
        {
            _hooker.Unhook();
            _hooker.Dispose();
            _hooker = null;
        }

        private void HookerOnStartGesture(object sender, HandledEventArgs handledEventArgs)
        {
            _targetHandle = GetWindowHandleAtCursorPosition();
            if (_targetHandle == IntPtr.Zero)
                return;

            if (!IsHandlable(_targetHandle))
            {
                _targetHandle = IntPtr.Zero;
                return;
            }
            
            handledEventArgs.Handled = true;
        }

        private void HookerOnEndGesture(object sender, HandledEventArgs handledEventArgs)
        {
            if (_targetHandle == IntPtr.Zero)
                return;

            SetForegroundWindow(_targetHandle);
            _targetHandle = IntPtr.Zero;

            _inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_N);
            handledEventArgs.Handled = true;
        }

        private IntPtr GetWindowHandleAtCursorPosition()
        {
            bool success = GetCursorPos(out POINT cusorPoint);
            if (!success)
                throw new Win32Exception();

            return WindowFromPoint(cusorPoint);
        }

        private bool IsHandlable(IntPtr handle)
        {
            GetWindowThreadProcessId(handle, out uint processId);
            Process process = Process.GetProcessById((int)processId);
            return ProcessNames.Contains(process.ProcessName);
        }

        public void Dispose()
        {
            _hooker?.Dispose();
        }
    }
}