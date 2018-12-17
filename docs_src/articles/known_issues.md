---
uid: known_issues
---

# Known Issues

This page contains information of known issues when programming WPF Docking.

## Windows Forms Interop (WindowsFormsHost)

By design, the WindowsFormsHost is not supported by WPF Docking, due to the [restriction of the Framework](https://docs.microsoft.com/en-us/dotnet/framework/wpf/advanced/wpf-and-win32-interoperation).

You will receive the error message: "Set focus to DockItem "ItemA" failed. This normally indicates the DockControl is not properly styled." This is because the keyboard focus is set to another HWND (WindowsFormsHost), therefore the IsKeyboardWithin property of the DockItem is false, as explained in the MSDN documentation: "While the HwndHost has keyboard focus, your application will not receive WPF keyboard events and the value of the WPF property IsKeyboardFocusWithin will be false."

Another most significant impact is layout: "HwndHost will appear on top of other WPF elements in the same top-level window." That means the auto-hide window will always draw beneath the WindowsFormsHost.

The design decision is to implement WPF Docking in a pure WPF way, as its name suggested. Otherwise we have to implement DockItem as top level Win32 window, and this will lose many of the WPF benefits and cause endless trouble. If you don't really have the option of not using the WindowsFormsHost, you can use Windows Forms solution DockPanel Suite.

## DockControl Must Be Loaded Before Showing DockItem

The DockControl is a window manager, it must be loaded before showing any DockItem, mainly for the following reasons:

- Set keyboard focus correctly;
- To show a floating window, the main window (owner window) needs to be retrieved.

If you attempt to show DockItem before DockControl is loaded (DockControl.IsLoaded="False"), an InvalidOperationException will throw. To avoid the exception, you could:

- Call DockControl.UpdateLayout before calling DockItem.Show;
- Set DockItem.ShowAction and add it to DockControl.Items collection. The DockControl wires Loaded event and do the DockItem.Show for you automatically.