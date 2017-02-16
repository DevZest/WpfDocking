using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace DevZest.Windows
{
    static partial class Extension
    {
        /// <summary>
        /// Returns true if the doubles are close (difference smaller than 10^-8).
        /// </summary>
        public static bool IsClose(this Size d1, Size d2)
        {
            return d1.Width.IsClose(d2.Width) && d1.Height.IsClose(d2.Height);
        }

        /// <summary>
        /// Returns true if the doubles are close (difference smaller than 10^-8).
        /// </summary>
        public static bool IsClose(this Vector d1, Vector d2)
        {
            return d1.X.IsClose(d2.X) && d1.Y.IsClose(d2.Y);
        }

        public static bool IsClose(this Point point1, Point point2)
        {
            return point1.X.IsClose(point2.X) && point1.Y.IsClose(point2.Y);
        }
    }
}
