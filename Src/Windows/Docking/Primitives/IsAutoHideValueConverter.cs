using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Converts a <see cref="DockPosition"/> value to a <see cref="Boolean"/> value, indicates whether the provided
    /// <see cref="DockPosition"/> value is auto-hide.</summary>
    /// <remarks><see cref="IsAutoHideValueConverter"/> only supports one way conversion from <see cref="DockPosition"/> to
    /// <see cref="Boolean"/>. Any other type of conversion will throw an exception.</remarks>
    [ValueConversion(typeof(DockPosition), typeof(bool))]
    public class IsAutoHideValueConverter : IValueConverter
    {
        /// <exclude/>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DockPosition dockPosition = (DockPosition)value;
            return DockPositionHelper.IsAutoHide(dockPosition);
        }

        /// <exclude/>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
