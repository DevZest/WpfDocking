using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;
using System.Security;
using DevZest.Windows;

[assembly: XmlnsDefinition("http://schemas.devzest.com/presentation", "DevZest.Windows")]
[assembly: XmlnsDefinition("http://schemas.devzest.com/presentation/docking", "DevZest.Windows")]
[assembly: XmlnsDefinition("http://schemas.devzest.com/presentation/docking", "DevZest.Windows.Docking")]
[assembly: XmlnsDefinition("http://schemas.devzest.com/presentation/docking", "DevZest.Windows.Docking.Primitives")]

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("DevZest WPF Docking")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("DevZest")]
[assembly: AssemblyProduct("WPF Docking")]
[assembly: AssemblyCopyright("Copyright © DevZest. All rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

//In order to begin building localizable applications, set 
//<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
//inside a <PropertyGroup>.  For example, if you are using US english
//in your source files, set the <UICulture> to en-US.  Then uncomment
//the NeutralResourceLanguage attribute below.  Update the "en-US" in
//the line below to match the UICulture setting in the project file.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]


[assembly: ThemeInfo(
    ResourceDictionaryLocation.SourceAssembly, //where theme specific resource dictionaries are located
                                               //(used if a resource is not found in the page, 
                                               // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                              //(used if a resource is not found in the page, 
                                              // app, or any theme specific resource dictionaries)
)]

[assembly: CLSCompliant(true)]