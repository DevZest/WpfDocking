using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Displays the overlay (docking guide and preview) of <see cref="DockTreeClient"/>.</summary>
    /// <remarks><see cref="DockTreeClientOverlay"/> is used inside the control template of <see cref="DockTreeClient"/>.</remarks>
    public class DockTreeClientOverlay : Control
    {
        /// <summary>Identifies the <see cref="SplitterSize"/> dependency property.</summary>
        public static readonly DependencyProperty SplitterSizeProperty = DependencyProperty.Register("SplitterSize", typeof(double), typeof(DockTreeClientOverlay),
            new FrameworkPropertyMetadata(4d));

        static readonly DependencyPropertyKey LeftTreeWidthPropertyKey = DependencyProperty.RegisterReadOnly("LeftTreeWidth", typeof(GridLength), typeof(DockTreeClientOverlay),
            new FrameworkPropertyMetadata());
        /// <summary>Identifies the <see cref="LeftTreeWidth"/> dependency property.</summary>
        public static readonly DependencyProperty LeftTreeWidthProperty = LeftTreeWidthPropertyKey.DependencyProperty;

        static readonly DependencyPropertyKey RightTreeWidthPropertyKey = DependencyProperty.RegisterReadOnly("RightTreeWidth", typeof(GridLength), typeof(DockTreeClientOverlay),
            new FrameworkPropertyMetadata());
        /// <summary>Identifies the <see cref="RightTreeWidth"/> dependency property.</summary>
        public static readonly DependencyProperty RightTreeWidthProperty = RightTreeWidthPropertyKey.DependencyProperty;

        static readonly DependencyPropertyKey TopTreeHeightPropertyKey = DependencyProperty.RegisterReadOnly("TopTreeHeight", typeof(GridLength), typeof(DockTreeClientOverlay),
            new FrameworkPropertyMetadata());
        /// <summary>Identifies the <see cref="TopTreeHeight"/> dependency property.</summary>
        public static readonly DependencyProperty TopTreeHeightProperty = TopTreeHeightPropertyKey.DependencyProperty;

        static readonly DependencyPropertyKey BottomTreeHeightPropertyKey = DependencyProperty.RegisterReadOnly("BottomTreeHeight", typeof(GridLength), typeof(DockTreeClientOverlay),
            new FrameworkPropertyMetadata());
        /// <summary>Identifies the <see cref="BottomTreeHeight"/> dependency property.</summary>
        public static readonly DependencyProperty BottomTreeHeightProperty = BottomTreeHeightPropertyKey.DependencyProperty;

        static readonly DependencyPropertyKey LeftSplitterWidthPropertyKey = DependencyProperty.RegisterReadOnly("LeftSplitterWidth", typeof(GridLength), typeof(DockTreeClientOverlay),
            new FrameworkPropertyMetadata());
        /// <summary>Identifies the <see cref="LeftSplitterWidth"/> dependency property.</summary>
        public static readonly DependencyProperty LeftSplitterWidthProperty = LeftSplitterWidthPropertyKey.DependencyProperty;

        static readonly DependencyPropertyKey RightSplitterWidthPropertyKey = DependencyProperty.RegisterReadOnly("RightSplitterWidth", typeof(GridLength), typeof(DockTreeClientOverlay),
            new FrameworkPropertyMetadata());
        /// <summary>Identifies the <see cref="RightSplitterWidth"/> dependency property.</summary>
        public static readonly DependencyProperty RightSplitterWidthProperty = RightSplitterWidthPropertyKey.DependencyProperty;

        static readonly DependencyPropertyKey TopSplitterHeightPropertyKey = DependencyProperty.RegisterReadOnly("TopSplitterHeight", typeof(GridLength), typeof(DockTreeClientOverlay),
            new FrameworkPropertyMetadata());
        /// <summary>Identifies the <see cref="TopSplitterHeight"/> dependency property.</summary>
        public static readonly DependencyProperty TopSplitterHeightProperty = TopSplitterHeightPropertyKey.DependencyProperty;

        static readonly DependencyPropertyKey BottomSplitterHeightPropertyKey = DependencyProperty.RegisterReadOnly("BottomSplitterHeight", typeof(GridLength), typeof(DockTreeClientOverlay),
            new FrameworkPropertyMetadata());
        /// <summary>Identifies the <see cref="BottomSplitterHeight"/> dependency property.</summary>
        public static readonly DependencyProperty BottomSplitterHeightProperty = BottomSplitterHeightPropertyKey.DependencyProperty;

        static readonly DependencyPropertyKey IsLeftGuideVisiblePropertyKey = DependencyProperty.RegisterReadOnly("IsLeftGuideVisible", typeof(bool), typeof(DockTreeClientOverlay),
            new FrameworkPropertyMetadata());
        /// <summary>Identifies the <see cref="IsLeftGuideVisible"/> dependency property.</summary>
        public static readonly DependencyProperty IsLeftGuideVisibleProperty = IsLeftGuideVisiblePropertyKey.DependencyProperty;

        static readonly DependencyPropertyKey IsRightGuideVisiblePropertyKey = DependencyProperty.RegisterReadOnly("IsRightGuideVisible", typeof(bool), typeof(DockTreeClientOverlay),
            new FrameworkPropertyMetadata());
        /// <summary>Identifies the <see cref="IsRightGuideVisible"/> dependency property.</summary>
        public static readonly DependencyProperty IsRightGuideVisibleProperty = IsRightGuideVisiblePropertyKey.DependencyProperty;

        static readonly DependencyPropertyKey IsTopGuideVisiblePropertyKey = DependencyProperty.RegisterReadOnly("IsTopGuideVisible", typeof(bool), typeof(DockTreeClientOverlay),
            new FrameworkPropertyMetadata());
        /// <summary>Identifies the <see cref="IsTopGuideVisible"/> dependency property.</summary>
        public static readonly DependencyProperty IsTopGuideVisibleProperty = IsTopGuideVisiblePropertyKey.DependencyProperty;

        static readonly DependencyPropertyKey IsBottomGuideVisiblePropertyKey = DependencyProperty.RegisterReadOnly("IsBottomGuideVisible", typeof(bool), typeof(DockTreeClientOverlay),
            new FrameworkPropertyMetadata());
        /// <summary>Identifies the <see cref="IsBottomGuideVisible"/> dependency property.</summary>
        public static readonly DependencyProperty IsBottomGuideVisibleProperty = IsBottomGuideVisiblePropertyKey.DependencyProperty;

        static readonly DependencyPropertyKey IsDocumentGuideVisiblePropertyKey = DependencyProperty.RegisterReadOnly("IsDocumentGuideVisible", typeof(bool), typeof(DockTreeClientOverlay),
            new FrameworkPropertyMetadata());
        /// <summary>Identifies the <see cref="IsDocumentGuideVisible"/> dependency property.</summary>
        public static readonly DependencyProperty IsDocumentGuideVisibleProperty = IsDocumentGuideVisiblePropertyKey.DependencyProperty;

        static DockTreeClientOverlay()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockTreeClientOverlay), new FrameworkPropertyMetadata(typeof(DockTreeClientOverlay)));
        }

        /// <summary>Gets or sets the size of the splitter, in device-independent units (1/96th inch per unit). This is a dependency property.</summary>
        /// <value>Representing the size of the splitter, in device-independent units (1/96th inch per unit). The default is 4.</value>
        public double SplitterSize
        {
            get { return (double)GetValue(SplitterSizeProperty); }
            set { SetValue(SplitterSizeProperty, value); }
        }

        /// <summary>Gets the width of left <see cref="DockTree"/>.</summary>
        /// <value>The left <see cref="DockTree"/> width.</value>
        public GridLength LeftTreeWidth
        {
            get { return (GridLength)GetValue(LeftTreeWidthProperty); }
            private set { SetValue(LeftTreeWidthPropertyKey, value); }
        }

        /// <summary>Gets the width of right <see cref="DockTree"/>.</summary>
        /// <value>The right <see cref="DockTree"/> width.</value>
        public GridLength RightTreeWidth
        {
            get { return (GridLength)GetValue(RightTreeWidthProperty); }
            private set { SetValue(RightTreeWidthPropertyKey, value); }
        }

        /// <summary>Gets the height of top <see cref="DockTree"/>.</summary>
        /// <value>The top <see cref="DockTree"/> height.</value>
        public GridLength TopTreeHeight
        {
            get { return (GridLength)GetValue(TopTreeHeightProperty); }
            private set { SetValue(TopTreeHeightPropertyKey, value); }
        }

        /// <summary>Gets the height of bottom <see cref="DockTree"/>.</summary>
        /// <value>The bottom <see cref="DockTree"/> height.</value>
        public GridLength BottomTreeHeight
        {
            get { return (GridLength)GetValue(BottomTreeHeightProperty); }
            private set { SetValue(BottomTreeHeightPropertyKey, value); }
        }

        /// <summary>Gets the width of left splitter.</summary>
        /// <value>Value of <see cref="SplitterSize"/> if left <see cref="DockTree"/> is visible, otherwise 0.</value>
        public GridLength LeftSplitterWidth
        {
            get { return (GridLength)GetValue(LeftSplitterWidthProperty); }
            private set { SetValue(LeftSplitterWidthPropertyKey, value); }
        }

        /// <summary>Gets the width of right splitter.</summary>
        /// <value>Value of <see cref="SplitterSize"/> if right <see cref="DockTree"/> is visible, otherwise 0.</value>
        public GridLength RightSplitterWidth
        {
            get { return (GridLength)GetValue(RightSplitterWidthProperty); }
            private set { SetValue(RightSplitterWidthPropertyKey, value); }
        }

        /// <summary>Gets the height of top splitter.</summary>
        /// <value>Value of <see cref="SplitterSize"/> if top <see cref="DockTree"/> is visible, otherwise 0.</value>
        public GridLength TopSplitterHeight
        {
            get { return (GridLength)GetValue(TopSplitterHeightProperty); }
            private set { SetValue(TopSplitterHeightPropertyKey, value); }
        }

        /// <summary>Gets the height of bottom splitter.</summary>
        /// <value>Value of <see cref="SplitterSize"/> if bottom <see cref="DockTree"/> is visible, otherwise 0.</value>
        public GridLength BottomSplitterHeight
        {
            get { return (GridLength)GetValue(BottomSplitterHeightProperty); }
            private set { SetValue(BottomSplitterHeightPropertyKey, value); }
        }

        /// <summary>Gets a valud indicates whether left docking guide is visible.</summary>
        /// <value><see langword="true"/> if left docking guide is visible, otherwise <see langword="false"/>.</value>
        public bool IsLeftGuideVisible
        {
            get { return (bool)GetValue(IsLeftGuideVisibleProperty); }
            private set { SetValue(IsLeftGuideVisiblePropertyKey, value); }
        }

        /// <summary>Gets a valud indicates whether right docking guide is visible.</summary>
        /// <value><see langword="true"/> if right docking guide is visible, otherwise <see langword="false"/>.</value>
        public bool IsRightGuideVisible
        {
            get { return (bool)GetValue(IsRightGuideVisibleProperty); }
            private set { SetValue(IsRightGuideVisiblePropertyKey, value); }
        }

        /// <summary>Gets a valud indicates whether top docking guide is visible.</summary>
        /// <value><see langword="true"/> if top docking guide is visible, otherwise <see langword="false"/>.</value>
        public bool IsTopGuideVisible
        {
            get { return (bool)GetValue(IsTopGuideVisibleProperty); }
            private set { SetValue(IsTopGuideVisiblePropertyKey, value); }
        }

        /// <summary>Gets a valud indicates whether bottom docking guide is visible.</summary>
        /// <value><see langword="true"/> if bottom docking guide is visible, otherwise <see langword="false"/>.</value>
        public bool IsBottomGuideVisible
        {
            get { return (bool)GetValue(IsBottomGuideVisibleProperty); }
            private set { SetValue(IsBottomGuideVisiblePropertyKey, value); }
        }

        /// <summary>Gets a valud indicates whether document docking guide is visible.</summary>
        /// <value><see langword="true"/> if document docking guide is visible, otherwise <see langword="false"/>.</value>
        public bool IsDocumentGuideVisible
        {
            get { return (bool)GetValue(IsDocumentGuideVisibleProperty); }
            private set { SetValue(IsDocumentGuideVisiblePropertyKey, value); }
        }

        DockControl DockControl
        {
            get { return DataContext as DockControl; }
        }

        GridLength ConvertSplitterSize(bool isVisible)
        {
            return isVisible ? new GridLength(SplitterSize) : new GridLength(0);
        }

        /// <exclude />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Refresh();
        }

        void Refresh()
        {
            if (DockControl == null)
                return;

            LeftSplitterWidth = ConvertSplitterSize(DockControl.LeftDockTree.IsVisible);
            RightSplitterWidth = ConvertSplitterSize(DockControl.RightDockTree.IsVisible);
            TopSplitterHeight = ConvertSplitterSize(DockControl.TopDockTree.IsVisible);
            BottomSplitterHeight = ConvertSplitterSize(DockControl.BottomDockTree.IsVisible);

            IsLeftGuideVisible = DockManager.GetShowsLeftGuide(DockControl);
            IsRightGuideVisible = DockManager.GetShowsRightGuide(DockControl);
            IsTopGuideVisible = DockManager.GetShowsTopGuide(DockControl);
            IsBottomGuideVisible = DockManager.GetShowsBottomGuide(DockControl);
            IsDocumentGuideVisible = DockManager.GetShowsFillGuide(DockControl);

            LeftTreeWidth = RightTreeWidth = TopTreeHeight = BottomTreeHeight = new GridLength();
            DockTreeClient dockTreeClient = FindVisualChild<DockTreeClient>(DockControl);
            foreach (DockTreeControl dockTreeControl in FindVisualChildren<DockTreeControl>(dockTreeClient))
            {
                DockTree dockTree = dockTreeControl.DockTree;
                if (dockTree == null)
                    continue;
                if (dockTree.Position == DockTreePosition.Left)
                    LeftTreeWidth = new GridLength(dockTreeControl.ActualWidth);
                else if (dockTree.Position == DockTreePosition.Right)
                    RightTreeWidth = new GridLength(dockTreeControl.ActualWidth);
                else if (dockTree.Position == DockTreePosition.Top)
                    TopTreeHeight = new GridLength(dockTreeControl.ActualHeight);
                else if (dockTree.Position == DockTreePosition.Bottom)
                    BottomTreeHeight = new GridLength(dockTreeControl.ActualHeight);
            }
        }

        static T FindVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            foreach (T child in FindVisualChildren<T>(depObj))
                return child;
            return null;
        }

        static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

    }
}
