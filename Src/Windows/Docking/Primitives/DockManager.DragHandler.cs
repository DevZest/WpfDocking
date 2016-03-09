using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Documents;

namespace DevZest.Windows.Docking.Primitives
{
    partial class DockManager
    {
        private class DragHandler : DragHandlerBase
        {
            private static DragHandler s_singleton = new DragHandler();
            public static DragHandler Singleton
            {
                get { return s_singleton; }
            }

            private IDragSource _source;
            private bool _isCtrlKeyDown;
            private DockPane _paneWithGuide;
            private Target _target;
            private Rect _floatingPreviewStartBounds;    // related to floating adorner window
            private Rect _floatingWindowStartBounds; // related to the screen

            private Rect FloatingStartBounds
            {
                get { return WindowHandler.FloatingStrategy == FloatingWindowStrategy.Wpf ? _floatingPreviewStartBounds : _floatingWindowStartBounds; }
            }

            private DragHandler()
            {
            }

            private DockControl DockControl
            {
                get { return _source.DockControl; }
            }

            private WindowHandler WindowHandler
            {
                get { return GetWindowHandler(DockControl); }
            }

            private bool ShowsDockControlGuide
            {
                get { return GetShowsGuide(DockControl); }
                set
                {
                    bool oldValue = ShowsDockControlGuide;
                    if (oldValue == value)
                        return;
                    SetShowsGuide(DockControl, value);

                    if (value)
                    {
                        SetShowsLeftGuide(DockControl,
                            !DockControl.GetDockTree(DockControlTreePosition.Left).IsVisible &&
                            _source.CanDrop(DockTreePosition.Left));
                        SetShowsRightGuide(DockControl,
                            !DockControl.GetDockTree(DockControlTreePosition.Right).IsVisible &&
                            _source.CanDrop(DockTreePosition.Right));
                        SetShowsTopGuide(DockControl,
                            !DockControl.GetDockTree(DockControlTreePosition.Top).IsVisible &&
                            _source.CanDrop(DockTreePosition.Top));
                        SetShowsBottomGuide(DockControl,
                            !DockControl.GetDockTree(DockControlTreePosition.Bottom).IsVisible &&
                            _source.CanDrop(DockTreePosition.Bottom));
                        SetShowsFillGuide(DockControl,
                            !DockControl.GetDockTree(DockControlTreePosition.Document).IsVisible &&
                            _source.CanDrop(DockTreePosition.Document));
                    }
                    else
                    {
                        DockControl.ClearValue(ShowsLeftGuidePropertyKey);
                        DockControl.ClearValue(ShowsRightGuidePropertyKey);
                        DockControl.ClearValue(ShowsTopGuidePropertyKey);
                        DockControl.ClearValue(ShowsBottomGuidePropertyKey);
                        DockControl.ClearValue(ShowsFillGuidePropertyKey);
                    }
                }
            }

            private DockPane PaneWithGuide
            {
                set
                {
                    if (_paneWithGuide == value)
                        return;

                    if (_paneWithGuide != null)
                        _paneWithGuide.ClearValue(ShowsGuidePropertyKey);

                    _paneWithGuide = value;

                    if (_paneWithGuide != null)
                        SetShowsGuide(_paneWithGuide, true);
                }
            }

            private Target Target
            {
                get { return _target; }
                set
                {
                    Target oldValue = _target;
                    _target = value;

                    if (oldValue.TargetPane != null)
                    {
                        if (oldValue.TargetPane != value.TargetPane)
                            oldValue.TargetPane.ClearValue(PreviewPropertyKey);
                        if (oldValue.TargetPosition == DropPosition.Tab)
                        {
                            if (oldValue.TargetItem != value.TargetItem || oldValue.TargetPosition != value.TargetPosition)
                                oldValue.TargetItem.ClearValue(PreviewPropertyKey);
                        }
                    }
                    else
                    {
                        if (value.TargetPane != null)
                        {
                            DockControl.ClearValue(PreviewPropertyKey);
                            ClearFloatingPreviewValues();
                        }
                    }

                    if (value.TargetPane != null)
                    {
                        SetPreview(value.TargetPane, value.TargetPosition);
                        if (value.TargetPosition == DropPosition.Tab)
                            SetPreview(value.TargetItem, DropPosition.Tab);
                    }
                    else
                    {
                        if (value.TargetPosition == DropPosition.None)
                            DockControl.ClearValue(PreviewPropertyKey);
                        else
                            SetPreview(DockControl, value.TargetPosition);

                        if (value.TargetPosition == DropPosition.Floating)
                            SetFloatingPreviewBounds(Rect.Offset(FloatingStartBounds, MouseDeltaX, MouseDeltaY));
                        else
                            ClearFloatingPreviewValues();
                    }
                }
            }

