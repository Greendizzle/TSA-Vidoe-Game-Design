using Runemark.VisualEditor.Actions;
using UnityEngine;
using UnityEditor;
using Runemark.VisualEditor.Utility;
using Runemark.Common;

namespace Runemark.VisualEditor
{
    [CustomNodeLayout(typeof(Operator), true)]
    public class OperatorLayout : CompactLayout
    {
        protected override string Title
        {
            get
            {
                string title = "";
                switch (_node.OperatorMode)
                {
                    case Operator.Mode.Add: title = "+"; break;
                    case Operator.Mode.Subtract: title = "-"; break;
                    case Operator.Mode.Multiply: title = "x"; break;
                    case Operator.Mode.Divide: title = "/"; break;
                    case Operator.Mode.Modulus: title = "%"; break;
                }
                return title;
            }
        }

        Operator _node { get { return ConvertNode<Operator>(); } }
        public OperatorLayout(Node node) : base(node)
        {
            headerColor = BuiltInColors.GetDark(_node.Type);
            headerColor.a = .8f;

            _node.OnTypeChanged += OnTypeChanged;
        }

        void OnTypeChanged()
        {
            headerColor = BuiltInColors.GetDark(_node.Type);
        }
    }

    [CustomEditor(typeof(Operator))]
    public class OperatorNodeInspector : NodeInspector
    {
        protected override void onGUI()
        {
            Operator myTarget = (Operator)target;

            EditorGUIExtension.SimpleBox("", 5, "", delegate ()
            {
                myTarget.OperatorMode = (Operator.Mode)EditorGUILayout.EnumPopup("Operator", myTarget.OperatorMode);
            });
        }


    }
}

