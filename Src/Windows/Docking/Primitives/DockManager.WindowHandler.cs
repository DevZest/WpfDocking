using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Input;
using System.Security;
using System.Security.Permissions;

namespace DevZest.Windows.Docking.Primitives
{
    partial class DockManager
    {
        private partial class WindowHandler
        {
            private DockControl _dockControl;
            private OverlayWindow _overlayWindow;
            private WpfFloatingWindowClient _wpfFloatingWindowClient;

            public WindowHandler(DockControl dockControl)
            {
                Debug.Assert(dockControl != null);
                _dockControl = dockControl;
                SetFloatingStrategy();
            }

            public void Close()
            {
                SetFloatingWindowStrategy(_dockControl, FloatingWindowStrategy.Unknown);
                _dockControl = null;
            }

            private DockControl DockControl
            {
                get { return _dockControl; }
            }

            private AdornerLayer AdornerLayer
            {
                get { return AdornerLayer.GetAdornerLayer(DockControl); }
            }

            public FloatingWindowStrategy FloatingStrategy
            {
                get { return GetFloatingWindowStrategy(_dockControl); }
                private set
                {
                    Debug.Assert(_dockControl.IsLoaded);
                    //Debug.Assert(FloatingStrategy == FloatingWindowStrategy.Unknown);
                    Debug.Assert(value == FloatingWindowStrategy.Wpf || value == FloatingWindowStrategy.Native);

                    SetFloatingWindowStrategy(_dockControl, value);
                    if (value == FloatingWindowStrategy.Wpf)
                        InitWpfStrategy();
                    else
                        InitNativeStrategy();
                }
            }

            private static FloatingWindowStrategy GetFloatingStrategy(DockControl dockControl)
            {
                if (!DesignerProperties.GetIsInDesignMode(dockControl) && FloatingWindow.CanBeNative)
                    return FloatingWindowStrategy.Native;
                else
                    return FloatingWindowStrategy.Wpf;
            }

            private void SetFloatingStrategy()
            {
                if (_dockControl.IsLoaded)
                    FloatingStrategy = GetFloatingStrategy(_dockControl);
                else
                    _dockControl.Loaded += new RoutedEventHandler(SetFloatingStrategyOnLoaded);
            }

            private void SetFloatingStrategyOnLoaded(object sender, RoutedEventArgs e)
            {
                _dockControl.Loaded -= new RoutedEventHandler(SetFloatingStrategyOnLoaded);
                FloatingStrategy = GetFloatingStrategy(_dockControl);
            }

            private void InitNativeStrategy()
            {
                Debug.Assert(DockControl.FloatingWindows.Count == 0);
                DockControl.FloatingWindows.CollectionChanged += new NotifyCollectionChangedEventHandler(OnFloatingWindowsChanged);
            }

            private void OnFloatingWindowsChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    FloatingWindow floatingWindow = (FloatingWindow)e.NewItems[0];
                    NativeFloatingWindow.SetNativeFloatingWindow(floatingWindow, new NativeFloatingWindow(floatingWindow));
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    FloatingWindow floatingWindow = (FloatingWindow)e.OldItems[0];
                    NativeFloatingWindow.SetNativeFloatingWindow(floatingWindow, null);
                }
            }

            private void InitWpfStrategy()
            {
                _wpfFloatingWindowClient = new WpfFloatingWindowClient(_dockControl);
                FloatingWindowAdorner.Add(_dockControl, _wpfFloatingWindowClient);
            }

            public Canvas OverlayCanvas
            {
                get { return FloatingStrategy == FloatingWindowStrategy.Native ? NativeOverlayCanvas : _wpfFloatingWindowClient.OverlayCanvas; }
            }

            Canvas NativeOverlayCanvas
            {
                get { return _overlayWindow.OverlayCanvas; }
            }

            private static readonly DependencyProperty StartMousePositionProperty = DependencyProperty.RegisterAttached("StartMousePosition", typeof(Point), typeof(WindowHandler),
                new FrameworkPropertyMetadata(new Point()));
            
            private static Point GetStartMousePosition(DependencyObject d)
            {
                return (Point)d.GetValue(StartMousePositionProperty);
            }

