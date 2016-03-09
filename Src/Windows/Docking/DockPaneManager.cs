using System;
using System.Threading;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Security;
using System.Security.Permissions;

namespace DevZest.Windows.Docking
{
    internal class DockPaneManager
    {
        private DockControl _dockControl;
        private DockPaneCollection _createdPanes = new DockPaneCollection();
        private DockPaneCollection _activatedPanes = new DockPaneCollection();
        private DockPaneCollection _panes = new DockPaneCollection();
        private DockItemCollection _documents = new DockItemCollection();
        private bool _isDockItemStateChanging;
        private DockItem _saveFocusItem;
        bool _focusedItemChanged, _activeItemChanged, _activeDocumentChanged;

        internal DockPaneManager(DockControl dockControl)
        {
            _dockControl = dockControl;
        }

        internal DockPane CreatePane(DockItem item, bool isAutoHide)
        {
            DockPane pane = new DockPane(item, isAutoHide);
            _createdPanes.Add(pane);
            return pane;
        }

        internal DockPaneCollection Panes
        {
            get { return _panes; }
        }

        internal DockItemCollection Documents
        {
            get { return _documents; }
        }

        private bool FlagSaveFocus
        {
            get { return _saveFocusItem != null; }
        }

        internal void SaveFocus()
        {
            if (_saveFocusItem != null || _dockControl.FocusedItem == null)
                return;

            _saveFocusItem = _dockControl.FocusedItem;
            Keyboard.Focus(_dockControl);
        }

        internal void RestoreFocus()
        {
            if (_saveFocusItem == null)
                return;

            if (_saveFocusItem.DockPosition != DockPosition.Hidden && _saveFocusItem.DockPosition != DockPosition.Unknown)
                Focus(_saveFocusItem);
            _saveFocusItem = null;
        }

        internal void Activate(DockItem item)
        {
            if (!item.IsSelected)
                throw new InvalidOperationException();
            Debug.Assert(item.IsSelected);

            if (_dockControl.FocusedItem != item || FlagSaveFocus)
            {
                Focus(item);
                _saveFocusItem = null;
            }
        }

        private void Focus(DockItem dockItem)
        {
            Debug.Assert(dockItem != null);
            _dockControl.UpdateLayout();
            if (FloatingWindow.CanBeNative)
                DoEvents(); // Wait for native floating window to show
            Keyboard.Focus(dockItem);
            if (!(DesignerProperties.GetIsInDesignMode(_dockControl) || dockItem.IsKeyboardFocusWithin))
                Trace.TraceError(@"Set focus to DockItem ""{0}"" failed. This normally indicates the DockControl is not properly styled.", dockItem.TabText);
        }

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        private static void DoEvents()
        {
            DispatcherFrame f = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
            (SendOrPostCallback)delegate(object arg)
            {
                DispatcherFrame fr = arg as DispatcherFrame;
                fr.Continue = false;
            }, f);
            Dispatcher.PushFrame(f);
        }

        internal void OnDockItemFocusEnter(DockItem item)
        {
            Debug.Assert(item.IsSelected);
            DockPane pane = item.FirstPane;
            FloatingWindow floatingWindow = pane.FloatingWindow;
            _dockControl.FloatingWindows.BringToFront(floatingWindow);
            BringToFront(pane);
            FocusedItem = item;
            if (!_isDockItemStateChanging)
            {
                CoerceValues();
                RaiseEvents();
            }
        }

        private void BringToFront(DockPane pane)
        {
            if (_activatedPanes.Count > 0 && _activatedPanes[_activatedPanes.Count - 1] == pane)
                return;

            if (_activatedPanes.Contains(pane))
                _activatedPanes.Remove(pane);
            _activatedPanes.Add(pane);
        }

        internal void OnDockItemFocusLeave(DockItem item)
        {
            if (_isDockItemStateChanging || _dockControl.FocusedItem != item || FlagSaveFocus)
                return;

            _dockControl.Dispatcher.BeginInvoke(DispatcherPriority.Send,
                new ThreadStart(delegate
                {
                    DockItem focusedItem = _dockControl.FocusedItem;
                    if (focusedItem != null && !focusedItem.IsKeyboardFocusWithin)
                    {
                        FocusedItem = null;
                        RaiseEvents();
                    }
                }));
        }

