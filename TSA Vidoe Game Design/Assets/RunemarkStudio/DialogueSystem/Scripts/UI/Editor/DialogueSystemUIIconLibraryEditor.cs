using Runemark.Common;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

namespace Runemark.DialogueSystem.UI
{
    [CustomEditor(typeof(IconLibrary), true)]
    public class DialogueSystemUIIconLibraryEditor : Editor
    {
        IconLibrary _myTarget;
        ReorderableList _iconList;

        private void OnEnable()
        {
            if (_myTarget == null) _myTarget = (IconLibrary)target;

            _iconList = new ReorderableList(_myTarget.Icons, typeof(IconData), true, true, true, true);
            _iconList.elementHeight = 50;
            _iconList.drawHeaderCallback = DrawIconListHeader;
            _iconList.drawElementCallback = DrawIconListElement;
        }

        public override void OnInspectorGUI()
        {
            if (_myTarget == null) _myTarget = (IconLibrary)target;

            var t = _myTarget.GetType().ToString().Split('.');
            string title = t[t.Length - 1].Replace("UI Icon Library", "");
            EditorGUIExtension.DrawInspectorTitle("Dialogue System - " + title, "");

            
            _iconList.DoLayoutList();
            EditorGUILayout.Space();
        }

        void DrawIconListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Icons");
        }

        void DrawIconListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            Rect r = new Rect(rect.x + 5, rect.y + 5, rect.height - 10, rect.height - 10);
            _myTarget.Icons[index].Icon = (Sprite)EditorGUI.ObjectField(r, _myTarget.Icons[index].Icon, typeof(Sprite), true);

            r = new Rect(rect.x + rect.height + 10, rect.y + (rect.height - 20) / 2, rect.width - rect.height - 15, 20);
            _myTarget.Icons[index].Name = EditorGUI.TextField(r, _myTarget.Icons[index].Name);
        }
     
    }

   
}