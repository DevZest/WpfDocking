using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace DevZest.Windows
{
    /// <summary>Represents a button that drops down a context menu.</summary>
    /// <remarks>
    /// <see cref="DropDownButton.DropDown"/> property defines the <see cref="ContextMenu"/> to drop down.
    /// <see cref="DropDownButton.IsDropDownOpen" /> property determines if the drop down <see cref="ContextMenu"/> is open.
    /// </remarks>
    /// <example>
    ///     <code lang="xaml" source="..\..\Samples\Common\CSharp\DropDownButtonSample\Window1.xaml" />
    /// </example>
    public class DropDownButton : Button
    {
        /// <summary>Identifies the <see cref="DropDown"/> dependency property.</summary>
        public static readonly DependencyProperty DropDownProperty = DependencyProperty.Register("DropDown", typeof(ContextMenu), typeof(DropDownButton),
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDropDownChanged)));
        private static readonly DependencyPropertyKey IsDropDownOpenPropertyKey = DependencyProperty.RegisterReadOnly("IsDropDownOpen", typeof(bool), typeof(DropDownButton),
            new FrameworkPropertyMetadata(BooleanBoxes.False));
        /// <summary>Identifies the <see cref="IsDropDownOpen" /> dependency property.</summary>
        public static readonly DependencyProperty IsDropDownOpenProperty = IsDropDownOpenPropertyKey.DependencyProperty;

        private static void OnDropDownChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DropDownButton)d).OnDropDownChanged((ContextMenu)e.OldValue, (ContextMenu)e.NewValue);
        }

        private void OnDropDownChanged(ContextMenu oldValue, ContextMenu newValue)
        {
            if (oldValue == newValue)
                return;

            if (oldValue != null)
                oldValue.Closed -= new RoutedEventHandler(OnDropDownClosed);

            if (newValue != null)
                newValue.Closed += new RoutedEventHandler(OnDropDownClosed);

            if (IsDropDownOpen)
                IsDropDownOpen = false;
        }

        private void OnDropDownClosed(object sender, RoutedEventArgs e)
        {
            if (IsDropDownOpen)
                IsDropDownOpen = false;
        }

        /// <summary>Gets or sets the drop down <see cref="ContextMenu"/>. This is a dependency property.</summary>
        /// <value>The <see cref="ContextMenu"/> to drop down. The default value is <see langword="null"/>.</value>
        public ContextMenu DropDown
        {
            get { return (ContextMenu)GetValue(DropDownProperty); }
            set { SetValue(DropDownProperty, value); }
        }

        /// <summary>Gets a value that indicates whether the drop down ContextMenu is visible. This is a dependency property.</summary>
        /// <value><see langword="true"/> if the ContextMenu is visible; otherwise, <see langword="false"/>.</value>
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            private set { SetValue(IsDropDownOpenPropertyKey, BooleanBoxes.Box(value)); }
        }

        /// <exclude/>
        protected override void OnClick()
        {
            if (DropDown != null && !IsDropDownOpen)
            {
                DropDown.PlacementTarget = this;
                DropDown.Placement = PlacementMode.Bottom;
                IsDropDownOpen = DropDown.IsOpen = true;
            }
        }

        /// <exclude/>
        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DropDown != null && IsDropDownOpen)
            {
                DropDown.IsOpen = false;
                e.Handled = true;
            }
            else
                base.OnMouseDown(e);
        }
    }
}