        private DockItem FocusedItem
        {
            get { return _dockControl.FocusedItem; }
            set
            {
                Debug.Assert(value == null || value.IsSelected);
                DockItem oldValue = FocusedItem;
                _dockControl.SetValue(DockControl.FocusedItemPropertyKey, value);
                if (oldValue != value)
                    _focusedItemChanged = true;
            }
        }

        private void CoerceValues()
        {
            UpdateCreatedPanes();
            UpdateActivatedPanes();
            UpdatePanes();
            UpdateDockTreeActivePanes();
            UpdateDocuments();
            CoerceActiveItem();
            CoerceActiveDocument();
        }

        private void UpdateCreatedPanes()
        {
            for (int i = _createdPanes.Count - 1; i >= 0; i--)
            {
                DockPane pane = _createdPanes[i];
                if (pane.DockPosition == DockPosition.Unknown)
                    _createdPanes.Remove(pane);
            }
        }

        private void UpdateActivatedPanes()
        {
            int lastActivePaneIndex = _activatedPanes.Count - 1;
            DockPane lastActivePane = lastActivePaneIndex >= 0 ? _activatedPanes[lastActivePaneIndex] : null;
            if (lastActivePane != null)
            {
                DockItem selectedItem = lastActivePane.SelectedItem;
                if (selectedItem == null || !selectedItem.IsSelected)
                    _activatedPanes.Remove(lastActivePane);
            }

            for (int i = lastActivePaneIndex - 1; i >= 0; i--)
            {
                DockPane pane = _activatedPanes[i];
                if (pane.SelectedItem == null || pane.IsAutoHide)
                    _activatedPanes.Remove(pane);
            }
        }

        private void UpdatePanes()
        {
            CollectionUtil.Synchronize(GetPanes(),
                _panes,
                delegate(int index, DockPane item) { _panes.Insert(index, item); },
                delegate(DockPane item) { _panes.Remove(item); });
            Debug.Assert(_panes.Count == _createdPanes.Count);
        }

        private IEnumerable<DockPane> GetPanes()
        {
            int activatedPanesCount = _activatedPanes.Count;
            DockPane lastActivePane = activatedPanesCount > 0 ? _activatedPanes[activatedPanesCount - 1] : null;
            foreach (DockPane pane in _createdPanes)
            {
                if (pane.SelectedItem == null || (pane.IsAutoHide && pane != lastActivePane))
                    yield return pane;
            }

            foreach (FloatingWindow floatingWindow in _dockControl.FloatingWindows.GetFloatingWindowsWithDockControl())
            {
                foreach (DockPane pane in _createdPanes)
                {
                    if (pane.SelectedItem == null || (pane.IsAutoHide && pane != lastActivePane))
                        continue;

                    if (pane.FloatingWindow == floatingWindow && !_activatedPanes.Contains(pane))
                        yield return pane;
                }

                foreach (DockPane pane in _activatedPanes)
                {
                    if (pane.FloatingWindow == floatingWindow)
                        yield return pane;
                }
            }
        }

        private void UpdateDockTreeActivePanes()
        {
            UpdateActivePanes(_dockControl.GetDockTree(DockControlTreePosition.Left));
            UpdateActivePanes(_dockControl.GetDockTree(DockControlTreePosition.Right));
            UpdateActivePanes(_dockControl.GetDockTree(DockControlTreePosition.Top));
            UpdateActivePanes(_dockControl.GetDockTree(DockControlTreePosition.Bottom));
            UpdateActivePanes(_dockControl.GetDockTree(DockControlTreePosition.Document));
            foreach (FloatingWindow floatingWindow in _dockControl.FloatingWindows)
                UpdateActivePanes(floatingWindow.DockTree);
        }

        private void UpdateActivePanes(DockTree dockTree)
        {
            CollectionUtil.Synchronize(GetActivePanes(dockTree),
                dockTree.ActivePanes,
                delegate(int index, DockPane item) { dockTree.ActivePanes.Insert(index, item); },
                delegate(DockPane item) { dockTree.ActivePanes.Remove(item); });
            if (dockTree.ActivePanes.Count != dockTree.VisiblePanes.Count)
                throw new InvalidOperationException();

            Debug.Assert(dockTree.ActivePanes.Count == dockTree.VisiblePanes.Count);
        }

