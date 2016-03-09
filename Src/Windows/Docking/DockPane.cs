using System;
using System.Collections.Generic;
using System.Windows;
using System.Diagnostics;

namespace DevZest.Windows.Docking
{
    /// <summary>Represents a container for a collection of <see cref="DockItem"/> objects.</summary>
    public sealed class DockPane : DockPaneNode
    {
        private static readonly DependencyPropertyKey CountOfVisibleItemsPropertyKey = DependencyProperty.RegisterReadOnly("CountOfVisibleItems", typeof(int), typeof(DockPane),
            new FrameworkPropertyMetadata(null));
        private static readonly DependencyPropertyKey SelectedItemPropertyKey = DependencyProperty.RegisterReadOnly("SelectedItem", typeof(DockItem), typeof(DockPane),
                new FrameworkPropertyMetadata(null));
        /// <summary>Identifies the <see cref="CountOfVisibleItems"/> dependency property.</summary>
        public static readonly DependencyProperty CountOfVisibleItemsProperty = CountOfVisibleItemsPropertyKey.DependencyProperty;
        /// <summary>Identifies the <see cref="SelectedItem"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedItemProperty = SelectedItemPropertyKey.DependencyProperty;

        private DockItemCollection _items = new DockItemCollection();
        private DockItemCollection _visibleItems = new DockItemCollection();
        private DockItemCollection _selectedItems = new DockItemCollection();
        private DockItemCollection _activeItems = new DockItemCollection();
        private int _selectedItemIndex = -1;
        private bool _isAutoHide;

        internal DockPane(DockItem item, bool isAutoHide)
        {
            IsAutoHide = isAutoHide;
            AddItem(item);
        }

        internal override DockPosition GetDockPosition()
        {
            DockTreePosition? dockTreePosition = DockTreePosition;

            if (Items.Count == 0)
                return DockPosition.Unknown;
            else if (VisibleItems.Count == 0)
                return DockPosition.Hidden;

            return DockPositionHelper.GetDockPosition(dockTreePosition, IsAutoHide);
        }

        /// <summary>Gets a collection of <see cref="DockItem"/> objects contained by this <see cref="DockPane"/>.</summary>
        /// <value>A collection of <see cref="DockItem"/> objects contained by this <see cref="DockPane"/>.</value>
        public DockItemCollection Items
        {
            get { return _items; }
        }

        /// <summary>Gets a collection of visible <see cref="DockItem"/> objects contained by this <see cref="DockPane"/>.</summary>
        /// <value>A collection of visible <see cref="DockItem"/> objects contained by this <see cref="DockPane"/>.</value>
        public DockItemCollection VisibleItems
        {
            get { return _visibleItems; }
        }

        /// <summary>Gets a collection of active <see cref="DockItem"/> objects contained by this <see cref="DockPane"/>.</summary>
        /// <value>A collection of active <see cref="DockItem"/> objects contained by this <see cref="DockPane"/>, 
        /// in order of activation (last active last).</value>
        public DockItemCollection ActiveItems
        {
            get { return _activeItems; }
        }

        internal bool IsAutoHide
        {
            get { return _isAutoHide; }
            set { _isAutoHide = value; }
        }

        /// <summary>Gets the currently selected <see cref="DockItem"/>. This is a dependency property.</summary>
        /// <value>The currently selected <see cref="DockItem"/>.</value>
        public DockItem SelectedItem
        {
            get { return (DockItem)GetValue(SelectedItemProperty); }
            internal set
            {
                Debug.Assert(value == null || _visibleItems.Contains(value));
                DockItem oldValue = SelectedItem;
                if (oldValue == value)
                {
                    if (value != null)
                        value.IsSelected = !IsAutoHide;
                    return;
                }

                SetValue(SelectedItemPropertyKey, value);

                if (oldValue != null && oldValue.FirstPane == this)
                    oldValue.IsSelected = false;

                if (value != null)
                {
                    if (_selectedItems.Contains(value))
                        _selectedItems.Remove(value);
                    _selectedItems.AddInternal(value);
                    value.IsSelected = !IsAutoHide;
                    _selectedItemIndex = _visibleItems.IndexOf(value);
                    UpdateActiveItems();
                }
                else
                    _selectedItemIndex = -1;
            }
        }

