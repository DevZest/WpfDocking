---
uid: detailed_window_hierarchy_and_customization
---

# Detailed Window Hierarchy and Customization

Although rarely used directly, the objects described in this topic can help you to further understance the window layout and how WPF Docking can be customized.

## DockTree to DockPane

@DevZest.Windows.Docking.DockControl contains N+5 @DevZest.Windows.Docking.DockTree, where N is the number of floating windows (accessible through FloatingWindows collection), plus LeftDockTree, TopDockTree, RightDockTree, BottomDockTree and DocumentDockTree, which docked to lef, top, right, bottom and fill respectively.

For floating windows, the Z-order is determined by the order of window being activated, reflected by FloatingWindows collection.

For docked DockTree, the Z-order is determined by the DockTreeZOrder property.

The RootNode property of DockTree represents a binary tree of DockPaneNode objects: DockPaneSplit as non-leaf nodes, and DockPane as leaf nodes.

Auto-hide DockPane is reflected in AutoHidePanes collection of containing DockTree and the ChildrenVisibility property of its parent DockPaneSplit (if any).

## DockPane to DockItem

@DevZest.Windows.Docking.DockPane contains a collection of @DevZest.Windows.Docking.DockItem objects, shown as tabbed, through its Items property. A DockItem can have two parent DockPane: FirstPane and SecondPane, one for floating and one for non-floating. It is always invisible in the SecondPane, reflected by the VisibleItems property.

## Customization

WPF Docking is fully customizable through styling and templating. The best practice is to take the Themes project in the installed Samples as a base to start.