using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace DevZest.Windows.Docking.Primitives
{
    internal class FloatingWindowAdorner : Adorner
    {
        public static FloatingWindowAdorner Add(DockControl dockControl, UIElement element)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(dockControl);
            FloatingWindowAdorner adorner = new FloatingWindowAdorner(adornerLayer, element);
            adornerLayer.Add(adorner);
            return adorner;
        }

        public static void Remove(DockControl dockControl, FloatingWindowAdorner adorner)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(dockControl);
            adornerLayer.Remove(adorner);
        }

        VisualCollection _visuals;
        UIElement _element;

        private FloatingWindowAdorner(FrameworkElement source, UIElement element)
            : base(source)
        {
            _visuals = new VisualCollection(this);
            _visuals.Add(element);
            _element = element;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _element.Arrange(new Rect(new Point(), finalSize));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return _visuals[index];
        }

        protected override int VisualChildrenCount
        {
            get { return _visuals.Count; }
        }
    }
}
