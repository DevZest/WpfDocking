using System;

namespace DevZest
{
    static class BooleanBoxes
    {
        public static readonly object True = true;
        public static readonly object False = false;

        public static object Box(bool value)
        {
            return value ? True : False;
        }
    }
}
