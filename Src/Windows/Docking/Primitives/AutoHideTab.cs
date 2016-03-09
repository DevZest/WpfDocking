using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Represents the auto-hide tab.</summary>
    /// <remarks><see cref="AutoHideTab"/> is the item container of <see cref="AutoHideStrip"/>. Its <see cref="ContentControl.Content" />
    /// property is set to a instance of <see cref="Docking.DockItem"/> object.</remarks>
    public class AutoHideTab : ContentControl
    {
        private DispatcherTimer _mouseHoverTimer;

        static AutoHideTab()
        {
            FocusableProperty.OverrideMetadata(typeof(AutoHideTab), new FrameworkPropertyMetadata(BooleanBoxes.False));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoHideTab), new FrameworkPropertyMetadata(typeof(AutoHideTab)));
        }

        private DockItem DockItem
        {
            get { return Content as DockItem; }
        }

        private bool IsMouseOverTab
        {
            get { return AutoHideClient.GetIsMouseOverTab(DockItem); }
            set { AutoHideClient.SetIsMouseOverTab(DockItem, value); }
        }

        private DispatcherTimer MouseHoverTimer
        {
            get { return _mouseHoverTimer; }
            set
            {
                if (_mouseHoverTimer != null)
                    _mouseHoverTimer.Stop();

                _mouseHoverTimer = value;
                if (value != null)
                    value.Start();
            }
        }

        /// <exclude />
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (DockItem.IsSelected)
                IsMouseOverTab = true;
            else
            {
                DispatcherTimer mouseHoverTimer = new DispatcherTimer();
                mouseHoverTimer.Interval = SystemParameters.MouseHoverTime;
                mouseHoverTimer.Tick += new EventHandler(OnMouseHoverTimerTick);
                MouseHoverTimer = mouseHoverTimer;
            }
        }

        /// <exclude />
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            IsMouseOverTab = false;
            if (MouseHoverTimer != null)
                MouseHoverTimer = null;
        }

        /// <exclude />
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (MouseHoverTimer == null)
                Select();
        }

        /// <exclude />
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            MouseHoverTimer = null;
            DockItem.Activate();
        }

        private void OnMouseHoverTimerTick(object sender, EventArgs e)
        {
            MouseHoverTimer = null;
            Select();
        }

        private void Select()
        {
            DockItem.Show(DockItemShowMethod.Select);
            IsMouseOverTab = true;
        }
    }
}
