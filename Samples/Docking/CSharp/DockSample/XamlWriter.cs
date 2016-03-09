// Code from http://social.msdn.microsoft.com/forums/en-US/wpf/thread/08aebbf1-0a61-4305-83b2-a0a37bb24002/
// Because System.Windows.Markup.XamlWriter.Save requires full trust permission, we have to build our own XamlWriter in partial trust.
// Will be eliminated in the future if the System.Windows.Markup.XamlWriter.Save enabled in partial trust.
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Markup.Primitives;
using System.IO;
using System.Windows.Markup;
using System.Collections;
using System.Globalization;
using System.Runtime.Serialization;

namespace DevZest.DockSample
{
    internal partial class XamlWriter
    {
        private NamespaceCache _namespaceCache = new NamespaceCache();
        private Dictionary<string, NamespaceMap> _namespaceMaps = new Dictionary<string, NamespaceMap>();
        private Dictionary<Type, string> _contentPropertiesCache = new Dictionary<Type, string>();
        private FormatterConverter _formatterConverter = new FormatterConverter();

        public static string Save(object obj)
        {
            XamlWriter writer = new XamlWriter();
            return writer.WriteObject(obj);
        }

        private XamlWriter()
        {
        }

        private string WriteObject(object obj)
        {
            ResolveXmlNamespaces(obj);
            StringBuilder stringBuilder = new StringBuilder();
            using (StringWriter writer = new StringWriter(stringBuilder, CultureInfo.InvariantCulture))
            {
                using (XmlTextWriter xmlWriter = new XmlTextWriter(writer))
                {
                    xmlWriter.Formatting = Formatting.Indented;
                    xmlWriter.Indentation = 4;
                    xmlWriter.Namespaces = false;
                    WriteObject(null, obj, xmlWriter, true);
                    xmlWriter.Flush();
                    xmlWriter.Close();
                }
            }
            return stringBuilder.ToString();
        }

        private void WriteObject(object key, object obj, XmlTextWriter writer, bool isRoot)
        {
            MarkupObject markupObj = MarkupWriter.GetMarkupObjectFor(obj);
            Type objectType = markupObj.ObjectType;

            _namespaceCache.GetXmlNamespace(objectType);
            string ns = _namespaceCache.GetXmlNamespace(objectType);
            string prefix = _namespaceCache.GetPrefixForNamespace(ns);

            WriteStartElement(writer, prefix, markupObj.ObjectType.Name);

            if (isRoot)
            {
                foreach (NamespaceMap map in _namespaceMaps.Values)
                {
                    if (string.IsNullOrEmpty(map.Prefix))
                        writer.WriteAttributeString("xmlns", map.XmlNamespace);
                    else
                        writer.WriteAttributeString("xmlns:" + map.Prefix, map.XmlNamespace);
                }

                if (!_namespaceMaps.ContainsKey(NamespaceCache.XamlNamespace))
                    writer.WriteAttributeString("xmlns:x", NamespaceCache.XamlNamespace);
            }

            if (key != null)
            {
                string keyString = key.ToString();
                if (keyString.Length > 0)
                    writer.WriteAttributeString("x:Key", keyString);
                else
                    //TODO: key may not be a string, what about x:Type...
                    throw new NotImplementedException();
            }

            List<MarkupProperty> propertyElements = new List<MarkupProperty>();
            MarkupProperty contentProperty = null;
            string contentString = string.Empty;
            foreach (MarkupProperty markupProperty in markupObj.Properties)
            {
                if (IsContentProperty(markupObj, markupProperty))
                {
                    contentProperty = markupProperty;
                    continue;
                }

                if (markupProperty.IsValueAsString)
                    contentString = markupProperty.Value as string;
                else if (!markupProperty.IsComposite)
                {
                    string temp = markupProperty.Value == null ? string.Empty : _formatterConverter.ToString(markupProperty.Value);

                    if (markupProperty.IsAttached)
                    {
                        string ns1 = _namespaceCache.GetXmlNamespace(markupProperty.DependencyProperty.OwnerType);
                        string prefix1 = _namespaceCache.GetPrefixForNamespace(ns1);
                        if (string.IsNullOrEmpty(prefix1))
                            writer.WriteAttributeString(markupProperty.Name, temp);
                        else
                            writer.WriteAttributeString(prefix1 + ":" + markupProperty.Name, temp);
                    }
                    else
                    {
                        if (markupProperty.Name == "Name" && NamespaceCache.GetAssemblyNameFromType(markupProperty.DependencyProperty.OwnerType).Equals("PresentationFramework"))
                            writer.WriteAttributeString("x:" + markupProperty.Name, temp);
                        else
                            writer.WriteAttributeString(markupProperty.Name, temp);
                    }
                }
                else if (markupProperty.Value.GetType() == typeof(NullExtension))
                    writer.WriteAttributeString(markupProperty.Name, "{x:Null}");
                else
                    propertyElements.Add(markupProperty);
            }

            if (contentProperty != null || propertyElements.Count > 0 || !string.IsNullOrEmpty(contentString))
            {
                foreach (MarkupProperty markupProp in propertyElements)
                {
                    string ns2 = _namespaceCache.GetXmlNamespace(markupObj.ObjectType);
                    string prefix2 = _namespaceCache.GetPrefixForNamespace(ns2);
                    string propElementName = markupObj.ObjectType.Name + "." + markupProp.Name;

                    WriteStartElement(writer, prefix2, propElementName);
                    WriteChildren(writer, markupProp);
                    writer.WriteEndElement();
                }

                if (!string.IsNullOrEmpty(contentString))
                    writer.WriteValue(contentString);
                else if (contentProperty != null)
                {
                    if (contentProperty.Value is string)
                        writer.WriteValue(contentProperty.StringValue);
                    else
                        WriteChildren(writer, contentProperty);
                }
            }

            writer.WriteEndElement();
        }

