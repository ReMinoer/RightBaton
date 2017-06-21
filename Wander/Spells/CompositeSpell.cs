using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wander.Spells
{
    public class CompositeSpell : ISpell, ICollection<ISpell>
    {
        private readonly List<ISpell> _list = new List<ISpell>();
        public string Description { get; set; }
        public int Count => _list.Count;
        public bool IsReadOnly => false;

        public CompositeSpell()
        {
        }

        public CompositeSpell(string description)
        {
            Description = description;
        }

        public async Task Cast(SpellContext context)
        {
            foreach (ISpell spell in _list)
                await spell.Cast(context);
        }

        public void Add(ISpell item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(ISpell item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(ISpell[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(ISpell item)
        {
            return _list.Remove(item);
        }

        public IEnumerator<ISpell> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }
    }
}