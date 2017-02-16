using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace DevZest
{
    static partial class Extension
    {
        /*
        const double Epsilon = 1e-8;

        /// <summary>
        /// Returns true if the doubles are close (difference smaller than 10^-8).
        /// </summary>
        public static bool IsClose(this double d1, double d2)
        {
            if (d1 == d2) // required for infinities
                return true;
            return Math.Abs(d1 - d2) < Epsilon;
        }
        */

        public static bool IsClose(this double value1, double value2)
        {
            if (value1 == value2)
                return true;
            double num = ((Math.Abs(value1) + Math.Abs(value2)) + 10.0) * 2.2204460492503131E-16;
            double num2 = value1 - value2;
            return ((-num < num2) && (num > num2));
        }
    }
}
