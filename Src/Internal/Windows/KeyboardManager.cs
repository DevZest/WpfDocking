using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace DevZest.Windows
{
    internal static class KeyboardManager
    {
        public static event Action<KeyEventArgs> KeyDown;
        public static event Action<KeyEventArgs> KeyUp;

        static KeyboardManager()
        {
            EventManager.RegisterClassHandler(typeof(UIElement), Keyboard.KeyDownEvent, new KeyEventHandler(OnKeyDown), true);
            EventManager.RegisterClassHandler(typeof(UIElement), Keyboard.KeyUpEvent, new KeyEventHandler(OnKeyUp), true);
        }

        private static void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (sender == e.OriginalSource && KeyDown != null)
                KeyDown(e);
        }

        private static void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (sender == e.OriginalSource && KeyUp != null)
                KeyUp(e);
        }


    }
}
