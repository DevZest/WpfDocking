using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;

namespace DevZest.Windows
{
    /// <summary>Converts the offset, extent and viewport values of <see cref="ScrollViewer"/> to
    /// a <see cref="Visibility" /> value indicates whether the scroll button should be displayed.</summary>
    public class ScrollButtonVisibilityConverter : IMultiValueConverter
    {
        /// <summary>Converts the offset, extent and viewport values of <see cref="ScrollViewer"/> to
        /// a <see cref="Visibility" /> value indicates the display state of scroll button.</summary>
        /// <param name="values">
        /// <para>The offset, extent and viewport values of <see cref="ScrollViewer"/>:</para>
        /// <list type="bullet">
        ///     <item><see cref="ScrollViewer.VerticalOffset" />, <see cref="ScrollViewer.ExtentHeight" />
        ///     and <see cref="ScrollViewer.ViewportHeight" />.</item>
        ///     <item><see cref="ScrollViewer.HorizontalOffset" />, <see cref="ScrollViewer.ExtentWidth" />
        ///     and <see cref="ScrollViewer.ViewportWidth" />.</item>
        /// </list>
        /// </param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use. Must be string value of "first" or "last".</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The <see cref="Visibility" /> value indicates whether the scroll button should be displayed.</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double verticalOffset = (double)values[0];
            double extentHeight = (double)values[1];
            double viewportHeight = (double)values[2];
            string strParam = ((string)parameter).ToUpperInvariant();

            if (strParam == "FIRST")
            {
                if (verticalOffset > 0)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
            else if (strParam == "LAST")
            {
                if (verticalOffset + viewportHeight < extentHeight)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
            else
                throw new ArgumentOutOfRangeException("parameter");
        }

        /// <exclude />
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
