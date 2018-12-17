using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DevZest.Windows.Docking
{
    /// <summary>Represents a control that contains multiple dockable items.</summary>
    /// <remarks>
    /// <para><see cref="DockControl"/> is the center of WPF docking library. It provides the following key services:</para>
    /// <para>
    /// <list type="table">
    ///     <listheader>
    ///         <term>Service</term>
    ///         <description>Properties and Methods</description>
    ///     </listheader>
    ///     <item>
    ///         <term>DockItem Management</term>
    ///         <description><see cref="DockItems"/>, <see cref="Documents"/>, <see cref="FocusedItem"/>, <see cref="ActiveItem"/>, <see cref="ActiveDocument"/>, <see cref="SelectedAutoHideItem"/></description>
    ///     </item>
    ///     <item>
    ///         <term>DockTree Management</term>
    ///         <description><see cref="LeftDockTree"/>, <see cref="RightDockTree"/>, <see cref="TopDockTree"/>, <see cref="BottomDockTree"/>, <see cref="DocumentDockTree"/>, <see cref="LeftDockTreeWidth"/>, <see cref="RightDockTreeWidth"/>, <see cref="TopDockTreeHeight"/>, <see cref="BottomDockTreeHeight"/>, <see cref="DockTreeZOrder"/></description>
    ///     </item>
    ///     <item>
    ///         <term>Floating Window Management</term>
    ///         <description><see cref="FloatingWindows"/>, <see cref="DefaultFloatingWindowSize"/></description>
    ///     </item>
    ///     <item>
    ///         <term>Save/Load</term>
    ///         <description><see cref="Save"/>, <see cref="Load"/></description>
    ///     </item>
    ///     <item>
    ///         <term>Undo/Redo</term>
    ///         <description><see cref="MaxUndoLevel"/>, <see cref="CanUndo"/>, <see cref="CanRedo"/>, <see cref="Undo"/>, <see cref="Redo"/>, <see cref="BeginUndoUnit"/>, <see cref="EndUndoUnit"/>, <see cref="UndoUnitLevel"/></description>
    ///     </item>
    /// </list>
    /// </para>
    /// <para>By default <see cref="DockControl"/> also uses following services that you can customize through the provided attached properties:</para>
    /// <para>
    /// <list type="table">
    ///     <listheader>
    ///         <term>Service</term>
    ///         <description>Attached Properties</description>
    ///     </listheader>
    ///     <item>
    ///         <term>AutoHide</term>
    ///         <description><see cref="P:DevZest.Windows.Docking.Primitives.AutoHide.Animation" />, <see cref="P:DevZest.Windows.Docking.Primitives.AutoHide.AnimationDuration" />.</description>
    ///     </item>
    ///     <item>
    ///         <term>DockManager</term>
    ///         <description><see cref="P:DevZest.Windows.Docking.Primitives.DockManager.DefaultFloatingPreviewSize" />, <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.FloatingWindowStrategy" />.</description>
    ///     </item>
    ///     <item>
    ///         <term>DocumentTab</term>
    ///         <description><see cref="P:DevZest.Windows.Docking.Primitives.DocumentTab.ShowsIcon" />.</description>
    ///     </item>
    ///     <item>
    ///         <term>WindowSwitcher</term>
    ///         <description><see cref="P:DevZest.Windows.Docking.Primitives.WindowSwitcher.Hotkey" /></description>
    ///     </item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    ///     <code lang="xaml" source="..\..\..\Samples\Docking\CSharp\QuickStart\Window1.xaml" />
    /// </example>
    [ContentProperty("DockItems")]
    public partial class DockControl : Control
    {
        private static readonly DependencyPropertyKey LeftDockTreePropertyKey;
        /// <summary>Identifies the <see cref="LeftDockTree"/> dependency property.</summary>
        public static readonly DependencyProperty LeftDockTreeProperty;
        private static readonly DependencyPropertyKey RightDockTreePropertyKey;
        /// <summary>Identifies the <see cref="RightDockTree"/> dependency property.</summary>
        public static readonly DependencyProperty RightDockTreeProperty;
        private static readonly DependencyPropertyKey TopDockTreePropertyKey;
        /// <summary>Identifies the <see cref="TopDockTree"/> dependency property.</summary>
        public static readonly DependencyProperty TopDockTreeProperty;
        private static readonly DependencyPropertyKey BottomDockTreePropertyKey;
        /// <summary>Identifies the <see cref="BottomDockTree"/> dependency property.</summary>
        public static readonly DependencyProperty BottomDockTreeProperty;
        private static readonly DependencyPropertyKey DocumentDockTreePropertyKey;
        /// <summary>Identifies the <see cref="DocumentDockTree"/> dependency property.</summary>
        public static readonly DependencyProperty DocumentDockTreeProperty;
        /// <summary>Identifies the <see cref="LeftDockTreeWidth"/> dependency property.</summary>
        public static readonly DependencyProperty LeftDockTreeWidthProperty;
        /// <summary>Identifies the <see cref="RightDockTreeWidth"/> dependency property.</summary>
        public static readonly DependencyProperty RightDockTreeWidthProperty;
        /// <summary>Identifies the <see cref="TopDockTreeHeight"/> dependency property.</summary>
        public static readonly DependencyProperty TopDockTreeHeightProperty;
        /// <summary>Identifies the <see cref="BottomDockTreeHeight"/> dependency property.</summary>
        public static readonly DependencyProperty BottomDockTreeHeightProperty;
        /// <summary>Identifies the <see cref="DockTreeZOrder"/> dependency property.</summary>
        public static readonly DependencyProperty DockTreeZOrderProperty;
        internal static readonly DependencyPropertyKey FocusedItemPropertyKey;
        internal static readonly DependencyPropertyKey ActiveItemPropertyKey;
        internal static readonly DependencyPropertyKey ActiveDocumentPropertyKey;
        private static readonly DependencyPropertyKey SelectedAutoHideItemPropertyKey;
        /// <summary>Identifies the <see cref="FocusedItem"/> dependency property.</summary>
        public static readonly DependencyProperty FocusedItemProperty;
        /// <summary>Identifies the <see cref="ActiveItem"/> dependency property.</summary>
        public static readonly DependencyProperty ActiveItemProperty;
        /// <summary>Identifies the <see cref="ActiveDocument"/> dependency property.</summary>
        public static readonly DependencyProperty ActiveDocumentProperty;
        /// <summary>Identifies the <see cref="SelectedAutoHideItem"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedAutoHideItemProperty;
        /// <summary>Identifies the <see cref="DefaultFloatingWindowSize"/> dependency property.</summary>
        public static readonly DependencyProperty DefaultFloatingWindowSizeProperty;
        /// <summary>Identifies the <see cref="MaxUndoLevel"/> dependency property.</summary>
        public static readonly DependencyProperty MaxUndoLevelProperty;
        private static readonly DependencyPropertyKey CanUndoPropertyKey;
        /// <summary>Identifies the <see cref="CanUndo"/> dependency property.</summary>
        public static readonly DependencyProperty CanUndoProperty;
        private static readonly DependencyPropertyKey CanRedoPropertyKey;
        /// <summary>Identifies the <see cref="CanRedo"/> dependency property.</summary>
        public static readonly DependencyProperty CanRedoProperty;

        internal DockPaneManager PaneManager;
        private bool _selectedAutoHideItemChanged;
        private DockItemCollection _dockItems;
        private FloatingWindowCollection _floatingWindows;
        private UndoRedoStack<ICommand> _undoStack;
        private UndoRedoStack<ICommand> _redoStack;
        private int _undoUnitLevel;
        private List<ICommand> _undoUnitCommands = new List<ICommand>();
        private int _executionLevel;
        private RecycleReference<ICommand> _recycleUndoCommand = new RecycleReference<ICommand>();

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static DockControl()
        {
            FocusableProperty.OverrideMetadata(typeof(DockControl), new FrameworkPropertyMetadata(BooleanBoxes.True));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockControl), new FrameworkPropertyMetadata(typeof(DockControl)));
            
            LeftDockTreePropertyKey = DependencyProperty.RegisterReadOnly("LeftDockTree", typeof(DockTree), typeof(DockControl),
                new FrameworkPropertyMetadata());
            LeftDockTreeProperty = LeftDockTreePropertyKey.DependencyProperty;

            RightDockTreePropertyKey = DependencyProperty.RegisterReadOnly("RightDockTree", typeof(DockTree), typeof(DockControl),
                new FrameworkPropertyMetadata());
            RightDockTreeProperty = RightDockTreePropertyKey.DependencyProperty;
            
            TopDockTreePropertyKey = DependencyProperty.RegisterReadOnly("TopDockTree", typeof(DockTree), typeof(DockControl),
                new FrameworkPropertyMetadata());
            TopDockTreeProperty = TopDockTreePropertyKey.DependencyProperty;
            
            BottomDockTreePropertyKey = DependencyProperty.RegisterReadOnly("BottomDockTree", typeof(DockTree), typeof(DockControl),
                new FrameworkPropertyMetadata());
            BottomDockTreeProperty = BottomDockTreePropertyKey.DependencyProperty;

            DocumentDockTreePropertyKey = DependencyProperty.RegisterReadOnly("DocumentDockTree", typeof(DockTree), typeof(DockControl),
                new FrameworkPropertyMetadata());
            DocumentDockTreeProperty = DocumentDockTreePropertyKey.DependencyProperty;

            LeftDockTreeWidthProperty = DependencyProperty.Register("LeftDockTreeWidth", typeof(SplitterDistance), typeof(DockControl),
                new FrameworkPropertyMetadata(new SplitterDistance(1d / 3d, SplitterUnitType.Star),
                new PropertyChangedCallback(OnDockTreeSizeChanged)));
            RightDockTreeWidthProperty = DependencyProperty.Register("RightDockTreeWidth", typeof(SplitterDistance), typeof(DockControl),
                new FrameworkPropertyMetadata(new SplitterDistance(1d / 3d, SplitterUnitType.Star),
                new PropertyChangedCallback(OnDockTreeSizeChanged)));
            TopDockTreeHeightProperty = DependencyProperty.Register("TopDockTreeHeight", typeof(SplitterDistance), typeof(DockControl),
                new FrameworkPropertyMetadata(new SplitterDistance(1d / 3d, SplitterUnitType.Star),
                new PropertyChangedCallback(OnDockTreeSizeChanged)));
            BottomDockTreeHeightProperty = DependencyProperty.Register("BottomDockTreeHeight", typeof(SplitterDistance), typeof(DockControl),
                new FrameworkPropertyMetadata(new SplitterDistance(1d / 3d, SplitterUnitType.Star),
                new PropertyChangedCallback(OnDockTreeSizeChanged)));
            DockTreeZOrderProperty = DependencyProperty.Register("DockTreeZOrder", typeof(DockTreeZOrder), typeof(DockControl),
                new FrameworkPropertyMetadata(DockTreeZOrder.Default, new PropertyChangedCallback(OnDockTreeZOrderChanged)));
            FocusedItemPropertyKey = DependencyProperty.RegisterReadOnly("FocusedItem", typeof(DockItem), typeof(DockControl),
                new FrameworkPropertyMetadata(null));
            ActiveItemPropertyKey = DependencyProperty.RegisterReadOnly("ActiveItem", typeof(DockItem), typeof(DockControl),
                new FrameworkPropertyMetadata(null));
            ActiveDocumentPropertyKey = DependencyProperty.RegisterReadOnly("ActiveDocument", typeof(DockItem), typeof(DockControl),
                new FrameworkPropertyMetadata(null));
            SelectedAutoHideItemPropertyKey = DependencyProperty.RegisterReadOnly("SelectedAutoHideItem", typeof(DockItem), typeof(DockControl),
                new FrameworkPropertyMetadata(null));
            FocusedItemProperty = FocusedItemPropertyKey.DependencyProperty;
            ActiveItemProperty = ActiveItemPropertyKey.DependencyProperty;
            ActiveDocumentProperty = ActiveDocumentPropertyKey.DependencyProperty;
            SelectedAutoHideItemProperty = SelectedAutoHideItemPropertyKey.DependencyProperty;
            DefaultFloatingWindowSizeProperty = DependencyProperty.Register("DefaultFloatingWindowSize", typeof(Size), typeof(DockControl),
                new FrameworkPropertyMetadata(new Size(300, 300)));
            MaxUndoLevelProperty = DependencyProperty.Register("MaxUndoLevel", typeof(int), typeof(DockControl),
                new FrameworkPropertyMetadata(int.MaxValue, new PropertyChangedCallback(OnMaxUndoLevelChanged)), new ValidateValueCallback(ValidateMaxUndoLevel));
            CanUndoPropertyKey = DependencyProperty.RegisterReadOnly("CanUndo", typeof(bool), typeof(DockControl),
                new FrameworkPropertyMetadata(BooleanBoxes.False, new PropertyChangedCallback(OnCanUndoCanRedoChanged)));
            CanUndoProperty = CanUndoPropertyKey.DependencyProperty;
            CanRedoPropertyKey = DependencyProperty.RegisterReadOnly("CanRedo", typeof(bool), typeof(DockControl),
                new FrameworkPropertyMetadata(BooleanBoxes.False, new PropertyChangedCallback(OnCanUndoCanRedoChanged)));
            CanRedoProperty = CanRedoPropertyKey.DependencyProperty;
            CommandBinding undoCommandBinding = new CommandBinding(DockCommands.Undo,
                new ExecutedRoutedEventHandler(OnUndoCommandExecuted),
                new CanExecuteRoutedEventHandler(CanExecuteUndoCommand));
            CommandBinding redoCommandBinding = new CommandBinding(DockCommands.Redo,
                new ExecutedRoutedEventHandler(OnRedoCommandExecuted),
                new CanExecuteRoutedEventHandler(CanExecuteRedoCommand));
            CommandManager.RegisterClassCommandBinding(typeof(DockControl), undoCommandBinding);
            CommandManager.RegisterClassCommandBinding(typeof(DockControl), redoCommandBinding);
        }

        private static void OnDockTreeSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockControl dockControl = (DockControl)d;
            Dock dock = GetDock(e.Property);
            ResizeDockTreeData oldValue = new ResizeDockTreeData(dock, (SplitterDistance)e.OldValue);
            ResizeDockTreeData newValue = new ResizeDockTreeData(dock, (SplitterDistance)e.NewValue);
            dockControl.OnValueChanged(oldValue, newValue,
                delegate(ResizeDockTreeData oldData, ResizeDockTreeData newData)
                {
                    Debug.Assert(oldData.Dock == newData.Dock);
                    if (oldData.Dock == Dock.Left)
                        return new ResizeLeftDockTreeCommand(oldData.Value, newData.Value);
                    else if (oldData.Dock == Dock.Right)
                        return new ResizeRightDockTreeCommand(oldData.Value, newData.Value);
                    else if (oldData.Dock == Dock.Top)
                        return new ResizeTopDockTreeCommand(oldData.Value, newData.Value);
                    else
                    {
                        Debug.Assert(oldData.Dock == Dock.Bottom);
                        return new ResizeBottomDockTreeCommand(oldData.Value, newData.Value);
                    }
                });
        }

        private static Dock GetDock(DependencyProperty dp)
        {
            if (dp == LeftDockTreeWidthProperty)
                return Dock.Left;
            else if (dp == RightDockTreeWidthProperty)
                return Dock.Right;
            else if (dp == TopDockTreeHeightProperty)
                return Dock.Top;
            else
            {
                Debug.Assert(dp == BottomDockTreeHeightProperty);
                return Dock.Bottom;
            }
        }

        private static void OnDockTreeZOrderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockControl dockControl = (DockControl)d;
            dockControl.OnValueChanged((DockTreeZOrder)e.OldValue, (DockTreeZOrder)e.NewValue,
                delegate(DockTreeZOrder oldValue, DockTreeZOrder newValue)
                {
                    return new UpdateDockTreeZOrderCommand(oldValue, newValue);
                });
        }

        private static void OnCanUndoCanRedoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
        }

        private static void OnMaxUndoLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockControl)d).OnMaxUndoLevelChanged((int)e.NewValue);
        }

        private void OnMaxUndoLevelChanged(int value)
        {
            if (value == 0)
                _undoUnitCommands.Clear();
            _undoStack.Capacity = _redoStack.Capacity = value;
            RefreshCanUndoCanRedo();
        }

        private static bool ValidateMaxUndoLevel(object value)
        {
            return ((int)value) >= 0;
        }

        private static void OnUndoCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ((DockControl)sender).Undo();
        }

        private static void CanExecuteUndoCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((DockControl)sender).CanUndo;
            e.Handled = true;
        }

        private static void OnRedoCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ((DockControl)sender).Redo();
        }

        private static void CanExecuteRedoCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((DockControl)sender).CanRedo;
            e.Handled = true;
        }

        partial void VerifyLicense(bool designMode);

        /// <summary>Initializes a new instance of the <see cref="DockControl"/> class.</summary>
        public DockControl()
        {
            bool designMode = DesignerProperties.GetIsInDesignMode(this);
            VerifyLicense(designMode);
            _dockItems = new DockItemCollection(this);
            _floatingWindows = new FloatingWindowCollection();
            LeftDockTree = new DockTree(this, DockTreePosition.Left);
            RightDockTree = new DockTree(this, DockTreePosition.Right);
            TopDockTree = new DockTree(this, DockTreePosition.Top);
            BottomDockTree = new DockTree(this, DockTreePosition.Bottom);
            DocumentDockTree = new DockTree(this, DockTreePosition.Document);
            _undoStack = new UndoRedoStack<ICommand>(MaxUndoLevel);
            _redoStack = new UndoRedoStack<ICommand>(MaxUndoLevel);
            PaneManager = new DockPaneManager(this);
            this.Loaded += new RoutedEventHandler(OnLoaded);
        }

        /// <summary>Gets a collection of <see cref="DockItem"/> objects currently associated with this <see cref="DockControl"/>.</summary>
        /// <value>A collection of <see cref="DockItem"/> objects, in order of first association with this <see cref="DockControl"/>.</value>
        /// <remarks><para>A <see cref="DockItem"/> is associated with <see cref="DockControl"/> when adding this
        /// <see cref="DockItem"/> in the colletion. <see cref="DockItem.DockControl">DockItem.DockControl</see>
        /// also reflects this association.</para>
        /// <para>Calling <see cref="DockItem.Close">DockItem.Close</see> removes
        /// the <see cref="DockItem"/> from the collection.</para>
        /// <para>Calling <see cref="DockItem.Show(DockControl)" /> implicitly
        /// adds <see cref="DockItem"/> in the collection if it's not yet associated.</para></remarks>
        public DockItemCollection DockItems
        {
            get { return _dockItems; }
        }

        /// <summary>Gets a collection of <see cref="FloatingWindow"/> objects currently created.</summary>
        /// <value>A collection of <see cref="FloatingWindow"/> objects currently created, in order of
        /// activation (last active last).</value>
        public FloatingWindowCollection FloatingWindows
        {
            get { return _floatingWindows; }
        }

        /// <summary>Gets a collection of <see cref="DockPane"/> objects currently created.</summary>
        /// <value>A collection of <see cref="DockPane"/> objects currently created, in order of
        /// activation (last active last).</value>
        public DockPaneCollection Panes
        {
            get { return PaneManager.Panes; }
        }

        /// <summary>Gets a collection of <see cref="DockItem"/> objects currently displayed as document.</summary>
        /// <value>A collection of <see cref="DockItem"/> objects currently displayed as document, in order of
        /// <see cref="DockPane"/> activation, then <see cref="DockItem"/> activation (last active last).</value>
        public DockItemCollection Documents
        {
            get { return PaneManager.Documents; }
        }

        /// <summary>Gets or sets the maximum number of undo/redo actions. This is a dependency property.</summary>
        /// <value>The maximum number of undo/redo actions. The default value is <see cref="Int32.MaxValue"/>.</value>
        public int MaxUndoLevel
        {
            get { return (int)GetValue(MaxUndoLevelProperty); }
            set { SetValue(MaxUndoLevelProperty, value); }
        }

        /// <summary>Gets a value that indicates whether the most recent action can be undone. This is a dependency property.</summary>
        /// <value><see langword="true"/> if the most recent action can be undone; otherwise, <see langword="false"/>. This property has no default value.</value>
        public bool CanUndo
        {
            get { return (bool)GetValue(CanUndoProperty); }
            private set { SetValue(CanUndoPropertyKey, BooleanBoxes.Box(value)); }
        }

        /// <summary>Gets a value that indicates whether the most recent action can be redone. This is a dependency property.</summary>
        /// <value><see langword="true"/> if the most recent action can be redone; otherwise, <see langword="false"/>. This property has no default value.</value>
        public bool CanRedo
        {
            get { return (bool)GetValue(CanRedoProperty); }
            private set { SetValue(CanRedoPropertyKey, BooleanBoxes.Box(value)); }
        }

        /// <exclude/>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                foreach (DockItem item in DockItems)
                    yield return item;

                foreach (FloatingWindow floatingWindow in FloatingWindows)
                    yield return floatingWindow;
            }
        }

        internal FloatingWindow CreateFloatingWindow(Rect bounds)
        {
            FloatingWindow floatingWindow = new FloatingWindow(this, bounds);
            FloatingWindows.Add(floatingWindow);
            return floatingWindow;
        }

        internal void RemoveFloatingWindow(FloatingWindow floatingWindow)
        {
            Debug.Assert(floatingWindow != null);
            Debug.Assert(FloatingWindows.Contains(floatingWindow));
            FloatingWindows.Remove(floatingWindow);
        }

        internal DockTree GetDockTree(DockControlTreePosition dockControlTreePosition)
        {
            if (dockControlTreePosition == DockControlTreePosition.Left)
                return LeftDockTree;
            else if (dockControlTreePosition == DockControlTreePosition.Right)
                return RightDockTree;
            else if (dockControlTreePosition == DockControlTreePosition.Top)
                return TopDockTree;
            else if (dockControlTreePosition == DockControlTreePosition.Bottom)
                return BottomDockTree;
            else
            {
                Debug.Assert(dockControlTreePosition == DockControlTreePosition.Document);
                return DocumentDockTree;
            }
        }

        /// <summary>Gets a value indicates the left <see cref="DockTree"/>. This is a dependency property.</summary>
        /// <value>The left <see cref="DockTree"/>.</value>
        public DockTree LeftDockTree
        {
            get { return (DockTree)GetValue(LeftDockTreeProperty); }
            private set { SetValue(LeftDockTreePropertyKey, value); }
        }

        /// <summary>Gets a value indicates the right <see cref="DockTree"/>. This is a dependency property.</summary>
        /// <value>The right <see cref="DockTree"/>.</value>
        public DockTree RightDockTree
        {
            get { return (DockTree)GetValue(RightDockTreeProperty); }
            private set { SetValue(RightDockTreePropertyKey, value); }
        }

        /// <summary>Gets a value indicates the top <see cref="DockTree"/>. This is a dependency property.</summary>
        /// <value>The top <see cref="DockTree"/>.</value>
        public DockTree TopDockTree
        {
            get { return (DockTree)GetValue(TopDockTreeProperty); }
            private set { SetValue(TopDockTreePropertyKey, value); }
        }

        /// <summary>Gets a value indicates the bottom <see cref="DockTree"/>. This is a dependency property.</summary>
        /// <value>The bottom <see cref="DockTree"/>.</value>
        public DockTree BottomDockTree
        {
            get { return (DockTree)GetValue(BottomDockTreeProperty); }
            private set { SetValue(BottomDockTreePropertyKey, value); }
        }

        /// <summary>Gets a value indicates the document <see cref="DockTree"/>. This is a dependency property.</summary>
        /// <value>The document <see cref="DockTree"/>.</value>
        public DockTree DocumentDockTree
        {
            get { return (DockTree)GetValue(DocumentDockTreeProperty); }
            private set { SetValue(DocumentDockTreePropertyKey, value); }
        }

        /// <summary>Gets or sets the width of left <see cref="DockTree"/>. This is a dependency property.</summary>
        /// <value>The left <see cref="DockTree"/> width, can be either auto pixel, auto star, pixel or star values.</value>
        public SplitterDistance LeftDockTreeWidth
        {
            get { return (SplitterDistance)GetValue(LeftDockTreeWidthProperty); }
            set { SetValue(LeftDockTreeWidthProperty, value); }
        }

        /// <summary>Gets or sets the width of right <see cref="DockTree"/>. This is a dependency property.</summary>
        /// <value>The right <see cref="DockTree"/> width, can be either auto pixel, auto star, pixel or star values.</value>
        public SplitterDistance RightDockTreeWidth
        {
            get { return (SplitterDistance)GetValue(RightDockTreeWidthProperty); }
            set { SetValue(RightDockTreeWidthProperty, value); }
        }

        /// <summary>Gets or sets the height of top <see cref="DockTree"/>. This is a dependency property.</summary>
        /// <value>The top <see cref="DockTree"/> height, can be either auto pixel, auto star, pixel or star values.</value>
        public SplitterDistance TopDockTreeHeight
        {
            get { return (SplitterDistance)GetValue(TopDockTreeHeightProperty); }
            set { SetValue(TopDockTreeHeightProperty, value); }
        }

        /// <summary>Gets or sets the height of bottom <see cref="DockTree"/>. This is a dependency property.</summary>
        /// <value>The bottom <see cref="DockTree"/> height, can be either auto pixel, auto star, pixel or star values.</value>
        public SplitterDistance BottomDockTreeHeight
        {
            get { return (SplitterDistance)GetValue(BottomDockTreeHeightProperty); }
            set { SetValue(BottomDockTreeHeightProperty, value); }
        }

        /// <summary>Gets or sets the z-order of <see cref="DockTree"/> objects. This is a dependency property.</summary>
        /// <value>The z-order of left, right, top and bottom <see cref="DockTree"/> objects. The document
        /// <see cref="DockTree"/> is always on top of the Z-order.</value>
        public DockTreeZOrder DockTreeZOrder
        {
            get { return (DockTreeZOrder)GetValue(DockTreeZOrderProperty); }
            set { SetValue(DockTreeZOrderProperty, value); }
        }

        /// <summary>Gets a value indicates <see cref="DockItem"/> currently has keyboard focus. This is a dependency property.</summary>
        /// <value>The <see cref="DockItem"/> currently has keyboard focus.</value>
        public DockItem FocusedItem
        {
            get { return (DockItem)GetValue(FocusedItemProperty); }
        }

        /// <summary>Gets a value indicates currently active <see cref="DockItem"/>. This is a dependency property.</summary>
        /// <value>The currently active <see cref="DockItem"/>.</value>
        /// <remarks>When <see cref="FocusedItem"/> changed to <see langword="null"/> as a result of clicking focusable controls
        /// outside <see cref="DockControl"/>, <see cref="ActiveItem"/> remains unchanged. This is useful
        /// to get currently active <see cref="DockItem"/> when the keyboard focus is not within <see cref="DockControl"/>,
        /// such as when a <see cref="MenuItem"/> is clicked.</remarks>
        public DockItem ActiveItem
        {
            get { return (DockItem)GetValue(ActiveItemProperty); }
        }

        /// <summary>Gets a value indicates currently active document <see cref="DockItem"/>. This is a dependency property.</summary>
        /// <value>The currently active document <see cref="DockItem"/>.</value>
        public DockItem ActiveDocument
        {
            get { return (DockItem)GetValue(ActiveDocumentProperty); }
        }

        /// <summary>Gets a value indicates currently selected auto hide <see cref="DockItem"/>. This is a dependency property.</summary>
        /// <value>The currently selected auto hide document <see cref="DockItem"/>.</value>
        public DockItem SelectedAutoHideItem
        {
            get { return (DockItem)GetValue(SelectedAutoHideItemProperty); }
            internal set
            {
                Debug.Assert(value == null || DockPositionHelper.IsAutoHide(value.DockPosition));

                DockItem oldValue = SelectedAutoHideItem;
                if (oldValue == value)
                    return;

                SetValue(SelectedAutoHideItemPropertyKey, value);

                if (oldValue != null && DockPositionHelper.IsAutoHide(oldValue.DockPosition))
                    oldValue.IsSelected = false;

                if (value != null)
                    value.IsSelected = true;

                if (value != oldValue)
                    _selectedAutoHideItemChanged = true;
            }
        }

        /// <summary>Gets or sets the default size of <see cref="FloatingWindow"/>.</summary>
        /// <value>The default size of <see cref="FloatingWindow"/>. The default value is (300,300).</value>
        public Size DefaultFloatingWindowSize
        {
            get { return (Size)GetValue(DefaultFloatingWindowSizeProperty); }
            set { SetValue(DefaultFloatingWindowSizeProperty, value); }
        }

        internal void OnItemAdded(DockItem item)
        {
            AddLogicalChild(item);
            item.SetDockControl(this);
        }

        internal void OnItemRemoved(DockItem item)
        {
            RemoveLogicalChild(item);
        }

        /// <summary>Raises the <see cref="FocusedItemChanged"/> event.</summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        /// <remarks>
        /// <para>This method allows derived classes to handle the event without attaching a delegate.
        /// This is the preferred technique for handling the event in a derived class.</para>
        /// <para>When overriding in a derived class, be sure to call the base class's
        /// method so that registered delegates receive the event.</para>
        /// </remarks>
        protected internal virtual void OnFocusedItemChanged(EventArgs e)
        {
            EventHandler<EventArgs> handler = FocusedItemChanged;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>Raises the <see cref="ActiveItemChanged"/> event.</summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        /// <remarks>
        /// <para>This method allows derived classes to handle the event without attaching a delegate.
        /// This is the preferred technique for handling the event in a derived class.</para>
        /// <para>When overriding in a derived class, be sure to call the base class's
        /// method so that registered delegates receive the event.</para>
        /// </remarks>
        protected internal virtual void OnActiveItemChanged(EventArgs e)
        {
            EventHandler<EventArgs> handler = ActiveItemChanged;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>Raises the <see cref="ActiveDocumentChanged"/> event.</summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        /// <remarks>
        /// <para>This method allows derived classes to handle the event without attaching a delegate.
        /// This is the preferred technique for handling the event in a derived class.</para>
        /// <para>When overriding in a derived class, be sure to call the base class's
        /// method so that registered delegates receive the event.</para>
        /// </remarks>
        protected internal virtual void OnActiveDocumentChanged(EventArgs e)
        {
            EventHandler<EventArgs> handler = ActiveDocumentChanged;
            if (handler != null)
                handler(this, e);
        }

        internal void RaiseSelectedAutoHideItemChangedEvent()
        {
            if (_selectedAutoHideItemChanged)
            {
                _selectedAutoHideItemChanged = false;
                OnSelectedAutoHideItemChanged(EventArgs.Empty);
            }
        }

        /// <summary>Raises the <see cref="SelectedAutoHideItemChanged"/> event.</summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        /// <remarks>
        /// <para>This method allows derived classes to handle the event without attaching a delegate.
        /// This is the preferred technique for handling the event in a derived class.</para>
        /// <para>When overriding in a derived class, be sure to call the base class's
        /// method so that registered delegates receive the event.</para>
        /// </remarks>
        protected virtual void OnSelectedAutoHideItemChanged(EventArgs e)
        {
            EventHandler<EventArgs> handler = SelectedAutoHideItemChanged;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>Raises the <see cref="DockItemStateChanging"/> event.</summary>
        /// <param name="e">A <see cref="DockItemStateEventArgs"/> that contains the event data.</param>
        /// <remarks>
        /// <para>This method allows derived classes to handle the event without attaching a delegate.
        /// This is the preferred technique for handling the event in a derived class.</para>
        /// <para>When overriding in a derived class, be sure to call the base class's
        /// method so that registered delegates receive the event.</para>
        /// </remarks>
        protected internal virtual void OnDockItemStateChanging(DockItemStateEventArgs e)
        {
            EventHandler<DockItemStateEventArgs> handler = DockItemStateChanging;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>Raises the <see cref="DockItemStateChanged"/> event.</summary>
        /// <param name="e">A <see cref="DockItemStateEventArgs"/> that contains the event data.</param>
        /// <remarks>
        /// <para>This method allows derived classes to handle the event without attaching a delegate.
        /// This is the preferred technique for handling the event in a derived class.</para>
        /// <para>When overriding in a derived class, be sure to call the base class's
        /// method so that registered delegates receive the event.</para>
        /// </remarks>
        protected internal virtual void OnDockItemStateChanged(DockItemStateEventArgs e)
        {
            EventHandler<DockItemStateEventArgs> handler = DockItemStateChanged;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>Occurs when the value of the <see cref="FocusedItem"/> property has changed.</summary>
        public event EventHandler<EventArgs> FocusedItemChanged;

        /// <summary>Occurs when the value of the <see cref="ActiveItem"/> property has changed.</summary>
        public event EventHandler<EventArgs> ActiveItemChanged;

        /// <summary>Occurs when the value of the <see cref="ActiveDocument"/> property has changed.</summary>
        public event EventHandler<EventArgs> ActiveDocumentChanged;

        /// <summary>Occurs when the value of the <see cref="SelectedAutoHideItemChanged"/> property has changed.</summary>
        public event EventHandler<EventArgs> SelectedAutoHideItemChanged;

        /// <summary>Occurs when the state of <see cref="DockItem"/> is changing.</summary>
        public event EventHandler<DockItemStateEventArgs> DockItemStateChanging;

        /// <summary>Occurs when the state of <see cref="DockItem"/> is changed.</summary>
        public event EventHandler<DockItemStateEventArgs> DockItemStateChanged;

        /// <summary>Saves the current window layout.</summary>
        /// <returns>The saved <see cref="DockLayout"/>.</returns>
        public DockLayout Save()
        {
            return DockLayoutAdapter.Save(this);
        }

        /// <summary>Loads the window layout.</summary>
        /// <param name="layout">The specified <see cref="DockLayout"/> instance.</param>
        /// <param name="loadDockItemCallback">Callback to load <see cref="DockItem"/>. This callback
        /// takes an <see cref="object"/> returned by <see cref="DockItem.Save">DockItem.Save</see> method, returns an
        /// <see cref="DockItem"/> instance.</param>
        /// <remarks>Calling this method will clear all undo stack.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="layout"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">The <see cref="DockItems"/> collection is not empty. Close all <see cref="DockItem"/> before calling <see cref="Load"/>.</exception>
        public void Load(DockLayout layout, Func<object, DockItem> loadDockItemCallback)
        {
            if (layout == null)
                throw new ArgumentNullException("layout");

            if (DockItems.Count != 0)
                throw new InvalidOperationException(SR.Exception_DockControl_Load_DockItemsNotEmpty);

            DockLayoutAdapter.Load(this, layout, loadDockItemCallback);
            ClearUndo();
        }

        private void RefreshCanUndoCanRedo()
        {
            CanUndo = _undoStack.Count > 0;
            CanRedo = _redoStack.Count > 0;
        }

        internal bool CanEnterUndo
        {
            //get { return IsLoaded && _executionLevel == 0 && MaxUndoLevel != 0 && !ShowActions.IsExecuting; }
            get { return IsLoaded && _executionLevel == 0 && MaxUndoLevel != 0; }
        }

        /// <summary>Marks the beginning of a reversible unit of work.</summary>
        /// <remarks>Use <see cref="BeginUndoUnit"/> and <see cref="EndUndoUnit"/> in tandem.
        /// Calling <see cref="BeginUndoUnit"/> increases <see cref="UndoUnitLevel"/> by 1,
        /// and calling <see cref="EndUndoUnit"/> decreases <see cref="UndoUnitLevel"/> by 1.
        /// When <see cref="UndoUnitLevel"/> is greater than 0, all undoable operations are considered as
        /// one single reversible unit of work.</remarks>
        public void BeginUndoUnit()
        {
            _undoUnitLevel++;
        }

        /// <summary>Marks the end of a reversible unit of work.</summary>
        /// <remarks>Use <see cref="BeginUndoUnit"/> and <see cref="EndUndoUnit"/> in tandem.
        /// Calling <see cref="BeginUndoUnit"/> increases <see cref="UndoUnitLevel"/> by 1,
        /// and calling <see cref="EndUndoUnit"/> decreases <see cref="UndoUnitLevel"/> by 1.
        /// When <see cref="UndoUnitLevel"/> is greater than 0, all undoable operations are considered as
        /// one single reversible unit of work.</remarks>
        public void EndUndoUnit()
        {
            Debug.Assert(_undoUnitLevel > 0);

            _undoUnitLevel--;
            if (_undoUnitLevel == 0 && _undoUnitCommands.Count > 0)
            {
                InternalAddCommand(new CompositeCommand(_undoUnitCommands));
                _undoUnitCommands.Clear();
                RefreshCanUndoCanRedo();
            }
        }

        /// <summary>Gets a value indicates the undo unit level.</summary>
        /// <value>The undo unit level. The default value is 0.</value>
        /// <remarks>Calling <see cref="BeginUndoUnit"/> increases <see cref="UndoUnitLevel"/> by 1,
        /// and calling <see cref="EndUndoUnit"/> decreases <see cref="UndoUnitLevel"/> by 1.
        /// When <see cref="UndoUnitLevel"/> greater than 0, all undoable operations are considered as
        /// one single reversible unit of work.</remarks>
        public int UndoUnitLevel
        {
            get { return _undoUnitLevel; }
        }

        /// <summary>Undoes the most recent undo command. In other words, undoes the most recent undo unit on the undo stack.</summary>
        /// <returns><see langword="true"/> if the undo operation was successful; otherwise, 
        /// <see langword="false"/>. This method returns <see langword="false"/> if the undo stack is empty.</returns>
        public bool Undo()
        {
            if (!CanUndo || UndoUnitLevel != 0)
                return false;

            _executionLevel++;
            ICommand command = _undoStack.Pop();
            command.UnExecute(this);
            _redoStack.Push(command);
            _executionLevel--;
            RefreshCanUndoCanRedo();
            return true;
        }

        /// <summary>Undoes the most recent undo command. In other words, redoes the most recent undo unit on the undo stack.</summary>
        /// <returns><see langword="true"/> if the redo operation was successful; otherwise, 
        /// <see langword="false"/>. This method returns <see langword="false"/> if the undo stack is empty.</returns>
        public bool Redo()
        {
            if (!CanRedo || UndoUnitLevel != 0)
                return false;

            _executionLevel++;
            ICommand command = _redoStack.Pop();
            command.Execute(this);
            _undoStack.Push(command);
            _executionLevel--;
            RefreshCanUndoCanRedo();
            return true;
        }

        internal void ExecuteCommand(ICommand command)
        {
            AddCommand(command, true);
        }

        private void AddCommand(ICommand command, bool execute)
        {
            Debug.Assert(CanEnterUndo);
            Debug.Assert(command != null);

            _executionLevel++;
            if (execute)
                command.Execute(this);

            if (UndoUnitLevel != 0)
                _undoUnitCommands.Add(command);
            else
                InternalAddCommand(command);

            _executionLevel--;
            RefreshCanUndoCanRedo();
        }

        private void InternalAddCommand(ICommand command)
        {
            Debug.Assert(MaxUndoLevel != 0);

            _undoStack.Push(command);
            _redoStack.Clear();
        }

        /// <summary>Clears all information from the undo redo buffer.</summary>
        public void ClearUndo()
        {
            _undoUnitCommands.Clear();
            _undoStack.Clear();
            _redoStack.Clear();
            RefreshCanUndoCanRedo();
        }

        internal void OnValueChanged<T>(T oldValue, T newValue, Func<T, T, IValueChangedCommand<T>> createCommandCallback)
        {
            Debug.Assert(createCommandCallback != null);

            if (!CanEnterUndo || oldValue.Equals(newValue))
                return;

            IValueChangedCommand<T> command = LastUndoCommand as IValueChangedCommand<T>;
            if (command != null)
            {
                if (command.ShouldRemove(this, newValue))
                {
                    RemoveLastUndoCommand();
                    return;
                }
                else if (command.Merge(this, newValue))
                    return;
            }

            command = _recycleUndoCommand.Target as IValueChangedCommand<T>;
            if (command != null && command.Reset(this, oldValue, newValue))
                _recycleUndoCommand.Target = null;
            else
                command = createCommandCallback(oldValue, newValue);
            AddCommand(command, false);
        }

        private ICommand LastUndoCommand
        {
            get
            {
                if (_undoUnitCommands.Count > 0)
                    return _undoUnitCommands[_undoUnitCommands.Count - 1];
                else if (_undoStack.Count == 0)
                    return null;
                else
                    return _undoStack.Peek();
            }
        }

        private void RemoveLastUndoCommand()
        {
            ICommand commandToRemove;
            if (_undoUnitCommands.Count > 0)
            {
                int lastIndex = _undoUnitCommands.Count - 1;
                commandToRemove = _undoUnitCommands[lastIndex];
                _undoUnitCommands.RemoveAt(lastIndex);
            }
            else
            {
                Debug.Assert(_undoStack.Count != 0);
                commandToRemove = _undoStack.Pop();
                RefreshCanUndoCanRedo();
            }
            _recycleUndoCommand.Target = commandToRemove;
        }

        internal void SaveFocus()
        {
            PaneManager.SaveFocus();
        }

        internal void RestoreFocus()
        {
            PaneManager.RestoreFocus();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= new RoutedEventHandler(OnLoaded);

            foreach (DockItem item in DockItems)
            {
                ShowAction showAction = item.ShowAction;
                if (showAction != null)
                {
                    item.ShowAction = null;
                    showAction.Source = DockItems.IndexOf(item);
                    showAction.Run(this);
                }
            }
        }
    }
}
