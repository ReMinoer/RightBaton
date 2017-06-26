using Wander.Utils;

namespace Wander.Wpf.Models
{
    public class SpellNodeItemModel
    {
        public ISpellNode SpellNode { get; }
        public Orientation[] Orientations { get; }
        public string OrientationsArrows => Orientations.ArrowString();

        public SpellNodeItemModel(ISpellNode spellNode, Orientation[] orientations)
        {
            SpellNode = spellNode;
            Orientations = orientations;
        }
    }
}