            private void SetFloatingPreviewBounds(Rect bounds)
            {
                SetFloatingPreviewLeft(DockControl, bounds.Left);
                SetFloatingPreviewTop(DockControl, bounds.Top);
                SetFloatingPreviewWidth(DockControl, bounds.Width);
                SetFloatingPreviewHeight(DockControl, bounds.Height);
            }

            private void ClearFloatingPreviewValues()
            {
                DockControl.ClearValue(FloatingPreviewLeftPropertyKey);
                DockControl.ClearValue(FloatingPreviewTopPropertyKey);
                DockControl.ClearValue(FloatingPreviewWidthPropertyKey);
                DockControl.ClearValue(FloatingPreviewHeightPropertyKey);
            }

            private bool IsShiftKeyDown
            {
                set
                {
                    bool oldValue = GetIsShiftKeyDown(DockControl);
                    if (oldValue == value)
                        return;

                    SetIsShiftKeyDown(DockControl, value);
                    TestDrop();
                }
            }

            private bool IsCtrlKeyDown
            {
                get { return _isCtrlKeyDown; }
                set
                {
                    if (_isCtrlKeyDown == value)
                        return;

                    _isCtrlKeyDown = value;
                    TestDrop();
                }
            }

            public void BeginDrag(IDragSource source, UIElement uiElement, MouseEventArgs e)
            {
                _source = source;
                WindowHandler.BeginDrag(e);
                DragDetect(uiElement, e);
            }

            public void BeginDrag(IDragSource source, UIElement uiElement, Point pt)
            {
                _source = source;
                WindowHandler.BeginDrag(pt);
                DragDetect(uiElement, pt);
            }

            protected override void OnBeginDrag()
            {
                WindowHandler.OnBeginDrag();
                IsShiftKeyDown = CheckShiftKeyDown();
                _floatingPreviewStartBounds = _source.GetFloatingWindowPreview(WindowHandler.PreviewStartMousePosition);
                _floatingWindowStartBounds = WindowHandler.GetFloatingWindowBounds(_floatingPreviewStartBounds);
                TestDrop();
            }

            protected override void OnDragDelta()
            {
                TestDrop();
            }

            protected override void OnEndDrag(UIElement dragElement, bool abort)
            {
                Target target = Target;
                bool isShiftKeyDown = GetIsShiftKeyDown(DockControl);
                DockControl.ClearValue(IsShiftKeyDownPropertyKey);
                ShowsDockControlGuide = false;
                PaneWithGuide = null;
                Target = Target.Empty;
                RestoreCursor(dragElement as FrameworkElement);
                WindowHandler.EndDrag();
                if (!abort)
                    Drop(target, isShiftKeyDown);
            }

            protected override void OnKeyDown(KeyEventArgs e)
            {
                IsShiftKeyDown = CheckShiftKeyDown();
                IsCtrlKeyDown = CheckCtrlKeyDown();
                base.OnKeyDown(e);
            }

            protected override void OnKeyUp(KeyEventArgs e)
            {
                base.OnKeyUp(e);
                IsShiftKeyDown = CheckShiftKeyDown();
                IsCtrlKeyDown = CheckCtrlKeyDown();
            }

            private static bool CheckShiftKeyDown()
            {
                return Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            }

            private static bool CheckCtrlKeyDown()
            {
                return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            }

