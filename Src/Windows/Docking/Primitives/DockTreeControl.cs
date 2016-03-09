using System;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Displays a <see cref="DockTree"/>.</summary>
    /// <remarks><see cref="DockTreeControl"/> is used inside data template of <see cref="DockTree"/>.</remarks>
    public class DockTreeControl : Control
    {
        static DockTreeControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockTreeControl), new FrameworkPropertyMetadata(typeof(DockTreeControl)));
        }

        /// <summary>Gets the <see cref="DockTree"/> to display.</summary>
        /// <value>The <see cref="DockTree"/> to display.</value>
        public DockTree DockTree
        {
            get { return DataContext as DockTree; }
        }
    }
}
