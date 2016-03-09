using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows
{
    /// <summary>Manages style of elements.</summary>
    /// <remarks><para>This class provides <see cref="P:DevZest.Windows.StyleManager.IsAutoMerge" /> attached property to automatically merge the default style change. The following XAML
    /// defines a style based on the dynamic default style:</para>
    /// <code lang="xaml">
    /// <![CDATA[
    /// <Style TargetType={x:Type MenuItem}>
    ///   ...
    ///   <Setter Property="StyleManager.IsAutoMerge" Value="True" />
    ///   ...
    /// </Style>
    /// ]]>
    /// </code>
    /// <para>This is equivalent to the following unsupported XAML (only static resource is allowed on <see cref="P:System.Windows.Style.BasedOn">Style.BasedOn</see>):</para>
    /// <code lang="xaml">
    /// <![CDATA[
    /// <Style BasedOn="{DynamicResource {x:Type MenuItem}}">
    ///   ...
    /// </Style>
    /// ]]>
    /// </code>
    /// </remarks>
    public static class StyleManager
    {
        /// <summary>Identifies the <see cref="P:DevZest.Windows.StyleManager.IsAutoMerge" /> attached property.</summary>
        /// <AttachedPropertyComments>
        /// <summary>Gets or sets the value that indicates whether merge the default style change automatically.</summary>
        /// <value><see langword="true"/> if automatically merge the default style change, otherwise <see langword="false"/>.</value>
        /// <remarks><para>You can use <see cref="P:DevZest.Windows.StyleManager.IsAutoMerge" /> attached property to automatically merge the default style change. The following XAML
        /// defines a style based on the dynamic default style:</para>
        /// <code lang="xaml">
        /// <![CDATA[
        /// <Style TargetType={x:Type MenuItem}>
        ///   ...
        ///   <Setter Property="StyleManager.IsAutoMerge" Value="True" />
        ///   ...
        /// </Style>
        /// ]]>
        /// </code>
        /// <para>This is equivalent to the following unsupported XAML (only static resource is allowed on <see cref="P:System.Windows.Style.BasedOn">Style.BasedOn</see>):</para>
        /// <code lang="xaml">
        /// <![CDATA[
        /// <Style BasedOn="{DynamicResource {x:Type MenuItem}}">
        ///   ...
        /// </Style>
        /// ]]>
        /// </code>
        /// </remarks>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty IsAutoMergeProperty = DependencyProperty.RegisterAttached("IsAutoMerge", typeof(bool), typeof(StyleManager),
            new FrameworkPropertyMetadata(BooleanBoxes.False, new PropertyChangedCallback(OnIsAutoMergeChanged)));

        /// <summary>Gets the value of <see cref="P:DevZest.Windows.StyleManager.IsAutoMerge" /> attached property
        /// from a given <see cref="DependencyObject" />.</summary>
        /// <param name="d">The <see cref="DependencyObject"/> from which to read the property value.</param>
        /// <returns>The value of <see cref="P:DevZest.Windows.StyleManager.IsAutoMerge" /> attached property.</returns>
        public static bool GetIsAutoMerge(DependencyObject d)
        {
            return (bool)d.GetValue(IsAutoMergeProperty);
        }

        /// <summary>Sets the value of <see cref="P:DevZest.Windows.StyleManager.IsAutoMerge" /> attached property
        /// to a given <see cref="DependencyObject" />.</summary>
        /// <param name="d">The <see cref="DependencyObject"/> on which to set the property value.</param>
        /// <param name="value">The value of <see cref="P:DevZest.Windows.StyleManager.IsAutoMerge" /> attached property.</param>
        public static void SetIsAutoMerge(DependencyObject d, bool value)
        {
            d.SetValue(IsAutoMergeProperty, value);
        }

        private static void OnIsAutoMergeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool oldValue = (bool)e.OldValue;
            bool newValue = (bool)e.NewValue;
            if (oldValue == newValue)
                return;

            Control control = d as Control;
            if (control == null)
                return;

            if (newValue)
            {
                Type type = d.GetType();
                control.SetResourceReference(StyleManager.BasedOnStyleProperty, type);
            }
            else
                control.ClearValue(StyleManager.BasedOnStyleProperty);
        }

        #region BaseOnStyle

        private static readonly DependencyProperty BasedOnStyleProperty = DependencyProperty.RegisterAttached("BasedOnStyle", typeof(Style), typeof(StyleManager),
                new FrameworkPropertyMetadata((Style)null, new PropertyChangedCallback(OnBasedOnStyleChanged)));

        private static Style GetBasedOnStyle(DependencyObject d)
        {
            return (Style)d.GetValue(BasedOnStyleProperty);
        }

        private static void SetBasedOnStyle(DependencyObject d, Style value)
        {
            d.SetValue(BasedOnStyleProperty, value);
        }

        private static void OnBasedOnStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Style oldValue = (Style)e.OldValue;
            Style newValue = (Style)e.NewValue;

            if (oldValue == newValue)
                return;

            Control control = d as Control;
            Debug.Assert(control != null);

            Style baseOnStyle = newValue;
            Style originalStyle = GetOriginalStyle(control);
            if (originalStyle == null)
            {
                originalStyle = control.Style;
                SetOriginalStyle(control, originalStyle);
            }
            Style newStyle = originalStyle;

            if (originalStyle.IsSealed)
            {
                newStyle = new Style();
                newStyle.TargetType = originalStyle.TargetType;

                //1. Copy resources, setters, triggers
                newStyle.Resources = originalStyle.Resources;
                foreach (var st in originalStyle.Setters)
                {
                    newStyle.Setters.Add(st);
                }
                foreach (var tg in originalStyle.Triggers)
                {
                    newStyle.Triggers.Add(tg);
                }

                //2. Set BaseOn Style
                newStyle.BasedOn = baseOnStyle;
            }
            else
            {
                originalStyle.BasedOn = baseOnStyle;
            }

            control.Style = newStyle;
        }

        #endregion

        private static readonly DependencyProperty OriginalStyleProperty = DependencyProperty.RegisterAttached("OriginalStyle", typeof(Style), typeof(StyleManager),
                new FrameworkPropertyMetadata((Style)null));

        private static Style GetOriginalStyle(DependencyObject d)
        {
            return (Style)d.GetValue(OriginalStyleProperty);
        }

        private static void SetOriginalStyle(DependencyObject d, Style value)
        {
            d.SetValue(OriginalStyleProperty, value);
        }
    }
}
