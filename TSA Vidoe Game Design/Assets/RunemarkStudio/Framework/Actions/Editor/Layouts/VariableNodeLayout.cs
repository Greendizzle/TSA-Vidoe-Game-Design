using Runemark.VisualEditor;
using Runemark.VisualEditor.Actions;
using Runemark.VisualEditor.Utility;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Runemark.DialogueSystem
{
    [CustomNodeLayout(typeof(GetVariableNode), true)]
    public class GetVariableNodeLayout : CompactLayout
    {
        protected override string Title
        {
            get
            {
                return string.Format("Get {0}", _myTarget.Scope);
            }
        }
        GetVariableNode _myTarget;

        public GetVariableNodeLayout(Node node) : base(node)
        {
            _myTarget = (GetVariableNode)node;
            width = 200;
            minHeaderHeight = 40;

            OnRepaint();
        }
        public override void OnRepaint()
        {
            var pin = Node.PinCollection.Get("Value");
            Color c = BuiltInColors.GetDark(pin.VariableType);
            headerColor = c;
        }

        protected GUIStyle nameStyle;
        protected override void InitStyle()
        {
            base.InitStyle();
            titleStyle.fontSize = 16;
            titleStyle.alignment = TextAnchor.UpperCenter;

            nameStyle = new GUIStyle(titleStyle);
            nameStyle.fontSize = 12;
            nameStyle.wordWrap = true;
            nameStyle.alignment = TextAnchor.LowerCenter;
            nameStyle.fontStyle = FontStyle.Normal;

            var pin = Node.PinCollection.Get("Value");
            Color c = BuiltInColors.GetDark(pin.VariableType);

            var background = VisualEditorGUIStyle.GetTexture(c, new Color(0, 0, 0, 0), true, true, true, true);

            fieldStyle.normal.background = background.Texture;
            fieldStyle.active.background = background.Texture;
            fieldStyle.focused.background = background.Texture;
            fieldStyle.hover.background = background.Texture;
        }

        protected override void onGUIHeader()
        {
            base.onGUIHeader();
            Rect rect = new Rect(headerRect.x,
                                 headerRect.y + headerRect.height - 30,
                                 width,
                                 30
                                 );
            GUI.Label(rect, _myTarget.VariableName, nameStyle);

        }
    }
    [CustomEditor(typeof(GetVariableNode))]
    public class GetVariableNodeInspector : VariableNodeInspector
    {
        private void OnEnable()
        {
            GetVariableNode myTarget = (GetVariableNode)target;
            CollectVariableNames(myTarget, myTarget.Scope);           
        }     
    }

    [CustomNodeLayout(typeof(SetVariableNode), true)]
    public class SetVariableNodeLayout : DefaultLayout
    {
        protected override string Title
        {
            get
            {
                return string.Format("Set {0}", _myTarget.Scope);
            }
        }
        SetVariableNode _myTarget;

        public SetVariableNodeLayout(Node node) : base(node)
        {
            _myTarget = (SetVariableNode)node;

            OnRepaint();

            showInputLabel = false;
            showOutputLabel = false;

            width = 200;
            minHeaderHeight = 40;
        }
        public override void OnRepaint()
        {
            var value = Node.PinCollection.Get("Value");
            Color c = BuiltInColors.GetDark(value.VariableType);
            headerColor = c;
        }
        protected override void onGUIHeader()
        {
            base.onGUIHeader();
            Rect rect = new Rect(headerRect.x,
                                 headerRect.y + headerHeight - 20,
                                 width,
                                 20
                                 );
            GUI.Label(rect, _myTarget.VariableName, nameStyle);
        }
        
        protected GUIStyle nameStyle;
        protected override void InitStyle()
        {
            base.InitStyle();
            titleStyle.fontSize = 16;
            titleStyle.alignment = TextAnchor.UpperCenter;

            nameStyle = new GUIStyle(titleStyle);
            nameStyle.fontSize = 12;
            nameStyle.wordWrap = true;
            nameStyle.alignment = TextAnchor.LowerCenter;
            nameStyle.fontStyle = FontStyle.Normal;

            var value = Node.PinCollection.Get("Value");
            Color c = BuiltInColors.GetDark(value.VariableType);

            var background = VisualEditorGUIStyle.GetTexture(c, new Color(0, 0, 0, 0), true, true, true, true);

            fieldStyle.normal.background = background.Texture;
            fieldStyle.active.background = background.Texture;
            fieldStyle.focused.background = background.Texture;
            fieldStyle.hover.background = background.Texture;
        }
    }
    [CustomEditor(typeof(SetVariableNode))]
    public class SetVariableNodeInspector : VariableNodeInspector
    {
        private void OnEnable()
        {
            SetVariableNode myTarget = (SetVariableNode)target;
            CollectVariableNames(myTarget, myTarget.Scope);
        }
    }


    public class VariableNodeInspector : NodeInspector
    {
        string[] _variables;
        int _selected = 0;

        protected void CollectVariableNames(IVariableNode node, VariableScope scope)
        {
            VariableCollection variables = null;
            if (scope == VariableScope.Local)
                variables = node.Root.Variables;
            else if (scope == VariableScope.Global)
            {
                var gList = Resources.LoadAll<DialogueSystemGlobals>("");
                var globals = (gList.Length > 0) ? gList[0] : null;
                if (globals != null)
                   variables = globals.Variables;
            }

            List<string> list = new List<string>();
            int cnt = 0;
            foreach (var g in variables.GetAll())
            {
                if (node.VariableName == g.Name)
                    _selected = cnt;
                list.Add(g.Name);
                cnt++;
            }
            _variables = list.ToArray();
        }

        protected override void onGUI()
        {
            IVariableNode myTarget = (IVariableNode)target;

            EditorGUI.BeginChangeCheck();
            myTarget.ChangeScope((VariableScope)EditorGUILayout.EnumPopup("Scope", myTarget.Scope));
            if (EditorGUI.EndChangeCheck())
                CollectVariableNames(myTarget, myTarget.Scope);

            EditorGUI.BeginChangeCheck();
            _selected = EditorGUILayout.Popup("Variable", _selected, _variables);
            if (EditorGUI.EndChangeCheck())
                myTarget.ChangeVariable(_variables[_selected]);
        }
    }
}