using Runemark.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Runemark.DialogueSystem.UI
{
    [CustomEditor(typeof(DialogueSystemUISkin), true)]
    public class DialogueSystemUISkinEditor : Editor
    {
        SkinNameList _skinNames = new SkinNameList();            

        bool _testFoldout;
        MessageType _testMessageType = MessageType.Info;
        string _testMessage = "Click on the test button to check if the skin is setup correctly.";


        void OnEnable()
        {
            DialogueSystemUISkin myTarget = (DialogueSystemUISkin)target;
            UpdateSkinNames(myTarget);
        }       

        void UpdateSkinNames(DialogueSystemUISkin myTarget)
        {
            int mode = (myTarget.UIType == DialogueUIType.Conversation) ? 0 : 1;
            _skinNames.CollectSkins(myTarget.Name, null, mode);
        }


        public override void OnInspectorGUI()
        {
            DialogueSystemUISkin myTarget = (DialogueSystemUISkin)target;
            float width = EditorGUIUtility.currentViewWidth;

            EditorGUIExtension.DrawInspectorTitle("Dialogue System UI Skin", "");

            // NAME
            if (_skinNames.DrawGUI())
            {
                myTarget.Name = SkinDatabase.Instance.Skins[_skinNames.Index].Name;
            }

            EditorGUI.BeginChangeCheck();
            myTarget.UIType = (DialogueUIType)EditorGUILayout.EnumPopup("UI Type", myTarget.UIType);
            if (EditorGUI.EndChangeCheck())
                UpdateSkinNames(myTarget);

            EditorGUILayout.Space();

            // NPC Details
            EditorGUIExtension.SimpleBox("UI Elements", 5, "", delegate ()
            {
                myTarget.ActorText = (UIElement)EditorGUILayout.ObjectField("Text", myTarget.ActorText, typeof(UIElement), true);

                GUILayout.Space(10);

                DrawOptions<UIElement>("Actor Name", ref myTarget.UseActorName, ref myTarget.ActorName);
                DrawOptions<UIElement>("Actor Portrait", ref myTarget.UseActorPortrait, ref myTarget.ActorPortrait);

                if (myTarget.UIType != DialogueUIType.AmbientDialogue)
                {
                    DrawOptions<UIElement>("Timer", ref myTarget.UseTimer, ref myTarget.Timer);
                }
            });

            EditorGUILayout.Space();

            if (myTarget.UIType != DialogueUIType.AmbientDialogue)
            {
                EditorGUIExtension.SimpleBox("Answers", 5, "", delegate ()
                {
                    myTarget.DynamicAnswerNumber = EditorGUILayout.Toggle("Dynamicaly Expand", myTarget.DynamicAnswerNumber);
                    EditorGUILayout.HelpBox("If Dynamicaly Expand is turned on, " +
                        "in runtime the system will generate additional answer buttons" +
                        "if there is more answer in the text node than button in the ui skin.", MessageType.Info);
                });

                EditorGUILayout.Space();
            }

            EditorGUIExtension.FoldoutBox("Test", ref _testFoldout, -1, delegate ()
            {
                EditorGUILayout.HelpBox(_testMessage, _testMessageType);
                if (GUILayout.Button("Test Skin"))
                    _testMessageType = (myTarget.Test(out _testMessage)) ? MessageType.Error : MessageType.Info;
            });

            EditorGUILayout.Space();
        }

        void DrawOptions<T>(string name, ref bool enabled, ref T value) where T : MonoBehaviour
        {
            enabled = EditorGUILayout.Toggle("Use " + name, enabled);
            if (enabled)
                value = (T)EditorGUILayout.ObjectField(name, value, typeof(T), true);
            GUILayout.Space(5);
        }
    }
}
