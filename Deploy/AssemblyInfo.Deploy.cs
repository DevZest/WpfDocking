using DevZest.Licensing;
using DevZest.Windows;
using System;

[assembly: LicensePublicKey("0024000004800000940000000602000000240000525341310004000001000100ed58bddcb7bb19" +
            "9ed08c99bd83f732f26d49db4be3ea11c03a0c01bc0774bdcf5bbd3f00fd853f761598dd28489d" +
            "9849a27e9eb901bb227d2c88b6644bd8e1b1453d021ea6b724995bdc5f839a608a5aa98f2ba6c6" +
            "02d25eaed7147e8046db369ad5ff0847423d926526176ff43902ee012d98f7010a598744834210" +
            "7eb632b8")]
[assembly: FileLicenseProvider(FolderOption = FolderOption.EnvironmentSpecial, SpecialFolder = Environment.SpecialFolder.LocalApplicationData, Name = @"DevZest\WPF Docking\RuntimeLicense.txt")]
[assembly: FileLicenseProvider(FolderOption = FolderOption.EnvironmentSpecial, SpecialFolder = Environment.SpecialFolder.LocalApplicationData, Name = @"DevZest\WPF Docking\DesignTimeLicense.txt", DesignMode = true)]
[assembly: AssemblyLicenseProvider(EntryAssemblyOnly = true)]

[assembly: LicenseItem(LicenseItems.WpfDocking)]
[assembly: LicenseItem(LicenseItems.CommonControls)]
