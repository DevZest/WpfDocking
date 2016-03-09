Imports System
Imports System.Windows.Navigation

Class Application

    ' Application events, such as Startup(), Exit(), and DispatcherUnhandledException
    ' can be handled in this file

    Private Sub Application_Navigating(ByVal sender As Object, ByVal e As NavigatingCancelEventArgs)
        If (e.NavigationMode = NavigationMode.Refresh) Then
            e.Cancel = True
        End If
    End Sub
End Class
