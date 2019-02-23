using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Runemark.Common;

namespace Runemark.DialogueSystem
{
    public class SkinDatabase
    {
        public static SkinDatabase Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SkinDatabase();
                return _instance;
            }
        }
        static SkinDatabase _instance;

        int _lastIndex
        {
            get
            {
                if (!EditorPrefs.HasKey(_key + "_LastIndex"))
                    _lastIndex = -1;
                return EditorPrefs.GetInt(_key + "_LastIndex");
            }
            set { EditorPrefs.SetInt(_key + "_LastIndex", value); }
        }
        
        string _key = "DialogueSystemSkins";
        public List<SkinInfo> Skins { get; private set; }

        public SkinDatabase()
        {
            Skins = new List<SkinInfo>();
            if (EditorPrefs.HasKey(_key))
            {
                string s = EditorPrefs.GetString(_key);
                var skinIds = s.Split('|');

                foreach (string id in skinIds)
                    Skins.Add(new SkinInfo(id));
            }
            else
            {
                Default();
            }
        }
        

        public void Add()
        {
            _lastIndex++;
            string id = _key + "_" + _lastIndex;
            Skins.Add(new SkinInfo(id));            
            SaveList();            
        }

        public void Remove(int index)
        {
            Skins[index].OnRemove();
            Skins.RemoveAt(index);
            SaveList();
        }

        void SaveList()
        {
            List<string> ids = new List<string>();
            foreach (var skin in Skins)
                ids.Add(skin.ID);

            string data = string.Join("|", ids.ToArray());
            EditorPrefs.SetString(_key, data);
        }

        public void Clear()
        {
            foreach (var skin in Skins)
                skin.OnRemove();
            Skins.Clear();
            EditorPrefs.DeleteKey(_key + "_LastIndex");
            EditorPrefs.DeleteKey(_key);
        }

        public void Default()
        {
            Clear();
            Skins.Add(new SkinInfo(_key + "_0") { Name = "Witcher", IsAmbient = false });
            Skins.Add(new SkinInfo(_key + "_1") { Name = "Fantasy", IsAmbient = false });            
            Skins.Add(new SkinInfo(_key + "_2") { Name = "Basic Ambient", IsAmbient = true });
            Skins.Add(new SkinInfo(_key + "_3") { Name = "Book", IsAmbient = false });
            _lastIndex = 2;
            SaveList();
        }

    }

    public class SkinInfo
    {
        public readonly string ID;

        public SkinInfo(string id)
        {
            ID = id;
        }

        public string Name
        {
            get
            {
                if (ID == "") return "";

                string key = ID + "_Name";
                if (!EditorPrefs.HasKey(key))
                    Name = "";
                return EditorPrefs.GetString(key);
            }
            set
            {
                if (ID == "") return;
                string key = ID + "_Name";
                EditorPrefs.SetString(key, value);
            }
        }
        public bool IsAmbient
        {
            get
            {
                if (ID == "") return false;

                string key = ID + "_IsAmbient";
                if (!EditorPrefs.HasKey(key))
                    IsAmbient = false;
                return EditorPrefs.GetBool(key);
            }
            set
            {
                if (ID == "") return;
                string key = ID + "_IsAmbient";
                EditorPrefs.SetBool(key, value);
            }
        }

        public void OnRemove()
        {
            EditorPrefs.DeleteKey(ID + "_Name");
            EditorPrefs.DeleteKey(ID + "_IsAmbient");
        }

        public override string ToString()
        {
            return ID + "["+Name + "], ["+((IsAmbient) ? "Ambient" : "Conversation")+"]";
        }
    }

    public class UISkinWindow : EditorWindow
    {
        public System.Action onChange;

        ReorderableList _reorderableList;
        SkinDatabase _skins;
        
        [MenuItem("Window/Runemark/UI Skins")]
        public static UISkinWindow Open()
        {
            return GetWindow<UISkinWindow>();
        }

        void OnEnable()
        {
            //_settings = Resources.Load<DialogueSystemSettings>("Editor/DialogueSystemSettings");
            _skins = SkinDatabase.Instance;
            _reorderableList = new ReorderableList(_skins.Skins, typeof(string), true, true, true, true);


            _reorderableList.drawHeaderCallback = delegate (Rect rect)
            {
                EditorGUI.LabelField(rect, "UI Skins");
            };

            _reorderableList.drawElementCallback = delegate (Rect rect, int index, bool isActive, bool isFocused) 
            {
                

                Rect r = rect;
                r.width = r.width - 20;
                _skins.Skins[index].Name = EditorGUI.TextField(r, _skins.Skins[index].Name);

                r.x += r.width;
                r.width = 20;
                _skins.Skins[index].IsAmbient = EditorGUI.Toggle(r, _skins.Skins[index].IsAmbient);
            };

            _reorderableList.onAddCallback = delegate (ReorderableList list)
            {
                _skins.Add();
            };

            _reorderableList.onRemoveCallback = delegate (ReorderableList list)
            {
                if (EditorUtility.DisplayDialog("Warning!", "Are you sure you want to delete this skin name?", "Yes", "No"))
                {
                    _skins.Remove(list.index);
                }
            };
           
        }

        private void OnDisable()
        {
            if (onChange != null) onChange.Invoke();
            onChange = null;
        }

        void OnGUI()
        {
            EditorGUIExtension.DrawInspectorTitle("Dialogue System UI Skins", 
                "Here you can define skin names. The toggle button after the name"+
                " determines whether this skin is an Ambient Dialogue skin or not.");

            EditorGUI.BeginChangeCheck();
            _reorderableList.DoLayoutList();
            if (EditorGUI.EndChangeCheck())
            {
                if (onChange != null) onChange.Invoke();
            }

                GUILayout.BeginHorizontal();

                if (GUILayout.Button(new GUIContent("Clear", "This will clear the ui skin name list."), GUILayout.Width(50)) &&
                    EditorUtility.DisplayDialog("Warning!", "Are you sure you want to clear the ui skin name list?", "Yes", "No"))
                    _skins.Clear();

                if (GUILayout.Button(new GUIContent("Default", "This will reset the ui skin name list to the default setting."), GUILayout.Width(75)) &&
                    EditorUtility.DisplayDialog("Warning!", "Are you sure you want to set the ui skin name list to default?", "Yes", "No"))
                    _skins.Default();

                GUILayout.EndHorizontal();
    
        }
    }
}