using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DevZest.Windows
{
    partial class SplitContainer
    {
        private class DragHandler : DragHandlerBase
        {
            private static DragHandler s_default;
            
            private SplitContainer _splitContainer;
            private SplitterDistance _originalSplitterDistance;

            public static DragHandler Default
            {
                get
                {
                    if (s_default == null)
                        s_default = new DragHandler();
                    return s_default;
                }
            }

            private DragHandler()
            {
            }

            private bool ShowsPreview
            {
                get { return _splitContainer.ShowsPreview; }
            }

            private double DragIncrement
            {
                get { return _splitContainer.DragIncrement; }
            }

            private bool IsPreviewVisible
            {
                get { return _splitContainer.IsPreviewVisible; }
            }

            private void ShowPreview()
            {
                Debug.Assert(!IsPreviewVisible);
                _splitContainer.IsPreviewVisible = true;
                PreviewOffsetX = PreviewOffsetY = 0;
            }

            private void ClosePreview()
            {
                Debug.Assert(IsPreviewVisible);
                PreviewOffsetX = PreviewOffsetY = 0;
                _splitContainer.IsPreviewVisible = false;
            }

            private double PreviewOffsetX
            {
                set
                {
                    Debug.Assert(IsPreviewVisible);
                    _splitContainer.PreviewOffsetX = value;
                }
            }

            private double PreviewOffsetY
            {
                set
                {
                    Debug.Assert(IsPreviewVisible);
                    _splitContainer.PreviewOffsetY = value;
                }
            }

            private void MoveSplitter(double offsetX, double offsetY)
            {
                _splitContainer.MoveSplitter(offsetX, offsetY);
            }

            public void BeginDrag(SplitContainer splitContainer, MouseEventArgs e)
            {
                _splitContainer = splitContainer;
                _originalSplitterDistance = _splitContainer.SplitterDistance;
                Debug.Assert(splitContainer.SplitterPresenter != null);
                DragDetect((UIElement)VisualTreeHelper.GetChild(splitContainer.SplitterPresenter, 0), e);
            }

            protected override void OnBeginDrag()
            {
                if (ShowsPreview)
                    ShowPreview();
            }

            protected override void OnDragDelta()
            {
                Point offset = GetOffset(MouseDeltaX, MouseDeltaY);
                if (IsPreviewVisible)
                {
                    PreviewOffsetX = offset.X;
                    PreviewOffsetY = offset.Y;
                }
                else
                    MoveSplitter(offset.X, offset.Y);
            }

            private Point GetOffset(double horizontalChange, double verticalChange)
            {
                return _splitContainer.GetOffset(horizontalChange, verticalChange, DragIncrement);
            }

            protected override void OnEndDrag(UIElement dragElement, bool abort)
            {
                if (IsPreviewVisible)
                    ClosePreview();

                if (abort)
                    _splitContainer.SplitterDistance = _originalSplitterDistance;
                else
                {
                    Point offset = GetOffset(MouseDeltaX, MouseDeltaY);
                    MoveSplitter(offset.X, offset.Y);
                }
            }
        }
    }
}
