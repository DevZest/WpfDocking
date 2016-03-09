using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Input;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Provides a set of static methods and attached properties to manage docking behaviors.</summary>
    /// <remarks>
    /// <para>By default, the <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.IsEnabled" /> attached property of <see cref="DockControl" /> is set to
    /// <see langword="true"/>. If you change this value, you must provide all the implementation.</para>
    /// <para><see cref="DockManager"/> class handles how floating windows are displayed. When running under
    /// partial trust, <see cref="WpfFloatingWindow"/> will be used; otherwise <see cref="NativeFloatingWindow"/> will be used.
    /// You can get the value from <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.FloatingWindowStrategy" /> attached property of <see cref="DockControl" />.</para>
    /// <para><see cref="DockManager"/> class handles drag and drop through the following attached properties:</para>
    /// <list type="bullet">
    /// <item>Set <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.CanDrag" /> attached property to <see langword="true"/> for element(s) in control template of
    /// <see cref="WpfFloatingWindow"/>, <see cref="NativeFloatingWindow"/>, and <see cref="DockWindow"/> derived classes (<see cref="ToolWindow"/> and
    /// <see cref="DocumentWindow"/>) to determine where drag and double click can be initiated.</item>
    /// <item>Set <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.TargetPosition"/> and <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.TargetItem"/>
    /// attached property for elements to indicate the drop target.</item>
    /// <item>The <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.ShowsGuide" /> attached property for <see cref="DockControl"/> and <see cref="DockPane"/> indicates
    /// whether docking guide should be displayed. The <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.ShowsLeftGuide" />,
    /// <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.ShowsRightGuide" />, <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.ShowsTopGuide" />,
    /// <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.ShowsBottomGuide" /> and <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.ShowsFillGuide" />
    /// attached properties for <see cref="DockControl"/> indicate whether the
    /// respective docking guide should be displayed. The <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.IsShiftKeyDown" /> attached property
    /// for <see cref="DockControl"/> indicates whether the SHIFT key is pressed down during drag and drop.</item>
    /// <item>The <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.Preview" /> attached property for <see cref="DockControl"/>, <see cref="DockPane"/> or <see cref="DockItem"/>
    /// indicates where the drop preview should be displayed. The <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.FloatingPreviewLeft" />,
    /// <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.FloatingPreviewTop" />, <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.FloatingPreviewWidth" />,
    /// <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.FloatingPreviewHeight" /> attached properties for <see cref="DockControl"/> indicates
    /// the floating preview size and location.</item>
    /// <item>In control template, set <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.Overlay" /> attached property for <see cref="FrameworkElement" /> to display the
    /// docking guide and preview overlay. Customize default floating window preview size by changing the value of
    /// <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.DefaultFloatingPreviewSize" /> attached property for <see cref="DockControl"/>.</item>
    /// </list>
    /// </remarks>
    public static partial class DockManager
    {
        /// <summary>Identifies the <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.IsEnabled"/> attached property.</summary>
        /// <AttachedPropertyComments>
        /// <summary>Gets or sets the value indicates whether <see cref="DockManager" /> is enabled for the <see cref="DockControl" />.</summary>
        /// <value><see langword="true" /> if <see cref="DockManager" /> is enabled, otherwise <see langword="false" />.</value>
        /// <remarks>The default style of <see cref="Docking.DockControl"/> class sets the <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.IsEnabled" />
        /// attached property to <see langword="true"/>. If you set the <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.IsEnabled" /> attached property
        /// to the default value <see langword="false" />, you must provide all the implementation.</remarks>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(DockManager),
            new FrameworkPropertyMetadata(BooleanBoxes.False, new PropertyChangedCallback(OnIsEnabledChanged)));

        private static readonly DependencyPropertyKey FloatingWindowStrategyPropertyKey = DependencyProperty.RegisterAttachedReadOnly("FloatingWindowStrategy", typeof(FloatingWindowStrategy), typeof(DockManager),
            new FrameworkPropertyMetadata(FloatingWindowStrategy.Unknown));
        /// <summary>Identifies the <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.FloatingWindowStrategy"/> attached property.</summary>
        /// <AttachedPropertyComments>
        /// <summary>Gets the value indicates how <see cref="FloatingWindow"/> objects are displayed for a given <see cref="DockControl"/>.</summary>
        /// <value>The <see cref="FloatingWindowStrategy"/> value indicates how <see cref="FloatingWindow"/> objects are displayed.</value>
        /// <remarks><see cref="DockManager"/> class handles how <see cref="FloatingWindow"/> objects are displayed. When running under
        /// partial trust, <see cref="WpfFloatingWindow"/> will be used; otherwise <see cref="NativeFloatingWindow"/> will be used.</remarks>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty FloatingWindowStrategyProperty = FloatingWindowStrategyPropertyKey.DependencyProperty;

        private static readonly DependencyProperty WindowHandlerProperty = DependencyProperty.RegisterAttached("WindowHandler", typeof(WindowHandler), typeof(DockManager),
            new FrameworkPropertyMetadata(null));
        private static WindowHandler GetWindowHandler(DockControl dockControl)
        {
            return (WindowHandler)dockControl.GetValue(WindowHandlerProperty);
        }
        private static void SetWindowHandler(DockControl dockControl, WindowHandler value)
        {
            Debug.Assert(dockControl != null);
            WindowHandler oldValue = GetWindowHandler(dockControl);
            Debug.Assert((oldValue == null) != (value == null));
            dockControl.SetValue(WindowHandlerProperty, value);
            if (oldValue != null)
                oldValue.Close();
        }

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockControl dockControl = d as DockControl;
            if (dockControl != null)
            {
                bool isEnabled = (bool)e.NewValue;
                SetWindowHandler(dockControl, isEnabled ? new WindowHandler(dockControl) : null);
            }
        }

        /// <summary>Gets the value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.IsEnabled" /> attached property
        /// from a given <see cref="DockControl" />.</summary>
        /// <param name="dockControl">The <see cref="DockControl"/> from which to read the property value.</param>
        /// <returns>The value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.IsEnabled" /> attached property.</returns>
        public static bool GetIsEnabled(DockControl dockControl)
        {
            return (bool)dockControl.GetValue(IsEnabledProperty);
        }

        /// <summary>Sets the value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.IsEnabled" /> attached property
        /// for a given <see cref="DockControl" />.</summary>
        /// <param name="dockControl">The <see cref="DockControl"/> on which to set the property value.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetIsEnabled(DockControl dockControl, bool value)
        {
            dockControl.SetValue(IsEnabledProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Gets the value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.FloatingWindowStrategy" /> attached property
        /// from a given <see cref="DockControl" />.</summary>
        /// <param name="dockControl">The <see cref="DockControl"/> from which to read the property value.</param>
        /// <returns>The value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.FloatingWindowStrategy" /> attached property.</returns>
        public static FloatingWindowStrategy GetFloatingWindowStrategy(DockControl dockControl)
        {
            return (FloatingWindowStrategy)dockControl.GetValue(FloatingWindowStrategyProperty);
        }

        private static void SetFloatingWindowStrategy(DockControl dockControl, FloatingWindowStrategy value)
        {
            if (value == FloatingWindowStrategy.Unknown)
                dockControl.ClearValue(FloatingWindowStrategyPropertyKey);
            else
                dockControl.SetValue(FloatingWindowStrategyPropertyKey, value);
        }

        /// <summary>Identifies the <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.Overlay"/> attached property.</summary>
        /// <AttachedPropertyComments>
        /// <summary>Gets or sets the <see cref="Overlay"/> for a given <see cref="FrameworkElement"/>.</summary>
        /// <value>The <see cref="Overlay"/> object.</value>
        /// <remarks>In control template, set <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.Overlay" /> attached property for <see cref="FrameworkElement" /> to display the
        /// docking guide and preview overlay.</remarks>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty OverlayProperty = DependencyProperty.RegisterAttached("Overlay", typeof(Overlay), typeof(DockManager),
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(Overlay.OnOverlayChanged)));
        private static readonly DependencyPropertyKey ShowsGuidePropertyKey = DependencyProperty.RegisterAttachedReadOnly("ShowsGuide", typeof(bool), typeof(DockManager),
            new FrameworkPropertyMetadata(BooleanBoxes.False));
        private static readonly DependencyPropertyKey IsShiftKeyDownPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsShiftKeyDown", typeof(bool), typeof(DockManager),
            new FrameworkPropertyMetadata(BooleanBoxes.False));
        private static readonly DependencyPropertyKey ShowsLeftGuidePropertyKey = DependencyProperty.RegisterAttachedReadOnly("ShowsLeftGuide", typeof(bool), typeof(DockManager),
            new FrameworkPropertyMetadata(BooleanBoxes.True));
        private static readonly DependencyPropertyKey ShowsRightGuidePropertyKey = DependencyProperty.RegisterAttachedReadOnly("ShowsRightGuide", typeof(bool), typeof(DockManager),
            new FrameworkPropertyMetadata(BooleanBoxes.True));
        private static readonly DependencyPropertyKey ShowsTopGuidePropertyKey = DependencyProperty.RegisterAttachedReadOnly("ShowsTopGuide", typeof(bool), typeof(DockManager),
            new FrameworkPropertyMetadata(BooleanBoxes.True));
        private static readonly DependencyPropertyKey ShowsBottomGuidePropertyKey = DependencyProperty.RegisterAttachedReadOnly("ShowsBottomGuide", typeof(bool), typeof(DockManager),
            new FrameworkPropertyMetadata(BooleanBoxes.True));
        private static readonly DependencyPropertyKey ShowsFillGuidePropertyKey = DependencyProperty.RegisterAttachedReadOnly("ShowsFillGuide", typeof(bool), typeof(DockManager),
            new FrameworkPropertyMetadata(BooleanBoxes.True));
        private static readonly DependencyPropertyKey PreviewPropertyKey = DependencyProperty.RegisterAttachedReadOnly("Preview", typeof(DropPosition), typeof(DockManager),
            new FrameworkPropertyMetadata(DropPosition.None));
        private static readonly DependencyPropertyKey FloatingPreviewLeftPropertyKey = DependencyProperty.RegisterAttachedReadOnly("FloatingPreviewLeft", typeof(double), typeof(DockManager),
            new FrameworkPropertyMetadata(double.NaN));
        private static readonly DependencyPropertyKey FloatingPreviewTopPropertyKey = DependencyProperty.RegisterAttachedReadOnly("FloatingPreviewTop", typeof(double), typeof(DockManager),
            new FrameworkPropertyMetadata(double.NaN));
        private static readonly DependencyPropertyKey FloatingPreviewWidthPropertyKey = DependencyProperty.RegisterAttachedReadOnly("FloatingPreviewWidth", typeof(double), typeof(DockManager),
            new FrameworkPropertyMetadata(double.NaN));
        private static readonly DependencyPropertyKey FloatingPreviewHeightPropertyKey = DependencyProperty.RegisterAttachedReadOnly("FloatingPreviewHeight", typeof(double), typeof(DockManager),
            new FrameworkPropertyMetadata(double.NaN));

        /// <summary>Identifies the <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.CanDrag" /> attached property.</summary>
        /// <AttachedPropertyComments>
        /// <summary>Gets or sets the value indicates whether drag and double click can be initiated for the specified element.</summary>
        /// <value><see langword="true"/> if drag and double click can be initiated, otherwise <see langword="false"/>.</value>
        /// <remarks>Set <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.CanDrag" /> attached property to <see langword="true"/> for element(s) in control template of
        /// <see cref="WpfFloatingWindow"/>, <see cref="NativeFloatingWindow"/>, and <see cref="DockWindow"/> derived classes (<see cref="ToolWindow"/> and
        /// <see cref="DocumentWindow"/>) to determine where drag and double click can be initiated.</remarks>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty CanDragProperty = DependencyProperty.RegisterAttached("CanDrag", typeof(bool), typeof(DockManager),
            new FrameworkPropertyMetadata(BooleanBoxes.False));

        /// <summary>Identifies the <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.TargetPosition"/> attached property.</summary>
        /// <AttachedPropertyComments>
        /// <summary>Gets or sets the target <see cref="DropPosition"/> for specified element.</summary>
        /// <value>The target <see cref="DropPosition"/>. The default value is <see cref="DropPosition.None"/>.</value>
        /// <remarks>
        /// <para><see cref="P:DevZest.Windows.Docking.Primitives.DockManager.TargetItem"/> and <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.TargetPosition"/>
        /// attached property are used together to determine the drop target:</para>
        /// <para>
        /// <list type="table">
        ///     <listheader>
        ///         <term>TargetItem</term>
        ///         <term>TargetPosition</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term>null</term>
        ///         <term><see cref="DropPosition.Left"/>, <see cref="DropPosition.Right"/>, <see cref="DropPosition.Top"/> or <see cref="DropPosition.Bottom"/></term>
        ///         <description>Show as tool window in the <see cref="DockControl"/>.</description>
        ///     </item>
        ///     <item>
        ///         <term>null</term>
        ///         <term><see cref="DropPosition.Fill"/></term>
        ///         <description>Show as document window in the <see cref="DockControl"/>.</description>
        ///     </item>
        ///     <item>
        ///         <term>not null</term>
        ///         <term><see cref="DropPosition.Left"/>, <see cref="DropPosition.Right"/>, <see cref="DropPosition.Top"/> or <see cref="DropPosition.Bottom"/></term>
        ///         <description>Show as side pane of specified <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.TargetItem" />'s <see cref="DockPane"/>.</description>
        ///     </item>
        ///     <item>
        ///         <term>not null</term>
        ///         <term><see cref="DropPosition.Fill"/></term>
        ///         <description>Show as last tab of specified <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.TargetItem"/>'s <see cref="DockPane"/>.</description>
        ///     </item>
        ///     <item>
        ///         <term>not null</term>
        ///         <term><see cref="DropPosition.Tab"/></term>
        ///         <description>Show as tab inserted before specified <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.TargetItem" />.</description>
        ///     </item>
        /// </list>
        /// </para>
        /// <para>Any other combinitions are invalid.</para>
        /// </remarks>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty TargetPositionProperty = DependencyProperty.RegisterAttached("TargetPosition", typeof(DropPosition), typeof(DockManager),
            new FrameworkPropertyMetadata(DropPosition.None));

        /// <summary>Identifies the <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.TargetItem"/> attached property.</summary>
        /// <AttachedPropertyComments>
        /// <summary>Gets or sets the drop target <see cref="DockItem"/> for specified element.</summary>
        /// <value>The drop target <see cref="DockItem"/>. The default value is <see langword="null"/>.</value>
        /// <remarks>
        /// <para><see cref="P:DevZest.Windows.Docking.Primitives.DockManager.TargetItem"/> and <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.TargetPosition"/>
        /// attached property are used together to determine the drop target:</para>
        /// <para>
        /// <list type="table">
        ///     <listheader>
        ///         <term>TargetItem</term>
        ///         <term>TargetPosition</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term>null</term>
        ///         <term><see cref="DropPosition.Left"/>, <see cref="DropPosition.Right"/>, <see cref="DropPosition.Top"/> or <see cref="DropPosition.Bottom"/></term>
        ///         <description>Show as tool window in the <see cref="DockControl"/>.</description>
        ///     </item>
        ///     <item>
        ///         <term>null</term>
        ///         <term><see cref="DropPosition.Fill"/></term>
        ///         <description>Show as document window in the <see cref="DockControl"/>.</description>
        ///     </item>
        ///     <item>
        ///         <term>not null</term>
        ///         <term><see cref="DropPosition.Left"/>, <see cref="DropPosition.Right"/>, <see cref="DropPosition.Top"/> or <see cref="DropPosition.Bottom"/></term>
        ///         <description>Show as side pane of specified <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.TargetItem" />'s <see cref="DockPane"/>.</description>
        ///     </item>
        ///     <item>
        ///         <term>not null</term>
        ///         <term><see cref="DropPosition.Fill"/></term>
        ///         <description>Show as last tab of specified <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.TargetItem"/>'s <see cref="DockPane"/>.</description>
        ///     </item>
        ///     <item>
        ///         <term>not null</term>
        ///         <term><see cref="DropPosition.Tab"/></term>
        ///         <description>Show as tab inserted before specified <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.TargetItem" />.</description>
        ///     </item>
        /// </list>
        /// </para>
        /// <para>Any other combinitions are invalid.</para>
        /// </remarks>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty TargetItemProperty = DependencyProperty.RegisterAttached("TargetItem", typeof(DockItem), typeof(DockManager),
            new FrameworkPropertyMetadata(null));

        /// <summary>Identifies the <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.ShowsGuide"/> attached property.</summary>
        /// <AttachedPropertyComments>
        /// <summary>Gets a value indicates whether docking guide should be displayed for specified element.</summary>
        /// <value><see langword="true"/> if docking guide should be displayed. Otherwise <see langword="false"/>. The default value is <see langword="false"/>.</value>
        /// <remarks>The drag and drop handler only sets value <see langword="true" /> for elements of type <see cref="DockControl"/> or <see cref="DockPane"/>.
        /// The template of these elements should show/hide the docking guide respectively.</remarks>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty ShowsGuideProperty = ShowsGuidePropertyKey.DependencyProperty;

        /// <summary>Identifies the <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.IsShiftKeyDown"/> attached property.</summary>
        /// <AttachedPropertyComments>
        /// <summary>Gets a value indicates whether the SHIFT key is pressed down during drag and drop for a given <see cref="DockControl"/>.</summary>
        /// <value><see langword="true"/> if the SHIFT key is pressed down during drag and drop, otherwise <see langword="false"/>. The default value is <see langword="false"/>.</value>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty IsShiftKeyDownProperty = IsShiftKeyDownPropertyKey.DependencyProperty;

        /// <summary>Identifies the <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.ShowsLeftGuide"/> attached property.</summary>
        /// <AttachedPropertyComments>
        /// <summary>Gets a value indicates whether to show left docking guide for a given <see cref="DockControl"/>.</summary>
        /// <value><see langword="true"/> to show left docking guide, otherwise <see langword="false"/>. The default value is <see langword="false"/>.</value>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty ShowsLeftGuideProperty = ShowsLeftGuidePropertyKey.DependencyProperty;

        /// <summary>Identifies the <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.ShowsRightGuide"/> attached property.</summary>
        /// <AttachedPropertyComments>
        /// <summary>Gets a value indicates whether to show right docking guide for a given <see cref="DockControl"/>.</summary>
        /// <value><see langword="true"/> to show right docking guide, otherwise <see langword="false"/>. The default value is <see langword="false"/>.</value>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty ShowsRightGuideProperty = ShowsRightGuidePropertyKey.DependencyProperty;

        /// <summary>Identifies the <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.ShowsTopGuide"/> attached property.</summary>
        /// <AttachedPropertyComments>
        /// <summary>Gets a value indicates whether to show top docking guide for a given <see cref="DockControl"/>.</summary>
        /// <value><see langword="true"/> to show top docking guide, otherwise <see langword="false"/>. The default value is <see langword="false"/>.</value>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty ShowsTopGuideProperty = ShowsTopGuidePropertyKey.DependencyProperty;

        /// <summary>Identifies the <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.ShowsBottomGuide"/> attached property.</summary>
        /// <AttachedPropertyComments>
        /// <summary>Gets a value indicates whether to show bottom docking guide for a given <see cref="DockControl"/>.</summary>
        /// <value><see langword="true"/> to show bottom docking guide, otherwise <see langword="false"/>. The default value is <see langword="false"/>.</value>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty ShowsBottomGuideProperty = ShowsBottomGuidePropertyKey.DependencyProperty;

        /// <summary>Identifies the <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.ShowsFillGuide"/> attached property.</summary>
        /// <AttachedPropertyComments>
        /// <summary>Gets a value indicates whether to show fill docking guide for a given <see cref="DockControl"/>.</summary>
        /// <value><see langword="true"/> to show fill docking guide, otherwise <see langword="false"/>. The default value is <see langword="false"/>.</value>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty ShowsFillGuideProperty = ShowsFillGuidePropertyKey.DependencyProperty;

        /// <summary>Identifies the <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.Preview"/> attached property.</summary>
        /// <AttachedPropertyComments>
        /// <summary>Gets a value indicates where the drop preview should be displayed for specified element.</summary>
        /// <value>A <see cref="DropPosition"/> value indicates where the drop preview should be displayed. The default value is <see cref="DropPosition.None"/>.</value>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty PreviewProperty = PreviewPropertyKey.DependencyProperty;

        /// <summary>Identifies the <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.FloatingPreviewLeft"/> attached property.</summary>
        /// <AttachedPropertyComments>
        /// <summary>Gets a value indicates the left position of the floating preview for a given <see cref="DockControl"/>.</summary>
        /// <value>A value indicates the left position of the floating preview. This value is valid only when the 
        /// <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.Preview"/> attached property is <see cref="DropPosition.Floating"/>.</value>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty FloatingPreviewLeftProperty = FloatingPreviewLeftPropertyKey.DependencyProperty;

        /// <summary>Identifies the <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.FloatingPreviewTop"/> attached property.</summary>
        /// <AttachedPropertyComments>
        /// <summary>Gets a value indicates the top position of the floating preview for a given <see cref="DockControl"/>.</summary>
        /// <value>A value indicates the top position of the floating preview. This value is valid only when the 
        /// <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.Preview"/> attached property is <see cref="DropPosition.Floating"/>.</value>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty FloatingPreviewTopProperty = FloatingPreviewTopPropertyKey.DependencyProperty;

        /// <summary>Identifies the <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.FloatingPreviewWidth"/> attached property.</summary>
        /// <AttachedPropertyComments>
        /// <summary>Gets a value indicates the width of the floating preview for a given <see cref="DockControl"/>.</summary>
        /// <value>A value indicates the width of the floating preview. This value is valid only when the 
        /// <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.Preview"/> attached property is <see cref="DropPosition.Floating"/>.</value>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty FloatingPreviewWidthProperty = FloatingPreviewWidthPropertyKey.DependencyProperty;

        /// <summary>Identifies the <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.FloatingPreviewHeight"/> attached property.</summary>
        /// <AttachedPropertyComments>
        /// <summary>Gets a value indicates the height of the floating preview for a given <see cref="DockControl"/>.</summary>
        /// <value>A value indicates the height of the floating preview. This value is valid only when the 
        /// <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.Preview"/> attached property is <see cref="DropPosition.Floating"/>.</value>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty FloatingPreviewHeightProperty = FloatingPreviewHeightPropertyKey.DependencyProperty;

        /// <summary>Identifies the <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.DefaultFloatingPreviewSize"/> attached property.</summary>
        /// <AttachedPropertyComments>
        /// <summary>Gets or sets the default size of floating preview for a given <see cref="DockControl"/>.</summary>
        /// <value>The default size of floating preview. The default value is (300, 300).</value>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty DefaultFloatingPreviewSizeProperty = DependencyProperty.RegisterAttached("DefaultFloatingPreviewSize", typeof(Size), typeof(DockManager),
            new FrameworkPropertyMetadata(new Size(300, 300)));

        /// <summary>Gets the value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.Overlay" /> attached property
        /// from a given <see cref="FrameworkElement" />.</summary>
        /// <param name="element">The <see cref="FrameworkElement"/> from which to read the property value.</param>
        /// <returns>The value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.Overlay" /> attached property.</returns>
        public static Overlay GetOverlay(FrameworkElement element)
        {
            return (Overlay)element.GetValue(OverlayProperty);
        }

        /// <summary>Sets the value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.Overlay" /> attached property
        /// for a given <see cref="FrameworkElement" />.</summary>
        /// <param name="element">The <see cref="FrameworkElement"/> on which to set the property value.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetOverlay(FrameworkElement element, Overlay value)
        {
            element.SetValue(OverlayProperty, value);
        }

        internal static void AddOverlay(Overlay overlay)
        {
            Debug.Assert(overlay != null);
            DockControl dockControl = GetDockControl(overlay.AssociatedElement);
            if (dockControl == null)
                return;

            WindowHandler handler = GetWindowHandler(dockControl);

            // Native floating window preview is handled specially by OverlayWindow, due to transparent window performance
            if (overlay.IsFloatingWindowPreview && DockManager.GetFloatingWindowStrategy(dockControl) == FloatingWindowStrategy.Native)
            {
                handler.AttachFloatingWindowPreviewOverlay(overlay);
                return;
            }

            Canvas overlayCanvas = handler.OverlayCanvas;
            if (!overlay.IsFloatingWindowPreview)
            {
                Point location = overlay.AssociatedElement.TranslatePoint(new Point(0, 0), overlayCanvas);
                Canvas.SetLeft(overlay.Container, location.X);
                Canvas.SetTop(overlay.Container, location.Y);
                overlay.Container.Width = overlay.AssociatedElement.RenderSize.Width;
                overlay.Container.Height = overlay.AssociatedElement.RenderSize.Height;
            }
            else
            {
                overlay.Container.SetBinding(Canvas.LeftProperty, new Binding() { Source = dockControl, Path = new PropertyPath(DockManager.FloatingPreviewLeftProperty) });
                overlay.Container.SetBinding(Canvas.TopProperty, new Binding() { Source = dockControl, Path = new PropertyPath(DockManager.FloatingPreviewTopProperty) });
                overlay.Container.SetBinding(FrameworkElement.WidthProperty, new Binding() { Source = dockControl, Path = new PropertyPath(DockManager.FloatingPreviewWidthProperty) });
                overlay.Container.SetBinding(FrameworkElement.HeightProperty, new Binding() { Source = dockControl, Path = new PropertyPath(DockManager.FloatingPreviewHeightProperty) });
            }
            overlayCanvas.Children.Add(overlay.Container);
        }

        internal static void RemoveOverlay(Overlay overlay)
        {
            Debug.Assert(overlay != null);
            DockControl dockControl = GetDockControl(overlay.AssociatedElement);
            if (dockControl == null)
                return;

            WindowHandler handler = GetWindowHandler(dockControl);
            Canvas overlayCanvas = handler.OverlayCanvas;
            overlayCanvas.Children.Remove(overlay.Container);
        }

        private static DockControl GetDockControl(FrameworkElement element)
        {
            for (Visual visual = element; visual != null; visual = VisualTreeHelper.GetParent(visual) as Visual)
            {
                IDragSource dragSource = visual as IDragSource;
                if (dragSource != null)
                    return dragSource.DockControl;

                DockControl dockControl = visual as DockControl;
                if (dockControl != null)
                    return dockControl;
            }

            return null;
        }

        /// <summary>Gets the value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.DefaultFloatingPreviewSize" /> attached property
        /// from a given <see cref="DockControl" />.</summary>
        /// <param name="dockControl">The <see cref="DockControl"/> from which to read the property value.</param>
        /// <returns>The value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.DefaultFloatingPreviewSize" /> attached property.</returns>
        public static Size GetDefaultFloatingPreviewSize(DockControl dockControl)
        {
            return (Size)dockControl.GetValue(DefaultFloatingPreviewSizeProperty);
        }

        /// <summary>Sets the value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.DefaultFloatingPreviewSize" /> attached property
        /// for a given <see cref="DockControl" />.</summary>
        /// <param name="dockControl">The <see cref="DockControl"/> on which to set the property value.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetDefaultFloatingPreviewSize(DockControl dockControl, Size value)
        {
            dockControl.SetValue(DefaultFloatingPreviewSizeProperty, value);
        }

        /// <summary>Gets the value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.CanDrag" /> attached property
        /// from a given element.</summary>
        /// <param name="element">The element from which to read the property value.</param>
        /// <returns>The value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.CanDrag" /> attached property.</returns>
        public static bool GetCanDrag(DependencyObject element)
        {
            return (bool)element.GetValue(CanDragProperty);
        }

        /// <summary>Sets the value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.CanDrag" /> attached property
        /// for a given element.</summary>
        /// <param name="element">The element on which to set the property value.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetCanDrag(DependencyObject element, bool value)
        {
            element.SetValue(CanDragProperty, BooleanBoxes.Box(value));
        }

        internal static bool CanDrag(MouseEventArgs e)
        {
            if (e.Handled)
                return false;

            for (Visual visual = e.OriginalSource as Visual; visual != null; visual = VisualTreeHelper.GetParent(visual) as Visual)
            {
                if (GetCanDrag(visual))
                    return true;
            }

            return false;
        }

        /// <summary>Gets the value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.ShowsGuide" /> attached property
        /// from a given element.</summary>
        /// <param name="element">The element from which to read the property value.</param>
        /// <returns>The value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.ShowsGuide" /> attached property.</returns>
        public static bool GetShowsGuide(DependencyObject element)
        {
            return (bool)element.GetValue(ShowsGuideProperty);
        }

        private static void SetShowsGuide(DependencyObject element, bool value)
        {
            element.SetValue(ShowsGuidePropertyKey, BooleanBoxes.Box(value));
        }

        /// <summary>Gets the value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.IsShiftKeyDown" /> attached property
        /// from a given <see cref="DockControl" />.</summary>
        /// <param name="dockControl">The <see cref="DockControl"/> from which to read the property value.</param>
        /// <returns>The value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.IsShiftKeyDown" /> attached property.</returns>
        public static bool GetIsShiftKeyDown(DockControl dockControl)
        {
            return (bool)dockControl.GetValue(IsShiftKeyDownProperty);
        }

        private static void SetIsShiftKeyDown(DockControl element, bool value)
        {
            element.SetValue(IsShiftKeyDownPropertyKey, BooleanBoxes.Box(value));
        }

        /// <summary>Gets the value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.ShowsLeftGuide" /> attached property
        /// from a given <see cref="DockControl" />.</summary>
        /// <param name="dockControl">The <see cref="DockControl"/> from which to read the property value.</param>
        /// <returns>The value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.ShowsLeftGuide" /> attached property.</returns>
        public static bool GetShowsLeftGuide(DockControl dockControl)
        {
            return (bool)dockControl.GetValue(ShowsLeftGuideProperty);
        }

        /// <summary>Gets the value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.ShowsRightGuide" /> attached property
        /// from a given <see cref="DockControl" />.</summary>
        /// <param name="dockControl">The <see cref="DockControl"/> from which to read the property value.</param>
        /// <returns>The value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.ShowsRightGuide" /> attached property.</returns>
        public static bool GetShowsRightGuide(DockControl dockControl)
        {
            return (bool)dockControl.GetValue(ShowsRightGuideProperty);
        }

        /// <summary>Gets the value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.ShowsTopGuide" /> attached property
        /// from a given <see cref="DockControl" />.</summary>
        /// <param name="dockControl">The <see cref="DockControl"/> from which to read the property value.</param>
        /// <returns>The value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.ShowsTopGuide" /> attached property.</returns>
        public static bool GetShowsTopGuide(DockControl dockControl)
        {
            return (bool)dockControl.GetValue(ShowsTopGuideProperty);
        }

        /// <summary>Gets the value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.ShowsBottomGuide" /> attached property
        /// from a given <see cref="DockControl" />.</summary>
        /// <param name="dockControl">The <see cref="DockControl"/> from which to read the property value.</param>
        /// <returns>The value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.ShowsBottomGuide" /> attached property.</returns>
        public static bool GetShowsBottomGuide(DockControl dockControl)
        {
            return (bool)dockControl.GetValue(ShowsBottomGuideProperty);
        }

        /// <summary>Gets the value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.ShowsFillGuide" /> attached property
        /// from a given <see cref="DockControl" />.</summary>
        /// <param name="dockControl">The <see cref="DockControl"/> from which to read the property value.</param>
        /// <returns>The value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.ShowsFillGuide" /> attached property.</returns>
        public static bool GetShowsFillGuide(DockControl dockControl)
        {
            return (bool)dockControl.GetValue(ShowsFillGuideProperty);
        }

        private static void SetShowsLeftGuide(DockControl element, bool value)
        {
            element.SetValue(ShowsLeftGuidePropertyKey, BooleanBoxes.Box(value));
        }

        private static void SetShowsRightGuide(DockControl element, bool value)
        {
            element.SetValue(ShowsRightGuidePropertyKey, BooleanBoxes.Box(value));
        }

        private static void SetShowsTopGuide(DockControl element, bool value)
        {
            element.SetValue(ShowsTopGuidePropertyKey, BooleanBoxes.Box(value));
        }

        private static void SetShowsBottomGuide(DockControl element, bool value)
        {
            element.SetValue(ShowsBottomGuidePropertyKey, BooleanBoxes.Box(value));
        }

        private static void SetShowsFillGuide(DockControl element, bool value)
        {
            element.SetValue(ShowsFillGuidePropertyKey, BooleanBoxes.Box(value));
        }

        /// <summary>Gets the value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.Preview" /> attached property
        /// from a given element.</summary>
        /// <param name="element">The element from which to read the property value.</param>
        /// <returns>The value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.Preview" /> attached property.</returns>
        public static DropPosition GetPreview(DependencyObject element)
        {
            return (DropPosition)element.GetValue(PreviewProperty);
        }

        private static void SetPreview(DependencyObject element, DropPosition value)
        {
            element.SetValue(PreviewPropertyKey, value);
        }

        /// <summary>Gets the value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.FloatingPreviewLeft" /> attached property
        /// from a given <see cref="DockControl" />.</summary>
        /// <param name="dockControl">The <see cref="DockControl"/> from which to read the property value.</param>
        /// <returns>The value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.FloatingPreviewLeft" /> attached property.</returns>
        public static double GetFloatingPreviewLeft(DockControl dockControl)
        {
            return (double)dockControl.GetValue(FloatingPreviewLeftProperty);
        }

        private static void SetFloatingPreviewLeft(DockControl dockControl, double value)
        {
            dockControl.SetValue(FloatingPreviewLeftPropertyKey, value);
        }

        /// <summary>Gets the value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.FloatingPreviewTop" /> attached property
        /// from a given <see cref="DockControl" />.</summary>
        /// <param name="dockControl">The <see cref="DockControl"/> from which to read the property value.</param>
        /// <returns>The value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.FloatingPreviewTop" /> attached property.</returns>
        public static double GetFloatingPreviewTop(DockControl dockControl)
        {
            return (double)dockControl.GetValue(FloatingPreviewTopProperty);
        }

        private static void SetFloatingPreviewTop(DockControl element, double value)
        {
            element.SetValue(FloatingPreviewTopPropertyKey, value);
        }

        /// <summary>Gets the value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.FloatingPreviewWidth" /> attached property
        /// from a given <see cref="DockControl" />.</summary>
        /// <param name="dockControl">The <see cref="DockControl"/> from which to read the property value.</param>
        /// <returns>The value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.FloatingPreviewWidth" /> attached property.</returns>
        public static double GetFloatingPreviewWidth(DockControl dockControl)
        {
            return (double)dockControl.GetValue(FloatingPreviewWidthProperty);
        }

        private static void SetFloatingPreviewWidth(DockControl element, double value)
        {
            element.SetValue(FloatingPreviewWidthPropertyKey, value);
        }

        /// <summary>Gets the value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.FloatingPreviewHeight" /> attached property
        /// from a given <see cref="DockControl" />.</summary>
        /// <param name="dockControl">The <see cref="DockControl"/> from which to read the property value.</param>
        /// <returns>The value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.FloatingPreviewHeight" /> attached property.</returns>
        public static double GetFloatingPreviewHeight(DockControl dockControl)
        {
            return (double)dockControl.GetValue(FloatingPreviewHeightProperty);
        }

        private static void SetFloatingPreviewHeight(DockControl element, double value)
        {
            element.SetValue(FloatingPreviewHeightPropertyKey, value);
        }

        /// <summary>Gets the value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.TargetItem" /> attached property
        /// from a given element.</summary>
        /// <param name="element">The element from which to read the property value.</param>
        /// <returns>The value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.TargetItem" /> attached property.</returns>
        public static DockItem GetTargetItem(DependencyObject element)
        {
            return (DockItem)element.GetValue(TargetItemProperty);
        }

        /// <summary>Sets the value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.TargetItem" /> attached property
        /// for a given element.</summary>
        /// <param name="element">The element on which to set the property value.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetTargetItem(DependencyObject element, DockItem value)
        {
            element.SetValue(TargetItemProperty, value);
        }

        /// <summary>Gets the value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.TargetPosition" /> attached property
        /// from a given element.</summary>
        /// <param name="element">The element from which to read the property value.</param>
        /// <returns>The value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.TargetPosition" /> attached property.</returns>
        public static DropPosition GetTargetPosition(DependencyObject element)
        {
            return (DropPosition)element.GetValue(TargetPositionProperty);
        }

        /// <summary>Sets the value of <see cref="P:DevZest.Windows.Docking.Primitives.DockManager.TargetPosition" /> attached property
        /// for a given element.</summary>
        /// <param name="element">The element on which to set the property value.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetTargetPosition(DependencyObject element, DropPosition value)
        {
            element.SetValue(TargetPositionProperty, value);
        }

        internal static void BeginDrag(IDragSource source, UIElement element, MouseEventArgs e)
        {
            DragHandler.Singleton.BeginDrag(source, element, e);
        }

        internal static void BeginDrag(IDragSource source, UIElement element, Point pt)
        {
            DragHandler.Singleton.BeginDrag(source, element, pt);
        }

        internal static Rect GetDefaultFloatingPreview(DockControl dockControl, Point mouseStartPosition)
        {
            Size defaultFloatingPreviewSize = GetDefaultFloatingPreviewSize(dockControl);
            Point location = mouseStartPosition;
            location.Offset(-SystemParameters.CaptionWidth, -SystemParameters.CaptionHeight);
            return new Rect(location, defaultFloatingPreviewSize);
        }

        internal static bool CanDrop(DockItem dockItem, DockTreePosition dockTreePosition)
        {
            return CanDrop(dockItem, dockItem.DockControl, dockTreePosition);
        }

        internal static bool CanDrop(DockItem dockItem, DockPaneNode targetPaneNode)
        {
            if (!CanDrop(dockItem, targetPaneNode.DockControl, targetPaneNode.DockTreePosition))
                return false;

            DockPane targetPane = targetPaneNode as DockPane;
            if (targetPane != null)
            {
                if (targetPane.VisibleItems.Count == 1 && targetPane.VisibleItems[0] == dockItem)
                    return false;
            }

            return true;
        }

        internal static bool CanDrop(DockItem dockItem, DockItem targetItem)
        {
            if (!CanDrop(dockItem, targetItem.DockControl, targetItem.DockTreePosition))
                return false;

            return (targetItem != dockItem);
        }

        private static bool CanDrop(DockItem dockItem, DockControl dockControl, DockTreePosition? dockTreePosition)
        {
            Debug.Assert(dockItem != null);
            Debug.Assert(dockControl != null);
            Debug.Assert(dockTreePosition != null);

            if (dockItem.DockControl != dockControl)
                return false;

            return DockPositionHelper.IsValid(dockTreePosition, dockItem.AllowedDockTreePositions);
        }

        internal static void Drop(DockItem dockItem)
        {
            dockItem.Show(dockItem.DockControl, DockPosition.Document, GetShowMethod(dockItem));
        }

        internal static void Drop(DockItem dockItem, Dock dock, bool sendToBack)
        {
            dockItem.Show(dockItem.DockControl, dock, sendToBack, GetShowMethod(dockItem));
        }

        internal static void Drop(DockItem dockItem, Rect floatingWindowBounds)
        {
            dockItem.Show(dockItem.DockControl, floatingWindowBounds, GetShowMethod(dockItem));
        }

        internal static void Drop(DockItem dockItem, DockPane targetPane, DockPanePreviewPlacement placement)
        {
            DockItemShowMethod showMethod = GetShowMethod(dockItem);
            if (placement == DockPanePreviewPlacement.Fill)
                dockItem.Show(targetPane, -1, showMethod);
            else if (placement == DockPanePreviewPlacement.Left)
                dockItem.Show(targetPane, false, Dock.Left, new SplitterDistance(1, SplitterUnitType.Star), false, showMethod);
            else if (placement == DockPanePreviewPlacement.Right)
                dockItem.Show(targetPane, false, Dock.Right, new SplitterDistance(1, SplitterUnitType.Star), false, showMethod);
            else if (placement == DockPanePreviewPlacement.Top)
                dockItem.Show(targetPane, false, Dock.Top, new SplitterDistance(1, SplitterUnitType.Star), false, showMethod);
            else
            {
                Debug.Assert(placement == DockPanePreviewPlacement.Bottom);
                dockItem.Show(targetPane, false, Dock.Bottom, new SplitterDistance(1, SplitterUnitType.Star), false, showMethod);
            }
        }

        internal static void Drop(DockItem dockItem, DockItem targetItem)
        {
            dockItem.Show(targetItem, GetShowMethod(dockItem));
        }

        internal static DockItemShowMethod GetShowMethod(DockItem dockItem)
        {
            return dockItem.DockControl.FocusedItem == dockItem ? DockItemShowMethod.Activate : DockItemShowMethod.Select;
        }

        internal static bool CanDrop(DockPane dockPane, DockTreePosition dockTreePosition)
        {
            return CanDrop(dockPane, dockPane.DockControl, dockTreePosition);
        }

        internal static bool CanDrop(DockPane dockPane, DockPane targetPane)
        {
            if (targetPane == dockPane)
                return false;

            return CanDrop(dockPane, targetPane.DockControl, targetPane.DockTreePosition);
        }

        internal static bool CanDrop(DockPane dockPane, DockItem targetItem)
        {
            if (targetItem.FirstPane == dockPane)
                return false;

            return CanDrop(dockPane, targetItem.DockControl, targetItem.DockTreePosition);
        }

        private static bool CanDrop(DockPane dockPane, DockControl dockControl, DockTreePosition? dockTreePosition)
        {
            Debug.Assert(dockPane != null);
            Debug.Assert(dockControl != null);
            Debug.Assert(dockTreePosition != null);

            if (dockControl != dockPane.DockControl)
                return false;

            foreach (DockItem dockItem in dockPane.VisibleItems)
            {
                if (CanDrop(dockItem, dockControl, dockTreePosition))
                    return true;
            }

            return false;
        }

        internal static void Drop(DockPane dockPane)
        {
            Drop(dockPane,
                delegate(DockItem dockItem) { return CanDrop(dockItem, DockTreePosition.Document); },
                delegate(DockItem dockItem) { Drop(dockItem); });
        }

        internal static void Drop(DockPane dockPane, Dock dock, bool sendToBack)
        {
            Drop(dockPane,
                delegate(DockItem dockItem) { return CanDrop(dockItem, DockPositionHelper.GetDockTreePosition(dock)); },
                delegate(DockItem dockItem) { Drop(dockItem, dock, sendToBack); });
        }

        internal static void Drop(DockPane dockPane, Rect floatingWindowBounds)
        {
            Drop(dockPane,
                delegate(DockItem dockItem) { return CanDrop(dockItem, DockTreePosition.Floating); },
                delegate(DockItem dockItem) { Drop(dockItem, floatingWindowBounds); });
        }

        internal static void Drop(DockPane dockPane, DockPane targetPane, DockPanePreviewPlacement placement)
        {
            Drop(dockPane,
                delegate(DockItem dockItem) { return CanDrop(dockItem, targetPane); },
                delegate(DockItem dockItem) { Drop(dockItem, targetPane, placement); });
        }

        internal static void Drop(DockPane dockPane, DockItem targetItem)
        {
            Drop(dockPane,
                delegate(DockItem dockItem) { return CanDrop(dockItem, targetItem); },
                delegate(DockItem dockItem) { Drop(dockItem, targetItem); });
        }

        private static DockItem[] GetActiveItems(DockPane dockPane)
        {
            DockItem[] items = new DockItem[dockPane.ActiveItems.Count];
            dockPane.ActiveItems.CopyTo(items, 0);
            return items;
        }

        private static DockItem[] GetVisibleItems(DockPane dockPane)
        {
            DockItem[] items = new DockItem[dockPane.VisibleItems.Count];
            dockPane.VisibleItems.CopyTo(items, 0);
            return items;
        }

        private static int GetDropPaneIndex(DockItem dockItem, DockItem[] visibleItems, DockPane targetPane)
        {
            for (int i = Array.IndexOf(visibleItems, dockItem) + 1; i < visibleItems.Length; i++)
            {
                int index = targetPane.Items.IndexOf(visibleItems[i]);
                if (index != -1)
                    return index;
            }
            return targetPane.Items.Count;
        }

        private static void Drop(DockPane dockPane, Predicate<DockItem> canDrop, Action<DockItem> dropFirstItem)
        {
            DockControl dockControl = dockPane.DockControl;
            DockItem[] visibleItems = GetVisibleItems(dockPane);
            DockItem[] activeItems = GetActiveItems(dockPane);
            bool flagFirstItem = true;
            DockPane targetPane = null;
            dockControl.BeginUndoUnit();
            foreach (DockItem item in activeItems)
            {
                if (!canDrop(item))
                    continue;

                if (flagFirstItem)
                {
                    flagFirstItem = false;
                    dropFirstItem(item);
                    targetPane = item.FirstPane;
                }
                else
                    item.Show(targetPane, GetDropPaneIndex(item, visibleItems, targetPane), GetShowMethod(item));
            }
            dockControl.EndUndoUnit();
        }

        internal static bool CanDrop(FloatingWindow floatingWindow, DockTreePosition dockTreePosition)
        {
            foreach (DockPane dockPane in floatingWindow.VisiblePanes)
            {
                if (CanDrop(dockPane, dockTreePosition))
                    return true;
            }

            return false;
        }

        internal static bool CanDrop(FloatingWindow floatingWindow, DockPane targetPane)
        {
            if (targetPane.FloatingWindow == floatingWindow)
                return false;

            foreach (DockPane dockPane in floatingWindow.VisiblePanes)
            {
                if (CanDrop(dockPane, targetPane))
                    return true;
            }            

            return false;
        }

        internal static bool CanDrop(FloatingWindow floatingWindow, DockItem targetItem)
        {
            if (targetItem.FirstPane.FloatingWindow == floatingWindow)
                return false;

            foreach (DockPane dockPane in floatingWindow.VisiblePanes)
            {
                if (CanDrop(dockPane, targetItem))
                    return true;
            }            

            return false;
        }

        internal static void Drop(FloatingWindow floatingWindow, Dock dock, bool sendToBack)
        {
            Drop(floatingWindow,
                delegate(DockPane dockPane) { return CanDrop(dockPane, DockPositionHelper.GetDockTreePosition(dock)); },
                delegate(DockPane dockPane) { Drop(dockPane, dock, sendToBack); });
        }

        internal static void Drop(FloatingWindow floatingWindow)
        {
            Drop(floatingWindow,
                delegate(DockPane dockPane) { return CanDrop(dockPane, DockTreePosition.Document); },
                delegate(DockPane dockPane) { Drop(dockPane); });
        }

        internal static void Drop(FloatingWindow floatingWindow, Rect floatingWindowBounds)
        {
            floatingWindow.Left = floatingWindowBounds.Left;
            floatingWindow.Top = floatingWindowBounds.Top;
            floatingWindow.Width = floatingWindowBounds.Width;
            floatingWindow.Height = floatingWindow.Height;
        }

        internal static void Drop(FloatingWindow floatingWindow, DockPane targetPane, DockPanePreviewPlacement placement)
        {
            DockControl dockControl = floatingWindow.DockControl;
            dockControl.BeginUndoUnit();
            if (placement != DockPanePreviewPlacement.Fill)
            {
                Drop(floatingWindow,
                    delegate(DockPane dockPane) { return CanDrop(dockPane, targetPane); },
                    delegate(DockPane dockPane) { Drop(dockPane, targetPane, placement); });
            }
            else
            {
                DockPane[] panes = GetPanes(floatingWindow,
                    delegate(DockPane dockPane) { return CanDrop(dockPane, targetPane); });
                NestedDockEntry[] enties = GetNestedDockEntries(panes);
                Drop(panes[0], targetPane, DockPanePreviewPlacement.Fill);
                foreach (NestedDockEntry entry in enties)
                    Drop(entry.SourcePane, targetPane, DockPanePreviewPlacement.Fill);
            }
            dockControl.EndUndoUnit();
        }

        internal static void Drop(FloatingWindow floatingWindow, DockItem targetItem)
        {
            DockControl dockControl = floatingWindow.DockControl;
            dockControl.BeginUndoUnit();
            DockPane[] panes = GetPanes(floatingWindow,
                delegate(DockPane dockPane) { return CanDrop(dockPane, targetItem); });
            NestedDockEntry[] enties = GetNestedDockEntries(panes);
            Drop(panes[0], targetItem);
            foreach (NestedDockEntry entry in enties)
                Drop(entry.SourcePane, targetItem);
            dockControl.EndUndoUnit();
        }

        private static void Drop(FloatingWindow floatingWindow, Predicate<DockPane> canDrop, Action<DockPane> dropFirstPane)
        {
            DockControl dockControl = floatingWindow.DockControl;
            dockControl.BeginUndoUnit();
            DockPane[] panes = GetPanes(floatingWindow, canDrop);
            NestedDockEntry[] enties = GetNestedDockEntries(panes);
            dropFirstPane(panes[0]);
            foreach (NestedDockEntry entry in enties)
                Drop(entry.SourcePane, entry.TargetPaneNode, entry.Dock, entry.SplitterDistance, entry.SwapChildren);
            dockControl.EndUndoUnit();
        }

        private static void Drop(DockPane dockPane, DockPaneNode targetPaneNode, Dock dock, SplitterDistance splitterDistance, bool swapChildren)
        {
            Drop(dockPane,
                delegate(DockItem dockItem)
                {
                    return CanDrop(dockItem, targetPaneNode);
                },
                delegate(DockItem dockItem)
                {
                    dockItem.Show(targetPaneNode, false, dock, splitterDistance, swapChildren, GetShowMethod(dockItem));
                });
        }

        private static DockPane[] GetPanes(FloatingWindow floatingWindow, Predicate<DockPane> canDrop)
        {
            int count = 0;
            DockTree dockTree = floatingWindow.DockTree;
            foreach (DockPane pane in dockTree.ActivePanes)
            {
                if (canDrop == null || canDrop(pane))
                    count++;
            }

            DockPane[] panes = new DockPane[count];
            int i = 0;
            foreach (DockPane pane in dockTree.ActivePanes)
            {
                if (canDrop == null || canDrop(pane))
                    panes[i++] = pane;
            }
            Debug.Assert(i == count);

            return panes;
        }

        internal static void ToggleFloating(DockItem dockItem)
        {
            dockItem.ToggleFloating();
        }

        internal static void ToggleFloating(DockPane pane)
        {
            DockControl dockControl = pane.DockControl;
            dockControl.BeginUndoUnit();

            bool isFloating = pane.IsFloating;

            DockItem[] activeItems = GetActiveItems(pane);
            if (isFloating)
            {
                foreach (DockItem item in activeItems)
                {
                    if (item.CanToggleFloating)
                        item.ToggleFloating(DockManager.GetShowMethod(item));
                }
            }
            else
            {
                DockItem targetItem = null;
                for (int i = activeItems.Length - 1; i >= 0; i--)
                {
                    DockItem item = activeItems[i];
                    if (item.CanToggleFloating && item.SecondPane != null)
                    {
                        targetItem = item;
                        break;
                    }
                }

                if (targetItem != null)
                {
                    DockPane targetPane = targetItem.SecondPane;
                    int targetIndex = targetPane.Items.IndexOf(targetItem);
                    Drop(pane,
                        delegate(DockItem dockItem) { return dockItem.CanToggleFloating; },
                        delegate(DockItem dockItem) { dockItem.Show(targetPane, targetIndex, GetShowMethod(dockItem)); });
                }
                else
                    Drop(pane,
                        delegate(DockItem dockItem) { return dockItem.CanToggleFloating; },
                        delegate(DockItem dockItem) { dockItem.ToggleFloating(GetShowMethod(dockItem)); });
            }

            dockControl.EndUndoUnit();
        }

        internal static void ToggleFloating(FloatingWindow floatingWindow)
        {
            DockControl dockControl = floatingWindow.DockControl;
            dockControl.BeginUndoUnit();

            DockPane[] panes = GetPanes(floatingWindow, null);
            foreach (DockPane pane in panes)
            {
                DockItem[] activeItems = GetActiveItems(pane);
                foreach (DockItem item in activeItems)
                {
                    if (item.CanToggleFloating)
                        item.ToggleFloating(GetShowMethod(item));
                }
            }

            dockControl.EndUndoUnit();
        }
    }
}
