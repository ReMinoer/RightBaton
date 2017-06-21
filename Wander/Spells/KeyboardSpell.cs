using System.Collections.Generic;
using System.Threading.Tasks;
using WindowsInput.Native;
using Wander.Utils;

namespace Wander.Spells
{
    public class KeyboardSpell : ISpell
    {
        public string Description { get; set; }
        public IEnumerable<VirtualKeyCode> Keys { get; }
        public IEnumerable<VirtualKeyCode> ModifierKeys { get; }

        public KeyboardSpell(VirtualKeyCode key, params VirtualKeyCode[] modifierKeys)
            : this(new []{key}, modifierKeys)
        {
        }

        public KeyboardSpell(IEnumerable<VirtualKeyCode> keys, IEnumerable<VirtualKeyCode> modifierKeys)
        {
            ModifierKeys = modifierKeys;
            Keys = keys;
        }

        public KeyboardSpell(string description, VirtualKeyCode key, params VirtualKeyCode[] modifierKeys)
            : this(key, modifierKeys)
        {
            Description = description;
        }

        public KeyboardSpell(string description, IEnumerable<VirtualKeyCode> keys, IEnumerable<VirtualKeyCode> modifierKeys)
            : this(keys, modifierKeys)
        {
            Description = description;
        }

        public Task Cast(SpellContext context)
        {
            WanderEngine.InputSimulator.Keyboard.ModifiedKeyStroke(ModifierKeys, Keys);
            return Task.CompletedTask;
        }
    }
}