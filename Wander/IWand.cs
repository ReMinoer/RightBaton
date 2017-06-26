namespace Wander
{
    public interface IWand
    {
        string DisplayName { get; }
        SpellNode.Root Root { get; set; }
    }
}