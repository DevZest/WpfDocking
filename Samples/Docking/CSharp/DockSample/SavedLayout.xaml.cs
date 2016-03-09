using System;
using System.Security;
using System.Security.Permissions;
using System.IO;
using System.Xml;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevZest.Windows.Docking;

namespace DevZest.DockSample
{
    /// <summary>
    /// Interaction logic for SavedLayout.xaml
    /// </summary>
    partial class SavedLayout
    {
        public SavedLayout()
        {
            InitializeComponent();
        }

        internal void Save(DockControl dockControl)
        {
            DockLayout layout = dockControl.Save();
            textBox.Text = XamlWriter.Save(layout);
            Show(DockControl);
        }

        internal void Load(DockControl dockControl, Func<object, DockItem> deserializeDockItem)
        {
            DockLayout layout = (DockLayout)XamlReader.Load(new XmlTextReader(new StringReader(textBox.Text)));
            dockControl.Load(layout, deserializeDockItem);
        }
    }
}
