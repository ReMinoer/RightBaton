using System.Collections.Generic;

namespace Wander
{
    public interface ISpellNode
    {
        string Description { get; }
        ISpell Spell { get; }
        IEnumerable<ISpellNode> Children { get; }
        ISpellNode this[Orientation orientation] { get; }
        ISpellNode this[IEnumerable<Orientation> orientations] { get; }
        ISpellNode this[params Orientation[] orientations] { get; }
    }
}