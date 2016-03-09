Imports System
Imports System.Diagnostics
Imports System.Reflection
Imports System.Windows

Namespace DevZest.DockSample

    Public Class Themes

        Private Shared s_currentTheme As ResourceDictionary = Nothing

        Public Overloads Shared Sub Load(ByVal themeName As String)
            Dim themeSource As Uri = New Uri(("/" _
                            + (Assembly.GetExecutingAssembly.FullName.Split(","c)(0) + (";component/themes/" _
                            + (themeName + ".xaml")))), UriKind.RelativeOrAbsolute)
            Load(themeSource)
        End Sub

        Public Overloads Shared Sub Load(ByVal themeSource As Uri)
            Reset()
            Debug.Assert((s_currentTheme Is Nothing))
            s_currentTheme = New ResourceDictionary
            s_currentTheme.Source = themeSource
            Application.Current.Resources.MergedDictionaries.Add(s_currentTheme)
        End Sub

        Public Shared Sub Reset()
            If (Not (s_currentTheme) Is Nothing) Then
                Application.Current.Resources.MergedDictionaries.Remove(s_currentTheme)
                s_currentTheme = Nothing
            End If
        End Sub
    End Class
End Namespace
