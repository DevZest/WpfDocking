using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Arranges the document tabs.</summary>
    /// <remarks>Use <see cref="DocumentTabPanel"/> as <see cref="ItemsControl.ItemsPanel"/> of <see cref="DocumentTabStrip"/>.</remarks>
    public class DocumentTabPanel : Panel
    {
        private struct TabData
        {
            public UIElement UIElement;
            public bool IsVisible;
            public double X;
            public double OriginWidth;
            public double FinalWidth;
        }

        private int _selectedIndex, _selectedOffset, _firstVisible;
        private static TabData[] s_emptyTabs = new TabData[0];
        private TabData[] _tabs = s_emptyTabs;

        private bool RefreshSelectedIndex()
        {
            int newValue = IndexOfSelectedItem;

            if (_selectedIndex != newValue)
            {
                _selectedIndex = newValue;
                _selectedOffset = Math.Max(newValue - _firstVisible, 0);
                return true;
            }
            return false;
        }

        private int IndexOfSelectedItem
        {
            get
            {
                int index = 0;
                foreach (UIElement element in InternalChildren)
                {
                    DocumentTab documentTab = element as DocumentTab;
                    if (documentTab != null && documentTab.DockItem.IsSelected)
                        return index;
                    index++;
                }
                return -1;
            }
        }

        /// <exclude/>
        protected override Size MeasureOverride(Size availableSize)
        {
            RefreshSelectedIndex();
            Size size = CalculateWidthes(availableSize);

            foreach (TabData tab in _tabs)
            {
                // FIX: Shouldn't Measure child element with FinalWidth - when child element width exceeds FinalWidth,
                // it will not invalidate this panel's measure
                //tab.UIElement.Measure(new Size(tab.FinalWidth, size.Height));
                tab.UIElement.Visibility = tab.IsVisible ? Visibility.Visible : Visibility.Hidden;
            }

            return size;
        }

        /// <exclude/>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double totalWidth = 0;
            _firstVisible = -1;
            for (int index = 0; index < _tabs.Length; index++)
            {
                TabData tab = _tabs[index];
                Rect rect = new Rect(new Point(tab.X, 0), new Size(tab.FinalWidth, finalSize.Height));
                tab.UIElement.Arrange(rect);
                DocumentTab documentTab = tab.UIElement as DocumentTab;
                if (documentTab != null)
                    documentTab.IsContentTrimmed = tab.OriginWidth > tab.FinalWidth;
                totalWidth += tab.FinalWidth;
                if (tab.IsVisible && _firstVisible == -1)
                    _firstVisible = index;
            }
            if (_firstVisible == -1)
                _firstVisible = 0;

            DocumentTabStrip tabStrip = ItemsControl.GetItemsOwner(this) as DocumentTabStrip;
            if (tabStrip != null)
                tabStrip.IsTabTrimmed = totalWidth > finalSize.Width;

            return finalSize;
        }

        private Size CalculateWidthes(Size availableSize)
        {
            Size totalSize = MeasureTotalSize(availableSize.Height);

            int countTabs = _tabs.Length;
            if (countTabs > 0)
            {
                if (_selectedIndex < 0)
                    _selectedIndex = 0;
                else if (_selectedIndex >= countTabs)
                    _selectedIndex = countTabs;

                if (_selectedOffset > _selectedIndex)
                    _selectedOffset = _selectedIndex;

                double availableWidth = availableSize.Width;
                int index = _selectedIndex;
                _tabs[index].X = 0;
                _tabs[index].IsVisible = true;
                _tabs[index].FinalWidth = Math.Min(availableWidth, _tabs[index].OriginWidth);
                DocumentTab documentTab = _tabs[index].UIElement as DocumentTab;
                if (documentTab != null)
                    documentTab.IsContentTrimmed = _tabs[index].OriginWidth > _tabs[index].FinalWidth;

                availableWidth -= _tabs[index].FinalWidth;

                double offset = 0;
                int i, index1;
                bool flag1 = true;
                index1 = index - 1;
                for (i = 1; i <= _selectedOffset; i++)
                {
                    _tabs[index - i].X = _tabs[index - i + 1].X - _tabs[index - i].FinalWidth;
                    index1--;
                    if (availableWidth > _tabs[index - i].FinalWidth)
                    {
                        _tabs[index - i].IsVisible = true;
                        availableWidth -= _tabs[index - i].FinalWidth;
                        offset += _tabs[index - i].FinalWidth;
                    }
                    else
                    {
                        _tabs[index - i].IsVisible = false;
                        flag1 = false;
                        break;
                    }
                }

                int index2 = index + 1;
                for (i = index + 1; i < _tabs.Length; i++)
                {
                    index2++;
                    _tabs[i].X = _tabs[i - 1].X + _tabs[i - 1].FinalWidth;
                    if (availableWidth > _tabs[i].FinalWidth)
                    {
                        _tabs[i].IsVisible = true;
                        availableWidth -= _tabs[i].FinalWidth;
                    }
                    else
                    {
                        _tabs[i].IsVisible = false;
                        break;
                    }
                }

                for (; index1 >= 0; index1--)
                {
                    _tabs[index1].X = _tabs[index1 + 1].X - _tabs[index1].FinalWidth;
                    if (flag1)
                    {
                        if (availableWidth > _tabs[index1].FinalWidth)
                        {
                            _tabs[index1].IsVisible = true;
                            availableWidth -= _tabs[index1].FinalWidth;
                            offset += _tabs[index1].FinalWidth;
                        }
                        else
                        {
                            _tabs[index1].IsVisible = false;
                            flag1 = false;
                        }
                    }
                    else
                        _tabs[index1].IsVisible = false;
                }

                for (; index2 < _tabs.Length; index2++)
                {
                    _tabs[index2].X = _tabs[index2 - 1].X + _tabs[index2 - 1].FinalWidth;
                    _tabs[index2].IsVisible = false;
                }

                for (i = 0; i < _tabs.Length; i++)
                    _tabs[i].X += offset;
            }

            if (double.IsPositiveInfinity(availableSize.Width))
                return totalSize;
            else
                return new Size(availableSize.Width, totalSize.Height);
        }

        private Size MeasureTotalSize(double availableHeight)
        {
            Size infinitySize = new Size(double.PositiveInfinity, availableHeight);
            double totalWidth = 0;
            double height = 0;

            if (InternalChildren.Count == 0)
                _tabs = s_emptyTabs;
            else
            {
                if (_tabs.Length != InternalChildren.Count)
                    _tabs = new TabData[InternalChildren.Count];
                for (int index = 0; index < InternalChildren.Count; index++)
                {
                    UIElement uiElement = InternalChildren[index];
                    uiElement.Measure(infinitySize);
                    totalWidth += uiElement.DesiredSize.Width;
                    if (uiElement.DesiredSize.Height > height)
                        height = uiElement.DesiredSize.Height;
                    _tabs[index].UIElement = uiElement;
                    _tabs[index].FinalWidth = _tabs[index].OriginWidth = uiElement.DesiredSize.Width;
                }
            }

            return new Size(totalWidth, height);
        }
    }
}
