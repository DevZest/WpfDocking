Imports System
Imports System.Security
Imports System.Security.Permissions
Imports System.IO
Imports System.Xml
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Markup
Imports DevZest.Windows.Docking

Namespace DevZest.DockSample
    Partial Public Class SavedLayout

        Friend Overloads Sub Save(ByVal dockControl As DockControl)
            Dim layout As DockLayout = dockControl.Save

            'Dim settings As New XmlWriterSettings()
            'settings.Indent = True
            'settings.IndentChars = New String(" "c, 4)
            'Dim strbuild As New StringBuilder()
            'Dim xmlWriter As XmlWriter = xmlWriter.Create(strbuild, settings)
            'System.Windows.Markup.XamlWriter.Save(layout, xmlWriter)
            'Console.WriteLine(strbuild.ToString())

            textBox.Text = XamlWriter.Save(layout)
            Show(dockControl)
        End Sub

        Friend Sub Load(ByVal dockControl As DockControl, ByVal deserializeDockItem As Func(Of Object, DockItem))
            Dim layout As DockLayout = CType(XamlReader.Load(New XmlTextReader(New StringReader(textBox.Text))), DockLayout)
            dockControl.Load(layout, deserializeDockItem)
        End Sub

    End Class
End Namespace
