using System;
using System.Collections.Generic;
using System.Linq;

namespace Wander
{
    public abstract class SpellNode : ISpellNode
    {
        private string _description;
        public string Description
        {
            get => _description ?? Spell.Description;
            set => _description = value;
        }

        public ISpell Spell { get; set; }
        public abstract ISpellNode this[Orientation orientation] { get; }
        public ISpellNode this[IEnumerable<Orientation> orientations] => orientations.Aggregate<Orientation, ISpellNode>(this, (current, orientation) => current?[orientation]);
        public ISpellNode this[params Orientation[] orientations] => this[(IEnumerable<Orientation>)orientations];
        public abstract IEnumerable<ISpellNode> Children { get; }

        protected SpellNode()
        {
        }

        protected SpellNode(ISpell spell)
        {
            Spell = spell;
        }

        protected SpellNode(string description, ISpell spell)
            : this(spell)
        {
            Description = description;
        }

        public class Root : ISpellNode
        {
            public Up Up { get; set; }
            public Right Right { get; set; }
            public Down Down { get; set; }
            public Left Left { get; set; }
            public string Description => "Root node";
            ISpell ISpellNode.Spell => null;

            public IEnumerable<ISpellNode> Children
            {
                get
                {
                    if (Up != null)
                        yield return Up;
                    if (Right != null)
                        yield return Right;
                    if (Down != null)
                        yield return Down;
                    if (Left != null)
                        yield return Left;
                }
            }

            public ISpellNode this[Orientation orientation]
            {
                get
                {
                    switch (orientation)
                    {
                        case Orientation.Up: return Up;
                        case Orientation.Right: return Right;
                        case Orientation.Down: return Down;
                        case Orientation.Left: return Left;
                        default: throw new NotSupportedException();
                    }
                }
            }

            public ISpellNode this[IEnumerable<Orientation> orientations] => orientations.Aggregate<Orientation, ISpellNode>(this, (current, orientation) => current?[orientation]);
            public ISpellNode this[params Orientation[] orientations] => this[(IEnumerable<Orientation>)orientations];
        }

        public class Up : SpellNode
        {
            public Right Right { get; set; }
            public Down Down { get; set; }
            public Left Left { get; set; }

            public override IEnumerable<ISpellNode> Children
            {
                get
                {
                    if (Right != null)
                        yield return Right;
                    if (Down != null)
                        yield return Down;
                    if (Left != null)
                        yield return Left;
                }
            }

            public Up()
            {
            }

            public Up(ISpell spell)
                : base(spell)
            {
            }

            public Up(string description, ISpell spell)
                : base(description, spell)
            {
            }

            public override ISpellNode this[Orientation orientation]
            {
                get
                {
                    switch (orientation)
                    {
                        case Orientation.Up: return null;
                        case Orientation.Right: return Right;
                        case Orientation.Down: return Down;
                        case Orientation.Left: return Left;
                        default: throw new NotSupportedException();
                    }
                }
            }
        }

        public class Right : SpellNode
        {
            public Up Up { get; set; }
            public Down Down { get; set; }
            public Left Left { get; set; }

            public override IEnumerable<ISpellNode> Children
            {
                get
                {
                    if (Up != null)
                        yield return Up;
                    if (Down != null)
                        yield return Down;
                    if (Left != null)
                        yield return Left;
                }
            }

            public Right()
            {
            }

            public Right(ISpell spell)
                : base(spell)
            {
            }

            public Right(string description, ISpell spell)
                : base(description, spell)
            {
            }

            public override ISpellNode this[Orientation orientation]
            {
                get
                {
                    switch (orientation)
                    {
                        case Orientation.Up: return Up;
                        case Orientation.Right: return null;
                        case Orientation.Down: return Down;
                        case Orientation.Left: return Left;
                        default: throw new NotSupportedException();
                    }
                }
            }
        }

        public class Down : SpellNode
        {
            public Up Up { get; set; }
            public Right Right { get; set; }
            public Left Left { get; set; }

            public override IEnumerable<ISpellNode> Children
            {
                get
                {
                    if (Up != null)
                        yield return Up;
                    if (Right != null)
                        yield return Right;
                    if (Left != null)
                        yield return Left;
                }
            }

            public Down()
            {
            }

            public Down(ISpell spell)
                : base(spell)
            {
            }

            public Down(string description, ISpell spell)
                : base(description, spell)
            {
            }

            public override ISpellNode this[Orientation orientation]
            {
                get
                {
                    switch (orientation)
                    {
                        case Orientation.Up: return Up;
                        case Orientation.Right: return Right;
                        case Orientation.Down: return null;
                        case Orientation.Left: return Left;
                        default: throw new NotSupportedException();
                    }
                }
            }
        }

        public class Left : SpellNode
        {
            public Up Up { get; set; }
            public Right Right { get; set; }
            public Down Down { get; set; }

            public override IEnumerable<ISpellNode> Children
            {
                get
                {
                    if (Up != null)
                        yield return Up;
                    if (Right != null)
                        yield return Right;
                    if (Down != null)
                        yield return Down;
                }
            }

            public Left()
            {
            }

            public Left(ISpell spell)
                : base(spell)
            {
            }

            public Left(string description, ISpell spell)
                : base(description, spell)
            {
            }

            public override ISpellNode this[Orientation orientation]
            {
                get
                {
                    switch (orientation)
                    {
                        case Orientation.Up: return Up;
                        case Orientation.Right: return Right;
                        case Orientation.Down: return Down;
                        case Orientation.Left: return null;
                        default: throw new NotSupportedException();
                    }
                }
            }
        }
    }
}