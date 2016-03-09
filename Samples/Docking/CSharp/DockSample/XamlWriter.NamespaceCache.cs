using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Markup;
using System.Globalization;
using System.Reflection;

namespace DevZest.DockSample
{
    partial class XamlWriter
    {
        private class NamespaceCache
        {
            public const string XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";

            private string _defaultNamespace;
            private Dictionary<string, string> _prefixes = new Dictionary<string, string>();  // Key: namespace, Value: prefix
            private Dictionary<string, Dictionary<string, string>> _assemblyNamespaces = new Dictionary<string, Dictionary<string, string>>();

            private string DefaultNamespace
            {
                get { return _defaultNamespace; }
            }

            public string GetPrefixForNamespace(string ns)
            {
                string ret = string.Empty;
                if (ns != DefaultNamespace && _prefixes.ContainsKey(ns))
                    ret = _prefixes[ns];
                return ret;
            }

            public string GetXmlNamespace(Type type)
            {
                string uri = EnsureNamespaceMaps(type);
                if (_defaultNamespace == null)
                    _defaultNamespace = uri;

                if (!_prefixes.ContainsKey(uri))
                {
                    string prefix = "ns" + (_prefixes.Count + 1).ToString(CultureInfo.InvariantCulture);
                    _prefixes[uri] = prefix;
                }
                return uri;
            }

            private string EnsureNamespaceMaps(Type type)
            {
                string assemblyFullName = type.Assembly.FullName;
                Dictionary<string, string> namespaces;
                if (_assemblyNamespaces.ContainsKey(assemblyFullName))
                    namespaces = _assemblyNamespaces[assemblyFullName];
                else
                {
                    foreach (XmlnsPrefixAttribute prefixAttribute in type.Assembly.GetCustomAttributes(typeof(XmlnsPrefixAttribute), true))
                        _prefixes[prefixAttribute.XmlNamespace] = prefixAttribute.Prefix;

                    namespaces = new Dictionary<string, string>();
                    _assemblyNamespaces.Add(assemblyFullName, namespaces);
                    foreach (XmlnsDefinitionAttribute definitionAttribute in type.Assembly.GetCustomAttributes(typeof(XmlnsDefinitionAttribute), true))
                    {
                        if (definitionAttribute.AssemblyName == null)
                            namespaces[definitionAttribute.ClrNamespace] = definitionAttribute.XmlNamespace;
                    }
                }

                if (!namespaces.ContainsKey(type.Namespace))
                {
                    string uri = string.Format(CultureInfo.InvariantCulture, "clr-namespace:{0};assembly={1}", type.Namespace, GetAssemblyNameFromType(type));
                    namespaces.Add(type.Namespace, uri);
                }

                return namespaces[type.Namespace];
            }

            public static string GetAssemblyNameFromType(Type type)
            {
                string[] names = type.Assembly.FullName.Split(',');
                return names.Length > 0 ? names[0] : string.Empty;
            }
        }
    }
}
