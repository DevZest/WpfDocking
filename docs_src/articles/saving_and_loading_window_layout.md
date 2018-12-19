---
uid: saving_and_loading_window_layout
---

# Saving and Loading Window Layout

You can save the window layout as @DevZest.Windows.Docking.DockLayout object, then load the layout at a later time.

## Saving Window Layout as DockLayout Object

Simply call [DockControl.Save](xref:DevZest.Windows.Docking.DockControl.Save*) method to save the current window layout as a @DevZest.Windows.Docking.DockLayout object. This @DevZest.Windows.Docking.DockLayout object, can be further persisted as XAML string:

```csharp
DockLayout layout = dockControl.Save();
XmlWriterSettings settings = new XmlWriterSettings();
settings.Indent = true;
settings.IndentChars = new string(' ', 4);
StringBuilder strbuild = new StringBuilder();
XmlWriter xmlWriter = XmlWriter.Create(strbuild, settings);
XamlWriter.Save(layout, xmlWriter);
Console.WriteLine(strbuild.ToString());
```

```vb
Dim layout As DockLayout = dockControl.Save() 
Dim settings As New XmlWriterSettings() 
settings.Indent = True 
settings.IndentChars = New String(" "c, 4) 
Dim strbuild As New StringBuilder() 
Dim xmlWriter As XmlWriter = XmlWriter.Create(strbuild, settings) 
XamlWriter.Save(layout, xmlWriter) 
Console.WriteLine(strbuild.ToString())
```

>[!Note]
>The XamlWriter.Save method requires full trust. If you want to save the XAML in partial trust, you must implement your own XAML writer, as demonstrated in DockSample.

A sample of persisted XAML string:

```xaml
<DockLayout xmlns="http://schemas.devzest.com/presentation/docking" xmlns:ns2="clr-namespace:DevZest.DockSample;assembly=DockSample" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" DockTreeZOrder="Right, Left, Top, Bottom">
    <DockLayout.DockItems>
        <DockItemReference>DevZest.DockSample.Output</DockItemReference>
        <DockItemReference>DevZest.DockSample.SavedState</DockItemReference>
        <DockItemReference>DevZest.DockSample.PropertiesWindow</DockItemReference>
        <DockItemReference>DevZest.DockSample.SolutionExplorer</DockItemReference>
        <DockItemReference>DevZest.DockSample.Welcome</DockItemReference>
        <DockItemReference>
            <ns2:Document DocumentId="1" />
        </DockItemReference>
        <DockItemReference>
            <ns2:Document DocumentId="2" />
        </DockItemReference>
    </DockLayout.DockItems>
    <DockLayout.ShowActions>
        <ShowAsDockPositionAction DockPosition="BottomAutoHide" Source="1" />
        <ShowAsSidePaneAction Side="Left" Size="*" IsSizeForTarget="True" Target="1" Source="0" />
        <ShowAsDockPositionAction DockPosition="Right" Source="2" />
        <ShowAsTabbedAction Target="2" Source="3" />
        <ShowAsDockPositionAction DockPosition="Document" Source="4" />
        <ShowAsTabbedAction Target="4" Source="5" />
        <ShowAsTabbedAction Target="4" Source="6" />
    </DockLayout.ShowActions>
</DockLayout>
```

>[!IMPORTANT]
>When saving the window layout, an instance of @DevZest.Windows.Docking.DockItemReference is created for each DockItem, with its [Target](xref:DevZest.Windows.Docking.DockItemReference.Target) property set to the object instance returned by [DockItem.Save](xref:DevZest.Windows.Docking.DockItem.Save*) method. By default[DockItem.Save](xref:DevZest.Windows.Docking.DockItem.Save*) method returns the string of the type when [HideOnPerformClose](xref:DevZest.Windows.Docking.DockItem.HideOnPerformClose) property is true, otherwise returns this DockItem itself. You may override [DockItem.Save](xref:DevZest.Windows.Docking.DockItem.Save*) to provide your own implementation. When loading the window layout, you need to provide a callback delegate to convert this saved object back into a @DevZest.Windows.Docking.DockItem.

## Loading Window Layout from DockLayout Object

You can call [DockControl.Load](xref:DevZest.Windows.Docking.DockControl.Load*) method to load the previous saved window layout. This method, takes two parameters, one is the previously saved @DevZest.Windows.Docking.DockLayout, the other is a callback delegate to convert the object instance as [DockItemReference.Target](xref:DevZest.Windows.Docking.DockItemReference.Target) to @DevZest.Windows.Docking.DockItem:

```csharp
private void LoadLayout_Click(object sender, RoutedEventArgs e)
{
    ...
    CloseAll();
    dockControl.Load(layout, LoadDockItem);
    ...
}

private void CloseAll()
{
    for (int i = dockControl.DockItems.Count - 1; i >= 0; i--)
    {
        DockItem item = dockControl.DockItems[i];
        item.Close();
    }
}

private DockItem LoadDockItem(object obj)
{
    if (welcome.GetType().ToString().Equals(obj))
        return welcome;
    else if (savedLayout.GetType().ToString().Equals(obj))
        return savedLayout;
    else if (output.GetType().ToString().Equals(obj))
        return output;
    else if (solutionExplorer.GetType().ToString().Equals(obj))
        return solutionExplorer;
    else if (propertiesWindow.GetType().ToString().Equals(obj))
        return propertiesWindow;
    else
        return obj as DockItem;
}
```

```vb
Private Sub LoadLayout_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
    ...
    CloseAll()
    dockControl.Load(layout, AddressOf LoadDockItem)
    ...
End Sub

Private Sub CloseAll()
    Dim i As Integer = (dockControl.DockItems.Count - 1)
    Do While (i >= 0)
        Dim item As DockItem = dockControl.DockItems(i)
        item.Close()
        i = (i - 1)
    Loop
End Sub

Private Function LoadDockItem(ByVal obj As Object) As DockItem
    If welcome.GetType.ToString.Equals(obj) Then
        Return welcome
    ElseIf savedLayout.GetType.ToString.Equals(obj) Then
        Return savedLayout
    ElseIf output.GetType.ToString.Equals(obj) Then
        Return output
    ElseIf solutionExplorer.GetType.ToString.Equals(obj) Then
        Return solutionExplorer
    ElseIf propertiesWindow.GetType.ToString.Equals(obj) Then
        Return propertiesWindow
    Else
        Return CType(obj, DockItem)
    End If
End Function
```

>[!Note]
>You must close all DockItem before loading the window layout, otherwise an exception will throw.