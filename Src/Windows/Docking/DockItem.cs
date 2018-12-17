using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

namespace DevZest.Windows.Docking
{
    /// <summary>Represents a dockable item hosted by a <see cref="Docking.DockControl"/>.</summary>
    /// <remarks>
    /// <para>Content Model: <see cref="DockItem"/> derives from <see cref="ContentControl" /> class, set the
    /// <see cref="ContentControl.Content" /> property to organize the UI of the dockable item.</para>
    /// <para>Use <see cref="AllowedDockTreePositions" />, <see cref="AutoHideSize" />, <see cref="DefaultDockPosition" />, 
    /// <see cref="Description" />, <see cref="Icon"/>, <see cref="TabContextMenu" />,
    /// <see cref="TabText"/> and <see cref="Title"/> properties to customize the DockItem.</para>
    /// <para>Call <see cref="Show(DockControl)">Show</see> method to show the DockItem; call
    /// <see cref="Activate">Activate</see> method to activate the DockItem; call
    /// <see cref="Close">Close</see> method to close the DockItem (disconnect the DockItem from DockControl); call
    /// <see cref="PerformClose">PerformClose</see> method to close or hide the DockItem, depending on the value of
    /// <see cref="HideOnPerformClose"/> property. You may intercept the <see cref="Closing">Closing</see> event to
    /// cancel the DockItem closing.</para>
    /// <para>The <see cref="DockItem" /> has the following state reflected by its <see cref="DockPosition"/>
    /// and other properties:</para>
    /// <list type="table">
    ///   <listheader>
    ///     <term>State</term>
    ///     <description>DockPosition</description>
    ///     <description>Description</description>
    ///   </listheader>
    ///   <item>
    ///     <term>Disconnected</term>
    ///     <description>Unknown</description>
    ///     <description>The DockItem is not connected to any DockControl. The values of its <see cref="DockControl"/>,
    ///     <see cref="FirstPane" /> and <see cref="SecondPane"/> properties are all <see langword="null" />.</description>
    ///   </item>
    ///   <item>
    ///     <term>Hidden</term>
    ///     <description>Hidden</description>
    ///     <description>The DockItem is hidden with its <see cref="IsHidden" /> property set to <see langword="true" />.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <term>Visible</term>
    ///     <description>other values</description>
    ///     <description>The DockItem is visible with its <see cref="IsHidden" /> property set to <see langword="false" />,
    ///     plus the following properties to reflect the state:
    ///     <list type="bullet">
    ///       <item>The <see cref="IsSelected" /> property indicates whether this DockItem is selected (<see cref="Docking.DockControl.SelectedAutoHideItem">DockControl.SelectedAutoHideItem</see>
    ///         or its <see cref="FirstPane"/>'s <see cref="DockPane.SelectedItem" /> returns this <see cref="DockItem"/>).
    ///       </item>
    ///       <item>The <see cref="IsActiveItem" /> property indicates whether this DockItem is active
    ///       (<see cref="Docking.DockControl.ActiveItem">DockControl.ActiveItem</see> returns this <see cref="DockItem"/>).
    ///       </item>
    ///       <item>The <see cref="IsActiveDocument" /> property indicates whether this DockItem is active document
    ///       (<see cref="Docking.DockControl.ActiveDocument">DockControl.ActiveDocument</see> returns this <see cref="DockItem"/>).
    ///       </item>
    ///       <item>The <see cref="IsAutoHide"/> property indicates whether this DockItem is in auto-hide mode.</item>
    ///     </list>
    ///     <para>Call <see cref="ToggleAutoHide(DockItemShowMethod)">ToggleAutoHide</see> method to toggle the auto hide
    ///     state of the DockItem's <see cref="FirstPane" />; call <see cref="ToggleFloating(DockItemShowMethod)">ToggleFloating</see>
    ///     method to toggle the floating state of the DockItem.</para>
    ///     </description>
    ///   </item>
    /// </list>
    /// <para>You may intercept the <see cref="StateChanging"/> or <see cref="StateChanged" /> event, which occurs before or after the
    /// state of DockItem changed.</para>
    /// <para>Derived class may override the <see cref="Save">Save</see> method to return a object instance represents
    /// this DockItem for saving/loading the window layout, or override the <see cref="UndoRedoReference"/> property
    /// to return a DockItem reference for undo/redo.</para>
    /// </remarks>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    public partial class DockItem : ContentControl, IDockItemUndoRedoReference
    {
        private static readonly DependencyPropertyKey DockControlPropertyKey;
        /// <summary>Identifies the <see cref="DockControl"/> dependency property.</summary>
        public static readonly DependencyProperty DockControlProperty;
        /// <summary>Identifies the <see cref="DefaultDockPosition"/> dependency property.</summary>
        public static readonly DependencyProperty DefaultDockPositionProperty;
        /// <summary>Identifies the <see cref="AllowedDockTreePositions"/> dependency property.</summary>
        public static readonly DependencyProperty AllowedDockTreePositionsProperty;
        /// <summary>Identifies the <see cref="HideOnPerformClose"/> dependency property.</summary>
        public static readonly DependencyProperty HideOnPerformCloseProperty;
        /// <summary>Identifies the <see cref="Icon"/> dependency property.</summary>
        public static readonly DependencyProperty IconProperty;
        /// <summary>Identifies the <see cref="Title"/> dependency property.</summary>
        public static readonly DependencyProperty TitleProperty;
        /// <summary>Identifies the <see cref="TabText"/> dependency property.</summary>
        public static readonly DependencyProperty TabTextProperty;
        /// <summary>Identifies the <see cref="Description"/> dependency property.</summary>
        public static readonly DependencyProperty DescriptionProperty;
        /// <summary>Identifies the <see cref="TabContextMenu"/> dependency property.</summary>
        public static readonly DependencyProperty TabContextMenuProperty;
        /// <summary>Identifies the <see cref="AutoHideSize"/> dependency property.</summary>
        public static readonly DependencyProperty AutoHideSizeProperty;
        private static readonly DependencyPropertyKey IsSelectedPropertyKey;
        private static readonly DependencyPropertyKey IsActiveItemPropertyKey;
        private static readonly DependencyPropertyKey IsActiveDocumentPropertyKey;
        private static readonly DependencyPropertyKey DockPositionPropertyKey;
        /// <summary>Identifies the <see cref="DockPosition"/> dependency property.</summary>
        public static readonly DependencyProperty DockPositionProperty;
        /// <summary>Identifies the <see cref="IsSelected"/> dependency property.</summary>
        public static readonly DependencyProperty IsSelectedProperty;
        /// <summary>Identifies the <see cref="IsActiveItem"/> dependency property.</summary>
        public static readonly DependencyProperty IsActiveItemProperty;
        /// <summary>Identifies the <see cref="IsActiveDocument"/> dependency property.</summary>
        public static readonly DependencyProperty IsActiveDocumentProperty;
        private static readonly DependencyPropertyKey FirstPanePropertyKey;
        /// <summary>Identifies the <see cref="FirstPane"/> dependency property.</summary>
        public static readonly DependencyProperty FirstPaneProperty;
        private static readonly DependencyPropertyKey SecondPanePropertyKey;
        /// <summary>Identifies the <see cref="SecondPane"/> dependency property.</summary>
        public static readonly DependencyProperty SecondPaneProperty;
        private static readonly DependencyPropertyKey IsHiddenPropertyKey;
        /// <summary>Identifies the <see cref="IsHidden"/> dependency property.</summary>
        public static readonly DependencyProperty IsHiddenProperty;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static DockItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockItem), new FrameworkPropertyMetadata(typeof(DockItem)));
            KeyboardNavigation.IsTabStopProperty.OverrideMetadata(typeof(DockItem), new FrameworkPropertyMetadata(BooleanBoxes.False));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(DockItem), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(DockItem), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
            KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(typeof(DockItem), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
            DockControlPropertyKey = DependencyProperty.RegisterReadOnly("DockControl", typeof(DockControl), typeof(DockItem),
                new FrameworkPropertyMetadata(null));
            DockControlProperty = DockControlPropertyKey.DependencyProperty;
            DefaultDockPositionProperty = DependencyProperty.Register("DefaultDockPosition", typeof(DockPosition), typeof(DockItem),
                new FrameworkPropertyMetadata(DockPosition.Unknown));
            AllowedDockTreePositionsProperty = DependencyProperty.Register("AllowedDockTreePositions", typeof(AllowedDockTreePositions), typeof(DockItem),
                new FrameworkPropertyMetadata(AllowedDockTreePositions.All, new PropertyChangedCallback(OnAllowedDockTreePositionsChanged)));
            HideOnPerformCloseProperty = DependencyProperty.Register("HideOnPerformClose", typeof(bool), typeof(DockItem),
                new FrameworkPropertyMetadata(BooleanBoxes.True));
            IconProperty = Window.IconProperty.AddOwner(typeof(DockItem));
            TitleProperty = Window.TitleProperty.AddOwner(typeof(DockItem));
            TabTextProperty = DependencyProperty.Register("TabText", typeof(string), typeof(DockItem),
                new FrameworkPropertyMetadata(null));
            DescriptionProperty = DependencyProperty.Register("Descrption", typeof(string), typeof(DockItem),
                new FrameworkPropertyMetadata(null));
            TabContextMenuProperty = DependencyProperty.Register("TabContextMenu", typeof(ContextMenu), typeof(DockItem),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnTabContextMenuChanged), new CoerceValueCallback(CoerceTabContextMenu)));
            AutoHideSizeProperty = DependencyProperty.Register("AutoHideSize", typeof(SplitterDistance), typeof(DockItem),
                new FrameworkPropertyMetadata(new SplitterDistance(1d/3d, SplitterUnitType.Star), new PropertyChangedCallback(OnAutoHideSizeChanged)));
            DockPositionPropertyKey = DependencyProperty.RegisterReadOnly("DockPosition", typeof(DockPosition), typeof(DockItem),
                new FrameworkPropertyMetadata(DockPosition.Unknown));
            IsSelectedPropertyKey = DependencyProperty.RegisterReadOnly("IsSelected", typeof(bool), typeof(DockItem),
                new FrameworkPropertyMetadata(BooleanBoxes.False));
            IsActiveItemPropertyKey = DependencyProperty.RegisterReadOnly("IsActiveItem", typeof(bool), typeof(DockItem),
                new FrameworkPropertyMetadata(BooleanBoxes.False));
            IsActiveDocumentPropertyKey = DependencyProperty.RegisterReadOnly("IsActiveDocument", typeof(bool), typeof(DockItem),
                new FrameworkPropertyMetadata(BooleanBoxes.False));
            FirstPanePropertyKey = DependencyProperty.RegisterReadOnly("FirstPane", typeof(DockPane), typeof(DockItem),
                new FrameworkPropertyMetadata(null));
            SecondPanePropertyKey = DependencyProperty.RegisterReadOnly("SecondPane", typeof(DockPane), typeof(DockItem),
                new FrameworkPropertyMetadata(null));
            IsHiddenPropertyKey = DependencyProperty.RegisterReadOnly("IsHidden", typeof(bool), typeof(DockItem),
                new FrameworkPropertyMetadata(BooleanBoxes.False));
            DockPositionProperty = DockPositionPropertyKey.DependencyProperty;
            IsSelectedProperty = IsSelectedPropertyKey.DependencyProperty;
            IsActiveItemProperty = IsActiveItemPropertyKey.DependencyProperty;
            IsActiveDocumentProperty = IsActiveDocumentPropertyKey.DependencyProperty;
            FirstPaneProperty = FirstPanePropertyKey.DependencyProperty;
            SecondPaneProperty = SecondPanePropertyKey.DependencyProperty;
            IsHiddenProperty = IsHiddenPropertyKey.DependencyProperty;
            CommandBinding toggleFloatingCommandBinding = new CommandBinding(DockCommands.ToggleFloating,
                new ExecutedRoutedEventHandler(OnToggleFloatingCommandExecuted),
                new CanExecuteRoutedEventHandler(CanExecuteToggleFloatingCommand));
            CommandBinding toggleAutoHideCommandBinding = new CommandBinding(DockCommands.ToggleAutoHide,
                new ExecutedRoutedEventHandler(OnToggleAutoHideCommandExecuted),
                new CanExecuteRoutedEventHandler(CanExecuteToggleAutoHideCommand));
            CommandBinding performCloseCommandBinding = new CommandBinding(DockCommands.PerformClose,
                new ExecutedRoutedEventHandler(OnPerformCloseCommandExecuted),
                new CanExecuteRoutedEventHandler(CanExecutePerformCloseCommand));
            CommandBinding showCommandBinding = new CommandBinding(DockCommands.Show,
                new ExecutedRoutedEventHandler(OnShowCommandExecuted),
                new CanExecuteRoutedEventHandler(CanExecuteShowCommand));
            CommandBinding undoCommandBinding = new CommandBinding(DockCommands.Undo,
                new ExecutedRoutedEventHandler(OnUndoCommandExecuted),
                new CanExecuteRoutedEventHandler(CanExecuteUndoCommand));
            CommandBinding redoCommandBinding = new CommandBinding(DockCommands.Redo,
                new ExecutedRoutedEventHandler(OnRedoCommandExecuted),
                new CanExecuteRoutedEventHandler(CanExecuteRedoCommand));
            CommandManager.RegisterClassCommandBinding(typeof(DockItem), toggleFloatingCommandBinding);
            CommandManager.RegisterClassCommandBinding(typeof(DockItem), toggleAutoHideCommandBinding);
            CommandManager.RegisterClassCommandBinding(typeof(DockItem), performCloseCommandBinding);
            CommandManager.RegisterClassCommandBinding(typeof(DockItem), showCommandBinding);
            CommandManager.RegisterClassCommandBinding(typeof(DockItem), undoCommandBinding);
            CommandManager.RegisterClassCommandBinding(typeof(DockItem), redoCommandBinding);
        }

        private static void OnAllowedDockTreePositionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AllowedDockTreePositions oldValue = (AllowedDockTreePositions)e.OldValue;
            AllowedDockTreePositions newValue = (AllowedDockTreePositions)e.NewValue;
            bool oldContainsFloating = (oldValue & AllowedDockTreePositions.Floating) == AllowedDockTreePositions.Floating;
            bool newContainsFloating = (newValue & AllowedDockTreePositions.Floating) == AllowedDockTreePositions.Floating;
            if (oldContainsFloating != newContainsFloating)
                CommandManager.InvalidateRequerySuggested();
        }

        private static void OnAutoHideSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockItem dockItem = (DockItem)d;
            DockControl dockControl = dockItem.DockControl;
            if (dockControl == null)
                return;

            UpdateAutoHideSizeData oldValue = new UpdateAutoHideSizeData(dockItem, (SplitterDistance)e.OldValue);
            UpdateAutoHideSizeData newValue = new UpdateAutoHideSizeData(dockItem, (SplitterDistance)e.NewValue);
            dockControl.OnValueChanged(oldValue, newValue,
                delegate(UpdateAutoHideSizeData oldData, UpdateAutoHideSizeData newData)
                {
                    return new UpdateAutoHideSizeCommand(oldData.DockItem, oldData.Value, newData.Value);
                });
        }

        private static void OnTabContextMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockItem dockItem = (DockItem)d;
            ContextMenu oldValue = e.OldValue as ContextMenu;

            if (oldValue != null && oldValue.DataContext == dockItem)
                oldValue.DataContext = null;
        }

        /// <remarks>This is a workaround because set DataContext in OnTabContextMenuChanged does
        /// not work in .Net Framework 4.0. Everytime OnTabContextMenuChanged is invoked, a temp value is
        /// retrieved from resource dictionary therefore DataContext set to a wrong instance.</remarks>
        private static object CoerceTabContextMenu(DependencyObject d, object baseValue)
        {
            ContextMenu newValue = (ContextMenu)baseValue;
            if (newValue != null)
            {
                if (newValue.DataContext != null)
                    throw new InvalidOperationException(SR.Exception_DockItem_ContextMenuAttachedAlready);
                newValue.DataContext = (DockItem)d;
            }

            return newValue;
        }

        private static void OnToggleFloatingCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ((DockItem)sender).ToggleFloating();
        }

        private static void CanExecuteToggleFloatingCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((DockItem)sender).CanToggleFloating;
            e.Handled = true;
        }

        private static void OnToggleAutoHideCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ((DockItem)sender).ToggleAutoHide();
        }

        private static void CanExecuteToggleAutoHideCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((DockItem)sender).CanToggleAutoHide;
            e.Handled = true;
        }

        private static void OnPerformCloseCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ((DockItem)sender).PerformClose();
        }

        private static void CanExecutePerformCloseCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private static void OnShowCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DockItem dockItem = (DockItem)sender;
            DockControl dockControl = e.Parameter as DockControl;
            if (dockControl != null)
                dockItem.Show(dockControl);
            else if (dockItem.DockControl != null)
                dockItem.Show(dockItem.DockControl);
        }

        private static void CanExecuteShowCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            DockItem dockItem = sender as DockItem;
            DockControl dockControl = e.Parameter as DockControl;
            e.CanExecute = (dockControl != null) || (dockItem != null && dockItem.DockControl != null);
            e.Handled = true;
        }

        private static void OnUndoCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DockItem dockItem = (DockItem)sender;
            if (dockItem == null || dockItem.DockControl == null)
                return;

            dockItem.DockControl.Undo();
        }

        private static void CanExecuteUndoCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            DockItem dockItem = (DockItem)sender;
            if (dockItem == null || dockItem.DockControl == null)
                e.CanExecute = false;
            else
            {
                e.CanExecute = dockItem.DockControl.CanUndo;
                e.Handled = true;
            }
        }

        private static void OnRedoCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DockItem dockItem = (DockItem)sender;
            if (dockItem == null || dockItem.DockControl == null)
                return;

            dockItem.DockControl.Redo();
        }

        private static void CanExecuteRedoCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            DockItem dockItem = (DockItem)sender;
            if (dockItem == null || dockItem.DockControl == null)
                e.CanExecute = false;
            else
            {
                e.CanExecute = dockItem.DockControl.CanRedo;
                e.Handled = true;
            }
        }

        /// <summary>Gets the <see cref="Docking.DockControl"/> object associated with this <see cref="DockItem"/>. This is a dependency property.</summary>
        /// <value>The <see cref="Docking.DockControl"/> object associated with this <see cref="DockItem"/>.</value>
        public DockControl DockControl
        {
            get { return (DockControl)GetValue(DockControlProperty); }
            private set { SetValue(DockControlPropertyKey, value); }
        }

        internal void SetDockControl(DockControl value)
        {
            Debug.Assert(DockControl == null);
            Debug.Assert(value != null);
            DockControl = value;
        }

        /// <summary>Gets or sets the <see cref="DockItem"/> object's icon. This is a dependency property.</summary>
        /// <value>An <see cref="ImageSource"/> object that represents the icon.</value>
        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>Gets or sets the <see cref="DockItem"/> object's title. This is a dependency property.</summary>
        /// <value>An <see cref="string"/> that represents the title.</value>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>Gets or sets the <see cref="DockItem"/> object's text when shown as tabbed. This is a dependency property.</summary>
        /// <value>An <see cref="string"/> that represents the text when shown as tabbed.</value>
        public string TabText
        {
            get { return (string)GetValue(TabTextProperty); }
            set { SetValue(TabTextProperty, value); }
        }

        /// <summary>Gets or sets the description of this <see cref="DockItem"/>. This is a dependency property.</summary>
        /// <value>The description of this <see cref="DockItem"/>.</value>
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        /// <summary>Gets or sets the <see cref="DockItem"/> object's context menu to display for the tab. This is a dependency property.</summary>
        /// <value>An <see cref="ContextMenu"/> object that represents the context menu to display for the tab.</value>
        /// <remarks>When setting <see cref="TabContextMenu"/> property, the <see cref="ContextMenu"/> object's <see cref="P:System.Windows.FrameworkElement.DataContext"/>
        /// property is set to the owner <see cref="DockItem"/>. This is useful
        /// when you want data binding to the owner <see cref="DockItem"/> or its properties from the <see cref="ContextMenu"/> or
        /// its descendant elements.</remarks>
        public ContextMenu TabContextMenu
        {
            get { return (ContextMenu)GetValue(TabContextMenuProperty); }
            set { SetValue(TabContextMenuProperty, value); }
        }

        /// <summary>Gets or sets the size of this <see cref="DockItem"/> when in auto hide mode. This is a dependency property.</summary>
        /// <value>The size of this <see cref="DockItem"/> when in auto hide mode.</value>
        public SplitterDistance AutoHideSize
        {
            get { return (SplitterDistance)GetValue(AutoHideSizeProperty); }
            set { SetValue(AutoHideSizeProperty, value); }
        }

        /// <summary>Gets or sets the allowed dock tree positions for this <see cref="DockItem"/>. This is a dependency property.</summary>
        /// <value>The allowed docktree positions for this <see cref="DockItem"/>. The default value is <see cref="Docking.AllowedDockTreePositions.All"/>.</value>
        public AllowedDockTreePositions AllowedDockTreePositions
        {
            get { return (AllowedDockTreePositions)GetValue(AllowedDockTreePositionsProperty); }
            set { SetValue(AllowedDockTreePositionsProperty, value); }
        }

        /// <summary>Gets or sets the default <see cref="DockPosition"/> for this <see cref="DockItem"/>. This is a dependency property.</summary>
        /// <value>The default <see cref="DockPosition"/> for this <see cref="DockItem"/>.</value>
        public DockPosition DefaultDockPosition
        {
            get { return (DockPosition)GetValue(DefaultDockPositionProperty); }
            set { SetValue(DefaultDockPositionProperty, value); }
        }

        /// <summary>Gets or sets the value indicates whether hides or closes this <see cref="DockItem"/> when calling <see cref="PerformClose()"/>.</summary>
        /// <value><see langword="true"/> to hide the <see cref="DockItem"/> when calling <see cref="PerformClose()"/>, otherwise <see langword="false"/>
        /// to close the <see cref="DockItem"/>.</value>
        public bool HideOnPerformClose
        {
            get { return (bool)GetValue(HideOnPerformCloseProperty); }
            set { SetValue(HideOnPerformCloseProperty, BooleanBoxes.Box(value)); }
        }

        /// <summary>Gets the dock position of this <see cref="DockItem"/>. This is a dependency property.</summary>
        /// <value>The dock position of this <see cref="DockItem"/>.</value>
        public DockPosition DockPosition
        {
            get { return (DockPosition)GetValue(DockPositionProperty); }
            private set { SetValue(DockPositionPropertyKey, value); }
        }

        /// <summary>Gets the dock tree position of this <see cref="DockItem"/>.</summary>
        /// <value>The dock tree position of this <see cref="DockItem"/>.</value>
        public DockTreePosition? DockTreePosition
        {
            get { return FirstPane == null ? null : FirstPane.DockTreePosition; }
        }

        /// <summary>Gets a value indicates whether the <see cref="DockItem"/> is in auto hide mode.</summary>
        /// <value><see langword="true"/> if <see cref="DockItem"/> is in auto hide mode, otherwise <see langword="false"/>.</value>
        public bool IsAutoHide
        {
            get { return FirstPane == null ? false : FirstPane.IsAutoHide; }
        }

        /// <summary>Gets a value indicates whether the <see cref="DockItem"/> is invisible. This is a dependency property.</summary>
        /// <value><see langword="true"/> if <see cref="DockItem"/> is invisible, otherwise <see langword="false"/>.</value>
        public bool IsHidden
        {
            get { return (bool)GetValue(IsHiddenProperty); }
            private set { SetValue(IsHiddenPropertyKey, BooleanBoxes.Box(value)); }
        }

        /// <summary>Gets a value indicates whether this <see cref="DockItem"/> is selected. This is a dependency property.</summary>
        /// <value><see langword="true"/> if this <see cref="DockItem"/> is selected (<see cref="Docking.DockControl.SelectedAutoHideItem">DockControl.SelectedAutoHideItem</see>
        /// or its <see cref="FirstPane"/>'s <see cref="DockPane.SelectedItem" /> returns this <see cref="DockItem"/>), otherwise <see langword="false"/>.</value>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            internal set { SetValue(IsSelectedPropertyKey, BooleanBoxes.Box(value)); }
        }

        /// <summary>Gets a value indicates whether this <see cref="DockItem"/> is active. This is a dependency property.</summary>
        /// <value><see langword="true"/> if this <see cref="DockItem"/> is active (<see cref="Docking.DockControl.ActiveItem">DockControl.ActiveItem</see> returns
        /// this <see cref="DockItem"/>), otherwise <see langword="false"/>.</value>
        public bool IsActiveItem
        {
            get { return (bool)GetValue(IsActiveItemProperty); }
            internal set { SetValue(IsActiveItemPropertyKey, BooleanBoxes.Box(value)); }
        }

        /// <summary>Gets a value indicates whether this <see cref="DockItem"/> is active document. This is a dependency property.</summary>
        /// <value><see langword="true"/> if this <see cref="DockItem"/> is active document (<see cref="Docking.DockControl.ActiveDocument">DockControl.ActiveDocument</see>
        /// returns this <see cref="DockItem"/>), otherwise <see langword="false"/>.</value>
        public bool IsActiveDocument
        {
            get { return (bool)GetValue(IsActiveDocumentProperty); }
            internal set { SetValue(IsActiveDocumentPropertyKey, BooleanBoxes.Box(value)); }
        }

        /// <summary>Gets the first <see cref="DockPane"/> that this <see cref="DockItem"/> associated with. This is a dependency property.</summary>
        /// <value>The first <see cref="DockPane"/> that this <see cref="DockItem"/> associated with.</value>
        /// <remarks>A <see cref="DockItem"/> object can contained by two <see cref="DockPane"/> objects, one for floating mode and one for non
        /// floating mode. The <see cref="FirstPane"/> property always returns the <see cref="DockPane"/> that the <see cref="DockItem"/> last
        /// shown. Calling <see cref="ToggleFloating()"/> swaps <see cref="FirstPane"/> and <see cref="SecondPane"/> if they both
        /// exist.</remarks>
        public DockPane FirstPane
        {
            get { return (DockPane)GetValue(FirstPaneProperty); }
            private set { SetValue(FirstPanePropertyKey, value); }
        }

        /// <summary>Gets the second <see cref="DockPane"/> that this <see cref="DockItem"/> associated with. This is a dependency property.</summary>
        /// <value>The second <see cref="DockPane"/> that this <see cref="DockItem"/> associated with.</value>
        /// <remarks>A <see cref="DockItem"/> object can contained by two <see cref="DockPane"/> objects, one for floating mode and one for non
        /// floating mode. The <see cref="FirstPane"/> property always returns the <see cref="DockPane"/> that the <see cref="DockItem"/> last
        /// shown. Calling <see cref="ToggleFloating()"/> swaps <see cref="FirstPane"/> and <see cref="SecondPane"/> if they both
        /// exist.</remarks>
        public DockPane SecondPane
        {
            get { return (DockPane)GetValue(SecondPaneProperty); }
            private set { SetValue(SecondPanePropertyKey, value); }
        }

        private DockTree GetTree(bool isFloating)
        {
            DockPane pane = GetPane(isFloating);
            return pane == null ? null : pane.DockTree;
        }

        internal DockPane GetPane(bool isFloating)
        {
            if (FirstPane != null && FirstPane.IsFloating == isFloating)
                return FirstPane;
            else if (SecondPane != null && SecondPane.IsFloating == isFloating)
                return SecondPane;
            else
                return null;
        }

        private DockPane SetPane(DockPane pane, DockItemShowMethod showMethod)
        {
            return SetPane(pane, null, showMethod);
        }

        private DockPane SetPane(DockPane pane, int? index, DockItemShowMethod showMethod)
        {
            Debug.Assert(pane != null);
            Debug.Assert(pane.DockTree != null);
            Debug.Assert(index.HasValue || (FirstPane != pane && SecondPane != pane));

            bool isFloating = pane.DockTree.IsFloating;
            DockPane oldFirstPane = FirstPane;
            DockPane oldSecondPane = SecondPane;
            DockTree dockTree = pane.DockTree;
            DockTree dockTree1 = null;
            DockTree dockTree2 = null;

            if (oldFirstPane != null)
            {
                if (oldFirstPane.IsFloating == isFloating)
                {
                    if (oldFirstPane != pane)
                        dockTree1 = oldFirstPane.DockTree;
                    oldFirstPane.RemoveItem(this, oldFirstPane != pane);
                }
                else
                {
                    SecondPane = oldFirstPane;
                    dockTree1 = oldFirstPane.DockTree;
                    dockTree1.AddDirtyNode(oldFirstPane);
                    if (oldSecondPane != null)
                    {
                        if (oldSecondPane != pane)
                            dockTree2 = oldSecondPane.DockTree;
                        oldSecondPane.RemoveItem(this, oldSecondPane != pane);
                    }
                }
            }

            if (index.HasValue)
            {
                int newIndex = index.Value;
                if (newIndex > pane.Items.Count)
                    newIndex = pane.Items.Count;

                dockTree.AddItem(this, pane, newIndex);
            }
            FirstPane = pane;
            IsHidden = (showMethod == DockItemShowMethod.Hide);
            RefreshDockPosition();

            if (dockTree1 != null)
                dockTree1.CommitChanges();
            if (dockTree2 != null)
                dockTree2.CommitChanges();
            FirstPane.DockTree.CommitChanges();

            return oldFirstPane;
        }

        private void CoerceValues(DockItemShowMethod showMethod, DockItem itemToFocus)
        {
            CoerceValues(null, showMethod, itemToFocus);
        }

        private void CoerceValues(DockPane oldPane, DockItemShowMethod showMethod, DockItem itemToFocus)
        {
            CoerceIsSelected(oldPane, showMethod);
            CoerceFocusedItem(showMethod, itemToFocus);
        }

        private void CoerceIsSelected(DockPane oldPane, DockItemShowMethod showMethod)
        {
            if (oldPane != null && oldPane != FirstPane && oldPane.SelectedItem == this)
            {
                oldPane.CoerceSelectedItem();
                Debug.Assert(oldPane.SelectedItem != this);
            }

            if (FirstPane == null)
            {
                Debug.Assert(showMethod == DockItemShowMethod.Hide);
                if (DockControl.SelectedAutoHideItem == this)
                    DockControl.SelectedAutoHideItem = null;
                IsSelected = false;
                return;
            }

            bool isSelected = showMethod == DockItemShowMethod.Activate || showMethod == DockItemShowMethod.Select;
            if (isSelected)
            {
                Debug.Assert(DockPositionHelper.IsVisible(DockPosition));
                FirstPane.SelectedItem = this;
                if (FirstPane.IsAutoHide)
                    DockControl.SelectedAutoHideItem = this;
                else if (DockControl.SelectedAutoHideItem == this)
                    DockControl.SelectedAutoHideItem = null;
                return;
            }

            if (FirstPane.SelectedItem == this)
            {
                if (DockControl.SelectedAutoHideItem == this)
                    DockControl.SelectedAutoHideItem = null;
                else
                {
                    FirstPane.CoerceSelectedItem();
                    Debug.Assert(FirstPane.SelectedItem != this);  // requires Deselect->Select detect
                }
                IsSelected = false;
            }
            else if (FirstPane.SelectedItem != null)
            {
                bool isSelected2 = FirstPane.IsAutoHide ? DockControl.SelectedAutoHideItem == FirstPane.SelectedItem : true;
                FirstPane.SelectedItem.IsSelected = isSelected2;
            }
        }

        private void CoerceFocusedItem(DockItemShowMethod showMethod, DockItem itemToFocus)
        {
            if (showMethod == DockItemShowMethod.Activate)
            {
                Debug.Assert(itemToFocus == null || itemToFocus == this);
                if (itemToFocus == null)
                    itemToFocus = this;
            }

            if (itemToFocus != null)
                DockControl.PaneManager.Activate(itemToFocus);
        }

        /// <exclude/>
        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsKeyboardFocusWithinChanged(e);
            if (DockControl == null)
                return;

            if ((bool)e.NewValue)
            {
                if (!IsSelected)
                    CoerceIsSelected(null, DockItemShowMethod.Select);
                DockControl.PaneManager.OnDockItemFocusEnter(this);
            }
            else if (DockControl.FocusedItem == this)
                DockControl.PaneManager.OnDockItemFocusLeave(this);
        }

        /// <overloads>Shows the <see cref="DockItem"/>.</overloads>
        /// <summary>Shows the <see cref="DockItem"/> targeting specified <see cref="Docking.DockControl"/>, as activated.</summary>
        /// <param name="dockControl">The <see cref="Docking.DockControl"/> to show this <see cref="DockItem"/>.</param>
        /// <remarks>This method is a wrapper for <see cref="Show(DockControl, DockItemShowMethod)"/> method.</remarks>
        public void Show(DockControl dockControl)
        {
            Show(dockControl, DockItemShowMethod.Activate);
        }

        /// <summary>Shows the <see cref="DockItem"/> targeting specified <see cref="Docking.DockControl"/>, using specified <see cref="DockItemShowMethod"/>.</summary>
        /// <param name="dockControl">The targeting <see cref="Docking.DockControl"/>.</param>
        /// <param name="showMethod">The specified show method.</param>
        /// <remarks>When <see cref="FirstPane"/> is not <see langword="null"/>, this method calls <see cref="Show(DockItemShowMethod)"/>;
        /// otherwise, this method calls <see cref="Show(DockControl, DockPosition, DockItemShowMethod)"/> with <see cref="DefaultDockPosition"/>.
        /// </remarks>
        public void Show(DockControl dockControl, DockItemShowMethod showMethod)
        {
            if (FirstPane != null)
                Show(showMethod);
            else
                Show(dockControl, DefaultDockPosition, showMethod);
        }

        /// <summary>Shows the <see cref="DockItem"/>, using specified <see cref="DockItemShowMethod"/>.</summary>
        /// <param name="showMethod">The specified the show method.</param>
        /// <exception cref="InvalidOperationException"><see cref="DockPosition"/> is <see cref="Docking.DockPosition.Unknown"/>.</exception>
        /// <value>Calling this method activates, selects, deselects or hides the <see cref="DockItem"/>, depending on the value of
        /// <paramref name="showMethod"/>.</value>
        public void Show(DockItemShowMethod showMethod)
        {
            if (DockPosition == DockPosition.Unknown)
                throw new InvalidOperationException();

            if (showMethod == DockItemShowMethod.Activate)
                Select(true);
            else if (showMethod == DockItemShowMethod.Select)
                Select(false);
            else if (showMethod == DockItemShowMethod.Deselect)
                Deselect();
            else if (showMethod == DockItemShowMethod.Hide)
                Hide();
        }

        /// <summary>Activates the <see cref="DockItem"/> and gives it focus.</summary>
        /// <exception cref="InvalidOperationException">The <see cref="DockPosition"/> 
        /// is <see cref="Docking.DockPosition.Unknown"/>.</exception>
        public void Activate()
        {
            if (DockPosition == DockPosition.Unknown)
                throw new InvalidOperationException(SR.Exception_DockItem_Activate_InvalidDockPosition);

            Select(true);
            UpdateLayout();
        }

        private void Select(bool activate)
        {
            if (!IsSelected)
                SelectCommand.Execute(this, activate);
            else if (activate)
                CoerceFocusedItem(DockItemShowMethod.Activate, this);
        }

        private void DoSelect(bool activate)
        {
            Debug.Assert(DockPosition != DockPosition.Unknown);
            Debug.Assert(!IsSelected);

            SelectEventArgs e = new SelectEventArgs(this, activate);
            RaiseStateChangingEvent(e);
            if (IsHidden)
                UnHide();
            CoerceValues(activate ? DockItemShowMethod.Activate : DockItemShowMethod.Select, null);
            RaiseStateChangedEvent(e);
        }

        private void Deselect()
        {
            Debug.Assert(DockPosition != DockPosition.Unknown);

            if (!FirstPane.IsAutoHide && FirstPane.VisibleItems.Count == 0)
                Select(false);
            else if (IsHidden || (IsSelected && FirstPane.IsAutoHide))
                DeselectCommand.Execute(this);
        }

        private void DoDeselect()
        {
            Debug.Assert(IsHidden || (IsSelected && DockPositionHelper.IsAutoHide(DockPosition)));

            DeselectEventArgs e = new DeselectEventArgs(this);
            RaiseStateChangingEvent(e);
            UnHide();
            CoerceValues(DockItemShowMethod.Deselect, null);
            RaiseStateChangedEvent(e);
        }

        private void UnHide()
        {
            DockTree dockTree = FirstPane.DockTree;
            dockTree.AddDirtyNode(FirstPane);
            IsHidden = false;
            RefreshDockPosition();
            dockTree.CommitChanges();
        }

        /// <summary>Makes the <see cref="DockItem"/> invisible.</summary>
        /// <exception cref="InvalidOperationException"><see cref="DockPosition"/> is <see cref="Docking.DockPosition.Unknown"/>.</exception>
        /// <remarks>When the <see cref="DockItem"/> is hidden, the <see cref="IsHidden"/> property is set to
        /// <see langword="true"/> and it is removed from <see cref="DockPane.VisibleItems"/>
        /// collection of its <see cref="FirstPane"/>.</remarks>
        public void Hide()
        {
            if (DockPosition == DockPosition.Unknown)
                throw new InvalidOperationException();

            if (!IsHidden)
                HideCommand.Execute(this);
        }

        private void DoHide()
        {
            Debug.Assert(!IsHidden);

            HideEventArgs e = new HideEventArgs(this);
            RaiseStateChangingEvent(e);
            DockTree dockTree = FirstPane.DockTree;
            dockTree.AddDirtyNode(FirstPane);
            IsHidden = true;
            RefreshDockPosition();
            dockTree.CommitChanges();
            CoerceValues(DockItemShowMethod.Hide, null);
            RaiseStateChangedEvent(e);
        }

        /// <overloads>Toggles the auto hide state of the <see cref="DockItem"/> object's <see cref="FirstPane"/>.</overloads>
        /// <summary>Toggles the auto hide state of the <see cref="DockItem"/> object's <see cref="FirstPane"/>, using 
        /// default <see cref="DockItemShowMethod"/> for this <see cref="DockItem"/>.</summary>
        /// <returns><see langword="true"/> if operation performed successfully, otherwise <see langword="false"/>.</returns>
        /// <remarks>This method is wrapper of <see cref="ToggleAutoHide(DockItemShowMethod)"/> method.
        /// It uses <see cref="DockItemShowMethod.Activate"/> from auto hide to non auto hide, uses
        /// <see cref="DockItemShowMethod.Select"/> from non auto hide to auto hide.</remarks>
        public bool ToggleAutoHide()
        {
            if (FirstPane.IsAutoHide)
                return ToggleAutoHide(DockItemShowMethod.Activate);
            else
                return ToggleAutoHide(DockItemShowMethod.Select);
        }

        /// <summary>Toggles the auto hide state of the <see cref="DockItem"/> object's <see cref="FirstPane"/>,
        /// using specified <see cref="DockItemShowMethod"/> for this <see cref="DockItem"/>.</summary>
        /// <param name="showMethod">The specified show method.</param>
        /// <returns><see langword="true"/> if operation performed successfully, otherwise <see langword="false"/>.</returns>
        public bool ToggleAutoHide(DockItemShowMethod showMethod)
        {
            if (!CanToggleAutoHide)
                return false;

            ToggleAutoHideCommand.Execute(this, showMethod);
            return true;
        }

        private void DoToggleAutoHide(DockItemShowMethod showMethod)
        {
            if (showMethod == DockItemShowMethod.Deselect &&
                IsAutoHide &&
                FirstPane.SelectedItem == this)
                showMethod = DockItemShowMethod.Select;
               
            ToggleAutoHideEventArgs[] args = new ToggleAutoHideEventArgs[FirstPane.VisibleItems.Count];
            for (int i = 0; i < FirstPane.VisibleItems.Count; i++)
                args[i] = new ToggleAutoHideEventArgs(FirstPane.VisibleItems[i], FirstPane.IsAutoHide, showMethod);

            foreach (ToggleAutoHideEventArgs e in args)
                RaiseStateChangingEvent(e);
            
            DockTree dockTree = FirstPane.DockTree;
            dockTree.AddDirtyNode(FirstPane);
            FirstPane.IsAutoHide = !FirstPane.IsAutoHide;
            IsHidden = (showMethod == DockItemShowMethod.Hide);
            foreach (ToggleAutoHideEventArgs e in args)
                e.DockItem.RefreshDockPosition();
            dockTree.CommitChanges();
            CoerceValues(showMethod, null);

            foreach (ToggleAutoHideEventArgs e in args)
                RaiseStateChangedEvent(e);
        }

        /// <summary>Gets a value indicates whether the auto hide mode of this <see cref="DockItem"/> can be toggled.</summary>
        /// <value><see langword="true"/> if the auto hide mode of this <see cref="DockItem"/> can be toggled, otherwise <see langword="false"/>.</value>
        public bool CanToggleAutoHide
        {
            get
            {
                DockPosition dockPosition = DockPosition;
                return (dockPosition == DockPosition.Left ||
                    dockPosition == DockPosition.LeftAutoHide ||
                    dockPosition == DockPosition.Right ||
                    dockPosition == DockPosition.RightAutoHide ||
                    dockPosition == DockPosition.Top ||
                    dockPosition == DockPosition.TopAutoHide ||
                    dockPosition == DockPosition.Bottom ||
                    dockPosition == DockPosition.BottomAutoHide);
            }
        }

        /// <overloads>Toggles the floating state of the <see cref="DockItem"/>.</overloads>
        /// <summary>Toggles the floating state of the <see cref="DockItem"/>, as activated.</summary>
        /// <returns><see langword="true"/> if operation performed successfully, otherwise <see langword="false"/>.</returns>
        /// <remarks><para>A <see cref="DockItem"/> object can contained by two <see cref="DockPane"/> objects, one for floating mode and one for non
        /// floating mode. The <see cref="FirstPane"/> property always returns the <see cref="DockPane"/> that the <see cref="DockItem"/> last
        /// shown. Calling <see cref="ToggleFloating()"/> swaps <see cref="FirstPane"/> and <see cref="SecondPane"/> if they both
        /// exist.</para>
        /// <para>This method is a wrapper of <see cref="ToggleFloating(DockItemShowMethod)"/> method using
        /// <see cref="DockItemShowMethod.Activate"/> value.</para></remarks>
        public bool ToggleFloating()
        {
            return ToggleFloating(DockItemShowMethod.Activate);
        }

        /// <summary>Toggles the floating state of the <see cref="DockItem"/>, using specified show method.</summary>
        /// <param name="showMethod">The specified show method.</param>
        /// <returns><see langword="true"/> if operation performed successfully, otherwise <see langword="false"/>.</returns>
        /// <remarks>A <see cref="DockItem"/> object can contained by two <see cref="DockPane"/> objects, one for floating mode and one for non
        /// floating mode. The <see cref="FirstPane"/> property always returns the <see cref="DockPane"/> that the <see cref="DockItem"/> last
        /// shown. Calling <see cref="ToggleFloating()"/> swaps <see cref="FirstPane"/> and <see cref="SecondPane"/> if they both
        /// exist.</remarks>
        public bool ToggleFloating(DockItemShowMethod showMethod)
        {
            if (!CanToggleFloating)
                return false;

            if (SecondPane != null)
                Show(SecondPane, SecondPane.Items.IndexOf(this), showMethod);
            else
                Show(DockControl, GetDefaultDockPosition(!FirstPane.IsFloating), showMethod);
            return true;
        }

        /// <summary>Gets a value indicates whether the floating mode of this <see cref="DockItem"/> can be toggled.</summary>
        /// <value><see langword="true"/> if the floating mode of this <see cref="DockItem"/> can be toggled, otherwise <see langword="false"/>.</value>
        public bool CanToggleFloating
        {
            get
            {
                if (FirstPane == null)
                    return false;

                if (SecondPane != null)
                    return true;

                return GetDefaultDockPosition(!FirstPane.IsFloating) != DockPosition.Unknown;
            }
        }

        private DockPosition GetDefaultDockPosition(bool isFloating)
        {
            if (isFloating)
                return DockPositionHelper.IsValid(DockPosition.Floating, AllowedDockTreePositions) ? DockPosition.Floating : DockPosition.Unknown;

            DockTreePosition? dockTreePosition = DockPositionHelper.GetDockTreePosition(DefaultDockPosition);
            if (dockTreePosition != null && dockTreePosition != Docking.DockTreePosition.Floating && DockPositionHelper.IsValid(dockTreePosition, AllowedDockTreePositions))
                return DefaultDockPosition;
            else if (DockPositionHelper.IsValid(DockPosition.Left, AllowedDockTreePositions))
                return DockPosition.Left;
            else if (DockPositionHelper.IsValid(DockPosition.Right, AllowedDockTreePositions))
                return DockPosition.Right;
            else if (DockPositionHelper.IsValid(DockPosition.Top, AllowedDockTreePositions))
                return DockPosition.Top;
            else if (DockPositionHelper.IsValid(DockPosition.Bottom, AllowedDockTreePositions))
                return DockPosition.Bottom;
            else if (DockPositionHelper.IsValid(DockPosition.Document, AllowedDockTreePositions))
                return DockPosition.Document;
            else
                return DockPosition.Unknown;
        }

        /// <summary>Shows the <see cref="DockItem"/> to the edge of <see cref="Docking.DockControl"/>, brings the respective dock tree to front
        /// or sends it to back, using specified show method.</summary>
        /// <param name="dockControl">The target <see cref="Docking.DockControl"/>.</param>
        /// <param name="dock">Indicates which edge of <see cref="Docking.DockControl"/> to show.</param>
        /// <param name="sendToBack"><see langword="true"/> if send the respective dock tree to back, <see langword="false"/> to
        /// bring the respective dock tree to front.</param>
        /// <param name="showMethod">The specified show method.</param>
        public void Show(DockControl dockControl, Dock dock, bool sendToBack, DockItemShowMethod showMethod)
        {
            VerifyShowParam(dockControl);
            dockControl.BeginUndoUnit();
            if (sendToBack)
                dockControl.DockTreeZOrder = dockControl.DockTreeZOrder.SendToBack(dock);
            else
                dockControl.DockTreeZOrder = dockControl.DockTreeZOrder.BringToFront(dock);
            if (dock == Dock.Left)
                Show(dockControl, DockPosition.Left, showMethod);
            else if (dock == Dock.Right)
                Show(dockControl, DockPosition.Right, showMethod);
            else if (dock == Dock.Top)
                Show(dockControl, DockPosition.Top, showMethod);
            else if (dock == Dock.Bottom)
                Show(dockControl, DockPosition.Bottom, showMethod);
            dockControl.EndUndoUnit();
        }

        /// <summary>Shows the <see cref="DockItem"/> to specified dock position, as activated.</summary>
        /// <param name="dockControl">The targeting <see cref="Docking.DockControl"/>.</param>
        /// <param name="dockPosition">The specified dock position.</param>
        /// <remarks>This method is wrapper of <see cref="Show(DockControl, DockPosition, DockItemShowMethod)"/>
        /// using <see cref="DockItemShowMethod.Activate"/> value.</remarks>
        public void Show(DockControl dockControl, DockPosition dockPosition)
        {
            Show(dockControl, dockPosition, DockItemShowMethod.Activate);
        }

        /// <summary>Shows the <see cref="DockItem"/> to specified dock position, using specified show method.</summary>
        /// <param name="dockControl">The targeting <see cref="Docking.DockControl"/>.</param>
        /// <param name="dockPosition">The specified dock position.</param>
        /// <param name="showMethod">The specified show method.</param>
        public void Show(DockControl dockControl, DockPosition dockPosition, DockItemShowMethod showMethod)
        {
            VerifyShowParam(dockControl);
            if (dockPosition == DockPosition.Hidden || dockPosition == DockPosition.Unknown)
                throw new ArgumentException(SR.Exception_DockItem_Show_InvalidDockPosition, "dockPosition");

            if (dockPosition == DockPosition.Floating)
            {
                Size defaultSize = dockControl.DefaultFloatingWindowSize;
                Show(dockControl, new Rect(new Point(double.NaN, double.NaN), defaultSize), showMethod);
                return;
            }

            DockTree dockTree = dockControl.GetDockTree(DockPositionHelper.GetDockControlTreePosition(dockPosition));
            if (dockTree.RootNode != null)
            {
                bool isAutoHide = DockPositionHelper.IsAutoHide(dockPosition);
                DockPane pane = dockTree.FindPane(isAutoHide);
                if (pane != null)
                    Show(pane, pane.Items.Count, showMethod);
                else
                {
                    pane = dockTree.FirstPane;
                    Dock dock = dockTree.Position == Docking.DockTreePosition.Left || dockTree.Position == Docking.DockTreePosition.Right ? Dock.Top : Dock.Left;
                    Show(pane, isAutoHide, dock, new SplitterDistance(1.0, SplitterUnitType.Star), false, showMethod);
                }
                return;
            }
            else
                ShowAsDockPositionCommand.Execute(this, dockControl, dockPosition, showMethod);
        }

        private void DoShowAsDockPosition(DockControl dockControl, DockPosition dockPosition, DockItemShowMethod showMethod)
        {
            EnsureAttached(dockControl);
            ShowAsDockPositionEventArgs e = new ShowAsDockPositionEventArgs(this, dockControl, dockPosition, showMethod);
            RaiseStateChangingEvent(e);
            DockTree dockTree = DockControl.GetDockTree(DockPositionHelper.GetDockControlTreePosition(dockPosition));
            ShowAsDockTreeRoot(dockTree, DockPositionHelper.IsAutoHide(dockPosition), showMethod);
            RaiseStateChangedEvent(e);
        }

        /// <summary>Shows the <see cref="DockItem"/> as activated floating window.</summary>
        /// <param name="dockControl">The target <see cref="Docking.DockControl"/>.</param>
        /// <param name="floatingWindowBounds">The size and position of the floating window.</param>
        /// <remarks>The method is wrapper of <see cref="Show(DockControl, Rect, DockItemShowMethod)"/> using
        /// <see cref="DockItemShowMethod.Activate"/> value.</remarks>
        public void Show(DockControl dockControl, Rect floatingWindowBounds)
        {
            Show(dockControl, floatingWindowBounds, DockItemShowMethod.Activate);
        }

        /// <summary>Shows the <see cref="DockItem"/> as floating window, using specified show method.</summary>
        /// <param name="dockControl">The target <see cref="Docking.DockControl"/>.</param>
        /// <param name="floatingWindowBounds">The size and position of the floating window.</param>
        /// <param name="showMethod">The specified show method.</param>
        public void Show(DockControl dockControl, Rect floatingWindowBounds, DockItemShowMethod showMethod)
        {
            VerifyShowParam(dockControl);
            ShowAsFloatingCommand.Execute(this, dockControl, floatingWindowBounds, showMethod);
        }

        private void DoShowAsFloating(DockControl dockControl, Rect floatingWindowBounds, DockItemShowMethod showMethod)
        {
            EnsureAttached(dockControl);
            ShowAsFloatingEventArgs e = new ShowAsFloatingEventArgs(this, dockControl, floatingWindowBounds, showMethod);
            RaiseStateChangingEvent(e);
            FloatingWindow floatingWindow = dockControl.CreateFloatingWindow(floatingWindowBounds);
            DockTree dockTree = floatingWindow.DockTree;
            ShowAsDockTreeRoot(dockTree, false, showMethod);
            RaiseStateChangedEvent(e);
        }

        private void ShowAsDockTreeRoot(DockTree dockTree, bool isAutoHide, DockItemShowMethod showMethod)
        {
            Debug.Assert(dockTree.RootNode == null);

            DockPane newPane = dockTree.AddItem(this, isAutoHide);
            DockPane oldPane = SetPane(newPane, showMethod);
            CoerceValues(oldPane, showMethod, null);
        }

        /// <summary>Shows the <see cref="DockItem"/> as activated and tabbed.</summary>
        /// <param name="pane">The target <see cref="DockPane"/>.</param>
        /// <param name="index">The position within target <see cref="DockPane.Items">DockPane.Items</see> at which
        /// this <see cref="DockItem"/> is inserted before.</param>
        public void Show(DockPane pane, int index)
        {
            Show(pane, index, DockItemShowMethod.Activate);
        }

        /// <summary>Shows the <see cref="DockItem"/> as tabeed, using specified show method.</summary>
        /// <param name="dockItem">The position at which this <see cref="DockItem"/> is inserted before.</param>
        /// <param name="showMethod">The specified show method.</param>
        public void Show(DockItem dockItem, DockItemShowMethod showMethod)
        {
            DockPane pane = dockItem.FirstPane;
            Show(pane, pane.Items.IndexOf(dockItem), showMethod);
        }

        /// <summary>Shows the <see cref="DockItem"/> as tabbed, using specified show method.</summary>
        /// <param name="pane">The target <see cref="DockPane"/>.</param>
        /// <param name="index">The position within target <see cref="DockPane.Items">DockPane.Items</see> at which
        /// this <see cref="DockItem"/> is inserted before.</param>
        /// <param name="showMethod">The specified show method.</param>
        public void Show(DockPane pane, int index, DockItemShowMethod showMethod)
        {
            VerifyShowParam(pane);
            if (index == -1)
                index = pane.Items.Count;
            if (index < 0 || index > pane.Items.Count)
                throw new ArgumentOutOfRangeException("index");

            ShowAsTabbedCommand.Execute(this, pane, index, showMethod);
        }

        private void DoShowAsTabbed(DockPane pane, int index, DockItemShowMethod showMethod)
        {
            EnsureAttached(pane.DockControl);
            ShowAsTabbedEventArgs e = new ShowAsTabbedEventArgs(this, pane, index, showMethod);
            RaiseStateChangingEvent(e);
            DockPane oldPane = SetPane(pane, index, showMethod);
            CoerceValues(oldPane, showMethod, null);
            RaiseStateChangedEvent(e);
        }

        /// <summary>Shows the <see cref="DockItem"/> as activated non auto hide <see cref="DockPane"/>, side by side of target <see cref="DockPaneNode"/>.</summary>
        /// <param name="paneNode">The target <see cref="DockPaneNode"/>.</param>
        /// <param name="side">Indicates the <see cref="DockItem"/> shows on which side of target <see cref="DockPaneNode"/>.</param>
        /// <param name="size">The size of to be created <see cref="DockPane"/>.</param>
        /// <remarks>This method is wrapper of <see cref="Show(DockPaneNode, bool, Dock, SplitterDistance, bool, DockItemShowMethod)"/>
        /// method.</remarks>
        public void Show(DockPaneNode paneNode, Dock side, SplitterDistance size)
        {
            Show(paneNode, false, side, size, false, DockItemShowMethod.Activate);
        }

        /// <summary>Shows the <see cref="DockItem"/> as <see cref="DockPane"/>, side by side of target <see cref="DockPane"/>, using
        /// specified auto hide state, size for target setting and show method.</summary>
        /// <param name="paneNode">The target <see cref="DockPaneNode"/>.</param>
        /// <param name="isAutoHide">Indicates whether to be created <see cref="DockPane"/> is auto hide.</param>
        /// <param name="side">Indicates the <see cref="DockItem"/> shows on which side of target <see cref="DockPaneNode"/>.</param>
        /// <param name="size">The size of to be created <see cref="DockPane"/> or target <see cref="DockPaneNode"/>, depending on the value of
        /// <paramref name="isSizeForTarget"/>.</param>
        /// <param name="isSizeForTarget">Indicates whether <paramref name="size"/> is for to be created <see cref="DockPane"/> or target
        /// <see cref="DockPaneNode"/>.</param>
        /// <param name="showMethod">The specified show method.</param>
        public void Show(DockPaneNode paneNode, bool isAutoHide, Dock side, SplitterDistance size, bool isSizeForTarget, DockItemShowMethod showMethod)
        {
            VerifyShowParam(paneNode);

            DockPane pane = paneNode as DockPane;
            bool selfDock = pane != null && pane.Items.Count == 1 && pane.Items[0] == this && pane.IsAutoHide == isAutoHide;
            if (selfDock)
                Show(showMethod);
            else
                ShowAsSidePaneCommand.Execute(this, paneNode, isAutoHide, side, size, isSizeForTarget, showMethod);
        }

        private void DoShowAsSidePane(DockPaneNode paneNode, bool isAutoHide, Dock side, SplitterDistance size, bool isSizeForTarget, DockItemShowMethod showMethod)
        {
            EnsureAttached(paneNode.DockControl);
            ShowAsSidePaneEventArgs e = new ShowAsSidePaneEventArgs(this, paneNode, isAutoHide, side, size, isSizeForTarget, showMethod);
            RaiseStateChangingEvent(e);
            DockPane newPane = paneNode.DockTree.AddItem(this, paneNode, isAutoHide, side, size, isSizeForTarget);
            DockPane oldPane = SetPane(newPane, showMethod);
            CoerceValues(oldPane, showMethod, null);
            RaiseStateChangedEvent(e);
        }

        private void EnsureAttached(DockControl dockControl)
        {
            EnsureAttached(dockControl, -1);
        }

        private void EnsureAttached(DockControl dockControl, int attachIndex)
        {
            if (DockControl != null)
            {
                Debug.Assert(dockControl == DockControl || attachIndex == -1);
                return;
            }

            Debug.Assert(dockControl != null);
            if (attachIndex == -1)
                dockControl.DockItems.AddInternal(this);
            else
                dockControl.DockItems.Insert(attachIndex, this);
        }

        /// <summary>Hides or closes the <see cref="DockItem"/>, depending on the value of <see cref="HideOnPerformClose"/> property.</summary>
        /// <returns><see langword="true"/> if <see cref="DockItem"/> is hidden or closed successfully, otherwise, <see langword="false"/>.</returns>
        public bool PerformClose()
        {
            if (HideOnPerformClose)
            {
                Hide();
                return true;
            }
            else
                return Close();
        }

        /// <summary>Closes the <see cref="DockItem"/>.</summary>
        /// <returns><see langword="true"/> if <see cref="DockItem"/> successfully closed, otherwise, <see langword="false"/>.</returns>
        /// <remarks>When a <see cref="DockItem"/> is closed, it is removed from <see cref="Docking.DockControl.DockItems"/>
        /// collection of its <see cref="DockControl"/>. You can prevent the closing of a <see cref="DockItem"/> at run time by handling the
        /// <see cref="Closing"/> event and setting the <see cref="CancelEventArgs.Cancel"/> property of the
        /// <see cref="CancelEventArgs"/> passed as a parameter to your event handler.</remarks>
        public bool Close()
        {
            if (DockControl == null)
                return false;

            CancelEventArgs e = new CancelEventArgs(false);
            OnClosing(e);
            if (e.Cancel)
                return false;

            CloseCommand.Execute(this);
            return true;
        }

        private void DoClose()
        {
            Debug.Assert(DockControl != null);

            CloseEventArgs e = new CloseEventArgs(this);
            RaiseStateChangingEvent(e);
            if (FirstPane != null)
            {
                DockTree dockTree = FirstPane.DockTree;
                FirstPane.RemoveItem(this, true);
                dockTree.CommitChanges();
            }
            if (SecondPane != null)
            {
                DockTree dockTree = SecondPane.DockTree;
                SecondPane.RemoveItem(this, true);
                dockTree.CommitChanges();
            }

            DockPane oldFirstPane = FirstPane;
            FirstPane = SecondPane = null;
            IsHidden = false;
            RefreshDockPosition();
            DockControl.DockItems.Remove(this);
            CoerceValues(oldFirstPane, DockItemShowMethod.Hide, null);
            DockControl = null;
            RaiseStateChangedEvent(e);
        }

        private void VerifyShowParam(DockPaneNode pane)
        {
            if (pane == null)
                throw new ArgumentNullException("pane");

            VerifyShowParam(pane.DockControl);
        }

        private void VerifyShowParam(DockControl dockControl)
        {
            if (dockControl == null)
                throw new ArgumentNullException("dockControl");
            if (DockControl != null && dockControl != DockControl)
                throw new ArgumentException(SR.Exception_DockItem_VerifyShowParam_DifferentDockControl, "dockControl");
            if (!dockControl.IsLoaded)
                throw new ArgumentException(SR.Exception_DockItem_VerifyShowParam_UnloadedDockControl, "dockControl");
        }
        
        private void RefreshDockPosition()
        {
            Debug.Assert(DockControl != null);
            DockPosition oldDockPosition = DockPosition;
            DockPosition newDockPosition = GetDockPosition(IsHidden);

            if (oldDockPosition == newDockPosition)
                return;

            DockPosition = newDockPosition;
        }

        private DockPosition GetDockPosition(bool isHidden)
        {
            DockPosition dockPosition;
            DockPane firstPane = FirstPane;
            if (firstPane == null)
                dockPosition = DockPosition.Unknown;
            else
                dockPosition = DockPositionHelper.GetDockPosition(DockTreePosition, FirstPane.IsAutoHide, isHidden);

            return dockPosition;
        }

        /// <exclude/>
        protected override Size MeasureOverride(Size constraint)
        {
            Size size = base.MeasureOverride(constraint);
            if (double.IsInfinity(constraint.Width) || double.IsInfinity(constraint.Width))
                return size;
            else
                return constraint;
        }

        /// <exclude/>
        protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            bool flag = false;
            if (e.OriginalSource == this)
                flag = MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

            if (flag)
                e.Handled = true;
            else
                base.OnPreviewGotKeyboardFocus(e);
        }

        /// <summary>Raises the <see cref="Closing"/> event.</summary>
        /// <param name="e">An <see cref="CancelEventArgs"/> that contains the event data.</param>
        /// <remarks>
        /// <para>This method allows derived classes to handle the event without attaching a delegate.
        /// This is the preferred technique for handling the event in a derived class.</para>
        /// <para>When overriding in a derived class, be sure to call the base class's
        /// method so that registered delegates receive the event.</para>
        /// </remarks>
        protected virtual void OnClosing(CancelEventArgs e)
        {
            CancelEventHandler handler = Closing;
            if (handler != null)
                handler(this, e);
        }

        private static void RaiseStateChangingEvent(DockItemStateEventArgs e)
        {
            if (!DockPositionHelper.IsValid(e.NewDockTreePosition, e.DockItem.AllowedDockTreePositions))
                throw new InvalidOperationException();
            e.DockControl.PaneManager.OnDockItemStateChanging(e);
            e.DockItem.OnStateChanging(e);
        }

        private static void RaiseStateChangedEvent(DockItemStateEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
            e.DockControl.PaneManager.OnDockItemStateChanged(e);
            e.DockItem.OnStateChanged(e);
        }

        /// <summary>Raises the <see cref="StateChanging"/> event.</summary>
        /// <param name="e">An <see cref="DockItemStateEventArgs"/> that contains the event data.</param>
        /// <remarks>
        /// <para>This method allows derived classes to handle the event without attaching a delegate.
        /// This is the preferred technique for handling the event in a derived class.</para>
        /// <para>When overriding in a derived class, be sure to call the base class's
        /// method so that registered delegates receive the event.</para>
        /// </remarks>
        protected virtual void OnStateChanging(DockItemStateEventArgs e)
        {
            EventHandler<DockItemStateEventArgs> handler = StateChanging;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>Raises the <see cref="StateChanged"/> event.</summary>
        /// <param name="e">An <see cref="DockItemStateEventArgs"/> that contains the event data.</param>
        /// <remarks>
        /// <para>This method allows derived classes to handle the event without attaching a delegate.
        /// This is the preferred technique for handling the event in a derived class.</para>
        /// <para>When overriding in a derived class, be sure to call the base class's
        /// method so that registered delegates receive the event.</para>
        /// </remarks>
        protected virtual void OnStateChanged(DockItemStateEventArgs e)
        {
            EventHandler<DockItemStateEventArgs> handler = StateChanged;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>Occurs when the <see cref="DockItem"/> is closing.</summary>
        /// <remarks> To cancel the closure of a <see cref="DockItem"/>, set the <see cref="CancelEventArgs.Cancel"/> property of the
        /// <see cref="CancelEventArgs"/> passed to your event handler to <see langword="true"/>.</remarks>
        public event CancelEventHandler Closing;

        /// <summary>Occurs when the state of <see cref="DockItem"/> is changing.</summary>
        public event EventHandler<DockItemStateEventArgs> StateChanging;

        /// <summary>Occurs when the state of <see cref="DockItem"/> is changed.</summary>
        public event EventHandler<DockItemStateEventArgs> StateChanged;

        /// <summary>Saves the <see cref="DockItem"/>.</summary>
        /// <returns>The saved object instance represents this <see cref="DockItem"/>.
        /// The default implementation returns string of the type when <see cref="HideOnPerformClose"/> is
        /// <see langword="true"/>, otherwise returns the <see cref="DockItem"/> itself. The derived class
        /// can override this method to provide its own implementation.</returns>
        /// <remarks>When saving state of <see cref="Docking.DockControl"/> by calling
        /// <see cref="Docking.DockControl.Save">DockControl.Save</see>, the returned object instance
        /// is set as <see cref="DockItemReference.Target"/> property of <see cref="DockItemReference"/>.
        /// This provides the flexibility that any object instance can be saved for the <see cref="DockItem"/>.</remarks>
        protected internal virtual object Save()
        {
            if (HideOnPerformClose)
                return GetType().ToString();
            else
                return this;
        }

        /// <summary>Gets a value provides a <see cref="DockItem"/> reference for undo/redo.</summary>
        /// <value>The <see cref="IDockItemUndoRedoReference"/> to provide a <see cref="DockItem"/> reference for undo/redo.
        /// The default implementation returns current <see cref="DockItem"/> when <see cref="HideOnPerformClose"/> is <see langword="true"/>,
        /// otherwise returns <see langword="null"/>. Derived class can override to provide its own implementation.</value>
        /// <remarks>
        /// <para>When undo/redo an operation that previously removes a <see cref="DockItem"/> from
        /// <see cref="Docking.DockControl.DockItems">DockControl.DockItems</see>, such as closing a <see cref="DockItem"/>,
        /// a instance of <see cref="DockItem"/> needs to be retrieved and added back to <see cref="Docking.DockControl.DockItems">DockControl.DockItems</see>.
        /// Instead of holding this <see cref="DockItem"/> instance in the undo stack and preventing it from garbage collected,
        /// a instance of <see cref="IDockItemUndoRedoReference"/> instance is kept in the undo stack. This provides flexibility to
        /// optimize memory usage.</para>
        /// <para><see cref="DockItem"/> implements <see cref="IDockItemUndoRedoReference"/>, which returns itself. Returns <see cref="DockItem"/>
        /// itself keeps this <see cref="DockItem"/> in undo stack and prevents it from garbage collected. Returns a <see langword="null"/>
        /// breaks the undo/redo chain and clears the undo/redo stack.</para>
        /// </remarks>
        protected internal virtual IDockItemUndoRedoReference UndoRedoReference
        {
            get { return HideOnPerformClose ? this : null; }
        }

        DockItem IDockItemUndoRedoReference.DockItem
        {
            get { return this; }
        }

        private ShowAction _showAction;
        /// <summary>Gets or sets the show action in XAML initialization.</summary>
        /// <value>The show action in XAML initialization.</value>
        /// <remarks>Set this property in XAML only. After XAML initialization, this property will be set to 
        /// <see langword="null"/>, and subsequent setting value has no effect.</remarks>
        public ShowAction ShowAction
        {
            get { return _showAction; }
            set { _showAction = value; }
        }

        /// <exclude />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
    }
}
