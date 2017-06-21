namespace Wander
{
    public interface IWand
    {
        string DisplayName { get; }
        string ProcessName { get; }
        SpellNode.Root Root { get; set; }
    }
}