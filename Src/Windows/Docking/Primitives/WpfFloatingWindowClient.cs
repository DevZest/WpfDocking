using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Documents;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Represents the client area to display a collection of <see cref="FloatingWindow"/> objects.</summary>
    /// <remarks>
    /// <para>The item container of <see cref="WpfFloatingWindowClient"/> is <see cref="WpfFloatingWindow"/>.
    /// The <see cref="ItemsControl.ItemsSource"/> property binds to the <see cref="Docking.DockControl.FloatingWindows"/> property
    /// of <see cref="DockControl"/>.</para></remarks>
    [TemplatePart(Name = "PART_OverlayCanvas", Type = typeof(Canvas))]
    public sealed class WpfFloatingWindowClient : ItemsControl
    {
        private static readonly DependencyPropertyKey DockControlPropertyKey;
        /// <summary>Identifies the <see cref="DockControl"/> dependency property.</summary>
        public static readonly DependencyProperty DockControlProperty;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static WpfFloatingWindowClient()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WpfFloatingWindowClient), new FrameworkPropertyMetadata(typeof(WpfFloatingWindowClient)));
            FocusableProperty.OverrideMetadata(typeof(WpfFloatingWindowClient), new FrameworkPropertyMetadata(BooleanBoxes.False));
            FocusManager.IsFocusScopeProperty.OverrideMetadata(typeof(WpfFloatingWindowClient), new FrameworkPropertyMetadata(BooleanBoxes.False));
            DockControlPropertyKey = DependencyProperty.RegisterReadOnly("DockControl", typeof(DockControl), typeof(WpfFloatingWindowClient),
                new FrameworkPropertyMetadata(null));
            DockControlProperty = DockControlPropertyKey.DependencyProperty;
        }

        internal WpfFloatingWindowClient(DockControl dockControl)
        {
            Debug.Assert(dockControl != null);
            DockControl = dockControl;
            ItemsSource = dockControl.FloatingWindows;
        }

        /// <summary>Gets the owner <see cref="Docking.DockControl"/>. This is a dependency property.</summary>
        /// <value>The owner <see cref="Docking.DockControl"/>.</value>
        public DockControl DockControl
        {
            get { return (DockControl)GetValue(DockControlProperty); }
            private set { SetValue(DockControlPropertyKey, value); }
        }

        /// <exclude />
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new WpfFloatingWindow();
        }

        /// <exclude />
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is WpfFloatingWindow;
        }

        internal Canvas OverlayCanvas { get; private set; }

        /// <exclude />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            OverlayCanvas = (Canvas)Template.FindName("PART_OverlayCanvas", this);
        }
    }
}
