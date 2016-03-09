using System;

namespace DevZest.DockSample
{
    partial class XamlWriter
    {
        private struct NamespaceMap
        {
            private string _prefix;
            private string _xmlNamespace;

            public NamespaceMap(string prefix, string xmlNamespace)
            {
                _prefix = prefix;
                _xmlNamespace = xmlNamespace;
            }

            public string Prefix
            {
                get { return _prefix; }
            }

            public string XmlNamespace
            {
                get { return _xmlNamespace; }
            }
        }
    }
}
