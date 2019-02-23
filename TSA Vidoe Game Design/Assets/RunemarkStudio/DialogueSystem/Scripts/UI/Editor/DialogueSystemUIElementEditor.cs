using Runemark.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Runemark.DialogueSystem.UI
{
    [CustomEditor(typeof(UIElement), true)]
    public class DialogueSystemUIElementEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            UIElement myTarget = (UIElement)target;

            var t = myTarget.GetType().ToString().Split('.');
            string title = t[t.Length-1].Replace("UIElement","");   
                     
            EditorGUIExtension.DrawInspectorTitle("Dialogue System - "+title, "");

            onGUI();

            EditorGUILayout.Space();
        }

        protected virtual void onGUI(){ DrawDefaultInspector();  }
    }
}
