namespace Wander
{
    public class Wand : IWand
    {
        public string DisplayName { get; set; }
        public string ProcessName { get; set; }
        public SpellNode.Root Root { get; set; }

        public Wand(string displayName, string processName)
        {
            DisplayName = displayName;
            ProcessName = processName;
        }
    }
}