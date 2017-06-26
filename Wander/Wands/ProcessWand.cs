namespace Wander.Wands
{
    public class ProcessWand : IWand
    {
        public string DisplayName { get; set; }
        public string ProcessName { get; set; }
        public SpellNode.Root Root { get; set; }

        public ProcessWand()
        {
        }

        public ProcessWand(string displayName, string processName)
        {
            DisplayName = displayName;
            ProcessName = processName;
        }
    }
}