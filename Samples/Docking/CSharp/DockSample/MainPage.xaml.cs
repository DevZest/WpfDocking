using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevZest.Windows.Docking;

namespace DevZest.DockSample
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    partial class MainPage
    {
        private static readonly DependencyPropertyKey IsLayoutSavedPropertyKey = DependencyProperty.RegisterReadOnly("IsLayoutSaved", typeof(bool), typeof(MainPage), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty IsLayoutSavedProperty = IsLayoutSavedPropertyKey.DependencyProperty;

        public bool IsLayoutSaved
        {
            get { return (bool)GetValue(IsLayoutSavedProperty); }
            private set { SetValue(IsLayoutSavedPropertyKey, value); }
        }

        private int _lastDocumentId;

        public MainPage()
        {
            InitializeComponent();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("***** Welcome to DevZest WPF docking! *****");
            sb.AppendLine(string.Format("===== CLR Version: {0}, Loaded Assemblies:", Environment.Version));
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                sb.AppendLine(assembly.FullName);
            output.AppendLog(sb.ToString());
        }

        private void NewDocument(object sender, ExecutedRoutedEventArgs e)
        {
            Document document = new Document(++_lastDocumentId);
            document.Show(dockControl);
        }

        private void CanExecuteCloseActiveDocument(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = dockControl.ActiveDocument != null;
        }
        private void CloseActiveDocument(object sender, ExecutedRoutedEventArgs e)
        {
            dockControl.ActiveDocument.PerformClose();
        }

        private void OnFocusedItemChanged(object sender, EventArgs e)
        {
            output.AppendLog(string.Format("FocusedItemChanged: FocusedItem={0}", GetString(dockControl.FocusedItem)));
        }

        private void OnActiveItemChanged(object sender, EventArgs e)
        {
            output.AppendLog(string.Format("ActiveItemChanged: ActiveItem={0}", GetString(dockControl.ActiveItem)));
        }

        private void OnActiveDocumentChanged(object sender, EventArgs e)
        {
            output.AppendLog(string.Format("ActiveDocumentChanged: ActiveDocument={0}", GetString(dockControl.ActiveDocument)));
        }

        private void OnDockItemStateChanging(object sender, DockItemStateEventArgs e)
        {
            output.AppendLog(string.Format("DockItemStateChanging: {0}", GetString(e)));
        }

        private void OnDockItemStateChanged(object sender, DockItemStateEventArgs e)
        {
            output.AppendLog(string.Format("DockItemStateChanged: {0}", GetString(e)));
        }

        private string GetString(DockItem item)
        {
            return item == null ? "null" : item.TabText;
        }

        private string GetString(DockItemStateEventArgs e)
        {
            if (e.OldDockPosition == e.NewDockPosition)
                return string.Format("DockItem={0}, StateChangeMethod={1}, ShowMethod={2}",
                    GetString(e.DockItem),
                    e.StateChangeMethod,
                    e.ShowMethod);
            else
                return string.Format("DockItem={0}, StateChangeMethod={1}, DockPosition={2}->{3}, ShowMethod={4}",
                    GetString(e.DockItem),
                    e.StateChangeMethod,
                    e.OldDockPosition,
                    e.NewDockPosition,
                    e.ShowMethod);
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

        private void SaveLayout_Click(object sender, RoutedEventArgs e)
        {
            savedLayout.Save(dockControl);
            IsLayoutSaved = true;
        }

        private void LoadLayout_Click(object sender, RoutedEventArgs e)
        {
            CloseAll();
            savedLayout.Load(dockControl, LoadDockItem);
        }

        private void CloseAll()
        {
            for (int i = dockControl.DockItems.Count - 1; i >= 0; i--)
            {
                DockItem item = dockControl.DockItems[i];
                item.Close();
            }
        }

        private void ChangeTheme(string themeName)
        {
            if (string.IsNullOrEmpty(themeName))
            {
                DockSample.Themes.Reset();
                DevZest.Windows.Docking.Themes.Reset();
            }
            else
            {
                DockSample.Themes.Load(themeName);
                DevZest.Windows.Docking.Themes.Load(themeName);
            }

            _defaultTheme.IsChecked = string.IsNullOrEmpty(themeName);
            _expressionDark.IsChecked = themeName == "ExpressionDark";
            _vs2010.IsChecked = themeName == "VS2010";
        }

        private void Theme_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            string theme = menuItem.CommandParameter as string;
            ChangeTheme(theme);
        }
    }
}
