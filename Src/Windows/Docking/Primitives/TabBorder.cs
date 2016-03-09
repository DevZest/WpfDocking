using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Draws the trapezoid tab border</summary>
    public class TabBorder : Decorator
    {
        private struct Points
        {
            private Point _pt0;
            private Point _pt1;
            private Point _pt2;
            private Point _pt3;
            private Point _pt4;
            private Point _pt5;
            private Point _pt6;
            private Point _pt7;

            public Points(Rect bounds, CornerRadius radius, double leftAngle, double rightAngle, bool isBottom)
            {
                double deltaX, deltaY;

                if (isBottom)
                {
                    // Top Left: pt0 and pt1
                    _pt0 = _pt1 = bounds.TopLeft;
                    deltaX = GetDeltaX(radius.TopLeft, leftAngle);
                    deltaY = GetDeltaY(radius.TopLeft, leftAngle);
                    _pt1.Offset(deltaX, deltaY);

                    // Bottom Left: pt2 and pt3
                    deltaX = GetDeltaX(radius.BottomLeft, leftAngle);
                    deltaY = GetDeltaY(radius.BottomLeft, leftAngle);
                    _pt2 = new Point(_pt1.X + GetExtentX(leftAngle, bounds.Bottom - _pt1.Y - deltaY), bounds.Bottom - deltaY);
                    _pt3 = new Point(_pt2.X + deltaX, bounds.Bottom);

                    // Bottom Right: pt7 and pt6
                    _pt7 = _pt6 = bounds.TopRight;
                    deltaX = GetDeltaX(radius.TopRight, rightAngle);
                    deltaY = GetDeltaY(radius.TopRight, rightAngle);
                    _pt6.Offset(-deltaX, deltaY);

                    // Top Right: pt5 and pt4
                    deltaX = GetDeltaX(radius.BottomRight, rightAngle);
                    deltaY = GetDeltaY(radius.BottomRight, rightAngle);
                    _pt5 = new Point(_pt6.X - GetExtentX(rightAngle, bounds.Bottom - _pt6.Y - deltaY), bounds.Bottom - deltaY);
                    _pt4 = new Point(_pt5.X - deltaX, bounds.Bottom);
                }
                else
                {
                    // Bottom Left: pt0 and pt1
                    _pt0 = _pt1 = bounds.BottomLeft;
                    deltaX = GetDeltaX(radius.BottomLeft, leftAngle);
                    deltaY = GetDeltaY(radius.BottomLeft, leftAngle);
                    _pt1.Offset(deltaX, -deltaY);

                    // Top Left: pt2 and pt3
                    deltaX = GetDeltaX(radius.TopLeft, leftAngle);
                    deltaY = GetDeltaY(radius.TopLeft, leftAngle);
                    _pt2 = new Point(_pt1.X + GetExtentX(leftAngle, _pt1.Y - bounds.Top - deltaY), bounds.Top + deltaY);
                    _pt3 = new Point(_pt2.X + deltaX, bounds.Top);

                    // Bottom Right: pt7 and pt6
                    _pt7 = _pt6 = bounds.BottomRight;
                    deltaX = GetDeltaX(radius.BottomRight, rightAngle);
                    deltaY = GetDeltaY(radius.BottomRight, rightAngle);
                    _pt6.Offset(-deltaX, -deltaY);

                    // Top Right: pt5 and pt4
                    deltaX = GetDeltaX(radius.TopRight, rightAngle);
                    deltaY = GetDeltaY(radius.TopRight, rightAngle);
                    _pt5 = new Point(_pt6.X - GetExtentX(rightAngle, _pt6.Y - bounds.Top - deltaY), bounds.Top + deltaY);
                    _pt4 = new Point(_pt5.X - deltaX, bounds.Top);
                }
            }

            private static double GetExtentX(double angle, double height)
            {
                return height * Math.Tan((90 - angle) * (Math.PI / 180));
            }

            public Point Pt0
            {
                get { return _pt0; }
            }

            public Point Pt1
            {
                get { return _pt1; }
            }

            public Point Pt2
            {
                get { return _pt2; }
            }

            public Point Pt3
            {
                get { return _pt3; }
            }

            public Point Pt4
            {
                get { return _pt4; }
            }

            public Point Pt5
            {
                get { return _pt5; }
            }

            public Point Pt6
            {
                get { return _pt6; }
            }

            public Point Pt7
            {
                get { return _pt7; }
            }

            static double GetDeltaX(double radius, double angle)
            {
                return radius * Math.Sin(DegreeToRadians(angle));
            }

            static double GetDeltaY(double radius, double angle)
            {
                return radius * (1 - Math.Cos(DegreeToRadians(angle)));
            }

            static double DegreeToRadians(double degree)
            {
                return degree * (Math.PI / 180);
            }
        }

        /// <summary>Identifies the <see cref="Background"/> dependency property.</summary>
        public static readonly DependencyProperty BackgroundProperty = Border.BackgroundProperty.AddOwner(typeof(TabBorder), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>Identifies the <see cref="BorderBrush"/> dependency property.</summary>
        public static readonly DependencyProperty BorderBrushProperty = Border.BorderBrushProperty.AddOwner(typeof(TabBorder), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>Identifies the <see cref="BorderThickness"/> dependency property.</summary>
        public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register("BorderThickness", typeof(double), typeof(TabBorder),
            new FrameworkPropertyMetadata((double)0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>Identifies the <see cref="CornerRadius"/> dependency property.</summary>
        public static readonly DependencyProperty CornerRadiusProperty = Border.CornerRadiusProperty.AddOwner(typeof(TabBorder), new FrameworkPropertyMetadata(new CornerRadius()));

        /// <summary>Identifies the <see cref="LeftAngle"/> dependency property.</summary>
        public static readonly DependencyProperty LeftAngleProperty = DependencyProperty.Register("LeftAngle", typeof(double), typeof(TabBorder),
            new FrameworkPropertyMetadata((double)90));

        /// <summary>Identifies the <see cref="RightAngle"/> dependency property.</summary>
        public static readonly DependencyProperty RightAngleProperty = DependencyProperty.Register("RightAngle", typeof(double), typeof(TabBorder),
            new FrameworkPropertyMetadata((double)90));

        /// <summary>Identifies the <see cref="IsBottom"/> dependency property.</summary>
        public static readonly DependencyProperty IsBottomProperty = DependencyProperty.Register("IsBottom", typeof(bool), typeof(TabBorder),
            new FrameworkPropertyMetadata(BooleanBoxes.False));

        static TabBorder()
        {
            FocusableProperty.OverrideMetadata(typeof(TabBorder), new FrameworkPropertyMetadata(BooleanBoxes.False));
        }

        /// <summary>Gets or sets the <see cref="Brush"/> that fills the area. This is a dependency property.</summary>
        /// <value>The <see cref="Brush"/> that draws the background. This property has no default value.</value>
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        /// <summary>Gets or sets the <see cref="Brush"/> that draws the outer border color. This is a dependency property.</summary>
        /// <value>The <see cref="Brush"/> that draws the outer border color. This property has no default value.</value>
        public Brush BorderBrush
        {
            get { return (Brush)base.GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        /// <summary>Gets or sets the border thickness. This is a dependency property.</summary>
        /// <value>The border thickness in device-independent units (1/96th inch per unit). The default value is 0.0.</value>
        public double BorderThickness
        {
            get { return (double)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        /// <summary>Gets or sets a value that represents the degree to which the corners of a Border are rounded. This is a dependency property.</summary>
        /// <value>The <see cref="CornerRadius" /> that describes the degree to which corners are rounded.</value>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>Gets or sets the angle of the left edge. This is a dependency property.</summary>
        /// <value>The angle of the left edge. The default value is 90.</value>
        public double LeftAngle
        {
            get { return (double)GetValue(LeftAngleProperty); }
            set { SetValue(LeftAngleProperty, value); }
        }

        /// <summary>Gets or sets the angle of the right edge. This is a dependency property.</summary>
        /// <value>The angle of the right edge. The default value is 90.</value>
        public double RightAngle
        {
            get { return (double)GetValue(RightAngleProperty); }
            set { SetValue(RightAngleProperty, value); }
        }

        /// <summary>Gets or sets the value indicates the placement of the tab.</summary>
        /// <value><see langword="true"/> if the tab is placed at bottom, otherwise <see langword="false"/>.</value>
        public bool IsBottom
        {
            get { return (bool)GetValue(IsBottomProperty); }
            set { SetValue(IsBottomProperty, value); }
        }

        /// <exclude />
        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            if (ClipToBounds)
            {
                Geometry clip = GetTrapezoidGeometry(layoutSlotSize, 0);

                Geometry baseClip = base.GetLayoutClip(layoutSlotSize);
                if (baseClip == null)
                    return clip;

                CombinedGeometry mergedClip = new CombinedGeometry(GeometryCombineMode.Intersect, baseClip, clip);
                mergedClip.Freeze();
                return mergedClip;
            }
            else
                return base.GetLayoutClip(layoutSlotSize);
        }

        /// <exclude />
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            Pen pen = null;
            if (BorderBrush != null)
            {
                pen = new Pen(BorderBrush, BorderThickness);
                if (pen.CanFreeze)
                    pen.Freeze();
            }
            if (Background != null && Background.CanFreeze)
                Background.Freeze();

            Geometry geometry = GetTrapezoidGeometry(RenderSize, BorderThickness);
            //drawingContext.PushClip(geometry);
            drawingContext.DrawGeometry(Background, pen, geometry);
            //drawingContext.Pop();
        }

        Geometry GetTrapezoidGeometry(Size size, double borderThickness)
        {
            bool hasBorder = borderThickness != 0;
            Rect rect;
            if (hasBorder)
            {
                double margin = borderThickness / 2;
                if (IsBottom)
                    rect = new Rect(margin, 0, size.Width - borderThickness, size.Height - margin);
                else
                    rect = new Rect(margin, margin, size.Width - borderThickness, size.Height - margin);
            }
            else
                rect = new Rect(size);

            StreamGeometry geometry = new StreamGeometry();
            using (StreamGeometryContext context = geometry.Open())
            {
                Points points = new Points(rect, CornerRadius, LeftAngle, RightAngle, IsBottom);
                if (IsBottom)
                {
                    context.BeginFigure(points.Pt0, true, false);
                    context.ArcTo(points.Pt1, new Size(CornerRadius.TopLeft, CornerRadius.TopLeft), 0, false, SweepDirection.Clockwise, hasBorder, false);
                    context.LineTo(points.Pt2, hasBorder, false);
                    context.ArcTo(points.Pt3, new Size(CornerRadius.BottomLeft, CornerRadius.BottomLeft), 0, false, SweepDirection.Counterclockwise, hasBorder, false);
                    context.LineTo(points.Pt4, hasBorder, false);
                    context.ArcTo(points.Pt5, new Size(CornerRadius.BottomRight, CornerRadius.BottomRight), 0, false, SweepDirection.Counterclockwise, hasBorder, false);
                    context.LineTo(points.Pt6, hasBorder, false);
                    context.ArcTo(points.Pt7, new Size(CornerRadius.TopRight, CornerRadius.TopRight), 0, false, SweepDirection.Clockwise, hasBorder, false);
                    context.LineTo(points.Pt0, false, false);
                }
                else
                {
                    context.BeginFigure(points.Pt0, true, false);
                    context.ArcTo(points.Pt1, new Size(CornerRadius.BottomLeft, CornerRadius.BottomLeft), 0, false, SweepDirection.Counterclockwise, hasBorder, false);
                    context.LineTo(points.Pt2, hasBorder, false);
                    context.ArcTo(points.Pt3, new Size(CornerRadius.TopLeft, CornerRadius.TopLeft), 0, false, SweepDirection.Clockwise, hasBorder, false);
                    context.LineTo(points.Pt4, hasBorder, false);
                    context.ArcTo(points.Pt5, new Size(CornerRadius.TopRight, CornerRadius.TopRight), 0, false, SweepDirection.Clockwise, hasBorder, false);
                    context.LineTo(points.Pt6, hasBorder, false);
                    context.ArcTo(points.Pt7, new Size(CornerRadius.BottomRight, CornerRadius.BottomRight), 0, false, SweepDirection.Counterclockwise, hasBorder, false);
                    context.LineTo(points.Pt0, false, false);
                }
            }

            geometry.Freeze();
            return geometry;
        }
    }
}
