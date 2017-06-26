namespace Wander.Wands
{
    public class DefaultWand : IWand
    {
        public string DisplayName => "Default Wand";
        public SpellNode.Root Root { get; set; }
    }
}