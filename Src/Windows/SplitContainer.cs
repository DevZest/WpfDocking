using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Data;

namespace DevZest.Windows
{
    /// <summary>Represents a control consisting of two resizable <see cref="UIElement"/> objects.</summary>
    /// <remarks>
    /// <para>
    /// You can add two <see cref="UIElement"/> children to the two resizable areas, and you can add other <see cref="SplitContainer"/>
    /// controls to existing <see cref="SplitContainer"/> to create many resizable display areas.
    /// </para>
    /// <para>
    /// Use the <see cref="SplitContainer"/> control to divide the display area of a container
    /// (such as a <see cref="Window"/>) and allow the user to resize UI elements that are added to the <see cref="SplitContainer"/> panels.
    /// When the user passes the mouse pointer over the splitter, the cursor changes to indicate that the
    /// controls inside the SplitContainer control can be resized.
    /// </para>
    /// <para>
    /// Use <see cref="Child1"/> and <see cref="Child2"/> to specify two resizable children.
    /// Use <see cref="Orientation"/> to specify horizontal orientation.
    /// The default orientation of the <see cref="SplitContainer"/> is vertical.
    /// </para>
    /// <para>
    /// Use <see cref="SplitterDistance"/> and <see cref="IsSplitterTopLeft"/> to specify where the splitter starts.
    /// Double click the splitter auto sizes <see cref="Child1"/> or <see cref="Child2"/>, depending on the value of
    /// <see cref="IsSplitterTopLeft"/>.
    /// Use <see cref="ShowsPreview"/> to indicate whether <see cref="SplitterDistance"/> updated as the user drags the splitter.
    /// Use <see cref="DragIncrement"/> and <see cref="KeyboardIncrement"/> to specify how far the splitter moves at a time.
    /// The default for <see cref="DragIncrement"/> is 1 and <see cref="KeyboardIncrement"/> is 10.
    /// </para>
    /// <para>
    /// Use <see cref="Child1MinSize"/> and <see cref="Child2MinSize"/> to specify how close the splitter bar can be moved
    /// to the outside edge of a <see cref="SplitContainer"/>. The default value is 20.
    /// </para>
    /// <para>
    /// Use <see cref="SplitterWidth" />, <see cref="SplitterPresenterStyle" />, 
    /// <see cref="SplitterTemplate"/>, <see cref="PreviewTemplate"/>, <see cref="IsPreviewVisible"/>, <see cref="PreviewOffsetX"/> and <see cref="PreviewOffsetY"/>
    /// properties to customize the splitter and drag preview.
    /// </para>
    /// </remarks>
    /// <example>
    ///     The following example shows a sample use of <see cref="SplitContainer" />.
    ///     <code lang="xaml" source="..\..\Samples\Common\CSharp\SplitContainerSample\Window1.xaml" />
    /// </example>
    public partial class SplitContainer : FrameworkElement
    {
        /// <summary>Identifies the <see cref="Background"/> dependency property.</summary>
        public static readonly DependencyProperty BackgroundProperty;
        /// <summary>Identifies the <see cref="Child1"/> dependency property.</summary>
        public static readonly DependencyProperty Child1Property;
        /// <summary>Identifies the <see cref="Child2"/> dependency property.</summary>
        public static readonly DependencyProperty Child2Property;
        /// <summary>Identifies the <see cref="Orientation"/> dependency property.</summary>
        public static readonly DependencyProperty OrientationProperty;
        /// <summary>Identifies the <see cref="IsSplitterTopLeft"/> dependency property.</summary>
        public static readonly DependencyProperty IsSplitterTopLeftProperty;
        /// <summary>Identifies the <see cref="SplitterDistance"/> dependency property.</summary>
        public static readonly DependencyProperty SplitterDistanceProperty;
        /// <summary>Identifies the <see cref="SplitterWidth"/> dependency property.</summary>
        public static readonly DependencyProperty SplitterWidthProperty;
        /// <summary>Identifies the <see cref="Child1MinSize"/> dependency property.</summary>
        public static readonly DependencyProperty Child1MinSizeProperty;
        /// <summary>Identifies the <see cref="Child2MinSize"/> dependency property.</summary>
        public static readonly DependencyProperty Child2MinSizeProperty;
        /// <summary>Identifies the <see cref="DragIncrement"/> dependency property.</summary>
        public static readonly DependencyProperty DragIncrementProperty;
        /// <summary>Identifies the <see cref="KeyboardIncrement"/> dependency property.</summary>
        public static readonly DependencyProperty KeyboardIncrementProperty;
        /// <summary>Identifies the <see cref="ShowsPreview"/> dependency property.</summary>
        public static readonly DependencyProperty ShowsPreviewProperty;
        private static readonly DependencyPropertyKey IsPreviewVisiblePropertyKey;
        /// <summary>Identifies the <see cref="IsPreviewVisible"/> dependency property.</summary>
        public static readonly DependencyProperty IsPreviewVisibleProperty;
        private static readonly DependencyPropertyKey PreviewOffsetXPropertyKey;
        /// <summary>Identifies the <see cref="PreviewOffsetX"/> dependency property.</summary>
        public static readonly DependencyProperty PreviewOffsetXProperty;
        private static readonly DependencyPropertyKey PreviewOffsetYPropertyKey;
        /// <summary>Identifies the <see cref="PreviewOffsetY"/> dependency property.</summary>
        public static readonly DependencyProperty PreviewOffsetYProperty;
        /// <summary>Identifies the <see cref="SplitterTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty SplitterTemplateProperty;
        /// <summary>Identifies the <see cref="PreviewTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty PreviewTemplateProperty;
        private static readonly DependencyPropertyKey SplitterPresenterPropertyKey;
        /// <summary>Identifies the <see cref="SplitterPresenter"/> dependency property.</summary>
        public static readonly DependencyProperty SplitterPresenterProperty;
        /// <summary>Identifies the <see cref="SplitterPresenterStyle"/> dependency property.</summary>
        public static readonly DependencyProperty SplitterPresenterStyleProperty;