        private IEnumerable<DockPane> GetActivePanes(DockTree dockTree)
        {
            foreach (DockPane dockPane in dockTree.VisiblePanes)
            {
                if (!_activatedPanes.Contains(dockPane))
                    yield return dockPane;
            }

            foreach (DockPane dockPane in _activatedPanes)
            {
                if (dockPane.DockTree == dockTree && dockTree.VisiblePanes.Contains(dockPane))
                    yield return dockPane;
            }
        }

        private void UpdateDocuments()
        {
            CollectionUtil.Synchronize(GetDocuments(),
                _documents,
                delegate(int index, DockItem item) { _documents.Insert(index, item); },
                delegate(DockItem item) { _documents.Remove(item); });
        }

        private IEnumerable<DockItem> GetDocuments()
        {
            foreach (DockPane pane in _panes)
            {
                if (pane.DockPosition != DockPosition.Document)
                    continue;

                foreach (DockItem item in pane.ActiveItems)
                    yield return item;
            }
        }

        private void CoerceActiveItem()
        {
            ActiveItem = GetActiveItem();
        }

        private DockItem GetActiveItem()
        {
            DockItem activeItem = _panes.Count > 0 ? _panes[_panes.Count - 1].SelectedItem : null;
            if (activeItem == null || !activeItem.IsSelected)
                return null;
            else if (DockPositionHelper.IsAutoHide(activeItem.DockPosition) && !activeItem.IsKeyboardFocusWithin)
                return null;
            else
                return activeItem;
        }

        private DockItem ActiveItem
        {
            get { return _dockControl.ActiveItem; }
            set
            {
                Debug.Assert(value == null || value.IsSelected);

                DockItem oldValue = ActiveItem;
                if (oldValue == value)
                    return;

                _dockControl.SetValue(DockControl.ActiveItemPropertyKey, value);

                if (oldValue != null)
                    oldValue.IsActiveItem = false;
                if (value != null)
                    value.IsActiveItem = true;
                _activeItemChanged = true;
            }
        }

        private void CoerceActiveDocument()
        {
            ActiveDocument = _documents.Count > 0 ? _documents[_documents.Count - 1] : null;
        }

        private DockItem ActiveDocument
        {
            get { return _dockControl.ActiveDocument; }
            set
            {
                Debug.Assert(value == null || (value.DockPosition == DockPosition.Document && value.IsSelected));

                DockItem oldValue = ActiveDocument;
                _dockControl.SetValue(DockControl.ActiveDocumentPropertyKey, value);

                if (oldValue != value)
                {
                    if (oldValue != null)
                        oldValue.IsActiveDocument = false;
                    if (value != null)
                        value.IsActiveDocument = true;
                    _activeDocumentChanged = true;
                }
            }
        }

        internal void OnDockItemStateChanging(DockItemStateEventArgs e)
        {
            _isDockItemStateChanging = true;
            _dockControl.SaveFocus();
            _dockControl.OnDockItemStateChanging(e);
        }

        internal void OnDockItemStateChanged(DockItemStateEventArgs e)
        {
            if (_isDockItemStateChanging)
            {
                if (FlagSaveFocus)
                {
                    DockItem focusedItem = FocusedItem;
                    if (focusedItem.IsAutoHide)
                        _activatedPanes.Remove(focusedItem.FirstPane);

                    CoerceValues();
                    
                    DockItem activeItem = ActiveItem;
                    if (activeItem != null)
                        activeItem.Activate();
                    else if (focusedItem != null)
                        FocusedItem = null;

                    _saveFocusItem = null;
                }
                CoerceValues();
                _isDockItemStateChanging = false;
            }
            _dockControl.OnDockItemStateChanged(e);
            RaiseEvents();
        }

        private void RaiseEvents()
        {
            if (_focusedItemChanged)
            {
                _focusedItemChanged = false;
                _dockControl.OnFocusedItemChanged(EventArgs.Empty);
            }
            if (_activeItemChanged)
            {
                _activeItemChanged = false;
                _dockControl.OnActiveItemChanged(EventArgs.Empty);
            }
            if (_activeDocumentChanged)
            {
                _activeDocumentChanged = false;
                _dockControl.OnActiveDocumentChanged(EventArgs.Empty);
            }
            _dockControl.RaiseSelectedAutoHideItemChangedEvent();
        }
    }
}
