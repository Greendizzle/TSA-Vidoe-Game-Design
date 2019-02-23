using Runemark.Common;
using Runemark.DialogueSystem.UI;
using Runemark.VisualEditor;
using UnityEngine;

namespace Runemark.DialogueSystem
{
    /// <summary>
    /// This class gives acces to the globam things.
    /// </summary>
    public static class DialogueSystem
    {
        static DialogueSystemGlobals _globals
        {
            get
            {
                if (_dsglobals == null)
                {
                    // Load the Dialogue System Globals scriptable asset from the Resources
                    var list = Resources.LoadAll<DialogueSystemGlobals>("");
                    // Set the private variable to the first found asset, or to null if none was find.
                    _dsglobals = (list.Length > 0) ? list[0] : null;
                    // Find the behaviours on the scene.
                    FindBehaviour();
                }
                return _dsglobals;
            }
        }
        static DialogueSystemGlobals _dsglobals;

        /// <summary>
        /// Gets the global variable by it's name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Variable GetGlobalVariable(string name)
        {              
            return (_globals != null) ?  _globals.Variables.GetByName(name) : null;
        }
        /// <summary>
        /// Set the global variable with the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void SetGlobalVariable<T>(string name, T value)
        {
            if (_globals == null) return;

            var v = _globals.Variables.GetByName(name);

            if (v.type == typeof(T))
                v.Value = value;
            else
                RunemarkDebug.Error("{0} global variable's type is {1}, and can't set it's value to {2}({3})",
                    name, v.type, value, typeof(T));
        }
        

        #region SAVE & LOAD Variables
        public static void SaveToPlayerPrefs()
        {
            if (_globals == null) return;
            if (_globals.Variables == null) return;

            _globals.Variables.SaveToPlayerPrefs();
        }
        public static void LoadFromPlayerPrefs()
        {
            if (_globals == null) return;
            if (_globals.Variables == null) return;

            _globals.Variables.LoadFromPlayerPrefs();
        }
        public static string SaveToString()
        {
            return _globals.Variables.SaveToString();
        }
        public static void LoadFromString(string s)
        {
            _globals.Variables.LoadFromString(s);
        }
        #endregion

        #region RESET ON APP QUIT
        /// <summary>
        /// This method will find a single Visual Editor Behaviour or IDialogueUIController
        /// just for tapping in the OnApplicationQuit event.
        /// If the selected component is deactivating the system will find a new one.
        /// </summary>
        static void FindBehaviour()
        {            
            var b = GameObject.FindObjectOfType<VisualEditorBehaviour>();
            if (b != null)
            {
                b.onApplicationQuit = OnApplicationQuit;
                b.onInactivated = FindBehaviour;
                return;
            }

            var c = UnityExtensions.FindObjectOfInterface<IDialogueUIController>();
            if (c != null)
            {
                c.onApplicationQuit = OnApplicationQuit;
                c.onInactivated = FindBehaviour;
                return;
            }                
        }

        /// <summary>
        /// OnApplicationQuit event, raised in a result component from the FindBehaviour method.
        /// </summary>
        static void OnApplicationQuit()
        {
            if (_globals == null) return;
            if (_globals.Variables == null) return;

            _globals.Variables.Reset();

        }
        #endregion


    }
}