            private void TestDrop()
            {
                if (IsCtrlKeyDown)
                {
                    ShowsDockControlGuide = false;
                    PaneWithGuide = null;
                    if (_source.CanDrop(DockTreePosition.Floating))
                        Target = Target.Floating;
                    else
                        Target = Target.Empty;
                }
                else
                {
                    ShowsDockControlGuide = true;
                    DockPane pane = PaneAtPoint();
                    Debug.Assert(pane == null || (pane != null && _source.CanDrop(pane)));
                    PaneWithGuide = pane;
                    Target = TargetAtPoint();
                }
                SetCursor();
            }

            private bool _isCursorChanged;
            private Cursor _originalCursor;
            private void SetCursor()
            {
                FrameworkElement dragElement = DragElement as FrameworkElement;
                if (dragElement == null)
                    return;

                if (Target == Target.Empty)
                {
                    if (!_isCursorChanged)
                    {
                        _isCursorChanged = true;
                        _originalCursor = dragElement.Cursor;
                        dragElement.Cursor = Cursors.No;
                    }
                }
                else
                    RestoreCursor(dragElement);
            }

            private void RestoreCursor(FrameworkElement dragElement)
            {
                if (dragElement == null || !_isCursorChanged)
                    return;

                _isCursorChanged = false;
                dragElement.Cursor = _originalCursor;
            }

            private DockPane PaneAtPoint()
            {
                return WindowHandler.PaneAtPoint(_source, MouseDeltaX, MouseDeltaY);
            }

            private Target TargetAtPoint()
            {
                Target target = WindowHandler.TargetAtPoint(MouseDeltaX, MouseDeltaY);
                if (CanDropAtTarget(target))
                    return target;

                return (_source.CanDrop(DockTreePosition.Floating)) ? Target.Floating : Target.Empty;
            }

            private bool CanDropAtTarget(Target target)
            {
                if (target.TargetPosition == DropPosition.Tab)
                {
                    if (_source.CanDrop(target.TargetItem))
                        return true;
                }
                else if (target.TargetPane != null)
                {
                    if (_source.CanDrop(target.TargetPane))
                        return true;
                }
                else if (target.TargetPosition != DropPosition.None)
                    return true;

                return false;
            }

            private void Drop(Target target, bool isShiftKeyDown)
            {
                if (target.TargetPosition == DropPosition.Floating)
                {
                    Rect rect = Rect.Offset(_floatingWindowStartBounds, MouseDeltaX, MouseDeltaY);
                    _source.Drop(rect);
                }
                else if (target.TargetPosition == DropPosition.Tab)
                    _source.Drop(target.TargetItem);
                else if (target.TargetPane != null)
                    _source.Drop(target.TargetPane, GetDockPanePreviewPlacement(target.TargetPosition));
                else if (target.TargetPosition == DropPosition.Left)
                    _source.Drop(Dock.Left, isShiftKeyDown);
                else if (target.TargetPosition == DropPosition.Right)
                    _source.Drop(Dock.Right, isShiftKeyDown);
                else if (target.TargetPosition == DropPosition.Top)
                    _source.Drop(Dock.Top, isShiftKeyDown);
                else if (target.TargetPosition == DropPosition.Bottom)
                    _source.Drop(Dock.Bottom, isShiftKeyDown);
                else if (target.TargetPosition == DropPosition.Fill)
                    _source.Drop();
            }

            private static DockPanePreviewPlacement GetDockPanePreviewPlacement(DropPosition position)
            {
                if (position == DropPosition.Left)
                    return DockPanePreviewPlacement.Left;
                else if (position == DropPosition.Right)
                    return DockPanePreviewPlacement.Right;
                else if (position == DropPosition.Top)
                    return DockPanePreviewPlacement.Top;
                else if (position == DropPosition.Bottom)
                    return DockPanePreviewPlacement.Bottom;
                else
                {
                    Debug.Assert(position == DropPosition.Fill);
                    return DockPanePreviewPlacement.Fill;
                }
            }
        }
    }
}