        static SplitContainer()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitContainer), new FrameworkPropertyMetadata(typeof(SplitContainer)));
            BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(SplitContainer),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender | FrameworkPropertyMetadataOptions.AffectsRender));
            Child1Property = DependencyProperty.Register("Child1", typeof(UIElement), typeof(SplitContainer),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnChildChanged)));
            Child2Property = DependencyProperty.Register("Child2", typeof(UIElement), typeof(SplitContainer),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnChildChanged)));
            OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(SplitContainer),
                new FrameworkPropertyMetadata(OrientationBoxes.Vertical,
                FrameworkPropertyMetadataOptions.AffectsMeasure),
                new ValidateValueCallback(ValidateOrientation));
            IsSplitterTopLeftProperty = DependencyProperty.Register("IsSplitterTopLeft", typeof(bool), typeof(SplitContainer),
                new FrameworkPropertyMetadata(BooleanBoxes.True, FrameworkPropertyMetadataOptions.AffectsMeasure));
            SplitterDistanceProperty = DependencyProperty.Register("SplitterDistance", typeof(SplitterDistance), typeof(SplitContainer),
                new FrameworkPropertyMetadata(new SplitterDistance(1.0, SplitterUnitType.Star),
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
            SplitterWidthProperty = DependencyProperty.Register("SplitterWidth", typeof(double), typeof(SplitContainer),
                new FrameworkPropertyMetadata(4d, FrameworkPropertyMetadataOptions.AffectsMeasure));
            Child1MinSizeProperty = DependencyProperty.Register("Child1MinSize", typeof(double), typeof(SplitContainer),
                new FrameworkPropertyMetadata(20d, FrameworkPropertyMetadataOptions.AffectsMeasure),
                new ValidateValueCallback(ValidateChildMinSize));
            Child2MinSizeProperty = DependencyProperty.Register("Child2MinSize", typeof(double), typeof(SplitContainer),
                new FrameworkPropertyMetadata(20d, FrameworkPropertyMetadataOptions.AffectsMeasure),
                new ValidateValueCallback(ValidateChildMinSize));
            KeyboardIncrementProperty = DependencyProperty.Register("KeyboardIncrement", typeof(double), typeof(SplitContainer),
                new FrameworkPropertyMetadata(10.0d), new ValidateValueCallback(ValidateIncrement));
            DragIncrementProperty = DependencyProperty.Register("DragIncrement", typeof(double), typeof(SplitContainer),
                new FrameworkPropertyMetadata(1.0d), new ValidateValueCallback(ValidateIncrement));
            ShowsPreviewProperty = DependencyProperty.Register("ShowsPreview", typeof(bool), typeof(SplitContainer),
                new FrameworkPropertyMetadata(BooleanBoxes.True));
            IsPreviewVisiblePropertyKey = DependencyProperty.RegisterReadOnly("IsPreviewVisible", typeof(bool), typeof(SplitContainer),
                new FrameworkPropertyMetadata(BooleanBoxes.False));
            IsPreviewVisibleProperty = IsPreviewVisiblePropertyKey.DependencyProperty;
            PreviewOffsetXPropertyKey = DependencyProperty.RegisterReadOnly("PreviewOffsetX", typeof(double), typeof(SplitContainer),
                new FrameworkPropertyMetadata(0d));
            PreviewOffsetXProperty = PreviewOffsetXPropertyKey.DependencyProperty;
            PreviewOffsetYPropertyKey = DependencyProperty.RegisterReadOnly("PreviewOffsetY", typeof(double), typeof(SplitContainer),
                new FrameworkPropertyMetadata(0d));
            PreviewOffsetYProperty = PreviewOffsetYPropertyKey.DependencyProperty;
            SplitterTemplateProperty = DependencyProperty.Register("SplitterTemplate", typeof(DataTemplate), typeof(SplitContainer),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSplitterTemplateChanged)));
            PreviewTemplateProperty = DependencyProperty.Register("PreviewTemplate", typeof(DataTemplate), typeof(SplitContainer),
                new FrameworkPropertyMetadata());
            SplitterPresenterPropertyKey = DependencyProperty.RegisterReadOnly("SplitterPresenter", typeof(ContentPresenter), typeof(SplitContainer),
                new FrameworkPropertyMetadata());
            SplitterPresenterProperty = SplitterPresenterPropertyKey.DependencyProperty;
            SplitterPresenterStyleProperty = DependencyProperty.Register("SplitterPresenterStyle", typeof(Style), typeof(SplitContainer),
                new FrameworkPropertyMetadata());
        }

        private static void OnChildChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SplitContainer)d).OnChildChanged((UIElement)e.OldValue, (UIElement)e.NewValue);
        }

        private void OnChildChanged(UIElement oldChild, UIElement newChild)
        {
            if (oldChild != null)
            {
                RemoveVisualChild(oldChild);
                RemoveLogicalChild(oldChild);
            }
            if (newChild != null)
            {
                AddLogicalChild(newChild);
                AddVisualChild(newChild);
            }
        }

        private static void OnSplitterTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SplitContainer)d).SetSplitterTemplate((DataTemplate)e.NewValue);
        }

        private static bool ValidateOrientation(object o)
        {
            Orientation orientation = (Orientation)o;
            return (orientation == Orientation.Horizontal || orientation == Orientation.Vertical);
        }

        private static bool ValidateChildMinSize(object o)
        {
            double size = (double)o;
            return !double.IsInfinity(size) && !double.IsNaN(size) && size >= 0;
        }

        private static bool ValidateIncrement(object o)
        {
            double d = (double)o;
            return ((d > 0.0) && !double.IsPositiveInfinity(d));
        }

        partial void VerifyLicense(bool designMode);

        partial void CheckLicense(ref UIElement licenseErrorElement);

        /// <summary>Initializes a new instance of the <see cref="SplitContainer"/> class.</summary>
        public SplitContainer()
        {
            SplitterPresenter = new ContentPresenter();
            AddLogicalChild(SplitterPresenter);
            AddVisualChild(SplitterPresenter);
            SetSplitterTemplate(SplitterTemplate);
            SplitterPresenter.SetBinding(StyleProperty, new Binding() { Source = this, Path = new PropertyPath(SplitterPresenterStyleProperty) });

            bool designMode = DesignerProperties.GetIsInDesignMode(this);
            VerifyLicense(designMode);
        }

        /// <summary>Gets or sets the <see cref="Brush"/> used to fill the background. This is a dependency property.</summary>
        /// <value>The brush used to fill the background, or <see langword="null"/> to not use a background brush. The default is <see langword="null"/>.</value>
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        /// <summary>Gets or sets the left or top child of the <see cref="SplitContainer"/>, depending on <see cref="Orientation"/>. This is a dependency property.</summary>
        /// <value>If <see cref="Orientation"/> is <see cref="System.Windows.Controls.Orientation.Vertical"/>, the top child of the <see cref="SplitContainer"/>.
        /// If <see cref="Orientation"/> is <see cref="System.Windows.Controls.Orientation.Horizontal"/>, the left child of the <see cref="SplitContainer"/>.</value>
        public UIElement Child1
        {
            get { return (UIElement)GetValue(Child1Property); }
            set { SetValue(Child1Property, value); }
        }

        /// <summary>Gets or sets the right or bottom child of the <see cref="SplitContainer"/>, depending on <see cref="Orientation"/>. This is a dependency property.</summary>
        /// <value>If <see cref="Orientation"/> is <see cref="System.Windows.Controls.Orientation.Vertical"/>, the bottom child of the <see cref="SplitContainer"/>.
        /// If <see cref="Orientation"/> is <see cref="System.Windows.Controls.Orientation.Horizontal"/>, the right child of the <see cref="SplitContainer"/>.</value>
        public UIElement Child2
        {
            get { return (UIElement)GetValue(Child2Property); }
            set { SetValue(Child2Property, value); }
        }

        /// <summary>Gets the <see cref="ContentPresenter"/> of the splitter. This is a dependency property.</summary>
        /// <value>The <see cref="ContentPresenter"/> of the splitter.</value>
        public ContentPresenter SplitterPresenter
        {
            get { return (ContentPresenter)GetValue(SplitterPresenterProperty); }
            private set { SetValue(SplitterPresenterPropertyKey, value); }
        }

        /// <summary>Gets or sets the splitter <see cref="ContentPresenter"/>. This is a dependency property.</summary>
        /// <value>The splitter <see cref="ContentPresenter"/>.</value>
        public Style SplitterPresenterStyle
        {
            get { return (Style)GetValue(SplitterPresenterStyleProperty); }
            set { SetValue(SplitterPresenterStyleProperty, value); }
        }

        private void SetSplitterTemplate(DataTemplate splitterTemplate)
        {
            SplitterPresenter.Content = splitterTemplate == null ? null : this;
            SplitterPresenter.ContentTemplate = splitterTemplate;
        }

        /// <summary>Gets or sets the data template of the splitter (movable bar). This is a dependency property.</summary>
        /// <remarks>Use <see cref="SplitterTemplate"/>, <see cref="IsPreviewVisible"/>, <see cref="PreviewOffsetX"/> and
        /// <see cref="PreviewOffsetY"/> properties to customize the splitter and drag preview. Refer to example of
        /// <see cref="SplitContainer"/> class.</remarks>
        /// <value>The <see cref="DataTemplate"/> of the splitter.</value>
        public DataTemplate SplitterTemplate
        {
            get { return (DataTemplate)GetValue(SplitterTemplateProperty); }
            set { SetValue(SplitterTemplateProperty, value); }
        }

        /// <summary>Gets or sets the <see cref="DataTemplate"/> of the dragging preview.</summary>
        /// <value>The <see cref="DataTemplate"/> of the dragging preview.</value>
        public DataTemplate PreviewTemplate
        {
            get { return (DataTemplate)GetValue(PreviewTemplateProperty); }
            set { SetValue(PreviewTemplateProperty, value); }
        }

        /// <summary>Gets the value indicates whether the preview is visible. This is a dependency property.</summary>
        /// <remarks>Use <see cref="SplitterTemplate"/>, <see cref="IsPreviewVisible"/>, <see cref="PreviewOffsetX"/> and
        /// <see cref="PreviewOffsetY"/> properties to customize the splitter and drag preview. Refer to example of
        /// <see cref="SplitContainer"/> class.</remarks>
        /// <value><see langword="true"/> if drag preview is visible, otherwise, <see langword="false"/>.</value>
        public bool IsPreviewVisible
        {
            get { return (bool)GetValue(IsPreviewVisibleProperty); }
            private set { SetValue(IsPreviewVisiblePropertyKey, BooleanBoxes.Box(value)); }
        }

        /// <summary>Gets the X-axis value of the drag preview offset. This is a dependency property.</summary>
        /// <remarks>Use <see cref="SplitterTemplate"/>, <see cref="IsPreviewVisible"/>, <see cref="PreviewOffsetX"/> and
        /// <see cref="PreviewOffsetY"/> properties to customize the splitter and drag preview. Refer to example of
        /// <see cref="SplitContainer"/> class.</remarks>
        /// <value>The X-axis value of the drag preview offset.</value>
        public double PreviewOffsetX
        {
            get { return (double)GetValue(PreviewOffsetXProperty); }
            private set { SetValue(PreviewOffsetXPropertyKey, value); }
        }

        /// <summary>Gets the Y-axis value of the drag preview offset. This is a dependency property.</summary>
        /// <remarks>Use <see cref="SplitterTemplate"/>, <see cref="IsPreviewVisible"/>, <see cref="PreviewOffsetX"/> and
        /// <see cref="PreviewOffsetY"/> properties to customize the splitter and drag preview. Refer to example of
        /// <see cref="SplitContainer"/> class.</remarks>
        /// <value>The Y-axis value of the drag preview offset.</value>
        public double PreviewOffsetY
        {
            get { return (double)GetValue(PreviewOffsetYProperty); }
            private set { SetValue(PreviewOffsetYPropertyKey, value); }
        }

        /// <summary>Gets or sets a value indicating the horizontal or vertical orientation of the <see cref="SplitContainer"/> children. This is a dependency property.</summary>
        /// <remarks>Use the <see cref="Orientation"/> property to change the <see cref="SplitContainer"/> from vertical to horizontal or from horizontal to vertical.</remarks>
        /// <value>One of the <see cref="Orientation"/> values. The default is <see cref="System.Windows.Controls.Orientation.Vertical"/>.</value>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, OrientationBoxes.Box(value)); }
        }

        /// <summary>Gets or sets a value indicating whether the <see cref="SplitterDistance"/> property specifies the size of <see cref="Child1"/> or <see cref="Child2"/>. This is a dependency property.</summary>
        /// <value><see langword="true"/> if <see cref="SplitterDistance"/> specifies the size of <see cref="Child1"/>; <see langword="false"/> if <see cref="SplitterDistance"/> specifies the size of <see cref="Child2"/>. The default value is <see langword="true"/>.</value>
        public bool IsSplitterTopLeft
        {
            get { return (bool)GetValue(IsSplitterTopLeftProperty); }
            set { SetValue(IsSplitterTopLeftProperty, BooleanBoxes.Box(value)); }
        }

        /// <summary>Gets or sets a value indicating the size of <see cref="Child1"/> or <see cref="Child2"/>, depending on the value of <see cref="IsSplitterTopLeft"/>. This is a dependency property.</summary>
        /// <value>
        /// If <see cref="IsSplitterTopLeft"/> is <see langword="true"/>, size of <see cref="Child1"/>;
        /// otherwise, size of <see cref="Child2"/>.
        /// The default value is "*" (50% of the total available size).
        /// </value>
        public SplitterDistance SplitterDistance
        {
            get { return (SplitterDistance)GetValue(SplitterDistanceProperty); }
            set { SetValue(SplitterDistanceProperty, value); }
        }

        /// <summary>Gets or sets the width of the splitter, in device-independent units (1/96th inch per unit). This is a dependency property.</summary>
        /// <remarks>Use the <see cref="SplitterWidth"/> property to change the width of the splitter itself, not the <see cref="SplitContainer"/>.</remarks>
        /// <value>Representing the width of the splitter,  in device-independent units (1/96th inch per unit). The default is 4.</value>
        public double SplitterWidth
        {
            get { return (double)GetValue(SplitterWidthProperty); }
            set { SetValue(SplitterWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the minimum distance, in device-independent units (1/96th inch per unit),
        /// of the splitter from the left or top edge of <see cref="SplitContainer"/>.
        /// This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Use the <see cref="Child1MinSize"/> property to prevent the splitter from moving too close to
        /// the left or top edge of the container. For example, you might want to prevent some of the
        /// display area of a <see cref="TreeView"/> from being covered.
        /// </para>
        /// <para>If the <see cref="Orientation"/> property is <see cref="System.Windows.Controls.Orientation.Vertical"/> (the
        /// default), <see cref="Child1MinSize"/> returns the minimum distance of the splitter from the top
        /// edge of <see cref="SplitContainer"/>. If the <see cref="Orientation"/> property is
        /// <see cref="System.Windows.Controls.Orientation.Horizontal"/>, <see cref="Child1MinSize"/> returns the minimum
        /// distance of the splitter from the left edge of <see cref="SplitContainer"/>.
        /// </para>
        /// </remarks>
        /// <value>
        /// Representing the minimum distance, in device-independent units (1/96th inch per unit),
        /// of the splitter from the left or top edge of <see cref="SplitContainer"/>.
        /// The default value is 20, regardless of <see cref="Orientation"/>.
        /// </value>
        public double Child1MinSize
        {
            get { return (double)GetValue(Child1MinSizeProperty); }
            set { SetValue(Child1MinSizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the minimum distance, in device-independent units (1/96th inch per unit),
        /// of the splitter from the right or bottom edge of <see cref="SplitContainer"/>.
        /// This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Use the <see cref="Child2MinSize"/> property to prevent the splitter from moving too close to
        /// the right or bottom edge of the container. For example, you might want to prevent some of the
        /// display area of a <see cref="TreeView"/> from being covered.
        /// </para>
        /// <para>If the <see cref="Orientation"/> property is <see cref="System.Windows.Controls.Orientation.Vertical"/> (the
        /// default), <see cref="Child2MinSize"/> returns the minimum distance of the splitter from the
        /// bottom edge of <see cref="SplitContainer"/>. If the <see cref="Orientation"/> property is
        /// <see cref="System.Windows.Controls.Orientation.Horizontal"/>, <see cref="Child2MinSize"/> returns the minimum
        /// distance of the splitter from the right edge of <see cref="SplitContainer"/>.
        /// </para>
        /// </remarks>
        /// <value>
        /// Representing the minimum distance, in device-independent units (1/96th inch per unit),
        /// of the splitter from the right or bottom edge of <see cref="SplitContainer"/>.
        /// The default value is 20, regardless of <see cref="Orientation"/>.
        /// </value>
        public double Child2MinSize
        {
            get { return (double)GetValue(Child2MinSizeProperty); }
            set { SetValue(Child2MinSizeProperty, value); }
        }

        /// <summary>Gets or sets the distance that each press of an arrow key moves the splitter. This is a dependency property.</summary>
        /// <value>The distance that the splitter moves for each press of an arrow key. The default is 10.</value>
        public double KeyboardIncrement
        {
            get { return (double)GetValue(KeyboardIncrementProperty); }
            set { SetValue(KeyboardIncrementProperty, value); }
        }

        /// <summary>Gets or sets the minimum distance that a user must use the mouse to drag the splitter. This is a dependency property.</summary>
        /// <value>A value that represents the minimum distance that a user must use the mouse to drag the splitter. The default is 1.</value>
        public double DragIncrement
        {
            get { return (double)GetValue(DragIncrementProperty); }
            set { SetValue(KeyboardIncrementProperty, value); }
        }

        /// <summary>Gets or sets a value that indicates whether the <see cref="SplitContainer"/> updates <see cref="SplitterDistance"/> as the user drags the splitter. This is a dependency property.</summary>
        /// <value><see langword="true"/> if a splitter preview is displayed; otherwise, <see langword="false"/>. The default is <see langword="true"/>.</value>
        public bool ShowsPreview
        {
            get { return (bool)GetValue(ShowsPreviewProperty); }
            set { SetValue(ShowsPreviewProperty, BooleanBoxes.Box(value)); }
        }

        private bool IsChild1Collapsed
        {
            get { return Child1 == null || Child1.Visibility == Visibility.Collapsed; }
        }

        private bool IsChild2Collapsed
        {
            get { return Child2 == null || Child2.Visibility == Visibility.Collapsed; }
        }

        /// <exclude/>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                if (Child1 != null)
                    yield return Child1;
                if (SplitterPresenter != null)
                    yield return SplitterPresenter;
                if (Child2 != null)
                    yield return Child2;
            }
        }

        /// <exclude/>
        protected override int VisualChildrenCount
        {
            get
            {
                UIElement licenseErrorElement = null;
                CheckLicense(ref licenseErrorElement);
                if (licenseErrorElement != null)
                    return 1;
                else
                    return 3;
            }
        }

        /// <exclude/>
        protected override Visual GetVisualChild(int index)
        {
            UIElement licenseErrorElement = null;
            CheckLicense(ref licenseErrorElement);
            if (licenseErrorElement != null)
                return licenseErrorElement;

            if (index < 0 || index > 2)
                throw new ArgumentOutOfRangeException("index");

            if (index == 0)
                return Child1;
            else if (index == 1)
                return SplitterPresenter;
            else
                return Child2;
        }

        /// <exclude/>
        protected override void OnRender(DrawingContext drawingContext)
        {
            Brush background = Background;
            if (background != null)
                drawingContext.DrawRectangle(background, null, new Rect(0.0, 0.0, RenderSize.Width, RenderSize.Height));
        }

        private Size _child1AutoSize, _child2AutoSize;
        private Rect _child1Rect, _child2Rect;
        private Size Child1DesiredSize
        {
            get
            {
                if (Child1 == null)
                    return new Size(Child1MinSize, Child1MinSize);
                else
                    return _child1AutoSize;
            }
        }

        private Size Child2DesiredSize
        {
            get
            {
                if (Child2 == null)
                    return new Size(Child2MinSize, Child2MinSize);
                else
                    return _child2AutoSize;
            }
        }

        /// <exclude/>
        protected override Size MeasureOverride(Size availableSize)
        {
            UIElement licenseErrorElement = null;
            CheckLicense(ref licenseErrorElement);
            if (licenseErrorElement != null)
            {
                licenseErrorElement.Measure(availableSize);
                return licenseErrorElement.DesiredSize;
            }

            if (Child1 != null)
            {
                Child1.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                _child1AutoSize = Child1.DesiredSize;
            }
            if (Child2 != null)
            {
                Child2.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                _child2AutoSize = Child2.DesiredSize;
            }

            Rect child1Rect, splitterRect, child2Rect;
            Size desiredSize = GetRects(availableSize, out child1Rect, out splitterRect, out child2Rect);
            if (Child1 != null)
                Child1.Measure(child1Rect.Size);
            if (SplitterPresenter != null)
                SplitterPresenter.Measure(splitterRect.Size);
            if (Child2 != null)
                Child2.Measure(child2Rect.Size);

            return desiredSize;
        }

        private Size GetRects(Size availableSize, out Rect child1Rect, out Rect splitterRect, out Rect child2Rect)
        {
            Orientation orientation = Orientation;

            double length1, splitterLength, length2;
            Size finalSize = CalculateLengthes(availableSize, out length1, out splitterLength, out length2);

            double x, y, width, height;
            x = y = 0;
            width = orientation == Orientation.Vertical ? finalSize.Width : length1;
            height = orientation == Orientation.Horizontal ? finalSize.Height : length1;

            child1Rect = new Rect(x, y, width, height);

            if (orientation == Orientation.Vertical)
            {
                y += height;
                height = splitterLength;
            }
            else
            {
                x += width;
                width = splitterLength;
            }
            splitterRect = new Rect(x, y, width, height);

            if (orientation == Orientation.Vertical)
            {
                y += height;
                height = length2;
            }
            else
            {
                x += width;
                width = length2;
            }
            child2Rect = new Rect(x, y, width, height);

            if (double.IsNaN(child1Rect.Width))
                child1Rect.Width = _child1AutoSize.Width;
            if (double.IsNaN(child1Rect.Height))
                child1Rect.Height = _child1AutoSize.Height;
            if (double.IsNaN(child2Rect.Width))
                child2Rect.Width = _child2AutoSize.Width;
            if (double.IsNaN(child2Rect.Height))
                child2Rect.Height = _child2AutoSize.Height;

            if (orientation == Orientation.Vertical)
                return new Size(finalSize.Width, child1Rect.Height + splitterLength + child2Rect.Height);
            else
                return new Size(child1Rect.Width + splitterLength + child2Rect.Width, finalSize.Height);
        }

        private Size CalculateLengthes(Size availableSize, out double child1Length, out double splitterLength, out double child2Length)
        {
            Orientation orientation = Orientation;
            double originalLength = orientation == Orientation.Vertical ? availableSize.Height : availableSize.Width;
            SplitterDistance splitterDistance = SplitterDistance;
            splitterLength = IsChild1Collapsed && IsChild2Collapsed ? 0 : SplitterWidth;

            double length1, length2;
            double minLength1 = IsSplitterTopLeft ? Child1MinSize : Child2MinSize;
            double minLength2 = IsSplitterTopLeft ? Child2MinSize : Child1MinSize;
            Size desiredSize1 = IsSplitterTopLeft ? Child1DesiredSize : Child2DesiredSize;
            Size desiredSize2 = IsSplitterTopLeft ? Child2DesiredSize : Child1DesiredSize;
            if (originalLength == double.PositiveInfinity)
            {
                if (splitterDistance.IsAbsolute)
                    length1 = splitterDistance.Value;
                else
                    length1 = orientation == Orientation.Vertical ? desiredSize1.Height : desiredSize1.Width;
                if (length1 < minLength1)
                    length1 = minLength1;

                length2 = orientation == Orientation.Vertical ? desiredSize2.Height : desiredSize2.Width;
            }
            else
            {
                double availableLength = originalLength - splitterLength;
                if (splitterDistance.IsAuto)
                    length1 = orientation == Orientation.Vertical ? desiredSize1.Height : desiredSize1.Width;
                else if (splitterDistance.IsStar)
                    length1 = availableLength * splitterDistance.Value / (splitterDistance.Value + 1);
                else
                    length1 = splitterDistance.Value;
                if (length1 > availableLength - minLength2)
                    length1 = availableLength - minLength2;
                if (length1 < minLength1)
                    length1 = minLength1;
                length2 = availableLength - length1;
            }

            if (length2 < minLength2)
                length2 = minLength2;

            child1Length = IsSplitterTopLeft ? length1 : length2;
            child2Length = IsSplitterTopLeft ? length2 : length1;

            double height, width;
            if (orientation == Orientation.Vertical)
            {
                height = length1 + length2 + splitterLength;
                if (double.IsPositiveInfinity(availableSize.Width))
                    width = Math.Max(Child1DesiredSize.Width, Child2DesiredSize.Width);
                else
                    width = availableSize.Width;
            }
            else
            {
                width = length1 + length2 + splitterLength;
                if (double.IsPositiveInfinity(availableSize.Height))
                    height = Math.Max(Child1DesiredSize.Height, Child2DesiredSize.Height);
                else
                    height = availableSize.Height;
            }
            return new Size(width, height);
        }

        /// <exclude/>
        protected override Size ArrangeOverride(Size finalSize)
        {
            UIElement licenseErrorElement = null;
            CheckLicense(ref licenseErrorElement);
            if (licenseErrorElement != null)
            {
                licenseErrorElement.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
                return finalSize;
            }

            Rect splitterRect;

            GetRects(finalSize, out _child1Rect, out splitterRect, out _child2Rect);

            if (Child1 != null)
                Child1.Arrange(_child1Rect);

            if (SplitterPresenter != null)
                SplitterPresenter.Arrange(splitterRect);

            if (Child2 != null)
                Child2.Arrange(_child2Rect);

            return finalSize;
        }

        /// <exclude/>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.Handled || e.Source != SplitterPresenter)
                return;

            if (e.ClickCount == 1)
                DragHandler.Default.BeginDrag(this, e);
            else if (e.ClickCount == 2)
            {
                if (SplitterDistance.IsAbsolute)
                    MoveSplitter(SplitterDistance.AutoPixel);
                else if (SplitterDistance.IsStar)
                    MoveSplitter(SplitterDistance.AutoStar);
            }

            e.Handled = true;
        }

        /// <exclude/>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Handled || e.Source != SplitterPresenter)
                return;

            switch (e.Key)
            {
                case Key.Left:
                    e.Handled = KeyboardMoveSplitter(-KeyboardIncrement, 0.0);
                    return;

                case Key.Up:
                    e.Handled = KeyboardMoveSplitter(0.0, -KeyboardIncrement);
                    return;

                case Key.Right:
                    e.Handled = KeyboardMoveSplitter(KeyboardIncrement, 0.0);
                    return;

                case Key.Down:
                    e.Handled = KeyboardMoveSplitter(0.0, KeyboardIncrement);
                    break;

                case Key.Return:
                    if (SplitterDistance.IsAbsolute)
                    {
                        SplitterDistance = SplitterDistance.AutoPixel;
                        e.Handled = true;
                    }
                    else if (SplitterDistance.IsStar)
                    {
                        SplitterDistance = SplitterDistance.AutoStar;
                        e.Handled = true;
                    }
                    break;
            }
        }

        private bool KeyboardMoveSplitter(double deltaX, double deltaY)
        {
            Point offset = GetOffset(deltaX, deltaY, 1);
            if (deltaX == 0 && deltaY == 0)
                return false;
            else
            {
                MoveSplitter(offset.X, offset.Y);
                return true;
            }
        }

        private void MoveSplitter(double offsetX, double offsetY)
        {
            double delta = Orientation == Orientation.Vertical ? offsetY : offsetX;
            double child1Length, child2Length, minDelta, maxDelta;
            GetChildrenLengthes(out child1Length, out child2Length);
            GetMinMaxDelta(out minDelta, out maxDelta);
            delta = Math.Min(Math.Max(delta, minDelta), maxDelta);

            if (!IsSplitterTopLeft)
                delta = -delta;

            double newValue;
            double length1 = IsSplitterTopLeft ? child1Length : child2Length;
            double length2 = IsSplitterTopLeft ? child2Length : child1Length;
            if (SplitterDistance.IsStar || SplitterDistance.IsAutoStar)
                newValue = (length1 + delta) / (length2 - delta);
            else
                newValue = length1 + delta;

            SplitterUnitType unitType = SplitterDistance.UnitType;

            MoveSplitter(new SplitterDistance(newValue, unitType));
        }

        private void MoveSplitter(SplitterDistance value)
        {
            SplitterDistance = value;
            UpdateLayout();
        }

        private void GetChildrenLengthes(out double child1Length, out double child2Length)
        {
            child1Length = Orientation == Orientation.Vertical ? _child1Rect.Height : _child1Rect.Width;
            child2Length = Orientation == Orientation.Vertical ? _child2Rect.Height : _child2Rect.Width;
        }

        private void GetMinMaxDelta(out double minDelta, out double maxDelta)
        {
            UpdateLayout();
            double child1Length, child2Length;
            GetChildrenLengthes(out child1Length, out child2Length);
            minDelta = Math.Min(Child1MinSize - child1Length, 0);
            maxDelta = Math.Max(child2Length - Child2MinSize, 0);
        }

        private Point GetOffset(double horizontalChange, double verticalChange, double increment)
        {
            double minChange, maxChange;
            GetMinMaxDelta(out minChange, out maxChange);

            double offsetX, offsetY;
            offsetX = offsetY = 0;
            if (Orientation == Orientation.Horizontal)
            {
                horizontalChange = Math.Round((double)(horizontalChange / increment)) * increment;
                offsetX = Math.Min(Math.Max(horizontalChange, minChange), maxChange);
            }
            else
            {
                verticalChange = Math.Round((double)(verticalChange / increment)) * increment;
                offsetY = Math.Min(Math.Max(verticalChange, minChange), maxChange);
            }

            return new Point(offsetX, offsetY);
        }
    }
}
