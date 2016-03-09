using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Represents the tab in the tool window.</summary>
    /// <remarks><see cref="ToolWindowTab" /> is the item container of <see cref="ToolWindowTabStrip"/>. Its <see cref="ContentControl.Content"/>
    /// property is set to a instance of <see cref="DockItem"/> object.</remarks>
    public class ToolWindowTab : DockWindowTab
    {
        static ToolWindowTab()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolWindowTab), new FrameworkPropertyMetadata(typeof(ToolWindowTab)));
        }
    }
}
