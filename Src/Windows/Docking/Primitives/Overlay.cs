using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Reprents the overlay definition.</summary>
    /// <remarks>You can attach <see cref="Overlay"/> object to <see cref="FrameworkElement"/> by setting <b>DockManager.Overlay</b> attached property.
    /// Once <see cref="Overlay"/> object attached, a <see cref="ContentPresenter"/> will be generated on top of the associated element.</remarks>
    public sealed class Overlay : Freezable
    {
        private static readonly DependencyPropertyKey AssociatedElementPropertyKey = DependencyProperty.RegisterReadOnly("AssociatedElement", typeof(FrameworkElement), typeof(Overlay),
            new FrameworkPropertyMetadata(null));
        /// <summary>Identifies the <see cref="AssociatedElement"/> dependency property.</summary>
        public static readonly DependencyProperty AssociatedElementProperty = AssociatedElementPropertyKey.DependencyProperty;

        /// <summary>Identifies the <see cref="Content"/> dependency property.</summary>
        public static readonly DependencyProperty ContentProperty = ContentPresenter.ContentProperty.AddOwner(typeof(Overlay));

        /// <summary>Identifies the <see cref="ContentTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty ContentTemplateProperty = ContentPresenter.ContentTemplateProperty.AddOwner(typeof(Overlay));

        /// <summary>Identifies the <see cref="ContentTemplateSelector"/> dependency property.</summary>
        public static readonly DependencyProperty ContentTemplateSelectorProperty = ContentPresenter.ContentTemplateSelectorProperty.AddOwner(typeof(Overlay));

        /// <summary>Identifies the <see cref="IsFloatingWindowPreview"/> dependency property.</summary>
        public static readonly DependencyProperty IsFloatingWindowPreviewProperty = DependencyProperty.Register("IsFloatingWindowPreview", typeof(bool), typeof(Overlay),
            new FrameworkPropertyMetadata(BooleanBoxes.False));

        internal static void OnOverlayChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            Overlay oldValue = (Overlay)e.OldValue;
            Overlay newValue = (Overlay)e.NewValue;

            // Attach(obj) will set AssociatedElement and cause Overlay changed again, resulting in StackOverflowException
            if (oldValue == newValue)
                return;

            if (oldValue != null)
                oldValue.Detach();
            if (newValue != null)
                newValue.Attach(obj as FrameworkElement);
        }

        /// <exclude />
        protected override Freezable CreateInstanceCore()
        {
            return new Overlay();
        }

        // We derive Overlay from Freezable to provide an inheritance context for bindings
        // however we don't really want this object to be frozen
        /// <exclude />
        protected override bool FreezeCore(bool isChecking)
        {
            return false;
        }

        /// <summary>Gets the associated element.</summary>
        /// <value>The associated element.</value>
        public FrameworkElement AssociatedElement
        {
            get { return (FrameworkElement)GetValue(AssociatedElementProperty); }
            private set { SetValue(AssociatedElementPropertyKey, value); }
        }

        internal ContentPresenter Container { get; set; }

        /// <summary>Gets or sets the value indicates whether the overlay is floating window preview.</summary>
        /// <value><see langword="true"/> if this overlay is floating window preview, otherwise <see langword="false"/>.</value>
        public bool IsFloatingWindowPreview
        {
            get { return (bool)GetValue(IsFloatingWindowPreviewProperty); }
            set { SetValue(IsFloatingWindowPreviewProperty, value); }
        }

        /// <summary>Gets or sets the data used to generate the overlay UI.</summary>
        /// <value>The data used to generate the overlay UI. The default value is <see langword="null"/>.</value>
        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>Gets or sets the template used to display the content of the overlay.</summary>
        /// <value>A <see cref="DataTemplate" /> that defines the visualization of the overlay. The default is <see langword="null" />.</value>
        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        /// <summary>Gets or sets the <see cref="DataTemplateSelector" />, which allows the application writer to provide custom logic for choosing the template that is used to display the content of the overlay.</summary>
        /// <value>A <see cref="DataTemplateSelector" /> object that supplies logic to return a <see cref="DataTemplate" /> to apply. The default is <see langword="null"/>.</value>
        public DataTemplateSelector ContentTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty); }
            set { SetValue(ContentTemplateSelectorProperty, value); }
        }

        private void Attach(FrameworkElement associatedElement)
        {
            Debug.Assert(associatedElement != null);

            if (AssociatedElement != null)
                throw new InvalidOperationException(SR.Exception_OverlayActivator_HostMultipleTimes);

            if (!associatedElement.IsLoaded)
                return;

            AssociatedElement = associatedElement;
            Container = new ContentPresenter();
            Container.Content = Content ?? AssociatedElement;
            Container.ContentTemplate = ContentTemplate;
            Container.ContentTemplateSelector = ContentTemplateSelector;
            DockManager.AddOverlay(this);
        }

        private void Detach()
        {
            if (AssociatedElement == null)
                return;

            DockManager.RemoveOverlay(this);
            AssociatedElement = null;
        }
    }
}
