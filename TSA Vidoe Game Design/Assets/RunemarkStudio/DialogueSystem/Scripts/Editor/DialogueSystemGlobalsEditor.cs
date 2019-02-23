using Runemark.Common;
using Runemark.VisualEditor;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Runemark.DialogueSystem
{
    public class DialogueSystemGlobalsEditor : EditorWindow
    {
        VariableReorderableList _variableList;
        DialogueSystemGlobals _globals;

        Variable _selected;
        
        [MenuItem("Window/Runemark/Global Variables")]
        public static void Open()
        {
            GetWindow<DialogueSystemGlobalsEditor>();
        }

        void OnEnable()
        {
            var list = Resources.LoadAll<DialogueSystemGlobals>("");
            _globals = (list.Length > 0) ? list[0] : null;            
            if (_globals == null) return;

            _variableList = new VariableReorderableList(_globals.Variables.GetAll());
            _variableList.HeaderTitle = "Global Variables";
            _variableList.onVariableSelected = OnVariableSelected;
            _variableList.onVariableDeleted = OnVariableDeleted;            
        }

        void OnGUI()
        {
            EditorGUIExtension.DrawInspectorTitle("Dialogue System UI Globals", "");
                 
            // If globals doesn't exists create new one.      
            if (_globals == null)
            {
                float w = position.width;
                float h = position.height;
                Rect r = new Rect(20, 60, w - 40, 30);

                EditorGUI.HelpBox(r, "You have to create a new Dialogue System Global asset. ", MessageType.Warning);
                r.y += r.height + 10;
                if (GUI.Button(r, "Create"))
                {
                    string path = EditorUtility.SaveFilePanelInProject(
                                        "Create a new Dialogue System Global asset",
                                        "Dialogue System Global.asset",
                                        "asset", ""
                                        );
                    // Check if the path is a Resources folder.
                    if (!path.Contains("Resources"))
                    {
                        var pathArr = path.Split('/').ToList();
                        string filename = pathArr[pathArr.Count - 1];
                        pathArr.RemoveAt(pathArr.Count - 1);
                        path = string.Join("/", pathArr.ToArray());
                                 
                        if(!Directory.Exists(path + "/Resources"))
                            AssetDatabase.CreateFolder(path, "Resources");

                        path = path + "/Resources";
                        path = path + "/" + filename;
                    }
                    RunemarkDebug.Log("Dialogue System Global created. Path: " + path);
                    _globals = AssetCreator.CreateAsset<DialogueSystemGlobals>(path);
                    OnEnable();
                }                   
                return;
            }

            Rect rectLeft = new Rect(5, 45, 250, this.position.height - 50);
            Rect rectRight = new Rect(265, 45, position.width - 275, this.position.height - 50);
            
            EditorGUI.BeginChangeCheck();

            _variableList.Draw(rectLeft);
                        
            VariableEditor.OnInspectorGUI(rectRight, _selected);
            
            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(_globals);
            
        }

        void OnVariableSelected(string id, string name)
        {
            _selected = _globals.Variables.GetByID(id);
        }

        void OnVariableDeleted(string id, string name)
        {
            _selected = null;
            Repaint();
        }
    }
}
