using Runemark.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Runemark.DialogueSystem.UI
{
    [CustomEditor(typeof(UIElementTimer), true)]
    public class DialogueSystemUIElementTimerEditor : DialogueSystemUIElementEditor
    {
        protected override void onGUI()
        {
            UIElementTimer myTarget = (UIElementTimer)target;

            EditorGUIExtension.SimpleBox("UI Elements", 5, "", delegate ()
            {
                myTarget.Slider = (Slider)EditorGUILayout.ObjectField("Slider", myTarget.Slider, typeof(Slider), true);
                myTarget.Label = (Text)EditorGUILayout.ObjectField("Text", myTarget.Label, typeof(Text), true);

            });
        }      

    }
}
