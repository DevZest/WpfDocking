Imports System

Partial Class XamlWriter

    Private Structure NamespaceMap

        Private _prefix As String
        Private _xmlNamespace As String

        Public Sub New(ByVal prefix As String, ByVal xmlNamespace As String)
            _prefix = prefix
            _xmlNamespace = xmlNamespace
        End Sub

        Public ReadOnly Property Prefix() As String
            Get
                Return _prefix
            End Get
        End Property

        Public ReadOnly Property XmlNamespace() As String
            Get
                Return _xmlNamespace
            End Get
        End Property
    End Structure
End Class
