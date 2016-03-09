using System;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Displays a <see cref="DockTreeSplit"/>.</summary>
    public class DockTreeSplitControl : Control
    {
        /// <summary>Identifies the <see cref="DockTreeSplit"/> dependency property.</summary>
        public static readonly DependencyProperty DockTreeSplitProperty = DependencyProperty.Register("DockTreeSplit", typeof(DockTreeSplit), typeof(DockTreeSplitControl),
            new FrameworkPropertyMetadata(null));

        static DockTreeSplitControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockTreeSplitControl), new FrameworkPropertyMetadata(typeof(DockTreeSplitControl)));
        }

        /// <summary>Gets or sets the <see cref="DockTreeSplit"/> to display.</summary>
        /// <value>The <see cref="DockTreeSplit"/> to display.</value>
        public DockTreeSplit DockTreeSplit
        {
            get { return (DockTreeSplit)GetValue(DockTreeSplitProperty); }
            set { SetValue(DockTreeSplitProperty, value); }
        }
    }
}
