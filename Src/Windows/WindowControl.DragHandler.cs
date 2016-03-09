using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DevZest.Windows
{
    partial class WindowControl
    {
        private class DragHandler : DragHandlerBase
        {
            private static DragHandler s_default;
            public static DragHandler Default
            {
                get
                {
                    if (s_default == null)
                        s_default = new DragHandler();
                    return s_default;
                }
            }

            private IWindow _window;
            private WindowHotspot _hotspot = WindowHotspot.None;
            private Rect _originWindowBounds;
            private Rect _lastWindowBounds;

            private DragHandler()
            {
            }

            private IWindow Window
            {
                get { return _window; }
            }

            public void BeginDrag(IWindow window, UIElement source, MouseEventArgs e)
            {
                _window = window;
                _hotspot = WindowControl.GetHotspot(source);

                Window.UpdateLayout();
                _lastWindowBounds = window.ActualBounds;
                _originWindowBounds = window.Bounds;
                DragDetect(source, e);
            }

            protected override void OnBeginDrag()
            {
            }

            protected override void OnDragDelta()
            {
                double deltaX = MouseDeltaX;
                double deltaY = MouseDeltaY;

                double width = GetWidth(ref deltaX);
                double height = GetHeight(ref deltaY);

                double left = GetLeft(deltaX);
                double top = GetTop(deltaY);

                _lastWindowBounds = new Rect(left, top, width, height);
                SetWindowBounds(_lastWindowBounds);
            }

            private double GetLeft(double horizontalChange)
            {
                if (_hotspot == WindowHotspot.Move ||
                    _hotspot == WindowHotspot.ResizeLeft ||
                    _hotspot == WindowHotspot.ResizeLeftTop ||
                    _hotspot == WindowHotspot.ResizeLeftBottom)
                    return _lastWindowBounds.Left + horizontalChange;
                else
                    return _lastWindowBounds.Left;
            }

            private double GetTop(double verticalChange)
            {
                if (_hotspot == WindowHotspot.Move ||
                    _hotspot == WindowHotspot.ResizeLeftTop ||
                    _hotspot == WindowHotspot.ResizeTop ||
                    _hotspot == WindowHotspot.ResizeRightTop)
                    return _lastWindowBounds.Top + verticalChange;
                else
                    return _lastWindowBounds.Top;

            }

            private double GetWidth(ref double horizontalChange)
            {
                if (_hotspot == WindowHotspot.Move)
                    return _originWindowBounds.Width;

                double sign, change, width;

                if (_hotspot == WindowHotspot.ResizeLeft ||
                    _hotspot == WindowHotspot.ResizeLeftTop ||
                    _hotspot == WindowHotspot.ResizeLeftBottom)
                {
                    sign = -1;
                    change = horizontalChange;
                }
                else if (_hotspot == WindowHotspot.ResizeRight ||
                    _hotspot == WindowHotspot.ResizeRightTop ||
                    _hotspot == WindowHotspot.ResizeRightBottom)
                {
                    sign = 1;
                    change = horizontalChange;
                }
                else
                    sign = change = 0;

                width = _lastWindowBounds.Width + sign * change;
                if (width > Window.MaxWidth)
                {
                    horizontalChange -= sign * (width - Window.MaxWidth);
                    width = Window.MaxWidth;
                }
                if (width < Window.MinWidth)
                {
                    horizontalChange += width - Window.MinWidth;
                    width = Window.MinWidth;
                }

                return width;
            }

            private double GetHeight(ref double verticalChange)
            {
                if (_hotspot == WindowHotspot.Move)
                    return _originWindowBounds.Height;

                double sign, change, height;

                if (_hotspot == WindowHotspot.ResizeTop ||
                    _hotspot == WindowHotspot.ResizeLeftTop ||
                    _hotspot == WindowHotspot.ResizeRightTop)
                {
                    sign = -1;
                    change = verticalChange;
                }
                else if (_hotspot == WindowHotspot.ResizeBottom ||
                    _hotspot == WindowHotspot.ResizeLeftBottom ||
                    _hotspot == WindowHotspot.ResizeRightBottom)
                {
                    sign = 1;
                    change = verticalChange;
                }
                else
                    sign = change = 0; ;

                height = _lastWindowBounds.Height + sign * change;
                if (height > Window.MaxHeight)
                {
                    verticalChange -= sign * (height - Window.MaxHeight);
                    height = Window.MaxHeight;
                }
                if (height < Window.MinHeight)
                {
                    verticalChange += height - Window.MinHeight;
                    height = Window.MinHeight;
                }

                return height;
            }

            protected override void OnEndDrag(UIElement dragElement, bool abort)
            {
                if (abort)
                    SetWindowBounds(_originWindowBounds);
                _window = null;
            }

            private void SetWindowBounds(Rect newBounds)
            {
                Window.SetBounds(newBounds);
            }
        }
    }
}
