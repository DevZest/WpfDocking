Imports System
Imports System.Windows
Imports System.Windows.Controls
Imports DevZest.Windows.Docking

Namespace DevZest.DockSample
    Partial Public Class Output

        Public Sub AppendLog(ByVal text As String)
            textBox.AppendText(String.Format("{0}: {1}", DateTime.Now.ToLongTimeString, (text + Environment.NewLine)))
            textBox.ScrollToEnd()
        End Sub

        Private Sub OnClearAllClick(ByVal sender As Object, ByVal e As RoutedEventArgs)
            textBox.Text = Nothing
        End Sub
    End Class
End Namespace
