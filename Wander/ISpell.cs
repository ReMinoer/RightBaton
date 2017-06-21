using System.Threading.Tasks;

namespace Wander
{
    public interface ISpell
    {
        string Description { get; }
        Task Cast(SpellContext context);
    }
}