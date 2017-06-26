using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Wander.Wpf.Models;

namespace Wander.Wpf
{
    public class OptionWindowModel : INotifyPropertyChanged
    {
        public WanderEngine Engine { get; }

        private IWand _selectedWand;
        public IWand SelectedWand
        {
            get => _selectedWand;
            set => SetProperty(ref _selectedWand, value);
        }

        private SpellNodeItemModel _selectedSpellNode;
        public SpellNodeItemModel SelectedSpellNode
        {
            get => _selectedSpellNode;
            set => SetProperty(ref _selectedSpellNode, value);
        }

        public OptionWindowModel(WanderEngine engine)
        {
            Engine = engine;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private bool SetProperty<T>(ref T currentValue, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
                return false;

            currentValue = newValue;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}