            private static void SetStartMousePosition(DependencyObject d, Point value)
            {
                d.SetValue(StartMousePositionProperty, value);
            }

            public void BeginDrag(MouseEventArgs e)
            {
                SetStartMousePosition(_dockControl, e.GetPosition(_dockControl));

                if (FloatingStrategy == FloatingWindowStrategy.Wpf)
                {
                    SetStartMousePosition(AdornerLayer, e.GetPosition(AdornerLayer));
                    SetStartMousePosition(_wpfFloatingWindowClient, e.GetPosition(_wpfFloatingWindowClient));
                }
                else
                    BeginDragNative(e);
            }

            void BeginDragNative(MouseEventArgs e)
            {
                _startMousePoint = _dockControl.PointToScreen(e.GetPosition(_dockControl));
            }

            public void BeginDrag(Point pt)
            {
                Debug.Assert(_overlayWindow == null);
                SetStartMousePosition(_dockControl, _dockControl.PointFromScreen(pt));
                _startMousePoint = pt;
            }

            public void OnBeginDrag()
            {
                if (FloatingStrategy == FloatingWindowStrategy.Native)
                    ShowOverlayWindow();
            }

            Point _startMousePoint;
            private void ShowOverlayWindow()
            {
                _overlayWindow = OverlayWindow.Show(DockControl);
                SetStartMousePosition(_overlayWindow, _overlayWindow.PointFromScreen(_startMousePoint));
                foreach (FloatingWindow floatingWindow in DockControl.FloatingWindows)
                {
                    NativeFloatingWindow nativeWindow = NativeFloatingWindow.GetNativeFloatingWindow(floatingWindow);
                    SetStartMousePosition(nativeWindow, nativeWindow.PointFromScreen(_startMousePoint));
                }
            }

            private void CloseOverlayWindow()
            {
                Debug.Assert(_overlayWindow != null);
                _overlayWindow.Close();
                _overlayWindow = null;
            }

            public void EndDrag()
            {
                if (FloatingStrategy == FloatingWindowStrategy.Native)
                    CloseOverlayWindow();
            }

            public RelativePoint PreviewStartMousePosition
            {
                get
                {
                    if (FloatingStrategy == FloatingWindowStrategy.Wpf)
                        return new RelativePoint(AdornerLayer, GetStartMousePosition(AdornerLayer));
                    else
                        // _adornerWindow requires UIPermission, wrap it in another method so that
                        // PreviewStartMousePosition can be called from partial trusted code.
                        return AdornerWindowStartMousePosition;
                }
            }

            private RelativePoint AdornerWindowStartMousePosition
            {
                get { return new RelativePoint(_overlayWindow, GetStartMousePosition(_overlayWindow)); }
            }

            public DockPane PaneAtPoint(IDragSource source, double deltaX, double deltaY)
            {
                DockPane paneAtPoint;
                bool stopHitTest;

                if (FloatingStrategy == FloatingWindowStrategy.Native)
                    paneAtPoint = PaneAtPointNativeFloatingWindows(source, deltaX, deltaY, out stopHitTest);
                else
                    paneAtPoint = PaneAtPointWpfFloatingWindows(source, deltaX, deltaY, out stopHitTest);

                if (stopHitTest)
                    return paneAtPoint;

                Point pt = GetStartMousePosition(DockControl);
                pt.Offset(deltaX, deltaY);
                return PaneFromHitTestResult(source, VisualTreeHelper.HitTest(DockControl, pt));
            }

