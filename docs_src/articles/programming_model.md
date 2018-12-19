---
uid: programming_model
---

# Programming Model

Although WPF Docking is a very sophisticated UI library, it has a very simple programming model. In most cases, you, as a programmer, only need to deal with two classes: @DevZest.Windows.Docking.DockControl and @DevZest.Windows.Docking.DockItem.

## Implement Your DockItem Objects

Implement your individual window as @DevZest.Windows.Docking.DockItem object, which derives from [ContentControl](https://docs.microsoft.com/en-us/dotnet/api/system.windows.controls.contentcontrol) class. Set the [Content](https://docs.microsoft.com/en-us/dotnet/api/system.windows.controls.contentcontrol.content) property to organize the UI. For example, the following XAML code defines a simple dockable window displays a welcome message:

```xaml
<dz:DockItem
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:dz="http://schemas.devzest.com/presentation/docking"
    TabText="Welcome" Title="Welcome">
    <FlowDocumentScrollViewer>
        <FlowDocumentScrollViewer.Document>
            <FlowDocument FontFamily="Calibri" FontSize="14.5" TextAlignment="Left">
                <Paragraph FontSize="22" FontWeight="Bold">Welcome to DevZest Docking</Paragraph>
            </FlowDocument>
        </FlowDocumentScrollViewer.Document>
    </FlowDocumentScrollViewer>
</dz:DockItem>
```

>[!Note]
>You may define the DockItem object as the root element of the XAML element tree, and add a `x:Class` attribute to join the code-behind partial class.

## Place a DockControl in Your Application Main Form/Window

@DevZest.Windows.Docking.DockControl is the core of docking window layout. You need to place a DockControl in your application main form/window. For example:

```xaml
<Window
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:dz="http://schemas.devzest.com/presentation/docking"
    Title="DockControl Sample"
    WindowState="Maximized">
    <dz:DockControl>
    ...
    </dz:DockControl>
</Window>
```

## Show DockItem by XAML or Code

### Show DockItem by XAML

To show DockItem by XAML, add DockItem objects to DockItems collection, with its ShowAction set:

```xaml
<Window
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:dz="http://schemas.devzest.com/presentation/docking"
    Title="DockControl Sample"
    WindowState="Maximized">
    <dz:DockControl>
        <dz:DockItem TabText="Welcome" Title="Welcome" ShowAction="{dz:ShowAsDockPositionAction DockPosition=Document}">
            <FlowDocumentScrollViewer>
                <FlowDocumentScrollViewer.Document>
                    <FlowDocument FontFamily="Calibri" FontSize="14.5" TextAlignment="Left">
                        <Paragraph FontSize="22" FontWeight="Bold">Welcome to DevZest Docking</Paragraph>
                    </FlowDocument>
                </FlowDocumentScrollViewer.Document>
            </FlowDocumentScrollViewer>
        </dz:DockItem>
        <dz:DockItem TabText="Saved State" Title="Saved State" ShowAction="{dz:ShowAsDockPositionAction DockPosition=Bottom}">
            <TextBox />
        </dz:DockItem>
        <dz:DockItem TabText="Output" Title="Output" ShowAction="{dz:ShowAsDockPositionAction DockPosition=Bottom}">
            <TextBox />
        </dz:DockItem>
        <dz:DockItem TabText="Solution Explorer" Title="Solution Explorer" ShowAction="{dz:ShowAsDockPositionAction DockPosition=Right}">
            <TextBox />
        </dz:DockItem>
        <dz:DockItem TabText="Properties" Title="Properties" ShowAction="{dz:ShowAsDockPositionAction DockPosition=Right}">
            <TextBox />
        </dz:DockItem>
    </dz:DockControl>
</Window>
```

List of ShowAction values:

| ShowAction | Description |
| --- | --- |
| @DevZest.Windows.Docking.ShowAsDockPositionAction | Show DockItem as specified dock position. |
| @DevZest.Windows.Docking.ShowAsFloatingAction | Show DockItem as floating window. |
| @DevZest.Windows.Docking.ShowAsSidePaneAction | Show DockItem as side by side @DevZest.Windows.Docking.DockPane. |
| @DevZest.Windows.Docking.ShowAsTabbedAction | Show DockItem as tabbed. |

## Show DockItem by Code

To show DockItem by code is extremely easy: simply call one of the overloaded [Show](xref:DevZest.Windows.Docking.DockItem.Show*) methods. The following code snippet is equivalent of the previous XAML code:

```csharp
... // Code omitted: create instance of welcome, savedState, output, solutionExplorer, properties
welcome.Show(dockControl, DockPosition.Document);
savedState.Show(dockControl, DockPosition.Bottom);
output.Show(dockControl, DockPosition.Bottom);
solutionExplorer.Show(dockControl, DockPosition.Right);
properties.Show(dockControl, DockPosition.Right);
```

```vb
... ' Code omitted: create instance of welcome, savedState, output, solutionExplorer, properties
welcome.Show(dockControl, DockPosition.Document)
savedState.Show(dockControl, DockPosition.Bottom)
output.Show(dockControl, DockPosition.Bottom)
solutionExplorer.Show(dockControl, DockPosition.Right)
properties.Show(dockControl, DockPosition.Right)
```

>[!Note]
>Calling Show method will implicitly add this DockItem into DockItems collection, you don't need to add it to the collection explicitly.