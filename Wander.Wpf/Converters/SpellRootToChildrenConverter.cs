using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Wander.Wpf.Models;

namespace Wander.Wpf.Converters
{
    public class SpellRootToChildrenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ISpellNode spellNode)
                return Convert(spellNode, new Stack<Orientation>());
            return Binding.DoNothing;
        }

        static private IEnumerable<SpellNodeItemModel> Convert(ISpellNode spellNode, Stack<Orientation> orientations)
        {
            var children = new List<SpellNodeItemModel>();

            SpellNodeItemModel result;
            if (ConvertChild(spellNode, Orientation.Up, orientations, children, out result))
                yield return result;
            if (ConvertChild(spellNode, Orientation.Right, orientations, children, out result))
                yield return result;
            if (ConvertChild(spellNode, Orientation.Down, orientations, children, out result))
                yield return result;
            if (ConvertChild(spellNode, Orientation.Left, orientations, children, out result))
                yield return result;

            foreach (SpellNodeItemModel model in children)
                yield return model;
        }

        static private bool ConvertChild(ISpellNode spellNode, Orientation orientation, Stack<Orientation> orientations, List<SpellNodeItemModel> children, out SpellNodeItemModel result)
        {
            ISpellNode child = spellNode[orientation];
            if (child != null)
            {
                orientations.Push(orientation);
                result = child.Spell != null ? new SpellNodeItemModel(child, orientations.Reverse().ToArray()) : null;
                children.AddRange(Convert(child, orientations));
                orientations.Pop();
                return result != null;
            }

            result = null;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}