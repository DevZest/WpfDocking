' Code from http://social.msdn.microsoft.com/forums/en-US/wpf/thread/08aebbf1-0a61-4305-83b2-a0a37bb24002/
' Because System.Windows.Markup.XamlWriter.Save requires full trust permission, we have to build our own XamlWriter in partial trust.
' Will be eliminated in the future if the System.Windows.Markup.XamlWriter.Save enabled in partial trust.
Imports System
Imports System.Diagnostics
Imports System.Collections.Generic
Imports System.Text
Imports System.Xml
Imports System.Windows.Markup.Primitives
Imports System.IO
Imports System.Windows.Markup
Imports System.Collections
Imports System.Globalization
Imports System.Runtime.Serialization

Partial Class XamlWriter

    Private _namespaceCache As NamespaceCache = New NamespaceCache
    Private _namespaceMaps As Dictionary(Of String, NamespaceMap) = New Dictionary(Of String, NamespaceMap)
    Private _contentPropertiesCache As Dictionary(Of Type, String) = New Dictionary(Of Type, String)
    Private _formatterConverter As FormatterConverter = New FormatterConverter()

    Private Sub New()
        MyBase.New()
    End Sub

    Public Shared Function Save(ByVal obj As Object) As String
        Dim writer As XamlWriter = New XamlWriter
        Return writer.WriteObject(obj)
    End Function

    Private Overloads Function WriteObject(ByVal obj As Object) As String
        ResolveXmlNamespaces(obj)
        Dim stringBuilder As StringBuilder = New StringBuilder
        Using writer As New StringWriter(stringBuilder, CultureInfo.InvariantCulture)
            Using xmlWriter As New XmlTextWriter(writer)
                xmlWriter.Formatting = Formatting.Indented
                xmlWriter.Indentation = 4
                xmlWriter.Namespaces = False
                WriteObject(Nothing, obj, xmlWriter, True)
                xmlWriter.Flush()
                xmlWriter.Close()
            End Using
        End Using
        Return stringBuilder.ToString
    End Function

    Private Overloads Sub WriteObject(ByVal key As Object, ByVal obj As Object, ByVal writer As XmlTextWriter, ByVal isRoot As Boolean)
        Dim markupObj As MarkupObject = MarkupWriter.GetMarkupObjectFor(obj)
        Dim objectType As Type = markupObj.ObjectType

        _namespaceCache.GetNamespace(objectType)
        Dim ns As String = _namespaceCache.GetNamespace(objectType)
        Dim prefix As String = _namespaceCache.GetPrefixForNamespace(ns)

        WriteStartElement(writer, prefix, markupObj.ObjectType.Name)

        If isRoot Then
            For Each map As NamespaceMap In _namespaceMaps.Values
                If String.IsNullOrEmpty(map.Prefix) Then
                    writer.WriteAttributeString("xmlns", map.XmlNamespace)
                Else
                    writer.WriteAttributeString(("xmlns:" + map.Prefix), map.XmlNamespace)
                End If
            Next

            If Not _namespaceMaps.ContainsKey(NamespaceCache.XamlNamespace) Then
                writer.WriteAttributeString("xmlns:x", NamespaceCache.XamlNamespace)
            End If
        End If

        If (Not (key) Is Nothing) Then
            Dim keyString As String = key.ToString
            If (keyString.Length > 0) Then
                writer.WriteAttributeString("x:Key", keyString)
            Else
                'TODO: key may not be a string, what about x:Type...
                Throw New NotImplementedException
            End If
        End If

        Dim propertyElements As List(Of MarkupProperty) = New List(Of MarkupProperty)
        Dim contentProperty As MarkupProperty = Nothing
        Dim contentString As String = String.Empty
        For Each markupProperty As MarkupProperty In markupObj.Properties
            If IsContentProperty(markupObj, markupProperty) Then
                contentProperty = markupProperty
                Continue For
            End If

            If markupProperty.IsValueAsString Then
                contentString = TryCast(markupProperty.Value, String)
            ElseIf Not markupProperty.IsComposite Then
                Dim temp As String
                If markupProperty.Value Is Nothing Then
                    temp = String.Empty
                Else
                    temp = _formatterConverter.ToString(markupProperty.Value)
                End If

                If markupProperty.IsAttached Then
                    Dim ns1 As String = _namespaceCache.GetNamespace(markupProperty.DependencyProperty.OwnerType)
                    Dim prefix1 As String = _namespaceCache.GetPrefixForNamespace(ns1)
                    If String.IsNullOrEmpty(prefix1) Then
                        writer.WriteAttributeString(markupProperty.Name, temp)
                    Else
                        writer.WriteAttributeString((prefix1 + (":" + markupProperty.Name)), temp)
                    End If
                Else
                    If (markupProperty.Name = "Name") AndAlso (NamespaceCache.GetAssemblyNameFromType(markupProperty.DependencyProperty.OwnerType).Equals("PresentationFramework")) Then
                        writer.WriteAttributeString(("x:" + markupProperty.Name), temp)
                    Else
                        writer.WriteAttributeString(markupProperty.Name, temp)
                    End If
                End If
            ElseIf (markupProperty.Value.GetType().Equals(GetType(NullExtension))) Then
                writer.WriteAttributeString(markupProperty.Name, "{x:Null}")
            Else
                propertyElements.Add(markupProperty)
            End If
        Next

        If (Not contentProperty Is Nothing) OrElse (propertyElements.Count > 0) OrElse (Not String.IsNullOrEmpty(contentString)) Then
            For Each markupProp As MarkupProperty In propertyElements
                Dim ns2 As String = _namespaceCache.GetNamespace(markupObj.ObjectType)
                Dim prefix2 As String = _namespaceCache.GetPrefixForNamespace(ns2)
                Dim propElementName As String = (markupObj.ObjectType.Name + ("." + markupProp.Name))

                WriteStartElement(writer, prefix2, propElementName)
                WriteChildren(writer, markupProp)
                writer.WriteEndElement()
            Next

            If Not String.IsNullOrEmpty(contentString) Then
                writer.WriteValue(contentString)
            ElseIf (Not contentProperty Is Nothing) Then
                If (TypeOf contentProperty.Value Is String) Then
                    writer.WriteValue(contentProperty.StringValue)
                Else
                    WriteChildren(writer, contentProperty)
                End If
            End If
        End If

        writer.WriteEndElement()
    End Sub

    Private Shared Sub WriteStartElement(ByVal writer As XmlWriter, ByVal prefix As String, ByVal name As String)
        If String.IsNullOrEmpty(prefix) Then
            writer.WriteStartElement(name)
        Else
            writer.WriteStartElement((prefix + (":" + name)))
        End If
    End Sub

    Private Sub WriteChildren(ByVal writer As XmlTextWriter, ByVal markupProp As MarkupProperty)
        If Not markupProp.IsComposite Then
            WriteObject(Nothing, markupProp.Value, writer, False)
        Else
            Dim collection As IList = TryCast(markupProp.Value, IList)
            Dim dictionary As IDictionary = TryCast(markupProp.Value, IDictionary)

            If (Not collection Is Nothing) Then
                For Each o As Object In collection
                    WriteObject(Nothing, o, writer, False)
                Next
            ElseIf (Not dictionary Is Nothing) Then
                For Each key As Object In dictionary.Keys
                    WriteObject(key, dictionary(key), writer, False)
                Next
            Else
                WriteObject(Nothing, markupProp.Value, writer, False)
            End If
        End If
    End Sub

    Private Sub ResolveXmlNamespaces(ByVal obj As Object)
        Dim markupObj As MarkupObject = MarkupWriter.GetMarkupObjectFor(obj)

        Dim ns As String = _namespaceCache.GetNamespace(markupObj.ObjectType)
        Dim prefix As String = _namespaceCache.GetPrefixForNamespace(ns)
        _namespaceMaps(ns) = New NamespaceMap(prefix, ns)

        For Each markupProperty As MarkupProperty In markupObj.Properties
            If IsContentProperty(markupObj, markupProperty) Then
                If Not (TypeOf markupProperty.Value Is String) Then
                    ResolveChildXmlNamespaces(markupProperty)
                    Continue For
                End If
            End If

            If markupProperty.Value.GetType.Equals(GetType(NullExtension)) OrElse markupProperty.IsValueAsString Then
                Continue For
            End If

            If Not markupProperty.IsComposite Then
                If (Not markupProperty.DependencyProperty Is Nothing) Then
                    Dim ns1 As String = _namespaceCache.GetNamespace(markupProperty.DependencyProperty.OwnerType)
                    Dim prefix1 As String = _namespaceCache.GetPrefixForNamespace(ns1)
                    If Not String.IsNullOrEmpty(prefix1) Then
                        _namespaceMaps(ns1) = New NamespaceMap(prefix1, ns1)
                    End If
                End If
            Else
                Dim ns2 As String = _namespaceCache.GetNamespace(markupObj.ObjectType)
                Dim prefix2 As String = _namespaceCache.GetPrefixForNamespace(ns2)
                _namespaceMaps(ns2) = New NamespaceMap(prefix2, ns2)
                ResolveChildXmlNamespaces(markupProperty)
            End If
        Next
    End Sub

    Private Function IsContentProperty(ByVal markupObj As MarkupObject, ByVal markupProperty As MarkupProperty) As Boolean
        Return (markupProperty.Name = GetContentPropertyName(markupObj))
    End Function

    Private Function GetContentPropertyName(ByVal markupObj As MarkupObject) As String
        Dim objectType As Type = markupObj.ObjectType
        If Not _contentPropertiesCache.ContainsKey(objectType) Then
            Dim lookedUpContentProperty As String = String.Empty
            For Each attr As Attribute In markupObj.Attributes
                Dim cpa As ContentPropertyAttribute = TryCast(attr, ContentPropertyAttribute)
                If (Not cpa Is Nothing) Then
                    lookedUpContentProperty = cpa.Name
                    Exit For   'Once content property is found, come out of the loop.
                End If
            Next
            _contentPropertiesCache.Add(objectType, lookedUpContentProperty)
        End If
        Return _contentPropertiesCache(objectType)
    End Function

    Private Sub ResolveChildXmlNamespaces(ByVal markupProp As MarkupProperty)
        If Not markupProp.IsComposite Then
            ResolveXmlNamespaces(markupProp)
        Else
            Dim collection As IList = TryCast(markupProp.Value, IList)
            Dim dictionary As IDictionary = TryCast(markupProp.Value, IDictionary)

            If (Not collection Is Nothing) Then
                For Each o As Object In collection
                    ResolveXmlNamespaces(o)
                Next
            ElseIf (Not dictionary Is Nothing) Then
                For Each key As Object In dictionary.Keys
                    ResolveXmlNamespaces(dictionary(key))
                Next
            Else
                ResolveXmlNamespaces(markupProp.Value)
            End If
        End If
    End Sub
End Class