            private DockPane PaneAtPointNativeFloatingWindows(IDragSource source, double deltaX, double deltaY, out bool stopHitTest)
            {
                stopHitTest = false;

                FloatingWindowCollection floatingWindows = DockControl.FloatingWindows;
                Point pt = GetStartMousePosition(_overlayWindow);
                pt.Offset(deltaX, deltaY);
                for (int i = floatingWindows.Count - 1; i >= 0; i--)
                {
                    FloatingWindow floatingWindow = floatingWindows[i];
                    if (!floatingWindow.IsVisible)
                        continue;
                    NativeFloatingWindow nativeWindow = NativeFloatingWindow.GetNativeFloatingWindow(floatingWindow);
                    Rect windowBounds = nativeWindow.GetPreviewBounds(_overlayWindow);
                    stopHitTest = windowBounds.Contains(pt);
                    if (stopHitTest)
                    {
                        pt = GetStartMousePosition(nativeWindow);
                        pt.Offset(deltaX, deltaY);
                        var result = PaneFromHitTestResult(source, VisualTreeHelper.HitTest(nativeWindow, pt));
                        if (result == null || !source.CanDrop(result))
                        {
                            stopHitTest = false;
                            continue;
                        }
                        return result;
                    }
                }

                return null;
            }

            private DockPane PaneAtPointWpfFloatingWindows(IDragSource source, double deltaX, double deltaY, out bool stopHitTest)
            {
                stopHitTest = false;

                FloatingWindowCollection floatingWindows = DockControl.FloatingWindows;
                Point pt = GetStartMousePosition(_wpfFloatingWindowClient);
                pt.Offset(deltaX, deltaY);
                for (int i = floatingWindows.Count - 1; i >= 0; i--)
                {
                    FloatingWindow floatingWindow = floatingWindows[i];
                    if (!floatingWindow.IsVisible)
                        continue;
                    WpfFloatingWindow window = _wpfFloatingWindowClient.ItemContainerGenerator.ContainerFromItem(floatingWindow) as WpfFloatingWindow;
                    if (window == null)
                        continue;
                    Rect windowBounds = new Rect(window.Left, window.Top, window.ActualWidth, window.ActualHeight);
                    stopHitTest = windowBounds.Contains(pt);
                    if (stopHitTest)
                    {
                        pt = _wpfFloatingWindowClient.TranslatePoint(pt, window);
                        var result = PaneFromHitTestResult(source, VisualTreeHelper.HitTest(window, pt));
                        if (result == null || !source.CanDrop(result))
                        {
                            stopHitTest = false;
                            continue;
                        }
                        return result;
                    }
                }

                return null;
            }

            private static DockPane PaneFromHitTestResult(IDragSource source, HitTestResult result)
            {
                if (result == null || result.VisualHit == null)
                    return null;

                for (Visual visual = result.VisualHit as Visual; visual != null; visual = VisualTreeHelper.GetParent(visual) as Visual)
                {
                    DockItem targetItem = GetTargetItem(visual);
                    DropPosition targetPosition = GetTargetPosition(visual);
                    if (targetItem != null && targetPosition == DropPosition.None && source.CanDrop(targetItem.FirstPane))
                        return targetItem.FirstPane;
                }

                return null;
            }

            public Target TargetAtPoint(double deltaX, double deltaY)
            {
                _targetAtPoint = null;

                if (FloatingStrategy == FloatingWindowStrategy.Wpf)
                    TargetAtPointWpfAdorner(deltaX, deltaY);
                else
                    TargetAtPointNativeWindows(deltaX, deltaY);

                if (_targetAtPoint.HasValue)
                    return _targetAtPoint.Value;

                Point pt = GetStartMousePosition(DockControl);
                pt.Offset(deltaX, deltaY);
                Target? target = TargetFromHitTestResult(VisualTreeHelper.HitTest(DockControl, pt));
                return target.HasValue ? target.Value : Target.Empty;
            }

            private void TargetAtPointWpfAdorner(double deltaX, double deltaY)
            {
                Point pt = GetStartMousePosition(AdornerLayer);
                pt.Offset(deltaX, deltaY);
                VisualTreeHelper.HitTest(AdornerLayer, null,
                    new HitTestResultCallback(TargetAtPointHitTestResultCallback),
                    new PointHitTestParameters(pt));
            }

