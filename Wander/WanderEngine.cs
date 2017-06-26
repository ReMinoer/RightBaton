using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WindowsInput;
using Wander.Wands;

namespace Wander
{
    public class WanderEngine : INotifyPropertyChanged, IDisposable
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern static private bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        extern static private uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        static private IInputSimulator _inputSimulator;
        static public IInputSimulator InputSimulator => _inputSimulator ?? (_inputSimulator = new InputSimulator());

        private WanderHooker _hooker;

        private readonly object _lock = new object();
        private readonly Dictionary<uint, (Process process, string name)> _processesCache = new Dictionary<uint, (Process, string)>();
        private IntPtr _windowHandle;
        private IWand _wand;

        private bool _useDefaultWand = true;
        public bool UseDefaultWand
        {
            get => _useDefaultWand;
            set
            {
                if (_useDefaultWand == value)
                    return;

                _useDefaultWand = value;
                OnPropertyChanged();
            }
        }

        public DefaultWand DefaultWand { get; } = new DefaultWand();
        public ObservableCollection<ProcessWand> ProcessWands { get; } = new ObservableCollection<ProcessWand>();

        public event TaskEventHandler<StartGestureEventArgs> GestureStarted;
        public event TaskEventHandler<GesturingEventArgs> Gesturing;
        public event TaskEventHandler<OrientationAddedEventArgs> OrientationAdded;
        public event TaskEventHandler<EndGestureEventArgs> GestureEnded;

        public void Start()
        {
            _hooker = new WanderHooker();
            _hooker.GestureStarting += HookerOnGestureStarting;
            _hooker.GestureStarted += HookerOnGestureStarted;
            _hooker.Gesturing += HookerOnGesturing;
            _hooker.OrientationAdded += HookerOnOrientationAdded;
            _hooker.GestureEnded += HookerOnGestureEnded;
            _hooker.Hook();
        }

        public void Stop()
        {
            _hooker.Dispose();
            _hooker = null;
        }

        public void Pause()
        {
            _hooker.Unhook();
        }

        public void Resume()
        {
            _hooker.Hook();
        }

        private void HookerOnGestureStarting(object sender, StartingGestureEventArgs e)
        {
            GetWindowThreadProcessId(e.WindowHandle, out uint processId);
            
            if (!_processesCache.TryGetValue(processId, out (Process process, string name) processTuple))
            {
                Process process = Process.GetProcessById((int)processId);
                process.EnableRaisingEvents = true;
                process.Exited += ProcessOnExited;
                
                _processesCache.Add(processId, (process: process, name: process.ProcessName));
            }

            lock (_lock)
            {
                _wand = ProcessWands.FirstOrDefault(x => x.ProcessName == processTuple.name);
                if (!UseDefaultWand && _wand == null)
                    return;

                _windowHandle = e.WindowHandle;
            }

            e.Handle();
        }

        private void ProcessOnExited(object sender, EventArgs eventArgs)
        {
            KeyValuePair<uint, (Process process, string name)> pair = _processesCache.First(x => x.Value.process == (Process)sender);

            pair.Value.process.Exited -= ProcessOnExited;
            _processesCache.Remove(pair.Key);
        }

        private async Task HookerOnGestureStarted(object sender, StartGestureEventArgs e)
        {
            if (GestureStarted != null)
                await GestureStarted.InvokeAsync(this, e);
        }

        private async Task HookerOnGesturing(object sender, GesturingEventArgs e)
        {
            if (Gesturing != null)
                await Gesturing.InvokeAsync(this, e);
        }

        private async Task HookerOnOrientationAdded(object sender, OrientationAddedEventArgs e)
        {
            if (OrientationAdded != null)
                await OrientationAdded.InvokeAsync(this, e);
        }

        private async Task HookerOnGestureEnded(object sender, EndGestureEventArgs e)
        {
            ISpell spell = _wand?.Root[e.Orientations].Spell;
            if (spell == null && UseDefaultWand)
                spell = DefaultWand.Root[e.Orientations].Spell;

            if (spell != null)
            {
                var context = new SpellContext(e.CursorPosition);

                SetForegroundWindow(_windowHandle);
                await spell.Cast(context);

                if (GestureEnded != null)
                    await GestureEnded.InvokeAsync(this, e);
            }

            lock (_lock)
            {
                _windowHandle = IntPtr.Zero;
                _wand = null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            _hooker?.Dispose();
        }
    }
}