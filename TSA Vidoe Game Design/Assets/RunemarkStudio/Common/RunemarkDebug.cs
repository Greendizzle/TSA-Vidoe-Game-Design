using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Runemark.Common
{
    public class RunemarkDebug
    {
        public enum LogLevel : int
        {
            Error = 0,
            Warning = 1,
            Log = 2,
            None = -1,
        }

        public static void Log(string msg)
        {
            if (checkDebugLevel(LogLevel.Log))
                Debug.Log(msg);
        }
        public static void Log(string msg, params object[] args)
        {
            if (checkDebugLevel(LogLevel.Log))
                Debug.LogFormat(msg, args);
        }

        public static void Warning(string msg)
        {
            if (checkDebugLevel(LogLevel.Warning))
                Debug.LogWarning(msg);
        }
        public static void Warning(string msg, params object[] args)
        {
            if (checkDebugLevel(LogLevel.Warning))
                Debug.LogWarningFormat(msg, args);
        }

        public static void Error(string msg)
        {
            if (checkDebugLevel(LogLevel.Error))
                Debug.LogError(msg);
        }
        public static void Error(string msg, params object[] args)
        {
            if (checkDebugLevel(LogLevel.Error))
                Debug.LogErrorFormat(msg, args);
        }

        static bool checkDebugLevel(LogLevel lvl)
        {
            bool result = false;

#if UNITY_EDITOR
            if (!EditorPrefs.HasKey("RunemarkVisualEditorDebugLevel"))
            {
                EditorPrefs.SetInt("RunemarkVisualEditorDebugLevel", -1);
                result = (int)lvl <= (int)LogLevel.Log;
            }
            else
                result =  (int)lvl <= EditorPrefs.GetInt("RunemarkVisualEditorDebugLevel");
#endif
            return result;
        }



#if UNITY_EDITOR
        [MenuItem("Window/Runemark/Debug Level/None")]
        static void debugMode_None() { EditorPrefs.SetInt("RunemarkVisualEditorDebugLevel", -1); }
        [MenuItem("Window/Runemark/Debug Level/Error")]
        static void debugMode_Error() { EditorPrefs.SetInt("RunemarkVisualEditorDebugLevel", 0); }
        [MenuItem("Window/Runemark/Debug Level/Warning")]
        static void debugMode_Warning() { EditorPrefs.SetInt("RunemarkVisualEditorDebugLevel", 1); }
        [MenuItem("Window/Runemark/Debug Level/Log")]
        static void debugMode_Log() { EditorPrefs.SetInt("RunemarkVisualEditorDebugLevel", 2); }
#endif
    }
}