        private static void WriteStartElement(XmlWriter writer, string prefix, string name)
        {
            if (string.IsNullOrEmpty(prefix))
                writer.WriteStartElement(name);
            else
                writer.WriteStartElement(prefix + ":" + name);
        }

        private void WriteChildren(XmlTextWriter writer, MarkupProperty markupProp)
        {
            if (!markupProp.IsComposite)
                WriteObject(null, markupProp.Value, writer, false);
            else
            {
                IList collection = markupProp.Value as IList;
                IDictionary dictionary = markupProp.Value as IDictionary;
                if (collection != null)
                {
                    foreach (object o in collection)
                        WriteObject(null, o, writer, false);
                }
                else if (dictionary != null)
                {
                    foreach (object key in dictionary.Keys)
                        WriteObject(key, dictionary[key], writer, false);
                }
                else
                    WriteObject(null, markupProp.Value, writer, false);
            }
        }

        private void ResolveXmlNamespaces(object obj)
        {
            MarkupObject markupObj = MarkupWriter.GetMarkupObjectFor(obj);

            string ns = _namespaceCache.GetXmlNamespace(markupObj.ObjectType);
            string prefix = _namespaceCache.GetPrefixForNamespace(ns);
            _namespaceMaps[ns] = new NamespaceMap(prefix, ns);

            foreach (MarkupProperty markupProperty in markupObj.Properties)
            {
                if (IsContentProperty(markupObj, markupProperty))
                {
                    if (!(markupProperty.Value is String))
                        ResolveChildXmlNamespaces(markupProperty);
                    continue;
                }

                if (markupProperty.Value.GetType() == typeof(NullExtension) || markupProperty.IsValueAsString)
                    continue;

                if (!markupProperty.IsComposite)
                {
                    if (markupProperty.DependencyProperty != null)
                    {
                        string ns1 = _namespaceCache.GetXmlNamespace(markupProperty.DependencyProperty.OwnerType);
                        string prefix1 = _namespaceCache.GetPrefixForNamespace(ns1);
                        if (!string.IsNullOrEmpty(prefix1))
                            _namespaceMaps[ns1] = new NamespaceMap(prefix1, ns1);
                    }
                }
                else
                {
                    string ns2 = _namespaceCache.GetXmlNamespace(markupObj.ObjectType);
                    string prefix2 = _namespaceCache.GetPrefixForNamespace(ns2);
                    _namespaceMaps[ns2] = new NamespaceMap(prefix2, ns2);
                    ResolveChildXmlNamespaces(markupProperty);
                }
            }
        }

        private bool IsContentProperty(MarkupObject markupObj, MarkupProperty markupProperty)
        {
            return markupProperty.Name == GetContentPropertyName(markupObj);
        }

        private string GetContentPropertyName(MarkupObject markupObj)
        {
            Type objectType = markupObj.ObjectType;

            if (!_contentPropertiesCache.ContainsKey(objectType))
            {
                string lookedUpContentProperty = string.Empty;

                foreach (Attribute attr in markupObj.Attributes)
                {
                    ContentPropertyAttribute cpa = attr as ContentPropertyAttribute;
                    if (cpa != null)
                    {
                        lookedUpContentProperty = cpa.Name;
                        break;  //Once content property is found, come out of the loop.
                    }
                }

                _contentPropertiesCache.Add(objectType, lookedUpContentProperty);
            }

            return _contentPropertiesCache[objectType];
        }

        private void ResolveChildXmlNamespaces(MarkupProperty markupProp)
        {
            if (!markupProp.IsComposite)
                ResolveXmlNamespaces(markupProp);
            else
            {
                IList collection = markupProp.Value as IList;
                IDictionary dictionary = markupProp.Value as IDictionary;

                if (collection != null)
                {
                    foreach (object o in collection)
                        ResolveXmlNamespaces(o);
                }
                else if (dictionary != null)
                {
                    foreach (object key in dictionary.Keys)
                        ResolveXmlNamespaces(dictionary[key]);
                }
                else
                    ResolveXmlNamespaces(markupProp.Value);
            }
        }
    }
}
