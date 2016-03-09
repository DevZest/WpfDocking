using DevZest.Licensing;
using System.Windows;
using System.Windows.Media;

namespace DevZest.Windows.Docking
{
    partial class DockControl
    {
        private LicenseErrorElement _licenseErrorElement;

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

        /// <exclude />
        protected override int VisualChildrenCount
        {
            get
            {
                if (_licenseErrorElement != null)
                    return 1;
                else
                    return base.VisualChildrenCount;
            }
        }

        /// <exclude />
        protected override Visual GetVisualChild(int index)
        {
            if (_licenseErrorElement != null)
                return _licenseErrorElement;
            else
                return base.GetVisualChild(index);
        }

        /// <exclude />
        protected override Size MeasureOverride(Size constraint)
        {
            if (_licenseErrorElement != null)
            {
                _licenseErrorElement.Measure(constraint);
                return _licenseErrorElement.DesiredSize;
            }
            else
                return base.MeasureOverride(constraint);
        }

        /// <exclude />
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            if (_licenseErrorElement != null)
            {
                _licenseErrorElement.Arrange(new Rect(0, 0, arrangeBounds.Width, arrangeBounds.Height));
                return arrangeBounds;
            }
            else
                return base.ArrangeOverride(arrangeBounds);
        }
    }
}
