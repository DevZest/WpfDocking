using System;
using System.Windows;
using System.Windows.Markup;
using System.Security;
using System.Security.Permissions;

namespace DevZest.Windows
{
    /// <summary>Provides conditional <see cref="Uri"/> in XAML markup.</summary>
    /// <remarks>Use this markup extension to avoid <see cref="SecurityException"/> when providing resource dictionary for both full trust and partial trust (XBAP, for example) applications.</remarks>
    /// <example>
    /// The following example shows how to avoid the <see cref="SecurityException"/> in XBAP application because <see cref="DevZest.Windows.Docking.Primitives.NativeFloatingWindow"/> requires <see cref="UIPermission"/>:
    /// <code lang="xaml">
    /// <![CDATA[
    /// <ResourceDictionary>
    ///     <ResourceDictionary.Source>
    ///         <dz:ConditionalUriExtension
    ///             Condition="{x:Static dz:FloatingWindow.CanBeNative}"
    ///             True="NativeFloatingWindow.xaml"
    ///             False="Empty.xaml" />
    ///     </ResourceDictionary.Source>
    /// </ResourceDictionary>
    /// ]]>
    /// </code>
    /// </example>

    [MarkupExtensionReturnType(typeof(Uri))]
    public class ConditionalUriExtension : MarkupExtension
    {
        /// <summary>Gets the condition to select the <see cref="Uri"/>.</summary>
        /// <value>The condition to select the <see cref="Uri"/>.</value>
        public bool Condition { get; set; }

        /// <summary>Gets the <see cref="Uri"/> when <see cref="Condition"/> is <see langword="true"/>.</summary>
        /// <value>The see cref="Uri"/> when <see cref="Condition"/> is <see langword="true"/>.</value>
        public Uri True { get; set; }

        /// <summary>Gets the <see cref="Uri"/> when <see cref="Condition"/> is <see langword="false"/>.</summary>
        /// <value>The see cref="Uri"/> when <see cref="Condition"/> is <see langword="false"/>.</value>
        public Uri False { get; set; }

        /// <exclude />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Condition ? True : False;
        }
    }
}
