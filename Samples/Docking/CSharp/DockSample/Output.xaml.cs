using System;
using System.Windows;
using System.Windows.Controls;
using DevZest.Windows.Docking;

namespace DevZest.DockSample
{
    /// <summary>
    /// Interaction logic for Output.xaml
    /// </summary>
    partial class Output
    {
        public Output()
        {
            InitializeComponent();
        }

        public void AppendLog(string text)
        {
            textBox.AppendText(string.Format("{0}: {1}", DateTime.Now.ToLongTimeString(), text + Environment.NewLine));
            textBox.ScrollToEnd();
        }

        private void OnClearAllClick(object sender, RoutedEventArgs e)
        {
            textBox.Text = null;
        }
    }
}
