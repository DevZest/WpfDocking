using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Screen point is not allowed in partial trust environment, point relative to visual must be used.</summary>
    internal struct RelativePoint
    {
        private Visual _visual;
        private Point _point;

        public RelativePoint(Visual visual, Point point)
        {
            _visual = visual;
            _point = point;
        }

        public Visual Visual
        {
            get { return _visual; }
        }

        public Point Point
        {
            get { return _point; }
        }
    }
}
