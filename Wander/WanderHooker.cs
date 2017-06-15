using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming
namespace Wander
{
    public class WanderHooker : IDisposable
    {
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        private const int WH_MOUSE_LL = 14;

        private enum MouseMessages
        {
            //WM_LBUTTONDOWN = 0x0201,
            //WM_LBUTTONUP = 0x0202,
            //WM_MOUSEMOVE = 0x0200,
            //WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }

        //[StructLayout(LayoutKind.Sequential)]
        //private struct POINT
        //{
        //    public int x;
        //    public int y;
        //}

        //[StructLayout(LayoutKind.Sequential)]
        //private struct MSLLHOOKSTRUCT
        //{
        //    public POINT pt;
        //    public uint mouseData;
        //    public uint flags;
        //    public uint time;
        //    public IntPtr dwExtraInfo;
        //}

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        extern static private IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern static private bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        extern static private IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        extern static private IntPtr GetModuleHandle(string lpModuleName);

        private readonly LowLevelMouseProc _lowLevelMouseProc;
        public IntPtr HookId { get; private set; }
        public event EventHandler<HandledEventArgs> StartGesture;
        public event EventHandler<HandledEventArgs> EndGesture;
        
        public WanderHooker()
        {
            _lowLevelMouseProc = HookCallback;
        }

        public void Hook()
        {
            HookId = SetWindowsHookEx(WH_MOUSE_LL, _lowLevelMouseProc, GetModuleHandle("user32"), 0);
            if (HookId == IntPtr.Zero)
                throw new Win32Exception();
        }

        public void Unhook()
        {
            UnhookWindowsHookEx(HookId);
            HookId = IntPtr.Zero;
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
                        var eventArgs = new HandledEventArgs();
                        StartGesture?.Invoke(this, eventArgs);
                        if (eventArgs.Handled)
                            return (IntPtr)1;
                        break;
                    }
                    case MouseMessages.WM_RBUTTONUP:
                    {
                        var eventArgs = new HandledEventArgs();
                        EndGesture?.Invoke(this, eventArgs);
                        if (eventArgs.Handled)
                            return (IntPtr)1;
                        break;
                    }
                }
            }

            return CallNextHookEx(HookId, nCode, wParam, lParam);
        }

        public void Dispose()
        {
            if (HookId != IntPtr.Zero)
                Unhook();
        }
    }
}
