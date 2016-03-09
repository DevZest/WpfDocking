using System;
using System.Diagnostics;

namespace DevZest.Windows.Docking.Primitives
{
    partial class DockManager
    {
        internal struct Target
        {
            private DockItem _targetItem;
            private DropPosition _targetPosition;

            public static Target Empty
            {
                get { return new Target(); }
            }

            public static Target LeftToolWindow
            {
                get { return new Target(DockTreePosition.Left); }
            }

            public static Target RightToolWindow
            {
                get { return new Target(DockTreePosition.Right); }
            }

            public static Target TopToolWindow
            {
                get { return new Target(DockTreePosition.Top); }
            }

            public static Target BottomToolWindow
            {
                get { return new Target(DockTreePosition.Bottom); }
            }

            public static Target Document
            {
                get { return new Target(DockTreePosition.Document); }
            }

            public static Target Floating
            {
                get { return new Target(DockTreePosition.Floating); }
            }

            internal static Target Tab(DockItem targetItem)
            {
                Debug.Assert(targetItem != null);
                return new Target(targetItem);
            }

            internal static Target LeftPane(DockItem targetItem)
            {
                Debug.Assert(targetItem != null);
                Debug.Assert(targetItem.FirstPane != null);
                return new Target(targetItem.FirstPane, DockPanePreviewPlacement.Left);
            }

            internal static Target RightPane(DockItem targetItem)
            {
                Debug.Assert(targetItem != null);
                Debug.Assert(targetItem.FirstPane != null);
                return new Target(targetItem.FirstPane, DockPanePreviewPlacement.Right);
            }

            internal static Target TopPane(DockItem targetItem)
            {
                Debug.Assert(targetItem != null);
                Debug.Assert(targetItem.FirstPane != null);
                return new Target(targetItem.FirstPane, DockPanePreviewPlacement.Top);
            }

            internal static Target BottomPane(DockItem targetItem)
            {
                Debug.Assert(targetItem != null);
                Debug.Assert(targetItem.FirstPane != null);
                return new Target(targetItem.FirstPane, DockPanePreviewPlacement.Bottom);
            }

            internal static Target FillPane(DockItem targetItem)
            {
                Debug.Assert(targetItem != null);
                Debug.Assert(targetItem.FirstPane != null);
                return new Target(targetItem.FirstPane, DockPanePreviewPlacement.Fill);
            }

            private Target(DockTreePosition dockTreePosition)
            {
                _targetItem = null;
                if (dockTreePosition == DockTreePosition.Left)
                    _targetPosition = DropPosition.Left;
                else if (dockTreePosition == DockTreePosition.Right)
                    _targetPosition = DropPosition.Right;
                else if (dockTreePosition == DockTreePosition.Top)
                    _targetPosition = DropPosition.Top;
                else if (dockTreePosition == DockTreePosition.Bottom)
                    _targetPosition = DropPosition.Bottom;
                else if (dockTreePosition == DockTreePosition.Document)
                    _targetPosition = DropPosition.Fill;
                else
                {
                    Debug.Assert(dockTreePosition == DockTreePosition.Floating);
                    _targetPosition = DropPosition.Floating;
                }
            }

            private Target(DockItem targetItem)
            {
                Debug.Assert(targetItem != null);
                _targetItem = targetItem;
                _targetPosition = DropPosition.Tab;
            }

            private Target(DockPane pane, DockPanePreviewPlacement placement)
            {
                _targetItem = pane.SelectedItem;
                if (placement == DockPanePreviewPlacement.Left)
                    _targetPosition = DropPosition.Left;
                else if (placement == DockPanePreviewPlacement.Right)
                    _targetPosition = DropPosition.Right;
                else if (placement == DockPanePreviewPlacement.Top)
                    _targetPosition = DropPosition.Top;
                else if (placement == DockPanePreviewPlacement.Bottom)
                    _targetPosition = DropPosition.Bottom;
                else
                {
                    Debug.Assert(placement == DockPanePreviewPlacement.Fill);
                    _targetPosition = DropPosition.Fill;
                }
            }

            public DockItem TargetItem
            {
                get { return _targetItem; }
            }

            public DockPane TargetPane
            {
                get { return _targetItem == null ? null : _targetItem.FirstPane; }
            }

            public DropPosition TargetPosition
            {
                get { return _targetPosition; }
            }

            public override bool Equals(object obj)
            {
                if (obj is Target)
                {
                    Target value = (Target)obj;
                    return Equals(value);
                }
                else
                    return false;
            }

            private bool Equals(Target value)
            {
                return _targetItem == value.TargetItem && _targetPosition == value.TargetPosition;
            }

            public override int GetHashCode()
            {
                return _targetItem.GetHashCode() << 3 + (int)_targetPosition;
            }

            public static bool operator ==(Target value1, Target value2)
            {
                return value1.Equals(value2);
            }

            public static bool operator !=(Target value1, Target value2)
            {
                return !value1.Equals(value2);
            }
        }
    }
}
