using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Runemark.VisualEditor.Utility;
using UnityEditorInternal;
using Runemark.Common;

namespace Runemark.VisualEditor
{
    public class LocalVariableReorderableList : VariableReorderableList
    {
        public LocalVariableReorderableList(List<Variable> list) : base(list)  { }
        protected override void drawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = list[index];

            GUI.Box(new Rect(rect.x, rect.y + 7.5f, 20, 5), "", styles[element.type]);

            if (element.Name == "" || changeNameIndex == index)
            {
                element.Name = GUI.TextField(new Rect(rect.x + 25, rect.y, rect.width - 30, 20), element.Name);
                if (Event.current.isKey)
                {
                    switch (Event.current.keyCode)
                    {
                        case KeyCode.KeypadEnter:
                        case KeyCode.Return:
                            changeNameIndex = -1;
                            break;
                    }
                }

            }
            else
                GUI.Label(new Rect(rect.x + 25, rect.y, rect.width - 30, 20), element.Name);


            // OBSELOTE
          /*  if (GUI.Button(new Rect(rect.x + rect.width - 25, rect.y + 2.5f, 25, 15), "GET", buttonStyle) && onCreateRootNode != null)
                onCreateRootNode(element.ID, true);
            if (GUI.Button(new Rect(rect.x + rect.width - 52.5f, rect.y + 2.5f, 25, 15), "SET", buttonStyle))
                onCreateRootNode(element.ID, false);*/
        }
    }

	public class VariableReorderableList : ReorderableListGUI
	{
        #region Delegates     
        public delegate void OnVariableAction(string id, string name);
        public OnVariableAction onVariableDeleted;
        public OnVariableAction onVariableSelected;
        #endregion

        public bool IsDirty;

        public string HeaderTitle = "Local Variables";
        public override string Title { get { return HeaderTitle; } }
        public override bool UseAddDropdown { get { return true; } }

        protected List<Variable> list;

        protected Dictionary<System.Type, GUIStyle> styles = new Dictionary<System.Type, GUIStyle>();
        protected GUIStyle buttonStyle;

        List<System.Type> _availableTypes = new List<System.Type>()
            {
                typeof(int), typeof(float), typeof(string), typeof(bool)
            };
        
        int _selectedIndex = -1;
        double _lastSelectTime = 0;
        protected int changeNameIndex = -1;

        public VariableReorderableList(List<Variable> list) : base (list)
		{
            this.list = list;
        }

        protected override void drawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = list[index];

            GUI.Box(new Rect(rect.x, rect.y + 7.5f, 20, 5), "", styles[element.type]);

            if (element.Name == "" || changeNameIndex == index)
            {
                element.Name = GUI.TextField(new Rect(rect.x + 25, rect.y, rect.width - 30, 20), element.Name);
                if (Event.current.isKey)
                {
                    switch (Event.current.keyCode)
                    {
                        case KeyCode.KeypadEnter:
                        case KeyCode.Return:
                            changeNameIndex = -1;
                            break;
                    }
                }

            }
            else
                GUI.Label(new Rect(rect.x + 25, rect.y, rect.width - 30, 20), element.Name);
        }

        protected override bool onCanRemoveCallback(UnityEditorInternal.ReorderableList list)
        {
            return list.count > 0;
        }

        protected override void onRemoveCallback(UnityEditorInternal.ReorderableList list)
        {
            if (EditorUtility.DisplayDialog("Warning!", "Are you sure you want to delete this variable?", "Yes", "No"))
            {
                if (onVariableDeleted != null)
                    onVariableDeleted(this.list[list.index].ID, this.list[list.index].Name);
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
            }
        }

        protected override void onSelectCallback(UnityEditorInternal.ReorderableList list)
        {
            if (_selectedIndex == list.index && _lastSelectTime + 1f >= EditorApplication.timeSinceStartup)
                changeNameIndex = list.index;

            _selectedIndex = list.index;
            _lastSelectTime = EditorApplication.timeSinceStartup;

            if (onVariableSelected != null)
                onVariableSelected(this.list[list.index].ID, this.list[list.index].Name);
        }

        protected override void onAddDropdownCallback(UnityEngine.Rect buttonRect, UnityEditorInternal.ReorderableList list)
        {
            var menu = new GenericMenu();
            foreach (var t in _availableTypes)
                menu.AddItem(new GUIContent(TypeUtils.GetPrettyName(t)), false, clickHandler, t);
            menu.ShowAsContext();
        }

        protected override void onReorderCallback(ReorderableList list)
        {
            IsDirty = true;
        }

        void clickHandler(object target)
        {
            list.Add(new Variable((System.Type)target));
        }

        // Not using this callback.
        protected override void onAddCallback(UnityEditorInternal.ReorderableList list) { }

        protected override void InitGUIStyles()
        {
            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 9;
            buttonStyle.margin = new RectOffset(0, 0, 0, 0);
            buttonStyle.padding = new RectOffset(0, 0, 0, 0);

            foreach (var t in _availableTypes)
            {
                var color = BuiltInColors.Get(t);
                var tex = VisualEditorGUIStyle.GetTexture(color, color, true, true, true, true);

                GUIStyle s = new GUIStyle(GUI.skin.box);
                s.normal.background = tex.Texture;
                s.border = tex.BorderOffset;
                styles.Add(t, s);
            }
            base.InitGUIStyles();
        }

                   
        
	}     
}