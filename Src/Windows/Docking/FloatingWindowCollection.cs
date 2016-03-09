using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace DevZest.Windows.Docking
{
    /// <summary>Represents a collection of <see cref="FloatingWindow"/> objects.</summary>
    public class FloatingWindowCollection : ReadOnlyCollection<FloatingWindow>, INotifyCollectionChanged
    {
        private int _dockControlZIndex;

        /// <exclude/>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }

        internal FloatingWindowCollection()
            : base(new List<FloatingWindow>())
        {
        }

        private int DockControlZIndex
        {
            get { return _dockControlZIndex; }
            set
            {
                _dockControlZIndex = value;
                Debug.Assert(value >= 0 && value <= Count);
            }
        }

        internal void Add(FloatingWindow item)
        {
            Debug.Assert(item != null);
            Debug.Assert(!Contains(item));
            Items.Insert(DockControlZIndex, item);
            DockControlZIndex++;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        internal void Remove(FloatingWindow item)
        {
            Debug.Assert(item != null);
            int index = IndexOf(item);
            Debug.Assert(index != -1);
            Items.Remove(item);
            if (index < DockControlZIndex)
                DockControlZIndex--;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
        }

        internal void BringToFront(FloatingWindow item)
        {
            if (item == null)
            {
                DockControlZIndex = Count;
                return;
            }

            int oldIndex = Items.IndexOf(item);
            Debug.Assert(oldIndex != -1);
            if (oldIndex != Count - 1)
            {
                Items.Remove(item);
                Items.Add(item);
            }
            if (oldIndex < DockControlZIndex)
                DockControlZIndex--;
            if (oldIndex != Count - 1)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, Items.Count - 1, oldIndex));
        }

        internal IEnumerable<FloatingWindow> GetFloatingWindowsWithDockControl()
        {
            for (int i = 0; i < DockControlZIndex; i++)
                yield return this[i];

            yield return null;

            for (int i = DockControlZIndex; i < Count; i++)
                yield return this[i];
        }
    }
}
