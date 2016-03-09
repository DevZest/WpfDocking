using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Represents the strip of document tabs.</summary>
    /// <remarks><para>Use the <see cref="DocumentTabStrip"/> class in the control template of <see cref="DocumentWindow"/> class. Bind
    /// <see cref="ItemsControl.ItemsSource"/> property to the <see cref="DockPane.VisibleItems"/> property of <see cref="DockPane"/>.</para>
    /// <para>The item container of <see cref="DocumentTabStrip"/> is <see cref="DocumentTab"/>.</para></remarks>
    public class DocumentTabStrip : ItemsControl 
    {
        private static readonly DependencyPropertyKey IsTabTrimmedPropertyKey;

        /// <summary>Identifies the <see cref="IsTabTrimmed"/> dependency property.</summary>
        public static readonly DependencyProperty IsTabTrimmedProperty;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static DocumentTabStrip()
        {
            FocusableProperty.OverrideMetadata(typeof(DocumentTabStrip), new FrameworkPropertyMetadata(BooleanBoxes.False));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DocumentTabStrip), new FrameworkPropertyMetadata(typeof(DocumentTabStrip)));
            IsTabTrimmedPropertyKey = DependencyProperty.RegisterReadOnly("IsTabTrimmed", typeof(bool), typeof(DocumentTabStrip),
                new FrameworkPropertyMetadata(BooleanBoxes.False));
            IsTabTrimmedProperty = IsTabTrimmedPropertyKey.DependencyProperty;
        }

        /// <summary>Gets a value indicates whether any of the tabs are trimmed. This is a dependency property.</summary>
        /// <value><see langword="true"/> if any of the tabs are trimmed, otherwise <see langword="false"/>.</value>
        public bool IsTabTrimmed
        {
            get { return (bool)GetValue(IsTabTrimmedProperty); }
            internal set
            {
                if (value)
                    SetValue(IsTabTrimmedPropertyKey, BooleanBoxes.Box(value));
                else
                    ClearValue(IsTabTrimmedPropertyKey);
            }
        }

        /// <exclude/>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DocumentTab();
        }

        /// <exclude/>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DocumentTab;
        }
    }
}
