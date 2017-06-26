using System;
using WindowsInput.Native;
using Wander.Spells;
using Wander.Wands;

namespace Wander.Wpf
{
    public partial class App : IDisposable
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

            var defaultWandRoot = new SpellNode.Root
            {
                Left = new SpellNode.Left
                {
                    Up = new SpellNode.Up(new KeyboardSpell("Zoom +", VirtualKeyCode.ADD, VirtualKeyCode.CONTROL)),
                    Down = new SpellNode.Down(new KeyboardSpell("Zoom -", VirtualKeyCode.SUBTRACT, VirtualKeyCode.CONTROL))
                },
                Right = new SpellNode.Right
                {
                    Up = new SpellNode.Up(new KeyboardSpell("Home", VirtualKeyCode.HOME, VirtualKeyCode.CONTROL)),
                    Down = new SpellNode.Down(new KeyboardSpell("End", VirtualKeyCode.END, VirtualKeyCode.CONTROL))
                }
            };

            var firefoxWand = new ProcessWand("Firefox", "firefox")
            {
                Root = new SpellNode.Root
                {
                    Up = new SpellNode.Up
                    {
                        Left = new SpellNode.Left(new KeyboardSpell("Close tab", VirtualKeyCode.VK_W, VirtualKeyCode.CONTROL))
                        {
                            Down = new SpellNode.Down(new KeyboardSpell("Close others", VirtualKeyCode.VK_W, VirtualKeyCode.LMENU))
                            {
                                Left = new SpellNode.Left(new KeyboardSpell("Close left", VirtualKeyCode.VK_W, VirtualKeyCode.SHIFT, VirtualKeyCode.LMENU)),
                                Right = new SpellNode.Right(new KeyboardSpell("Close right", VirtualKeyCode.VK_W, VirtualKeyCode.CONTROL, VirtualKeyCode.LMENU))
                            }
                        },
                        Right = new SpellNode.Right(new KeyboardSpell("New tab", VirtualKeyCode.VK_T, VirtualKeyCode.CONTROL))
                    },
                    Down = new SpellNode.Down(new MouseButtonSpell("Open in new tab", MouseButton.Left, VirtualKeyCode.CONTROL) { MoveCursorBackToGestureOrigin = true })
                    {
                        Left = new SpellNode.Left(new KeyboardSpell("Previous", VirtualKeyCode.LEFT, VirtualKeyCode.LMENU)),
                        Right = new SpellNode.Right(new KeyboardSpell("Next", VirtualKeyCode.RIGHT, VirtualKeyCode.LMENU))
                    },
                    Left = new SpellNode.Left(new KeyboardSpell("Previous tab", VirtualKeyCode.TAB, VirtualKeyCode.CONTROL, VirtualKeyCode.SHIFT)),
                    Right = new SpellNode.Right(new KeyboardSpell("Next tab", VirtualKeyCode.TAB, VirtualKeyCode.CONTROL))
                }
            };

            _engine = new WanderEngine();
            _engine.DefaultWand.Root = defaultWandRoot;
            _engine.ProcessWands.Add(firefoxWand);
            _engine.Start();
        }

        private void ConfigureItemOnClick(object sender, EventArgs eventArgs)
        {
            _engine.Pause();

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

            _engine.Resume();
        }

        public void Dispose()
        {
            _notifyIcon?.Dispose();
            _engine?.Dispose();
        }
    }
}
