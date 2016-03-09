using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DevZest.Windows.Docking
{
    /// <summary>Represents a collection of <see cref="DockItem"/> objects.</summary>
    public sealed class DockItemCollection : ReadOnlyObservableCollection<DockItem>, IList
    {
        private DockControl _dockControl;

        internal DockItemCollection()
            : this(null)
        {
        }

        internal DockItemCollection(DockControl dockControl)
            : base(new ObservableCollection<DockItem>())
        {
            _dockControl = dockControl;
        }

        private DockControl DockControl
        {
            get { return _dockControl; }
        }

        internal void AddInternal(DockItem item)
        {
            Debug.Assert(!Items.Contains(item));
            Insert(Count, item);
        }

        internal void Insert(int index, DockItem item)
        {
            Items.Insert(index, item);
            if (DockControl != null)
                DockControl.OnItemAdded(item);
        }

        internal void Remove(DockItem item)
        {
            Debug.Assert(Items.Contains(item));
            Items.Remove(item);
            if (DockControl != null)
                DockControl.OnItemRemoved(item);
        }

        // IList.Add cannot be explicitly implemented, otherwise it will not work in
        // Expression Blend
        /// <exclude />
        public int Add(object value)
        {
            if (DockControl == null)
                throw new NotSupportedException();

            if (!DesignerProperties.GetIsInDesignMode(DockControl) && DockControl.IsLoaded)
                throw new NotSupportedException();

            if (DockControl.IsLoaded)
                return -1;

            DockItem item = value as DockItem;
            if (item == null || Contains(item))
                throw new ArgumentException(SR.Exception_DockItemCollection_AddItem_InvalidValue, "value");
            this.AddInternal(item);
            return  Items.Count - 1;
        }

        void IList.Clear()
        {
            // Expression Blend will always call IList.Clear(), throwing exception
            // will break Expression Blend XAML designer
            if (!DesignerProperties.GetIsInDesignMode(_dockControl))
                throw new NotSupportedException();
        }

        // Required by Expression Blend
        bool IList.IsFixedSize
        {
            get { return false; }
        }

        // Required by Expression Blend
        bool IList.IsReadOnly
        {
            get { return false; }
        }
    }
}
