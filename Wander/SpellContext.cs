namespace Wander
{
    public class SpellContext
    {
        public Point CursorOrigin { get; }

        public SpellContext(Point cursorOrigin)
        {
            CursorOrigin = cursorOrigin;
        }
    }
}