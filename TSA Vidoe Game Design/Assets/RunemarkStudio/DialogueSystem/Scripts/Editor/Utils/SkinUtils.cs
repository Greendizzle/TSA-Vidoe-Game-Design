using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Runemark.DialogueSystem
{
    public class SkinNameList
    {
        /// <summary>
        /// Array of the selectable list of skin names.
        /// </summary>
        public string[] Names;
        /// <summary>
        /// Index of the skin.
        /// </summary>
        public int Index = -1;
        
        int _index = -1;
        int _lastIndex = -1;

        string _lastName = "";
        List<string> _lastFilter = new List<string>();
        int _lastType = -1;
        
        /// <summary>
        /// Collects all visible skins.
        /// </summary>
        /// <param name="currentName"></param>
        /// <param name="filter"> list of skin names that will be shown </param>
        /// <param name="type">-1: no type filter, 0: conversation only, 1: ambient only</param>
        public void CollectSkins(string currentName, List<string> filter, int type = -1)
        {
            _lastName = currentName;
            _lastFilter = filter;
            _lastType = type;

            Index = -1;
            int cnt = 0;
            List<string> names = new List<string>();
            foreach (var skin in SkinDatabase.Instance.Skins)
            {
                if (filter == null || filter.Contains(skin.Name))
                {
                    if (type == -1 || (type == 0 && !skin.IsAmbient) || (type == 1 && skin.IsAmbient))
                    {
                        string name = skin.Name;
                        if (skin.IsAmbient && type == -1)
                            name += " (Ambient Skin)";

                        names.Add(name);                     

                        if (skin.Name == currentName)
                            _index = cnt;

                        cnt++;
                    }
                }
            }
            Names = names.ToArray();            
        }


        public void RecollectSkins()
        {
            CollectSkins(_lastName, _lastFilter, _lastType);
        }


        public bool DrawGUI(bool button = true)
        {
            EditorGUILayout.BeginHorizontal();
            _index = EditorGUILayout.Popup("Skin Name", _index, Names);

            if (button && GUILayout.Button("", (GUIStyle)"PaneOptions", GUILayout.Width(25)))
            {
                var window = UISkinWindow.Open();
                window.onChange = RecollectSkins;
            }

            EditorGUILayout.EndHorizontal();

            if (_lastIndex != _index)
            { 
                 Index = ConvertIndex();
                _lastIndex = _index;
                return true;
            }
            return false;
        }

        int ConvertIndex()
        {
            string selectedName = Names[_index].Replace(" (Ambient Skin)", "");

            for (int i = 0; i < SkinDatabase.Instance.Skins.Count; i++)
            {
                if (selectedName == SkinDatabase.Instance.Skins[i].Name)
                    return i;
            }
            return -1;
        }

        
               


    }
}