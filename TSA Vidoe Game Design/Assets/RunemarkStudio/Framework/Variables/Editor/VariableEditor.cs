using Runemark.Common;
using Runemark.VisualEditor;
using UnityEditor;
using UnityEngine;

namespace Runemark.VisualEditor
{
    public static class VariableEditor
    {
        public static void OnInspectorGUI(Variable variable)
        {
            if (variable == null) return;

            EditorGUIExtension.SimpleBox("", 5, "", delegate ()
            {
                variable.Name = EditorGUILayout.TextField("Name", variable.Name);
                variable.Group = EditorGUILayout.TextField("Group", variable.Group);

                EditorGUILayout.Space();

                variable.DefaultValue.Value = EditorGUIExtension.SmartField("Default Value", variable.type, variable.DefaultValue.Value);

                variable.Save = EditorGUILayout.Toggle("Saveable", variable.Save);
            });
        
        }
        public static void OnInspectorGUI(Rect rect, Variable variable)
        {
            EditorGUIExtension.SimpleBox(rect, "", 5, "", delegate (Rect dRect)
            {
                if (variable != null)
                {
                    Rect r = dRect;
                    r.x += 5;
                    r.width -= 10;
                    r.height = 20;

                    variable.Name = EditorGUI.TextField(r, "Name", variable.Name);
                    r.y += 30;

                    variable.Group = EditorGUI.TextField(r, "Group", variable.Group);
                    r.y += 30;

                    r.y += 5;

                    variable.DefaultValue.Value = EditorGUIExtension.SmartField(r, "Default Value", variable.type, variable.DefaultValue.Value);
                    r.y += 30;

                    variable.Save = EditorGUI.Toggle(r, "Save", variable.Save);
                }
            });

        }

        public static void VariableField(Variable variable, GUIStyle style = null, bool showLabel = false)
        {
            try
            {
                string label = (showLabel) ? variable.Name : "";

                if (variable.type == typeof(string))
                {
                    if (style == null) style = GUI.skin.textField;
                    variable.Value = EditorGUILayout.TextField(label, variable.ConvertedValue<string>(), style);
                }
                else if (variable.type == typeof(int))
                {
                    if (style == null) style = GUI.skin.textField;
                    variable.Value = EditorGUILayout.IntField(label, variable.ConvertedValue<int>(), style);
                }
                else if (variable.type == typeof(float))
                {
                    if (style == null) style = GUI.skin.textField;
                    variable.Value = EditorGUILayout.FloatField(label, variable.ConvertedValue<float>(), style);
                }
                else if (variable.type == typeof(bool))
                {
                    variable.Value = EditorGUILayout.Toggle(label, variable.ConvertedValue<bool>());
                }
                else
                {
                    if (style == null) style = GUI.skin.label;
                    EditorGUILayout.LabelField(label, "[" + variable.type + "]", style);
                }
            }
            catch (System.InvalidCastException e)
            {
                RunemarkDebug.Error(e.Message);
            }
        }
        public static void VariableField(Rect rect, Variable variable, GUIStyle style, bool showLabel = false)
        {
            try
            {
                string label = (showLabel) ? variable.Name : "";
                if (variable.type == typeof(string))
                {
                    if (style == null) style = GUI.skin.textField;
                    variable.Value = EditorGUI.TextField(rect, label, variable.ConvertedValue<string>(), style);
                }
                else if (variable.type == typeof(int))
                {
                    if (style == null) style = GUI.skin.textField;
                    variable.Value = EditorGUI.IntField(rect, label, variable.ConvertedValue<int>(), style);
                }
                else if (variable.type == typeof(float))
                {
                    if (style == null) style = GUI.skin.textField;
                    variable.Value = EditorGUI.FloatField(rect, label, variable.ConvertedValue<float>(), style);
                }
                else if (variable.type == typeof(bool))
                {
                    variable.Value = EditorGUI.Toggle(rect, label, variable.ConvertedValue<bool>());
                }
                else
                {
                    if (style == null) style = GUI.skin.label;
                    EditorGUI.LabelField(rect, label, "[" + variable.type + "]", style);
                }               
            }
            catch (System.InvalidCastException e)
            {
                RunemarkDebug.Error(e.Message);
            }
        }
    }
}