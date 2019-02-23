
using System.Collections.Generic;
using UnityEngine;

namespace Runemark.VisualEditor.Utility
{
    public static class BuiltInColors
    {
        public static Color Default = new Color(.7f, .7f, .7f);
        public static Color DarkDefault = new Color(.22f, .22f, .21f);
        public static Color ExecutionFlow = new Color(1f, 1f, 1f);

        // Primitives
        public static Color Integer = new Color(0.12f, 0.89f, 0.68f, 1.00f);    // #2ac79d
        public static Color Float = new Color(0.62f, 0.98f, 0.27f, 1.00f);      // #91d949
        public static Color Bool = new Color(0.58f, 0.00f, 0.00f, 1.00f);       // #861111
        public static Color String = new Color(0.94f, 0.01f, 0.78f, 1.00f);     // #d114b1

        public static Color DarkInteger = new Color(0.04f, 0.27f, 0.20f, 1.00f);    // #094534
        public static Color DarkFloat = new Color(0.19f, 0.29f, 0.08f, 1.00f);      // #304b15
        public static Color DarkBool = new Color(0.18f, 0.00f, 0.00f, 1.00f);       // #2d0000
        public static Color DarkString = new Color(0.32f, 0.12f, 0.28f, 1.00f);     // #511f48


        static Dictionary<System.Type, Color> _colors = new Dictionary<System.Type, Color>()
        {
            { typeof(string), String },
            { typeof(int), Integer },
            { typeof(float), Float },
            { typeof(bool), Bool },
            { typeof(ExecutableNode), ExecutionFlow },

        };
        static Dictionary<System.Type, Color> _darkColors = new Dictionary<System.Type, Color>()
        {
            { typeof(string), DarkString },
            { typeof(int), DarkInteger },
            { typeof(float), DarkFloat },
            { typeof(bool), DarkBool },

        };
        static Color _defaultColor = Default;
        static Color _darkDefaultColor = DarkDefault;

        public static Color Get<T>()
        {
            if (_colors.ContainsKey(typeof(T))) return _colors[typeof(T)];
            return _defaultColor;
        }

        public static Color Get(System.Type t)
        {
            if (t != null && _colors.ContainsKey(t)) return _colors[t];
            return _defaultColor;
        }

        public static Color GetDark(System.Type t)
        {
            if (t != null && _darkColors.ContainsKey(t)) return _darkColors[t];
            return _darkDefaultColor;
        }

    }
}