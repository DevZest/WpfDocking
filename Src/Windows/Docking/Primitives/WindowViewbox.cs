using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Collections;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Defines a content decorator that can preview a window.</summary>
    /// <remarks>This class is used in the control template of <see cref="WindowSwitcher" /> class to
    /// preview selected window.</remarks>
    public class WindowViewbox : Decorator
    {
        private ContainerVisual _internalVisual;

        static WindowViewbox()
        {
            ClipToBoundsProperty.OverrideMetadata(typeof(WindowViewbox), new FrameworkPropertyMetadata(BooleanBoxes.True));
        }

        /// <exclude/>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            UIElement internalChild = this.InternalChild;
            if (internalChild != null)
            {
                Size desiredSize = internalChild.DesiredSize;
                double factor = ComputeScaleFactor(arrangeSize, desiredSize);
                InternalTransform = new ScaleTransform(factor, factor);
                internalChild.Arrange(new Rect(new Point(), internalChild.DesiredSize));
            }
            return arrangeSize;
        }

        private static double ComputeScaleFactor(Size availableSize, Size contentSize)
        {
            if (contentSize.Width <= availableSize.Width || contentSize.Height < availableSize.Height)
                return 1;
            else
                return Math.Max(ComputeScaleFactor(availableSize.Width, contentSize.Width),
                    ComputeScaleFactor(availableSize.Height, contentSize.Height));
        }

        private static double ComputeScaleFactor(double availableLength, double contentLength)
        {
            if (double.IsInfinity(availableLength) || contentLength.IsClose(0))
                return 1;
            return availableLength / contentLength;
        }

        /// <exclude/>
        protected override Visual GetVisualChild(int index)
        {
            if (index != 0)
                throw new ArgumentOutOfRangeException("index");

            return InternalVisual;
        }

        /// <exclude/>
        protected override Size MeasureOverride(Size constraint)
        {
            UIElement internalChild = this.InternalChild;
            Size size = new Size();
            if (internalChild != null)
            {
                Size availableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
                internalChild.Measure(availableSize);
                Size desiredSize = internalChild.DesiredSize;
                double factor = ComputeScaleFactor(constraint, desiredSize);
                if (double.IsInfinity(constraint.Width))
                    size.Width = factor * desiredSize.Width;
                if (double.IsInfinity(constraint.Height))
                    size.Height = factor * desiredSize.Height;
            }
            return size;
        }

        /// <exclude/>
        public override UIElement Child
        {
            get { return InternalChild; }
            set
            {
                UIElement internalChild = this.InternalChild;
                if (internalChild != value)
                {
                    base.RemoveLogicalChild(internalChild);
                    if (value != null)
                    {
                        base.AddLogicalChild(value);
                    }
                    this.InternalChild = value;
                    base.InvalidateMeasure();
                }
            }
        }

        private UIElement InternalChild
        {
            get
            {
                VisualCollection children = InternalVisual.Children;
                if (children.Count != 0)
                    return (children[0] as UIElement);
                return null;
            }
            set
            {
                VisualCollection children = InternalVisual.Children;
                if (children.Count != 0)
                    children.Clear();
                children.Add(value);
            }
        }

        private Transform InternalTransform
        {
            get { return InternalVisual.Transform; }
            set { InternalVisual.Transform = value; }
        }

        private ContainerVisual InternalVisual
        {
            get
            {
                if (this._internalVisual == null)
                {
                    _internalVisual = new ContainerVisual();
                    AddVisualChild(this._internalVisual);
                }
                return _internalVisual;
            }
        }

        /// <exclude/>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                if (InternalChild != null)
                    yield return InternalChild;
            }
        }

        /// <exclude/>
        protected override int VisualChildrenCount
        {
            get { return 1; }
        }
    }
}
