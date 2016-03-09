using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Navigation;

namespace BrowserDockSample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Refresh)
                e.Cancel = true;
        }
    }
}
