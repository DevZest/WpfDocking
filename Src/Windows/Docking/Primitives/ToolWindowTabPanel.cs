using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Arranges the tool window tabs.</summary>
    /// <remarks>Use <see cref="ToolWindowTabPanel"/> as <see cref="ItemsControl.ItemsPanel"/> of <see cref="ToolWindowTabStrip"/>.</remarks>
    public class ToolWindowTabPanel : Panel
    {
        private struct TabData
        {
            public UIElement UIElement;
            public bool Flag;
            public double OriginWidth;
            public double FinalWidth;
        }

        private static TabData[] s_emptyTabs = new TabData[0];
        private TabData[] _tabs = s_emptyTabs;

        /// <exclude/>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = CalculateWidthes(availableSize);

            // FIX: Shouldn't Measure child element with FinalWidth - when child element width exceeds FinalWidth,
            // it will not invalidate this panel's measure
            //foreach (TabData tab in _tabs)
            //    tab.UIElement.Measure(new Size(tab.FinalWidth, size.Height));

            return size;
        }

        /// <exclude/>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double x = 0;
            foreach (TabData tab in _tabs)
            {
                Rect rect = new Rect(new Point(x, 0), new Size(tab.FinalWidth, finalSize.Height));
                tab.UIElement.Arrange(rect);
                x += tab.FinalWidth;
                ToolWindowTab toolWindowTab = tab.UIElement as ToolWindowTab;
                if (toolWindowTab != null)
                    toolWindowTab.IsContentTrimmed = tab.OriginWidth > tab.FinalWidth;
            }

            return finalSize;
        }

        private Size CalculateWidthes(Size availableSize)
        {
            Size totalSize = MeasureTotalSize(availableSize.Height);
            if (availableSize.Width >= totalSize.Width)
            {
                if (double.IsPositiveInfinity(availableSize.Width))
                    return totalSize;
                else
                    return new Size(availableSize.Width, totalSize.Height);
            }

            int countTabs = _tabs.Length;

            // Set tab whose max width less than average width
            bool anyWidthWithinAverage = true;
            double totalWidth = availableSize.Width;
            double totalAllocatedWidth = 0;
            double averageWidth = totalWidth / countTabs;
            double remainedTabs = countTabs;
            for (anyWidthWithinAverage = true; anyWidthWithinAverage && remainedTabs > 0; )
            {
                anyWidthWithinAverage = false;
                for (int i = 0; i < _tabs.Length; i++)
                {
                    if (_tabs[i].Flag)
                        continue;

                    if (_tabs[i].FinalWidth <= averageWidth)
                    {
                        _tabs[i].Flag = true;
                        totalAllocatedWidth += _tabs[i].FinalWidth;
                        anyWidthWithinAverage = true;
                        remainedTabs--;
                    }
                }
                if (remainedTabs != 0)
                    averageWidth = (totalWidth - totalAllocatedWidth) / remainedTabs;
            }

            // If any tab width not set yet, set it to the average width
            if (remainedTabs > 0)
            {
                for (int i = 0; i < _tabs.Length; i++)
                {
                    if (_tabs[i].Flag)
                        continue;

                    _tabs[i].Flag = true;
                    _tabs[i].FinalWidth = averageWidth;
                }
            }

            return new Size(availableSize.Width, totalSize.Height);
        }

        private Size MeasureTotalSize(double availableHeight)
        {
            Size infinitySize = new Size(double.PositiveInfinity, availableHeight);
            double totalWidth = 0;
            double height = 0;
            int countVisibleChildren = 0;

            foreach (UIElement uiElement in InternalChildren)
            {
                if (uiElement == null)
                    continue;

                if (uiElement.Visibility == Visibility.Collapsed)
                    continue;

                uiElement.Measure(infinitySize);
                totalWidth += uiElement.DesiredSize.Width;
                if (uiElement.DesiredSize.Height > height)
                    height = uiElement.DesiredSize.Height;
                countVisibleChildren++;
            }

            if (countVisibleChildren == 0)
                _tabs = s_emptyTabs;
            else
            {
                if (_tabs.Length != countVisibleChildren)
                    _tabs = new TabData[countVisibleChildren];
                int index = 0;
                foreach (UIElement uiElement in InternalChildren)
                {
                    if (uiElement == null)
                        continue;

                    if (uiElement.Visibility == Visibility.Collapsed)
                        continue;

                    _tabs[index].UIElement = uiElement;
                    _tabs[index].Flag = false;
                    _tabs[index].OriginWidth = _tabs[index].FinalWidth = uiElement.DesiredSize.Width;
                    index++;
                }
            }

            return new Size(totalWidth, height);
        }
    }
}
