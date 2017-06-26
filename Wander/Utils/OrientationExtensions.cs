using System;
using System.Collections.Generic;
using System.Linq;

namespace Wander.Utils
{
    static public class OrientationExtensions
    {
        static public char ArrowChar(this Orientation orientation)
        {
            switch (orientation)
            {
                case Orientation.Up: return '↑';
                case Orientation.Right: return '→';
                case Orientation.Left: return '←';
                case Orientation.Down: return '↓';
                default: throw new NotSupportedException();
            }
        }

        static public string ArrowString(this IEnumerable<Orientation> orientations)
        {
            return string.Join("", orientations.Select(x => ArrowChar(x)));
        }
    }
}