Imports System
Imports System.Diagnostics
Imports System.Collections.Generic
Imports System.Reflection
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports DevZest.Windows.Docking

Namespace DevZest.DockSample

    ''' <summary>
    ''' Interaction logic for MainPage.xaml
    ''' </summary>
    Partial Class MainPage

        Private Shared IsLayoutSavedPropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly("IsLayoutSaved", GetType(System.Boolean), GetType(MainPage), New FrameworkPropertyMetadata(False))

        Public Shared IsLayoutSavedProperty As DependencyProperty = IsLayoutSavedPropertyKey.DependencyProperty

        Public Property IsLayoutSaved As Boolean
            Get
                Return CType(GetValue(IsLayoutSavedProperty), Boolean)
            End Get
            Set(value As Boolean)
                SetValue(IsLayoutSavedPropertyKey, value)
            End Set
        End Property

        Private _lastDocumentId As Integer

        Public Sub New()
            MyBase.New()
            InitializeComponent()
            Dim sb As StringBuilder = New StringBuilder
            sb.AppendLine("***** Welcome to DevZest WPF docking! *****")
            sb.AppendLine(String.Format("===== CLR Version: {0}, Loaded Assemblies:", Environment.Version))
            For Each assembly As Assembly In AppDomain.CurrentDomain.GetAssemblies
                sb.AppendLine(assembly.FullName)
            Next
            Output.AppendLog(sb.ToString)
        End Sub

        Private Sub NewDocument(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
            _lastDocumentId = _lastDocumentId + 1
            Dim document As Document = New Document(_lastDocumentId)
            document.Show(dockControl)
        End Sub

        Private Sub CanExecuteCloseActiveDocument(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
            e.CanExecute = (Not (DockControl.ActiveDocument) Is Nothing)
        End Sub

        Private Sub CloseActiveDocument(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
            DockControl.ActiveDocument.PerformClose()
        End Sub

        Private Sub OnFocusedItemChanged(ByVal sender As Object, ByVal e As EventArgs)
            Output.AppendLog(String.Format("FocusedItemChanged: FocusedItem={0}", GetString(DockControl.FocusedItem)))
        End Sub

        Private Sub OnActiveItemChanged(ByVal sender As Object, ByVal e As EventArgs)
            Output.AppendLog(String.Format("ActiveItemChanged: ActiveItem={0}", GetString(DockControl.ActiveItem)))
        End Sub

        Private Sub OnActiveDocumentChanged(ByVal sender As Object, ByVal e As EventArgs)
            Output.AppendLog(String.Format("ActiveDocumentChanged: ActiveDocument={0}", GetString(DockControl.ActiveDocument)))
        End Sub

        Private Sub OnDockItemStateChanging(ByVal sender As Object, ByVal e As DockItemStateEventArgs)
            Output.AppendLog(String.Format("DockItemStateChanging: {0}", GetString(e)))
        End Sub

        Private Sub OnDockItemStateChanged(ByVal sender As Object, ByVal e As DockItemStateEventArgs)
            Output.AppendLog(String.Format("DockItemStateChanged: {0}", GetString(e)))
        End Sub

        Private Overloads Function GetString(ByVal item As DockItem) As String
            If (item Is Nothing) Then
                Return "null"
            Else
                Return item.TabText
            End If
        End Function

        Private Overloads Function GetString(ByVal e As DockItemStateEventArgs) As String
            If (e.OldDockPosition = e.NewDockPosition) Then
                Return String.Format("DockItem={0}, StateChangeMethod={1}, ShowMethod={2}", GetString(e.DockItem), e.StateChangeMethod, e.ShowMethod)
            Else
                Return String.Format("DockItem={0}, StateChangeMethod={1}, DockPosition={2}->{3}, ShowMethod={4}", GetString(e.DockItem), e.StateChangeMethod, e.OldDockPosition, e.NewDockPosition, e.ShowMethod)
            End If
        End Function

        Private Function LoadDockItem(ByVal obj As Object) As DockItem
            If Welcome.GetType.ToString.Equals(obj) Then
                Return Me.welcome
            ElseIf SavedLayout.GetType.ToString.Equals(obj) Then
                Return Me.savedLayout
            ElseIf Output.GetType.ToString.Equals(obj) Then
                Return Me.output
            ElseIf SolutionExplorer.GetType.ToString.Equals(obj) Then
                Return Me.solutionExplorer
            ElseIf PropertiesWindow.GetType.ToString.Equals(obj) Then
                Return Me.propertiesWindow
            Else
                Return CType(obj, DockItem)
            End If
        End Function

        Private Sub SaveLayout_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
            SavedLayout.Save(DockControl)
            IsLayoutSaved = True
        End Sub

        Private Sub LoadLayout_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
            CloseAll()
            savedLayout.Load(dockControl, AddressOf LoadDockItem)
        End Sub

        Private Sub CloseAll()
            Dim i As Integer = (DockControl.DockItems.Count - 1)
            Do While (i >= 0)
                Dim item As DockItem
                item = dockControl.DockItems(i)
                item.Close()
                i = (i - 1)
            Loop
        End Sub

        Private Sub ChangeTheme(ByVal themeName As String)
            If String.IsNullOrEmpty(themeName) Then
                DockSample.Themes.Reset()
                DevZest.Windows.Docking.Themes.Reset()
            Else
                DockSample.Themes.Load(themeName)
                DevZest.Windows.Docking.Themes.Load(themeName)
            End If
            _defaultTheme.IsChecked = String.IsNullOrEmpty(themeName)
            _expressionDark.IsChecked = (themeName = "ExpressionDark")
            _vs2010.IsChecked = (themeName = "VS2010")
        End Sub

        Private Sub Theme_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Dim menuItem As MenuItem = CType(sender, MenuItem)
            Dim theme As String = CType(menuItem.CommandParameter, String)
            ChangeTheme(theme)
        End Sub
    End Class
End Namespace