using DevZest.Licensing;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows.Controls;

namespace DevZest.Windows
{
    internal class LicenseErrorElement : TextBox
    {
        public LicenseErrorElement(Type type, bool designMode, LicenseError licenseError)
        {
            Debug.Assert(licenseError != null);
            IsReadOnly = true;
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, @"This control ""{0}"" is not property licensed.", type));
            stringBuilder.AppendLine("Use License Console program to obtain a license.");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("Detailed Information:");
            stringBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, @"Assembly: {0}", licenseError.Assembly));
            stringBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, @"Reason: {0}", licenseError.Reason));
            stringBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, @"Message: {0}", licenseError.Message));
            stringBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, @"DesignMode: {0}", designMode));
            stringBuilder.AppendLine("License:");
            if (licenseError.License != null)
                stringBuilder.AppendLine(licenseError.License.SignedString);
            Text = stringBuilder.ToString();
        }
    }
}