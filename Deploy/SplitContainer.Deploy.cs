using DevZest.Licensing;
using System.Windows;

namespace DevZest.Windows
{
    partial class SplitContainer
    {
        partial void VerifyLicense(bool designMode)
        {
            LicenseError licenseError = DevZest.Licensing.LicenseManager.Check(LicenseItems.WpfDocking, designMode);
            if (licenseError != null)
            {
                _licenseErrorElement = new LicenseErrorElement(this.GetType(), designMode, licenseError);
                AddLogicalChild(_licenseErrorElement);
                AddVisualChild(_licenseErrorElement);
            }
        }

        private LicenseErrorElement _licenseErrorElement;

        partial void CheckLicense(ref UIElement licenseErrorElement)
        {
            licenseErrorElement = _licenseErrorElement;
        }
    }
}
