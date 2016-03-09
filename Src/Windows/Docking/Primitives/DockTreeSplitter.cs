using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Represents the splitter of <see cref="Primitives.DockTreeSplit"/>.</summary>
    /// <remarks>This class handles the SHIFT + double click to send the respective <see cref="DockTree"/> to the back of Z-order (
    /// take the full edge). Use this class in the data template of <see cref="Primitives.DockTreeSplit"/> class.</remarks>
    public class DockTreeSplitter : Control
    {
        private static readonly DependencyPropertyKey IsShiftKeyDownPropertyKey;
        private static readonly DependencyProperty IsShiftKeyDownProperty;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static DockTreeSplitter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockTreeSplitter), new FrameworkPropertyMetadata(typeof(DockTreeSplitter)));
            IsShiftKeyDownPropertyKey = DependencyProperty.RegisterReadOnly("IsShiftKeyDown", typeof(bool), typeof(DockTreeSplitter),
                new FrameworkPropertyMetadata(false));
            IsShiftKeyDownProperty = IsShiftKeyDownPropertyKey.DependencyProperty;
        }

        /// <summary>Gets a value indicates whether is SHIFT key is pressed down when mouse is over this control.</summary>
        /// <value><see langword="true"/> if SHIFT key is pressed down when mouse is over this control, otherwise
        /// <see langword="false"/>. The default value is <see langword="false" />.</value>
        public bool IsShiftKeyDown
        {
            get { return (bool)GetValue(IsShiftKeyDownProperty); }
            private set
            {
                if (value)
                    SetValue(IsShiftKeyDownPropertyKey, BooleanBoxes.True);
                else
                    ClearValue(IsShiftKeyDownPropertyKey);
            }
        }

        private void SetIsShiftKeyDown()
        {
            IsShiftKeyDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
        }

        private SplitContainer SplitContainer
        {
            get { return DataContext as SplitContainer; }
        }

        private DockTreeSplit DockTreeSplit
        {
            get { return SplitContainer == null ? null : SplitContainer.DataContext as DockTreeSplit; }
        }

        /// <exclude />
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (DockTreeSplit != null && DockTreeSplit.DockControl != null)
            {
                SetIsShiftKeyDown();
                if (IsShiftKeyDown)
                {
                    DockControl dockControl = DockTreeSplit.DockControl;
                    dockControl.DockTreeZOrder = dockControl.DockTreeZOrder.SendToBack(DockTreeSplit.Position);
                    e.Handled = true;
                    return;
                }
            }
            
            base.OnMouseLeftButtonDown(e);
        }

        /// <exclude />
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            SetIsShiftKeyDown();
            KeyboardManager.KeyDown += new Action<KeyEventArgs>(CheckKey);
            KeyboardManager.KeyUp += new Action<KeyEventArgs>(CheckKey);
        }

        /// <exclude />
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            KeyboardManager.KeyDown -= new Action<KeyEventArgs>(CheckKey);
            KeyboardManager.KeyUp -= new Action<KeyEventArgs>(CheckKey);
        }

        private void CheckKey(KeyEventArgs e)
        {
            SetIsShiftKeyDown();
        }
    }
}
