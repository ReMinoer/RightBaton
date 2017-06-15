using System;

namespace Wander.Wpf
{
    public partial class App
    {
        private readonly System.Windows.Forms.NotifyIcon _notifyIcon;
        private readonly WanderEngine _engine;
        private OptionsWindow _optionsWindow;

        public App()
        {
            _notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                Icon = Wpf.Properties.Resources.NotifyIcon,
                Visible = true
            };

            var contextMenu = new System.Windows.Forms.ContextMenu();
            contextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("&Configure", ConfigureItemOnClick));
            contextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("&Exit", ExitItemOnClick));
            _notifyIcon.ContextMenu = contextMenu;

            _notifyIcon.DoubleClick += ConfigureItemOnClick;

            _engine = new WanderEngine();
            _engine.Start();
        }

        private void ConfigureItemOnClick(object sender, EventArgs eventArgs)
        {
            if (_optionsWindow == null)
            {
                _optionsWindow = new OptionsWindow();
                _optionsWindow.Closed += OptionsWindowOnClosed;
            }

            _optionsWindow.Show();
        }

        private void ExitItemOnClick(object sender, EventArgs eventArgs)
        {
            _engine.Stop();
            _notifyIcon.Dispose();

            Current.Shutdown();
        }

        private void OptionsWindowOnClosed(object sender, EventArgs eventArgs)
        {
            _optionsWindow.Closed -= OptionsWindowOnClosed;
            _optionsWindow = null;
        }
    }
}
