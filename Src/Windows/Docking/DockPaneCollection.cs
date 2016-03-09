using System;
using System.Collections.ObjectModel;

namespace DevZest.Windows.Docking
{
    /// <summary>Represents a collection of <see cref="DockPane"/> objects.</summary>
    public class DockPaneCollection : ReadOnlyObservableCollection<DockPane>
    {
        internal DockPaneCollection()
            : base(new ObservableCollection<DockPane>())
        {
        }

        internal void Add(DockPane item)
        {
            Items.Add(item);
        }

        internal void Insert(int index, DockPane item)
        {
            Items.Insert(index, item);
        }

        internal void Remove(DockPane item)
        {
            Items.Remove(item);
        }
    }
}
