
using Runemark.Common;
using UnityEditor;
using UnityEngine;

namespace Runemark.DialogueSystem
{
    [CustomEditor(typeof(BasicSaveLoad), true)]
    public class BasicSaveLoadEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            BasicSaveLoad myTarget = (BasicSaveLoad)target;
            float width = EditorGUIUtility.currentViewWidth;

            EditorGUIExtension.DrawInspectorTitle("Basic Save & Load",
                "This component will save and load your saveable Local and Global variables.");

            EditorGUIExtension.SimpleBox("Settings", 5, "", delegate () {

                myTarget.mode = (BasicSaveLoad.Mode) EditorGUILayout.EnumPopup("Save Mode", myTarget.mode);

                if (myTarget.mode == BasicSaveLoad.Mode.File)
                {
                    myTarget.FileName = EditorGUILayout.TextField("File Name", myTarget.FileName);
                }

                EditorGUILayout.Space();
                myTarget.SaveOnExit = EditorGUILayout.Toggle("Save on Exit", myTarget.SaveOnExit);
                myTarget.LoadOnStart = EditorGUILayout.Toggle("Load on Start", myTarget.LoadOnStart);

            });
        }
    }
}