        internal void CoerceSelectedItem()
        {
            if (IsAutoHide)
                SelectedItem = null;
            else if (_selectedItems.Count > 0)
                SelectedItem = _selectedItems[_selectedItems.Count - 1];
            else if (_visibleItems.Count == 0)
                SelectedItem = null;
            else
            {
                int index = _selectedItemIndex;
                if (index < 0)
                    index = 0;
                else if (index >= _visibleItems.Count)
                    index = _visibleItems.Count - 1;
                SelectedItem = _visibleItems[index];
            }
        }

        /// <summary>Gets the number of visible <see cref="DockItem"/> objects. This is a dependency property.</summary>
        /// <value>The number of visible <see cref="DockItem"/> objects.</value>
        public int CountOfVisibleItems
        {
            get { return (int)GetValue(CountOfVisibleItemsProperty); }
            private set { SetValue(CountOfVisibleItemsPropertyKey, value); }
        }

        internal void AddItem(DockItem item)
        {
            Items.Insert(Items.Count, item);
        }

        internal void InsertItem(int index, DockItem item)
        {
            Debug.Assert(item != null);
            Debug.Assert(!Items.Contains(item));
            Items.Insert(index, item);
        }

        internal void RemoveItem(DockItem item, bool removePane)
        {
            Debug.Assert(Items.Contains(item));
            Debug.Assert(DockTree != null);

            DockTree.AddDirtyNode(this);
            Items.Remove(item);
            if (Items.Count == 0 && removePane)
                DockTree.RemovePane(this);
        }

        private void UpdateVisibleItems()
        {
            CollectionUtil.Synchronize(_items,
                delegate(DockItem item) { return TestItemVisibility(item); },
                _visibleItems,
                delegate(int index, DockItem item) { InsertVisibleItem(index, item); },
                delegate(DockItem item) { RemoveVisibleItem(item); });

            CountOfVisibleItems = _visibleItems.Count;
        }

        private void UpdateSelectedItems()
        {
            for (int i = _selectedItems.Count - 1; i >= 0; i--)
            {
                DockItem item = _selectedItems[i];
                if (!_visibleItems.Contains(item))
                    _selectedItems.Remove(item);
            }
        }

        private void UpdateActiveItems()
        {
            CollectionUtil.Synchronize(GetActiveItems(),
                _activeItems,
                delegate(int index, DockItem item) { _activeItems.Insert(index, item); },
                delegate(DockItem item) { _activeItems.Remove(item); });

            Debug.Assert(_activeItems.Count == _visibleItems.Count);
        }

        private IEnumerable<DockItem> GetActiveItems()
        {
            foreach (DockItem item in _items)
            {
                if (_visibleItems.Contains(item) && !_selectedItems.Contains(item))
                    yield return item;
            }

            foreach (DockItem item in _selectedItems)
            {
                Debug.Assert(_visibleItems.Contains(item));
                yield return item;
            }
        }

        private bool TestItemVisibility(DockItem item)
        {
            Debug.Assert(Items.Contains(item));
            return (!item.IsHidden && item.FirstPane == this);
        }

        private void InsertVisibleItem(int index, DockItem item)
        {
            Debug.Assert(!_visibleItems.Contains(item));
            if (index < _selectedItemIndex)
                _selectedItemIndex++;
            _visibleItems.Insert(index, item);
        }

        private void RemoveVisibleItem(DockItem item)
        {
            Debug.Assert(_visibleItems.Contains(item));
            int index = _visibleItems.IndexOf(item);
            if (index < _selectedItemIndex)
                _selectedItemIndex--;
            _visibleItems.Remove(item);
        }

        internal override void RefreshStateCore()
        {
            UpdateVisibleItems();
            UpdateSelectedItems();
            UpdateActiveItems();
            base.RefreshStateCore();
        }
    }
}
