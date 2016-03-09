using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking.Primitives
{
    partial class DockManager
    {
        partial class WindowHandler
        {
            private class OverlayWindow : Window
            {
                [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
                static OverlayWindow()
                {
                    TopmostProperty.OverrideMetadata(typeof(OverlayWindow), new FrameworkPropertyMetadata(BooleanBoxes.True));
                }

                private static readonly DependencyProperty AdornerProperty = DependencyProperty.RegisterAttached("AdornerProperty", typeof(ContentPresenter), typeof(OverlayWindow),
                    new FrameworkPropertyMetadata(null));

                private static ContentPresenter GetAdorner(UIElement element)
                {
                    return (ContentPresenter)element.GetValue(AdornerProperty);
                }

                private static void SetAdorner(UIElement element, ContentPresenter value)
                {
                    element.SetValue(AdornerProperty, value);
                }

                private Canvas _canvas;
                private DockControl DockControl { get; set; }
                private NativeFloatingWindowPreview _nativeFloatingWindowPreview;

                private OverlayWindow(DockControl dockControl)
                {
                    DockControl = dockControl;
                    _canvas = new Canvas();
                    _nativeFloatingWindowPreview = new NativeFloatingWindowPreview(dockControl);

                    WindowStyle = WindowStyle.None;
                    ResizeMode = ResizeMode.NoResize;
                    ShowInTaskbar = false;
                    SizeToContent = SizeToContent.Manual;
                    SetResourceReference(LeftProperty, SystemParameters.VirtualScreenLeftKey);
                    SetResourceReference(TopProperty, SystemParameters.VirtualScreenTopKey);
                    SetResourceReference(WidthProperty, SystemParameters.VirtualScreenWidthKey);
                    SetResourceReference(HeightProperty, SystemParameters.VirtualScreenHeightKey);
                    Focusable = false;
                    AllowsTransparency = true;
                    Background = null;
                    Content = _canvas;
                    ShowActivated = false;

                    Window owner = Window.GetWindow(dockControl);
                    Debug.Assert(owner != null);
                    Owner = owner;
                }

                public Canvas OverlayCanvas
                {
                    get { return _canvas; }
                }

                public static OverlayWindow Show(DockControl dockControl)
                {
                    OverlayWindow window = new OverlayWindow(dockControl);
                    window.Show();
                    return window;
                }

                public void AttachFloatingWindowPreviewOverlay(Overlay overlay)
                {
                    _nativeFloatingWindowPreview.Content = overlay.Container;
                }

                protected override void OnClosed(EventArgs e)
                {
                    base.OnClosed(e);
                    _nativeFloatingWindowPreview.Close();
                }
            }
        }
    }
}
