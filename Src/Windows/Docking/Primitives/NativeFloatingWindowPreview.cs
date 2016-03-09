using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace DevZest.Windows.Docking.Primitives
{
    internal sealed class NativeFloatingWindowPreview : Window
    {
        static NativeFloatingWindowPreview()
        {
            VisibilityProperty.OverrideMetadata(typeof(NativeFloatingWindowPreview), new FrameworkPropertyMetadata(Visibility.Hidden));
        }

        internal NativeFloatingWindowPreview(DockControl dockControl)
        {
            DockControl = dockControl;
            Owner = Window.GetWindow(dockControl);
            AllowsTransparency = true;
            Background = Brushes.Transparent;
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            IsHitTestVisible = false;
            ShowActivated = false;
            ShowInTaskbar = false;
        }

        DockControl _dockControl;
        public DockControl DockControl
        {
            get { return _dockControl; }
            private set
            {
                DockControl oldValue = DockControl;
                _dockControl = value;

                if (oldValue != null)
                {
                    DependencyPropertyDescriptor.FromProperty(DockManager.PreviewProperty, typeof(DockControl)).RemoveValueChanged(oldValue, OnPreviewChanged);
                    DependencyPropertyDescriptor.FromProperty(DockManager.FloatingPreviewLeftProperty, typeof(DockControl)).RemoveValueChanged(oldValue, OnFloatingPreviewLeftChanged);
                    DependencyPropertyDescriptor.FromProperty(DockManager.FloatingPreviewTopProperty, typeof(DockControl)).RemoveValueChanged(oldValue, OnFloatingPreviewTopChanged);
                    DependencyPropertyDescriptor.FromProperty(DockManager.FloatingPreviewWidthProperty, typeof(DockControl)).RemoveValueChanged(oldValue, OnFloatingPreviewWidthChanged);
                    DependencyPropertyDescriptor.FromProperty(DockManager.FloatingPreviewHeightProperty, typeof(DockControl)).RemoveValueChanged(oldValue, OnFloatingPreviewHeightChanged);
                }

                if (value != null)
                {
                    DependencyPropertyDescriptor.FromProperty(DockManager.PreviewProperty, typeof(DockControl)).AddValueChanged(value, OnPreviewChanged);
                    DependencyPropertyDescriptor.FromProperty(DockManager.FloatingPreviewLeftProperty, typeof(DockControl)).AddValueChanged(value, OnFloatingPreviewLeftChanged);
                    DependencyPropertyDescriptor.FromProperty(DockManager.FloatingPreviewTopProperty, typeof(DockControl)).AddValueChanged(value, OnFloatingPreviewTopChanged);
                    DependencyPropertyDescriptor.FromProperty(DockManager.FloatingPreviewWidthProperty, typeof(DockControl)).AddValueChanged(value, OnFloatingPreviewWidthChanged);
                    DependencyPropertyDescriptor.FromProperty(DockManager.FloatingPreviewHeightProperty, typeof(DockControl)).AddValueChanged(value, OnFloatingPreviewHeightChanged);
                }
            }
        }

        void OnPreviewChanged(object sender, EventArgs e)
        {
            if (DockControl != null)
            {
                bool isVisible = DockManager.GetPreview(DockControl) == DropPosition.Floating;
                Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
            }
        }

        void OnFloatingPreviewLeftChanged(object sender, EventArgs e)
        {
            if (DockControl != null)
                Left = DockManager.GetFloatingPreviewLeft(DockControl);
        }

        void OnFloatingPreviewTopChanged(object sender, EventArgs e)
        {
            if (DockControl != null)
                Top = DockManager.GetFloatingPreviewTop(DockControl);
        }

        void OnFloatingPreviewWidthChanged(object sender, EventArgs e)
        {
            if (DockControl != null)
                Width = DockManager.GetFloatingPreviewWidth(DockControl);
        }

        void OnFloatingPreviewHeightChanged(object sender, EventArgs e)
        {
            if (DockControl != null)
                Height = DockManager.GetFloatingPreviewHeight(DockControl);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            DockControl = null;
        }
    }
}
