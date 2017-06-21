using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WindowsInput.Native;
using Wander.Utils;

namespace Wander.Spells
{
    public enum MouseButton
    {
        Left,
        Right
    }

    public class MouseButtonSpell : ISpell
    {
        public string Description { get; set; }
        public MouseButton Button { get; }
        public IEnumerable<VirtualKeyCode> ModifierKeys { get; }
        public bool MoveCursorBackToGestureOrigin { get; set; }

        public MouseButtonSpell(MouseButton button, params VirtualKeyCode[] modifierKeys)
        {
            Button = button;
            ModifierKeys = modifierKeys;
        }

        public MouseButtonSpell(string description, MouseButton button, params VirtualKeyCode[] modifierKeys)
            : this(button, modifierKeys)
        {
            Description = description;
        }

        public Task Cast(SpellContext context)
        {
            Point cursorPosition = CursorUtils.GetPosition();

            if (MoveCursorBackToGestureOrigin)
                CursorUtils.SetPosition(context.CursorOrigin);

            foreach (VirtualKeyCode modifierKey in ModifierKeys)
                WanderEngine.InputSimulator.Keyboard.KeyDown(modifierKey);

            switch (Button)
            {
                case MouseButton.Left:
                    WanderEngine.InputSimulator.Mouse.LeftButtonClick();
                    break;
                case MouseButton.Right:
                    WanderEngine.InputSimulator.Mouse.RightButtonClick();
                    break;
                default:
                    throw new NotSupportedException();
            }

            foreach (VirtualKeyCode modifierKey in ModifierKeys)
                WanderEngine.InputSimulator.Keyboard.KeyUp(modifierKey);

            if (MoveCursorBackToGestureOrigin)
                CursorUtils.SetPosition(cursorPosition);

            return Task.CompletedTask;
        }
    }
}