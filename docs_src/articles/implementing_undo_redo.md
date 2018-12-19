---
uid: implementing_undo_redo
---

# Implementing Undo Redo

Undo/Redo is one of the pieces of functionality that users expect to see but is often overlooked, perhaps because it is hard to implement. WPF Docking makes your application more user friendly to allow the end user to undo/redo the window layout change easily.

## Undo Unit and Undo/Redo Stack

Undo/redo is performed based on undo unit, which consists of one or more window layout change(s). By default, one single window layout change forms a undo unit automatically, however you can call [DockControl.BeginUndoUnit](xref:DevZest.Windows.Docking.DockControl.BeginUndoUnit*) and [DockControl.EndUndoUnit](xref:DevZest.Windows.Docking.DockControl.EndUndoUnit*) to group multiple window layout changes together as one single undo unit.

WPF Docking maintains two stacks of undo units for undo and redo respectively:

- Window layout change pushes undo unit into undo stack and clears the redo stack;
- Undo the window layout by calling [DockControl.Undo](xref:DevZest.Windows.Docking.DockControl.Undo*) will pop the last undo unit from undo stack, perform the undo, and push this undo unit into redo stack;
- Redo the window layout by calling [DockControl.Redo](xref:DevZest.Windows.Docking.DockControl.Redo*) will pop the last undo unit from redo stack, perform the redo, and push this undo unit back into undo stack.

The [DockControl.MaxUndoLevel](xref:DevZest.Windows.Docking.DockControl.MaxUndoLevel) property controls the size of undo/redo stack.

Since the content of DockItem may intercept the system undo/redo command, WPF Docking implements its own [DockCommands.Undo](xref:DevZest.Windows.Docking.DockCommands.Undo) command with key gesture of CTRL-U and [DockCommands.Redo](xref:DevZest.Windows.Docking.DockCommands.Redo) command with key gesture of CTRL-R.

## DockItem Undo/Redo Reference

To undo closing of DockItem, an instance of DockItem object needs to be retrieved. Instead of always holding DockItem object directly in the undo stack and keep it from being garbage collected, an instance of @DevZest.Windows.Docking.IDockItemUndoRedoReference returned by [DockItem.UndoRedoReference](xref:DevZest.Windows.Docking.DockItem.UndoRedoReference) property is stored, or both undo and redo stack are cleared if [DockItem.UndoRedoReference](xref:DevZest.Windows.Docking.DockItem.UndoRedoReference) property returns null. The default implementation of [DockItem.UndoRedoReference](xref:DevZest.Windows.Docking.DockItem.UndoRedoReference) property returns this DockItem itself or null depending on the HideOnPerformClose property:

| HideOnPerformClose Value | Returns | Description |
| --- | --- | --- |
| True | This DockItem | HideOnPerformClose property is true suggests this is a singleton DockItem. It's okay to hold this DockItem object directly in the undo stack and keep it from being garbage collected. |
| False | Null | When closing the DockItem, both undo and redo stack are cleared. |

You may override [DockItem.UndoRedoReference](xref:DevZest.Windows.Docking.DockItem.UndoRedoReference) property to provide your own implementation, such as returning a custom @DevZest.Windows.Docking.IDockItemUndoRedoReference object holding a weak reference of the DockItem and recreate it when necessary, as demonstrated by DockSample's Document class.