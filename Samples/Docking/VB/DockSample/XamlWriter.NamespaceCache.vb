Imports System
Imports System.Diagnostics
Imports System.Collections.Generic
Imports System.Windows.Markup
Imports System.Globalization
Imports System.Reflection

Partial Class XamlWriter

    Private Class NamespaceCache

        Public Const XamlNamespace As String = "http://schemas.microsoft.com/winfx/2006/xaml"

        Private _defaultNamespace As String
        Private _prefixes As Dictionary(Of String, String) = New Dictionary(Of String, String)  ' Key: namespace, Value: prefix
        Private _assemblyNamespaces As Dictionary(Of String, Dictionary(Of String, String)) = New Dictionary(Of String, Dictionary(Of String, String))

        Private ReadOnly Property DefaultNamespace() As String
            Get
                Return _defaultNamespace
            End Get
        End Property

        Public Function GetPrefixForNamespace(ByVal ns As String) As String
            Dim ret As String = String.Empty
            If ns <> DefaultNamespace AndAlso _prefixes.ContainsKey(ns) Then
                ret = _prefixes(ns)
            End If
            Return ret
        End Function

        Public Function GetNamespace(ByVal type As Type) As String
            Dim uri As String = EnsureNamespaceMaps(type)
            If (_defaultNamespace Is Nothing) Then
                _defaultNamespace = uri
            End If
            If Not _prefixes.ContainsKey(uri) Then
                Dim prefix As String = ("ns" + (_prefixes.Count + 1).ToString(CultureInfo.InvariantCulture))
                _prefixes(uri) = prefix
            End If
            Return uri
        End Function

        Private Function EnsureNamespaceMaps(ByVal type As Type) As String
            Dim assemblyFullName As String = type.Assembly.FullName
            Dim namespaces As Dictionary(Of String, String)
            If _assemblyNamespaces.ContainsKey(assemblyFullName) Then
                namespaces = _assemblyNamespaces(assemblyFullName)
            Else
                For Each prefixAttribute As XmlnsPrefixAttribute In type.Assembly.GetCustomAttributes(GetType(XmlnsPrefixAttribute), True)
                    _prefixes(prefixAttribute.XmlNamespace) = prefixAttribute.Prefix
                Next
                namespaces = New Dictionary(Of String, String)
                _assemblyNamespaces.Add(assemblyFullName, namespaces)
                For Each definitionAttribute As XmlnsDefinitionAttribute In type.Assembly.GetCustomAttributes(GetType(XmlnsDefinitionAttribute), True)
                    If (definitionAttribute.AssemblyName Is Nothing) Then
                        namespaces(definitionAttribute.ClrNamespace) = definitionAttribute.XmlNamespace
                    End If
                Next
            End If
            If Not namespaces.ContainsKey(type.Namespace) Then
                Dim uri As String = String.Format(CultureInfo.InvariantCulture, "clr-namespace:{0};assembly={1}", type.Namespace, GetAssemblyNameFromType(type))
                namespaces.Add(type.Namespace, uri)
            End If
            Return namespaces(type.Namespace)
        End Function

        Public Shared Function GetAssemblyNameFromType(ByVal type As Type) As String
            Dim names() As String = type.Assembly.FullName.Split(",")
            If names.Length > 0 Then
                Return names(0)
            Else
                Return String.Empty
            End If
        End Function
    End Class
End Class

