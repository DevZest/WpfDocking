using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Represents the area to display auto-hide <see cref="DockItem"/>.</summary>
    /// <remarks>Use <see cref="AutoHideClient"/> in control template of <see cref="DockControl"/>. Bind the <see cref="DockControl"/>
    /// property to the <see cref="Docking.DockControl.SelectedAutoHideItem"/> property of <see cref="Docking.DockControl"/>. Setting
    /// <b>IsMouseOverTab</b> attached property on <see cref="DockItem"/> to <see langword="true"/> keeps this <see cref="DockItem"/> visible.</remarks>
    [TemplatePart(Name = "PART_AutoHideWindow", Type = typeof(AutoHideWindow))]
    public class AutoHideClient : Control
    {
        /// <summary>Identifies the <see cref="DockControl"/> dependency property.</summary>
        public static readonly DependencyProperty DockControlProperty;
        private static readonly DependencyProperty _SelectedItemProperty;
        private static readonly DependencyPropertyKey SelectedItemPropertyKey;
        /// <summary>Identifies the <see cref="SelectedItem"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedItemProperty;
        /// <summary>Identifies the <b>IsMouseOverTab</b> attached property.</summary>
        private static readonly DependencyProperty IsMouseOverTabProperty;

        private AutoHideWindow _autoHideWindow;
        private DispatcherTimer _timer;
        private DockItem _closingItem;
        private AnimationClock _animationClock;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static AutoHideClient()
        {
            FocusableProperty.OverrideMetadata(typeof(AutoHideClient), new FrameworkPropertyMetadata(BooleanBoxes.False));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoHideClient), new FrameworkPropertyMetadata(typeof(AutoHideClient)));
            DockControlProperty = DependencyProperty.Register("DockControl", typeof(DockControl), typeof(AutoHideClient),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDockControlChanged)));
            _SelectedItemProperty = DependencyProperty.Register("_SelectedItem", typeof(DockItem), typeof(AutoHideClient),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSelectedItemChanged)));
            SelectedItemPropertyKey = DependencyProperty.RegisterReadOnly("SelectedItem", typeof(DockItem), typeof(AutoHideClient),
                new FrameworkPropertyMetadata(null));
            SelectedItemProperty = SelectedItemPropertyKey.DependencyProperty;
            IsMouseOverTabProperty = DependencyProperty.RegisterAttached("IsMouseOverTab", typeof(bool), typeof(AutoHideClient),
                new FrameworkPropertyMetadata(BooleanBoxes.False));
        }

        private static void OnDockControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AutoHideClient)d).OnDockControlChanged((DockControl)e.NewValue);
        }

        private void OnDockControlChanged(DockControl newValue)
        {
            Binding binding = new Binding();
            binding.Source = newValue;
            binding.Path = new PropertyPath(DockControl.SelectedAutoHideItemProperty);
            SetBinding(_SelectedItemProperty, binding);
        }

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AutoHideClient)d).OnSelectedItemChanged((DockItem)e.OldValue, (DockItem)e.NewValue);
        }

        private void OnSelectedItemChanged(DockItem oldValue, DockItem newValue)
        {
            SelectedItem = newValue;
            if (_closingItem != null)
                _closingItem = null;
            
            if (oldValue != null)
                oldValue.StateChanged -= new EventHandler<DockItemStateEventArgs>(OnSelectedItemStateChanged);

            if (newValue != null)
            {
                if (_timer == null)
                {
                    _timer = new DispatcherTimer();
                    _timer.Interval = TimerInterval;
                    _timer.Tick += new EventHandler(OnTimerTick);
                }
                _timer.Start();
                if (DockPositionHelper.IsAutoHide(newValue.DockPosition))
                    newValue.StateChanged += new EventHandler<DockItemStateEventArgs>(OnSelectedItemStateChanged);
            }
            else
            {
                if (_timer != null)
                    _timer.Stop();
            }
        }

        /// <summary>Gets a value indicates whether the mouse is over the auto-hide tab of specified <see cref="DockItem"/>.
        /// Getter of <b>IsMouseOverTab</b> attached property.</summary>
        /// <param name="element">The specified <see cref="DockItem"/>.</param>
        /// <returns><see langword="true"/> if the mouse is over the auto-hide tab of specified <see cref="DockItem"/>, otherwise <see langword="false"/>.</returns>
        internal static bool GetIsMouseOverTab(DockItem element)
        {
            return (bool)element.GetValue(IsMouseOverTabProperty);
        }

        /// <summary>Sets a value indicates whether the mouse is over the auto-hide tab of specified <see cref="DockItem"/>.
        /// Setter of <b>IsMouseOverTab</b> attached property.</summary>
        /// <param name="element">The specified <see cref="DockItem"/>.</param>
        /// <param name="value"><see langword="true"/> if the mouse is over the auto-hide tab of specified <see cref="DockItem"/>, otherwise <see langword="false"/>.</param>
        internal static void SetIsMouseOverTab(DockItem element, bool value)
        {
            element.SetValue(IsMouseOverTabProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Gets or sets the <see cref="Docking.DockControl"/> associated with this <see cref="AutoHideClient"/>. This is a dependency property.</summary>
        /// <value>The <see cref="Docking.DockControl"/> associated with this <see cref="AutoHideClient"/>.</value>
        public DockControl DockControl
        {
            get { return (DockControl)GetValue(DockControlProperty); }
            set { SetValue(DockControlProperty, value); }
        }

        /// <summary>Gets the selected auto-hide <see cref="DockItem"/>. This is a dependency property.</summary>
        /// <value>The selected auto-hide <see cref="DockItem"/>.</value>
        public DockItem SelectedItem
        {
            get { return (DockItem)GetValue(SelectedItemProperty); }
            private set { SetValue(SelectedItemPropertyKey, value); }
        }

        /// <exclude/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _autoHideWindow = (AutoHideWindow)Template.FindName("PART_AutoHideWindow", this);
        }

        private AutoHideAnimation Animation
        {
            get { return AutoHide.GetAnimation(DockControl); }
        }

        private Duration AnimationDuration
        {
            get { return AutoHide.GetAnimationDuration(DockControl); }
        }

        private TimeSpan TimerInterval
        {
            get
            {
                TimeSpan span = AutoHide.GetAnimationDuration(DockControl).TimeSpan;
                return span.Add(span);
            }
        }

        private UIElement VisualChild
        {
            get { return VisualChildrenCount == 0 ? null : GetVisualChild(0) as UIElement; }
        }

        private AnimationClock AnimationClock
        {
            get { return _animationClock; }
            set
            {
                if (_animationClock != null)
                {
                    _animationClock.Controller.Remove();
                    _animationClock.Completed -= new EventHandler(OnAnimationCompleted);
                    if (_closingItem != null)
                        _closingItem.Show(DockItemShowMethod.Deselect);
                }
                _animationClock = value;
                if (_animationClock != null)
                    _animationClock.Completed += new EventHandler(OnAnimationCompleted);
            }
        }

        private AutoHideWindow AutoHideWindow
        {
            get { return _autoHideWindow; }
        }

        private void SetupShowAnimation()
        {
            switch (Animation)
            {
                case AutoHideAnimation.Slide:
                    Dispatcher.BeginInvoke(DispatcherPriority.Send, new ThreadStart(SetupSlideShowAnimation));
                    break;
                case AutoHideAnimation.Fade:
                    Dispatcher.BeginInvoke(DispatcherPriority.Send, new ThreadStart(SetupFadeShowAnimation));
                    break;
            }
        }

        private void SetupSlideShowAnimation()
        {
            if (VisualChild == null || AutoHideWindow == null)
                return;
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            DockPosition dockPosition = SelectedItem.DockPosition;
            switch (dockPosition)
            {
                case DockPosition.LeftAutoHide:
                    doubleAnimation.From = -AutoHideWindow.ActualWidth;
                    break;
                case DockPosition.RightAutoHide:
                    doubleAnimation.From = AutoHideWindow.ActualWidth;
                    break;
                case DockPosition.TopAutoHide:
                    doubleAnimation.From = -AutoHideWindow.ActualHeight;
                    break;
                default:
                    Debug.Assert(SelectedItem.DockPosition == DockPosition.BottomAutoHide);
                    doubleAnimation.From = AutoHideWindow.ActualHeight;
                    break;
            }
            doubleAnimation.To = 0;
            doubleAnimation.Duration = AnimationDuration;
            TranslateTransform transform = VisualChild.RenderTransform as TranslateTransform;
            if (transform == null)
                VisualChild.RenderTransform = transform = new TranslateTransform();
            AnimationClock = doubleAnimation.CreateClock();
            if (dockPosition == DockPosition.LeftAutoHide || dockPosition == DockPosition.RightAutoHide)
                transform.ApplyAnimationClock(TranslateTransform.XProperty, AnimationClock);
            else
                transform.ApplyAnimationClock(TranslateTransform.YProperty, AnimationClock);
        }

        private void SetupFadeShowAnimation()
        {
            if (VisualChild == null)
                return;

            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = 0;
            doubleAnimation.To = 1;
            doubleAnimation.Duration = AnimationDuration;
            AnimationClock = doubleAnimation.CreateClock();
            VisualChild.ApplyAnimationClock(UIElement.OpacityProperty, AnimationClock);
        }

        private void SetupCloseAnimation()
        {
            switch (Animation)
            {
                case AutoHideAnimation.Slide:
                    Dispatcher.BeginInvoke(DispatcherPriority.Send, new ThreadStart(SetupSlideCloseAnimation));
                    break;
                case AutoHideAnimation.Fade:
                    Dispatcher.BeginInvoke(DispatcherPriority.Send, new ThreadStart(SetupFadeCloseAnimation));
                    break;
            }
        }

        private void SetupSlideCloseAnimation()
        {
            if (VisualChild == null || AutoHideWindow == null)
                return;
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = 0;
            DockPosition dockPosition = SelectedItem.DockPosition;
            switch (dockPosition)
            {
                case DockPosition.LeftAutoHide:
                    doubleAnimation.To = -AutoHideWindow.ActualWidth;
                    break;
                case DockPosition.RightAutoHide:
                    doubleAnimation.To = AutoHideWindow.ActualWidth;
                    break;
                case DockPosition.TopAutoHide:
                    doubleAnimation.To = -AutoHideWindow.ActualHeight;
                    break;
                default:
                    Debug.Assert(SelectedItem.DockPosition == DockPosition.BottomAutoHide);
                    doubleAnimation.To = AutoHideWindow.ActualHeight;
                    break;
            }
            doubleAnimation.Duration = AnimationDuration;
            TranslateTransform transform = VisualChild.RenderTransform as TranslateTransform;
            if (transform == null)
                VisualChild.RenderTransform = transform = new TranslateTransform();
            AnimationClock = doubleAnimation.CreateClock();
            if (dockPosition == DockPosition.LeftAutoHide || dockPosition == DockPosition.RightAutoHide)
                transform.ApplyAnimationClock(TranslateTransform.XProperty, AnimationClock);
            else
                transform.ApplyAnimationClock(TranslateTransform.YProperty, AnimationClock);
        }

        private void SetupFadeCloseAnimation()
        {
            var visualChild = VisualChild;
            if (visualChild == null)
                return;

            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = 1;
            doubleAnimation.To = 0;
            doubleAnimation.Duration = AnimationDuration;
            AnimationClock = doubleAnimation.CreateClock();
            visualChild.ApplyAnimationClock(UIElement.OpacityProperty, AnimationClock);
        }

        private void OnAnimationCompleted(object sender, EventArgs e)
        {
            AnimationClock = null;
        }

        private void OnSelectedItemStateChanged(object sender, DockItemStateEventArgs e)
        {
            DockItem selectedItem = SelectedItem;
            Debug.Assert(DockPositionHelper.IsAutoHide(selectedItem.DockPosition));
            if (e.StateChangeMethod == DockItemStateChangeMethod.ToggleAutoHide && e.ShowMethod == DockItemShowMethod.Select)
                BeginClose();
            else
                SetupShowAnimation();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            DockItem selectedItem = SelectedItem;
            if (selectedItem == null)
                _timer.Stop();
            else if (!selectedItem.IsActiveItem)
            {
                if (_closingItem != null)
                    _closingItem.Show(DockItemShowMethod.Deselect);
                else if (!IsMouseOver && !GetIsMouseOverTab(selectedItem))
                    BeginClose();
            }
        }

        private void BeginClose()
        {
            _closingItem = SelectedItem;
            SetupCloseAnimation();
        }
    }
}
