using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace DevZest.Windows.Docking
{
    /// <summary>Provides a standard set of docking related commands.</summary>
    public static class DockCommands
    {
        private static RoutedCommand s_toggleFloating;
        private static RoutedCommand s_toggleAutoHide;
        private static RoutedCommand s_performClose;
        private static RoutedCommand s_show;
        private static RoutedCommand s_undo;
        private static RoutedCommand s_redo;
        private static RoutedCommand s_toggleWindowState;
        private static RoutedCommand s_makeFloating;

        /// <summary>Gets the value that represents the Toggle Floating command.</summary>
        /// <value>The command.</value>
        public static RoutedCommand ToggleFloating
        {
            get
            {
                if (s_toggleFloating == null)
                    s_toggleFloating = new RoutedCommand("ToggleFloating", typeof(DockCommands));
                return s_toggleFloating;
            }
        }

        /// <summary>Gets the value that represents the Toggle Auto Hide command.</summary>
        /// <value>The command.</value>
        public static RoutedCommand ToggleAutoHide
        {
            get
            {
                if (s_toggleAutoHide == null)
                    s_toggleAutoHide = new RoutedCommand("ToggleAutoHide", typeof(DockCommands));
                return s_toggleAutoHide;
            }
        }

        /// <summary>Gets the value that represents the Perform Close command.</summary>
        /// <value>
        /// <para>The command.</para>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Default Values</term>
        ///         <description></description>
        ///     </listheader>
        ///     <item>
        ///         <term>Key Gesture</term>
        ///         <description>CTRL+F4</description>
        ///     </item>
        /// </list>
        /// </value>
        public static RoutedCommand PerformClose
        {
            get
            {
                if (s_performClose == null)
                {
                    s_performClose = new RoutedCommand("PerformClose", typeof(DockCommands));
                    s_performClose.InputGestures.Add(new KeyGesture(Key.F4, ModifierKeys.Control));
                }
                return s_performClose;
            }
        }

        /// <summary>Gets the value that represents the Show command.</summary>
        /// <value>The command.</value>
        public static RoutedCommand Show
        {
            get
            {
                if (s_show == null)
                    s_show = new RoutedCommand("Show", typeof(DockCommands));
                return s_show;
            }
        }

        /// <summary>Gets the value that represents the Undo command.</summary>
        /// <value>
        /// <para>The command.</para>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Default Values</term>
        ///         <description></description>
        ///     </listheader>
        ///     <item>
        ///         <term>Key Gesture</term>
        ///         <description>CTRL+U</description>
        ///     </item>
        /// </list>
        /// </value>
        public static RoutedCommand Undo
        {
            get
            {
                if (s_undo == null)
                {
                    s_undo = new RoutedCommand("Undo", typeof(DockCommands));
                    s_undo.InputGestures.Add(new KeyGesture(Key.U, ModifierKeys.Control));
                }

                return s_undo;
            }
        }

        /// <summary>Gets the value that represents the Redo command.</summary>
        /// <value>
        /// <para>The command.</para>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Default Values</term>
        ///         <description></description>
        ///     </listheader>
        ///     <item>
        ///         <term>Key Gesture</term>
        ///         <description>CTRL+R</description>
        ///     </item>
        /// </list>
        /// </value>
        public static RoutedCommand Redo
        {
            get
            {
                if (s_redo == null)
                {
                    s_redo = new RoutedCommand("Redo", typeof(DockCommands));
                    s_redo.InputGestures.Add(new KeyGesture(Key.R, ModifierKeys.Control));
                }

                return s_redo;
            }
        }

        /// <summary>Gets the value that represents the Toggle Window State command.</summary>
        /// <value>The command.</value>
        public static RoutedCommand ToggleWindowState
        {
            get
            {
                if (s_toggleWindowState == null)
                    s_toggleWindowState = new RoutedCommand("ToggleWindowState", typeof(DockCommands));

                return s_toggleWindowState;
            }
        }

        /// <summary>Gets the value that represents the Make Floating command.</summary>
        /// <value>The command.</value>
        public static RoutedCommand MakeFloating
        {
            get
            {
                if (s_makeFloating == null)
                    s_makeFloating = new RoutedCommand("MakeFloating", typeof(DockCommands));

                return s_makeFloating;
            }
        }

        internal static bool Execute(object sender, System.Windows.Input.ICommand command)
        {
            return Execute(sender, command, null);
        }

        internal static bool Execute(object sender, System.Windows.Input.ICommand command, object commandParameter)
        {
            if (command != null)
            {
                RoutedCommand routedCommand = command as RoutedCommand;
                IInputElement inputElement = sender as IInputElement;
                if (routedCommand != null && routedCommand.CanExecute(commandParameter, inputElement))
                {
                    routedCommand.Execute(commandParameter, inputElement);
                    return true;
                }
                else if (command.CanExecute(commandParameter))
                {
                    command.Execute(commandParameter);
                    return true;
                }
            }

            return false;
        }
    }
}
