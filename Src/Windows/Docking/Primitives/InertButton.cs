using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Represents a button with optional drop-down context menu.</summary>
    /// <remarks>You can assign a <see cref="ContextMenu"/> to <see cref="DropDown"/> property directly, or assign a collection to <see cref="DropDownItemsSource"/>
    /// to generate a context menu, and use <see cref="DropDownItemStyle"/> property to apply a <see cref="Style"/> to the generated <see cref="MenuItem"/>.
    /// The <see cref="DropDown"/> property takes precedence over <see cref="DropDownItemsSource"/> property.</remarks>
    public class InertButton : Button
    {
        /// <summary>Identifies the <see cref="IsDropDownOpen"/> dependency property.</summary>
        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(InertButton),
            new FrameworkPropertyMetadata(BooleanBoxes.False, new PropertyChangedCallback(OnIsDropDownOpenChanged)));

        /// <summary>Identifies the <see cref="DropDownItemsSource"/> dependency property.</summary>
        public static readonly DependencyProperty DropDownItemsSourceProperty = DependencyProperty.Register("DropDownItemsSource", typeof(IEnumerable), typeof(InertButton),
            new FrameworkPropertyMetadata(null));

        /// <summary>Identifies the <see cref="DropDownItemStyle"/> dependency property.</summary>
        public static readonly DependencyProperty DropDownItemStyleProperty = DependencyProperty.Register("DropDownItemStyle", typeof(Style), typeof(InertButton),
            new FrameworkPropertyMetadata(null));

        /// <summary>Identifies the <see cref="DropDown"/> dependency property.</summary>
        public static readonly DependencyProperty DropDownProperty = DependencyProperty.Register("DropDown", typeof(ContextMenu), typeof(InertButton),
            new FrameworkPropertyMetadata(null));

        static InertButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(InertButton), new FrameworkPropertyMetadata(typeof(InertButton)));
        }

        static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool oldValue = (bool)e.OldValue;
            bool newValue = (bool)e.NewValue;
            if (oldValue == newValue)
                return;

            ((InertButton)d).OnIsDropDownOpenChanged(newValue);
        }

        void OnIsDropDownOpenChanged(bool newValue)
        {
            if (DropDown != null)
            {
                if (newValue)
                {
                    DropDown.PlacementTarget = this;
                    DropDown.Placement = PlacementMode.Bottom;
                    DropDown.SetBinding(ContextMenu.IsOpenProperty, new Binding() { Source = this, Path = new PropertyPath(IsDropDownOpenProperty), Mode = BindingMode.TwoWay });
                }
                else
                {
                    DropDown.ClearValue(ContextMenu.IsOpenProperty);
                    DropDown.ClearValue(ContextMenu.PlacementTargetProperty);
                    DropDown.ClearValue(ContextMenu.PlacementProperty);
                }
            }
            else
            {
                if (newValue)
                {
                    if (_contextMenu != null)
                        return;

                    _contextMenu = new ContextMenu();
                    _contextMenu.PlacementTarget = this;
                    _contextMenu.Placement = PlacementMode.Bottom;
                    _contextMenu.SetBinding(ContextMenu.ItemsSourceProperty, new Binding() { Source = this, Path = new PropertyPath(DropDownItemsSourceProperty) });
                    _contextMenu.SetBinding(ContextMenu.ItemContainerStyleProperty, new Binding() { Source = this, Path = new PropertyPath(DropDownItemStyleProperty) });
                    _contextMenu.SetBinding(ContextMenu.IsOpenProperty, new Binding() { Source = this, Path = new PropertyPath(IsDropDownOpenProperty), Mode = BindingMode.TwoWay });
                }
                else
                {
                    _contextMenu.ClearValue(ContextMenu.IsOpenProperty);
                    _contextMenu = null;
                }
            }
        }

        ContextMenu _contextMenu;

        /// <summary>Gets or sets the drop-down <see cref="ContextMenu"/>.</summary>
        /// <value>The drop-down <see cref="ContextMenu"/>.</value>
        /// <remarks>You can assign a <see cref="ContextMenu"/> to <see cref="DropDown"/> property directly, or assign a collection to <see cref="DropDownItemsSource"/>
        /// to generate a context menu, and use <see cref="DropDownItemStyle"/> property to apply a <see cref="Style"/> to the generated <see cref="MenuItem"/>.
        /// The <see cref="DropDown"/> property takes precedence over <see cref="DropDownItemsSource"/> property.</remarks>
        public ContextMenu DropDown
        {
            get { return (ContextMenu)GetValue(DropDownProperty); }
            set { SetValue(DropDownProperty, value); }
        }

        bool HasDropDown
        {
            get { return DropDown != null || DropDownItemsSource != null; }
        }

        /// <summary>Gets or sets a value that indicates whether the drop-down context menu is currently open. This is a dependency property.</summary>
        /// <value><see langword="true" /> if the drop-down is open; otherwise, <see langword="false" />. The default is <see langword="false" />.</value>
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        /// <summary>Gets or sets a collection used to generate the content of the drop-down context menu.</summary>
        /// <value>A collection that is used to generate the content of the drop-down context menu. The default is <see langword="null"/>.</value>
        /// <remarks>You can assign a <see cref="ContextMenu"/> to <see cref="DropDown"/> property directly, or assign a collection to <see cref="DropDownItemsSource"/>
        /// to generate a context menu, and use <see cref="DropDownItemStyle"/> property to apply a <see cref="Style"/> to the generated <see cref="MenuItem"/>.
        /// The <see cref="DropDown"/> property takes precedence over <see cref="DropDownItemsSource"/> property.</remarks>
        [BindableAttribute(true)]
        public IEnumerable DropDownItemsSource
        {
            get { return (IEnumerable)GetValue(DropDownItemsSourceProperty); }
            set { SetValue(DropDownItemsSourceProperty, value); }
        }

        /// <summary>Gets or sets the <see cref="Style" /> that is applied to the <see cref="MenuItem"/> of generated drop-down <see cref="ContextMenu"/>.</summary>
        /// <value>the <see cref="Style" /> that is applied to the <see cref="MenuItem"/> of generated drop-down <see cref="ContextMenu"/>.</value>
        public Style DropDownItemStyle
        {
            get { return (Style)GetValue(DropDownItemStyleProperty); }
            set { SetValue(DropDownItemStyleProperty, value); }
        }

        /// <exclude/>
        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            bool revertIsDropDown = HasDropDown && (IsDropDownOpen || (!IsDropDownOpen && e.LeftButton == MouseButtonState.Pressed));
            if (revertIsDropDown)
            {
                IsDropDownOpen = !IsDropDownOpen;
                e.Handled = true;
            }

            base.OnMouseDown(e);
        }
    }
}
