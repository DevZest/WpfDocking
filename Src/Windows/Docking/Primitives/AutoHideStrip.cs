using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Represents the strip of auto-hide tabs.</summary>
    /// <remarks><para>Use the <see cref="AutoHideStrip"/> in the control template of <see cref="DockControl"/> class. Bind
    /// <see cref="ItemsControl.ItemsSource"/> property to the <see cref="DockTree.AutoHideItems"/> property of <see cref="DockTree"/>.</para>
    /// <para>The item container of <see cref="AutoHideStrip"/> is <see cref="AutoHideTab"/>.</para></remarks>
    public class AutoHideStrip : ItemsControl 
    {
        /// <summary>Identifies the <see cref="Placement"/> dependency property.</summary>
        public static readonly DependencyProperty PlacementProperty;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static AutoHideStrip()
        {
            FocusableProperty.OverrideMetadata(typeof(AutoHideStrip), new FrameworkPropertyMetadata(BooleanBoxes.False));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoHideStrip), new FrameworkPropertyMetadata(typeof(AutoHideStrip)));
            PlacementProperty = DependencyProperty.Register("Placement", typeof(Dock), typeof(AutoHideStrip),
                new FrameworkPropertyMetadata(Dock.Top));
        }

        /// <summary>Gets or sets the value indicates where the auto-hide strip is placed.</summary>
        /// <value>The value indicates where the auto-hide strip is placed.</value>
        public Dock Placement
        {
            get { return (Dock)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        /// <exclude/>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new AutoHideTab();
        }

        /// <exclude/>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is AutoHideTab;
        }
    }
}
