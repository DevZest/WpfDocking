using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace DevZest.Windows
{
    partial class AdornerManager
    {
        private class DecoratorAdorner : Adorner
        {
            private VisualCollection _visuals;
            private UIElement _child;
            private AdornerLayer _adornerLayer;

            public DecoratorAdorner(FrameworkElement source, DataTemplate adorner)
                : base(source)
            {
                Debug.Assert(source != null);
                Debug.Assert(adorner != null);
                _visuals = new VisualCollection(this);
                ContentPresenter contentPresenter = new ContentPresenter();
                contentPresenter.Content = source;
                contentPresenter.ContentTemplate = adorner;
                _child = contentPresenter;
                _visuals.Add(_child);
            }

            public DecoratorAdorner(FrameworkElement source, UIElement adorner)
                : base(source)
            {
                _visuals = new VisualCollection(this);
                DataContext = source;
                _child = adorner;
                _visuals.Add(_child);
            }

            private FrameworkElement Source
            {
                get { return AdornedElement as FrameworkElement; }
            }

            protected override Size ArrangeOverride(Size finalSize)
            {
                _child.Arrange(new Rect(new Point(), finalSize));
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

            public void Show()
            {
                Debug.Assert(_adornerLayer == null);

                if (!Source.IsLoaded)
                    Source.Loaded += new RoutedEventHandler(OnLoaded);
                else
                    AddToAdornerLayer();
            }

            private void OnLoaded(object sender, RoutedEventArgs e)
            {
                Source.Loaded -= new RoutedEventHandler(OnLoaded);
                AddToAdornerLayer();
            }

            private void AddToAdornerLayer()
            {
                _adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
                if (_adornerLayer != null)
                    _adornerLayer.Add(this);
            }

            public void Close()
            {
                Source.Loaded -= new RoutedEventHandler(OnLoaded);
                if (_adornerLayer != null)
                {
                    _adornerLayer.Remove(this);
                    _adornerLayer = null;
                }
            }
        }
    }
}
