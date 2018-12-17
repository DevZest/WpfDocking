using System.Windows;

namespace DevZest.Windows
{
    /// <summary>Provides attached properties to get or set a <see cref="UIElement"/> or <see cref="DataTemplate"/> as the adorner of
    /// <see cref="FrameworkElement"/>.</summary>
    /// <remarks><para><see cref="AdornerManager"/> provides two attached properties: <see cref="P:DevZest.Windows.AdornerManager.Adorner"/> to get or set <see cref="UIElement"/>
    /// as adorner of <see cref="FrameworkElement"/>; and <see cref="P:DevZest.Windows.AdornerManager.AdornerTemplate" /> to get or set <see cref="DataTemplate"/>
    /// as adorner of <see cref="FrameworkElement"/>. You can use these attached properties to declare adorner of
    /// <see cref="FrameworkElement"/> in XAML.</para>
    /// <para><see cref="P:DevZest.Windows.AdornerManager.AdornerTemplate" /> provides similar functionality as <see cref="P:DevZest.Windows.AdornerManager.Adorner"/>, however it can be used in style setters because
    /// direct UI child is not allowed. Both way the provided adorner inherits DataContext as adorned element.</para>
    /// <para>Setting adorner for specified <see cref="FrameworkElement"/> will clear the adorner previously set, no matter using
    /// <see cref="P:DevZest.Windows.AdornerManager.Adorner"/> or <see cref="P:DevZest.Windows.AdornerManager.AdornerTemplate" /> attached properties.</para></remarks>
    /// <example>The following example demostrates the usage of <see cref="P:DevZest.Windows.AdornerManager.Adorner"/> and <see cref="P:DevZest.Windows.AdornerManager.AdornerTemplate" /> attached property:
    ///     <code lang="xaml" source="..\..\Samples\Common\CSharp\AdornerManagerSample\Window1.xaml" />
    /// </example>
    public static partial class AdornerManager
    {
        /// <summary>Identifies the <see cref="P:DevZest.Windows.AdornerManager.Adorner"/> attached property.</summary>
        /// <AttachedPropertyComments>
        /// <summary>Gets or sets the adorner <see cref="UIElement"/>.</summary>
        /// <value>The adorner <see cref="UIElement"/>.</value>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty AdornerProperty = DependencyProperty.RegisterAttached("Adorner", typeof(UIElement), typeof(AdornerManager),
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnAdornerChanged)));
        /// <summary>Identifies the <see cref="P:DevZest.Windows.AdornerManager.AdornerTemplate"/> attached property.</summary>
        /// <AttachedPropertyComments>
        /// <summary>Gets or sets the adorner <see cref="DataTemplate"/>.</summary>
        /// <value>The adorner <see cref="DataTemplate"/>.</value>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty AdornerTemplateProperty = DependencyProperty.RegisterAttached("AdornerTemplate", typeof(DataTemplate), typeof(AdornerManager),
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnAdornerChanged)));
        private static readonly DependencyPropertyKey DecoratorAdornerPropertyKey = DependencyProperty.RegisterAttachedReadOnly("DecoratorAdorner", typeof(DecoratorAdorner), typeof(AdornerManager),
            new FrameworkPropertyMetadata(null));

        private static void OnAdornerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = d as FrameworkElement;
            if (element == null)
                return;

            DecoratorAdorner oldDecorator = GetDecoratorAdorner(element);
            DecoratorAdorner newDecorator = GetNewDecorator(element, e.NewValue);
            SetDecoratorAdorner(element, newDecorator);
            if (oldDecorator != null)
                oldDecorator.Close();
            if (newDecorator != null)
                newDecorator.Show();
        }

        private static DecoratorAdorner GetNewDecorator(FrameworkElement element, object newValue)
        {
            DataTemplate newTemplate = newValue as DataTemplate;
            if (newTemplate != null)
                return new DecoratorAdorner(element, newTemplate);

            UIElement newUIElement = newValue as UIElement;
            if (newUIElement != null)
                return new DecoratorAdorner(element, newUIElement);

            return null;
        }

        /// <summary>Gets the value of <see cref="P:DevZest.Windows.AdornerManager.Adorner"/> attached property
        /// from a given <see cref="FrameworkElement"/>.</summary>
        /// <param name="element">The element from which to read the property value.</param>
        /// <returns>The value of <see cref="P:DevZest.Windows.AdornerManager.Adorner"/> attached property.</returns>
        public static UIElement GetAdorner(FrameworkElement element)
        {
            return (UIElement)element.GetValue(AdornerProperty);
        }

        /// <summary>Sets the value of <see cref="P:DevZest.Windows.AdornerManager.Adorner"/> attached property
        /// to a given <see cref="FrameworkElement"/>.</summary>
        /// <param name="element">The element on which to set the property value.</param>
        /// <param name="value">The value of <see cref="P:DevZest.Windows.AdornerManager.Adorner"/> attached property.</param>
        public static void SetAdorner(FrameworkElement element, UIElement value)
        {
            element.SetValue(AdornerProperty, value);
        }

        /// <summary>Gets the value of <see cref="P:DevZest.Windows.AdornerManager.AdornerTemplate"/> attached property
        /// from a given <see cref="FrameworkElement"/>.</summary>
        /// <param name="element">The element from which to read the property value.</param>
        /// <returns>The value of <see cref="P:DevZest.Windows.AdornerManager.AdornerTemplate"/> attached property.</returns>
        public static DataTemplate GetAdornerTemplate(FrameworkElement element)
        {
            return (DataTemplate)element.GetValue(AdornerTemplateProperty);
        }

        /// <summary>Sets the value of <see cref="P:DevZest.Windows.AdornerManager.AdornerTemplate"/> attached property
        /// to a given <see cref="FrameworkElement"/>.</summary>
        /// <param name="element">The element on which to set the property value.</param>
        /// <param name="value">The value of <see cref="P:DevZest.Windows.AdornerManager.AdornerTemplate"/> attached property.</param>
        public static void SetAdornerTemplate(FrameworkElement element, DataTemplate value)
        {
            element.SetValue(AdornerTemplateProperty, value);
        }

        private static DecoratorAdorner GetDecoratorAdorner(DependencyObject element)
        {
            return (DecoratorAdorner)element.GetValue(DecoratorAdornerPropertyKey.DependencyProperty);
        }

        private static void SetDecoratorAdorner(DependencyObject element, DecoratorAdorner value)
        {
            element.SetValue(DecoratorAdornerPropertyKey, value);
        }
    }
}