            private void TargetAtPointNativeWindows(double deltaX, double deltaY)
            {
                Point pt = GetStartMousePosition(_overlayWindow);
                pt.Offset(deltaX, deltaY);
                VisualTreeHelper.HitTest(_overlayWindow, null,
                    new HitTestResultCallback(TargetAtPointHitTestResultCallback),
                    new PointHitTestParameters(pt));

                if (_targetAtPoint.HasValue)
                    return;

                FloatingWindowCollection floatingWindows = DockControl.FloatingWindows;
                for (int i = floatingWindows.Count - 1; i >= 0; i--)
                {
                    FloatingWindow floatingWindow = floatingWindows[i];
                    if (!floatingWindow.IsVisible)
                        continue;
                    NativeFloatingWindow nativeWindow = NativeFloatingWindow.GetNativeFloatingWindow(floatingWindow);
                    Rect windowBounds = nativeWindow.GetPreviewBounds(_overlayWindow);
                    if (windowBounds.Contains(pt))
                    {
                        pt = GetStartMousePosition(nativeWindow);
                        pt.Offset(deltaX, deltaY);
                        _targetAtPoint = TargetFromHitTestResult(VisualTreeHelper.HitTest(nativeWindow, pt));
                        if (!_targetAtPoint.HasValue)
                            _targetAtPoint = Target.Empty;
                        return;
                    }
                }
            }

            private Target? _targetAtPoint;
            private HitTestResultBehavior TargetAtPointHitTestResultCallback(HitTestResult result)
            {
                if (result == null || result.VisualHit == null)
                    return HitTestResultBehavior.Stop;

                _targetAtPoint = TargetFromHitTestResult(result);
                return _targetAtPoint.HasValue ?  HitTestResultBehavior.Stop : HitTestResultBehavior.Continue;
            }

            private Target? TargetFromHitTestResult(HitTestResult result)
            {
                if (result == null || result.VisualHit == null)
                    return null;

                for (Visual visual = result.VisualHit as Visual; visual != null; visual = VisualTreeHelper.GetParent(visual) as Visual)
                {
                    DockItem targetItem = GetTargetItem(visual);
                    if (targetItem == null)
                    {
                        if (visual.GetValue(FrameworkElement.DataContextProperty) != DockControl)
                            continue;
                    }
                    else
                    {
                        if (targetItem.DockControl != DockControl)
                            continue;
                    }

                    DropPosition targetPosition = GetTargetPosition(visual);
                    Target? target = GetTarget(targetItem, targetPosition);
                    if (target.HasValue)
                        return target;

                    if (visual is IDragSource)
                        return Target.Empty;
                }

                return null;
            }

            private static Target? GetTarget(DockItem targetItem, DropPosition targetPosition)
            {
                if (targetPosition == DropPosition.Left)
                    return targetItem == null ? Target.LeftToolWindow : Target.LeftPane(targetItem);
                else if (targetPosition == DropPosition.Right)
                    return targetItem == null ? Target.RightToolWindow : Target.RightPane(targetItem);
                else if (targetPosition == DropPosition.Top)
                    return targetItem == null ? Target.TopToolWindow : Target.TopPane(targetItem);
                else if (targetPosition == DropPosition.Bottom)
                    return targetItem == null ? Target.BottomToolWindow : Target.BottomPane(targetItem);
                else if (targetPosition == DropPosition.Fill)
                    return targetItem == null ? Target.Document : Target.FillPane(targetItem);
                else if (targetPosition == DropPosition.Tab && targetItem != null)
                    return Target.Tab(targetItem);

                return null;
            }

            public Rect GetFloatingWindowBounds(Rect floatingPreviewBounds)
            {
                if (FloatingStrategy == FloatingWindowStrategy.Wpf)
                    return floatingPreviewBounds;
                else
                {
                    Point pt = ScreenPointFromOverlayWindow(floatingPreviewBounds.Location);
                    return new Rect(pt, floatingPreviewBounds.Size);
                }
            }

            private Point ScreenPointFromOverlayWindow(Point point)
            {
                Point ptScreen = _overlayWindow.PointToScreen(point);
                PresentationSource source = PresentationSource.FromVisual(_overlayWindow);
                return source.CompositionTarget.TransformFromDevice.Transform(ptScreen);
            }

            public void AttachFloatingWindowPreviewOverlay(Overlay overlay)
            {
                _overlayWindow.AttachFloatingWindowPreviewOverlay(overlay);
            }
        }
    }
}
