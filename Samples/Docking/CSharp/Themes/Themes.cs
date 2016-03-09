using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace DevZest.Windows.Docking
{
    public static class Themes
    {
        static ResourceDictionary s_currentTheme = null;

        public static void Load(string themeName)
        {
            Uri themeSource = new Uri("/" + Assembly.GetExecutingAssembly().FullName.Split(',')[0] + ";component/themes/" + themeName + ".xaml", UriKind.RelativeOrAbsolute);
            Load(themeSource);
        }

        public static void Load(Uri themeSource)
        {
            Reset();
            Debug.Assert(s_currentTheme == null);
            s_currentTheme = new ResourceDictionary();
            s_currentTheme.Source = themeSource;
            Application.Current.Resources.MergedDictionaries.Add(s_currentTheme);
        }

        public static void Reset()
        {
            if (s_currentTheme != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(s_currentTheme);
                s_currentTheme = null;
            }
        }
    }
}
