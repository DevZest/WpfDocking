Imports System
Imports System.ComponentModel
Imports System.Windows
Imports System.Windows.Controls
Imports DevZest.Windows.Docking

''' <summary>
''' Interaction logic for Document.xaml
''' </summary>
Partial Class Document
    Implements ICustomTypeDescriptor

    Private Shared s_propertyDescriptors As PropertyDescriptorCollection
    Public Shared DocumentIdProperty As DependencyProperty = DependencyProperty.Register("DocumentId", GetType(Integer), GetType(Document), New FrameworkPropertyMetadata(New PropertyChangedCallback(AddressOf DocumentId_Changed)))

    Private _undoRedoReference As DocumentUndoRedoReference

    Public Sub New()
        MyBase.New()
        InitializeComponent()
    End Sub

    Public Sub New(ByVal documentId As Integer)
        MyBase.New()
        InitializeComponent()
        Me.DocumentId = documentId
    End Sub

    Public Property DocumentId() As Integer
        Get
            Return CType(GetValue(DocumentIdProperty), Integer)
        End Get
        Set(ByVal value As Integer)
            SetValue(DocumentIdProperty, value)
        End Set
    End Property

    Protected Overrides ReadOnly Property UndoRedoReference() As IDockItemUndoRedoReference
        Get
            If (_undoRedoReference Is Nothing) Then
                _undoRedoReference = New DocumentUndoRedoReference(Me)
            End If
            Return _undoRedoReference
        End Get
    End Property

    Private Shared Sub DocumentId_Changed(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
        Dim document As Document = CType(d, Document)
        Dim documentId As Integer = CType(e.NewValue, Integer)
        document.OnDocumentIdChanged(documentId)
    End Sub

    Private Sub OnDocumentIdChanged(ByVal documentId As Integer)
        TabText = String.Format("Document{0}", documentId)
        Description = String.Format("Sample document{0}", documentId)
        Title = String.Format("Sample document{0}", documentId)
        paragraph.Inlines.Clear()
        paragraph.Inlines.Add(Title)
    End Sub

    Function ICustomTypeDescriptor_GetAttributes() As AttributeCollection Implements ICustomTypeDescriptor.GetAttributes
        Return New AttributeCollection(Nothing)
    End Function

    Function ICustomTypeDescriptor_GetClassName() As String Implements ICustomTypeDescriptor.GetClassName
        Return Nothing
    End Function

    Function ICustomTypeDescriptor_GetComponentName() As String Implements ICustomTypeDescriptor.GetComponentName
        Return Nothing
    End Function

    Function ICustomTypeDescriptor_GetConverter() As TypeConverter Implements ICustomTypeDescriptor.GetConverter
        Return Nothing
    End Function

    Function ICustomTypeDescriptor_GetDefaultEvent() As EventDescriptor Implements ICustomTypeDescriptor.GetDefaultEvent
        Return Nothing
    End Function

    Function ICustomTypeDescriptor_GetDefaultProperty() As PropertyDescriptor Implements ICustomTypeDescriptor.GetDefaultProperty
        Return Nothing
    End Function

    Function ICustomTypeDescriptor_GetEditor(ByVal editorBaseType As Type) As Object Implements ICustomTypeDescriptor.GetEditor
        Return Nothing
    End Function

    Function ICustomTypeDescriptor_GetEvents(ByVal attributes() As Attribute) As EventDescriptorCollection Implements ICustomTypeDescriptor.GetEvents
        Return New EventDescriptorCollection(Nothing)
    End Function

    Function ICustomTypeDescriptor_GetEvents() As EventDescriptorCollection Implements ICustomTypeDescriptor.GetEvents
        Return New EventDescriptorCollection(Nothing)
    End Function

    Function ICustomTypeDescriptor_GetProperties(ByVal attributes() As Attribute) As PropertyDescriptorCollection Implements ICustomTypeDescriptor.GetProperties
        If (s_propertyDescriptors Is Nothing) Then
            Dim properties(0) As PropertyDescriptor
            properties(0) = DependencyPropertyDescriptor.FromProperty(DocumentIdProperty, GetType(Document))
            s_propertyDescriptors = New PropertyDescriptorCollection(properties, True)
        End If
        Return s_propertyDescriptors
    End Function

    Function ICustomTypeDescriptor_GetProperties() As PropertyDescriptorCollection Implements ICustomTypeDescriptor.GetProperties
        Return CType(Me, ICustomTypeDescriptor).GetProperties(Nothing)
    End Function

    Function ICustomTypeDescriptor_GetPropertyOwner(ByVal pd As PropertyDescriptor) As Object Implements ICustomTypeDescriptor.GetPropertyOwner
        Return Me
    End Function

    Private Class DocumentUndoRedoReference
        Implements IDockItemUndoRedoReference

        Private _weakReference As WeakReference

        Private _documentId As Integer

        Public Sub New(ByVal document As Document)
            MyBase.New()
            _weakReference = New WeakReference(document)
            _documentId = document.DocumentId
        End Sub

        Public ReadOnly Property DockItem() As DockItem Implements IDockItemUndoRedoReference.DockItem
            Get
                If Not _weakReference.IsAlive Then
                    Dim document As Document = New Document(_documentId)
                    _weakReference.Target = document
                End If
                Return CType(_weakReference.Target, DockItem)
            End Get
        End Property
    End Class
End Class

