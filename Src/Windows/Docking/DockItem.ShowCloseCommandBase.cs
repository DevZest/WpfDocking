using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking
{
    partial class DockItem
    {
        private abstract class ShowCloseCommandBase : CommandBase
        {
            private IDockItemUndoRedoReference _undoRedoReference;
            private ShowAction _firstPaneShowAction;
            private ShowAction _secondPaneShowAction;

            protected ShowCloseCommandBase(DockItem dockItem)
                : base(dockItem)
            {
                Initialize(dockItem);
            }

            private void Initialize(DockItem dockItem)
            {
                _undoRedoReference = dockItem.UndoRedoReference;
                if (dockItem.FirstPane != null)
                {
                    _firstPaneShowAction = GetShowAction(dockItem, dockItem.FirstPane);
                    Debug.Assert(DockItemIndex == _firstPaneShowAction.Source);
                }
                if (dockItem.SecondPane != null)
                {
                    _secondPaneShowAction = GetShowAction(dockItem, dockItem.SecondPane);
                    Debug.Assert(DockItemIndex == _secondPaneShowAction.Source);
                }
            }

            protected IDockItemUndoRedoReference UndoRedoReference
            {
                get { return _undoRedoReference; }
            }

            protected ShowAction FirstPaneShowAction
            {
                get { return _firstPaneShowAction; }
            }

            protected ShowAction SecondPaneShowAction
            {
                get { return _secondPaneShowAction; }
            }
        }
    }
}
