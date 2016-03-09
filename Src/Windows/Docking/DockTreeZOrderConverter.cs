using System;
using System.Diagnostics;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace DevZest.Windows.Docking
{
    /// <summary>Converts instances of <see cref="String"/> type to and from <see cref="DockTreeZOrder"/> instances.</summary>
    /// <remarks><see cref="DockTreeZOrderConverter"/> converts <see cref="DockTreeZOrder"/> instances to and from comma delimited string,
    /// such as "Left, Top, Right, Bottom".</remarks>
    [ValueConversion(typeof(string), typeof(DockTreeZOrder))]
    public class DockTreeZOrderConverter : TypeConverter
    {
        private static EnumConverter s_dockConverter = new EnumConverter(typeof(Dock));

        /// <exclude/>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string));
        }

        /// <exclude/>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return (destinationType == typeof(InstanceDescriptor) || destinationType == typeof(string));
        }

        /// <exclude/>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string s = value as string;
            if (s != null)
                return FromString(s);

            throw GetConvertFromException(value);
        }

        /// <exclude/>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (!CanConvertTo(destinationType))
                throw GetConvertToException(value, destinationType);

            if ((value != null) && (value is DockTreeZOrder))
            {
                DockTreeZOrder z = (DockTreeZOrder)value;
                if (destinationType == typeof(string))
                    return z.ToString();

                if (destinationType == typeof(InstanceDescriptor))
                    return new InstanceDescriptor(typeof(DockTreeZOrder).GetConstructor(new Type[] { typeof(Dock), typeof(Dock), typeof(Dock), typeof(Dock) }), new object[] { z.First, z.Second, z.Third, z.Fourth });
            }

            throw GetConvertToException(value, destinationType);
        }

        private DockTreeZOrder FromString(string s)
        {
            Dock first, second, third, fourth;

            string[] strings = s.Split(',');

            if (strings.Length != 4)
                throw GetConvertFromException(s);

            first = (Dock)s_dockConverter.ConvertFrom(strings[0].Trim());
            second = (Dock)s_dockConverter.ConvertFrom(strings[1].Trim());
            third = (Dock)s_dockConverter.ConvertFrom(strings[2].Trim());
            fourth = (Dock)s_dockConverter.ConvertFrom(strings[3].Trim());

            if (!DockTreeZOrder.CheckValues(first, second, third, fourth))
                throw GetConvertFromException(s);

            return new DockTreeZOrder(first, second, third, fourth);
        }

    }
}
