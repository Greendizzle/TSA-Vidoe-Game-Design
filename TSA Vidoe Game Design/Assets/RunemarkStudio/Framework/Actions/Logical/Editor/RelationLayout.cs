using Runemark.VisualEditor.Actions;
using UnityEngine;
using UnityEditor;
using Runemark.VisualEditor.Utility;
using Runemark.Common;

namespace Runemark.VisualEditor
{
	[CustomNodeLayout(typeof(Relation), true)]
	public class RelationLayout : CompactLayout 
	{
        protected override string Title
        {
            get
            {
                string title = "";
                switch (_node.RelationMode)
                {
                    case Relation.Mode.Less: title = "<"; break;
                    case Relation.Mode.LessOrEqual: title = "<="; break;
                    case Relation.Mode.Equals: title = "=="; break;
                    case Relation.Mode.NotEqual: title = "!="; break;
                    case Relation.Mode.GreaterOrEqual: title = ">="; break;
                    case Relation.Mode.Greater: title = ">"; break;
                }
                return title;
            }
        }

        Relation _node { get { return ConvertNode<Relation>(); } }

		public RelationLayout(Node node) : base(node)
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


	[CustomEditor(typeof(Relation))]
	public class RelationNodeInspector : NodeInspector
	{
        protected override void onGUI()
        {
            Relation myTarget = (Relation)target;

            EditorGUIExtension.SimpleBox("", 5, "", delegate ()
            {
                var rm = myTarget.RelationMode;            
                myTarget.RelationMode = (Relation.Mode)EditorGUILayout.EnumPopup("Relation", myTarget.RelationMode);
                if (rm != myTarget.RelationMode)
                    ResetTypePopup();
            });
        }

    }
}

