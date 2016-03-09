using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Represents the strip of tool window tabs.</summary>
    /// <remarks><para>Use the <see cref="ToolWindowTabStrip"/> class in the control template of <see cref="ToolWindow"/> class. Bind
    /// <see cref="ItemsControl.ItemsSource"/> property to the <see cref="DockPane.VisibleItems"/> property of <see cref="DockPane"/>.</para>
    /// <para>The item container of <see cref="ToolWindowTabStrip"/> is <see cref="ToolWindowTab"/>.</para></remarks>
    public class ToolWindowTabStrip : ItemsControl 
    {
        static ToolWindowTabStrip()
        {
            FocusableProperty.OverrideMetadata(typeof(ToolWindowTabStrip), new FrameworkPropertyMetadata(BooleanBoxes.False));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolWindowTabStrip), new FrameworkPropertyMetadata(typeof(ToolWindowTabStrip)));
        }

        /// <exclude/>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ToolWindowTab();
        }

        /// <exclude/>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ToolWindowTab;
        }
    }
}
