using Runemark.VisualEditor.Actions;
using UnityEngine;
using UnityEditor;
using Runemark.VisualEditor.Utility;
using Runemark.Common;

namespace Runemark.VisualEditor
{
	[CustomNodeLayout(typeof(Logical), true)]
	public class LogicalLayout : CompactLayout 
	{
        protected override string Title
        {
            get
            {
                return _node.LogicalMode.ToString();
            }
        }

        Logical _node { get { return ConvertNode<Logical>(); } }
		public LogicalLayout(Node node) : base(node)
		{			
			headerColor = BuiltInColors.GetDark(typeof(bool));
			headerColor.a = .8f;
		}

        public override void OnRepaint()
        {
            MapConnections();
        }             
    }


	[CustomEditor(typeof(Logical))]
	public class LogicalNodeInspector : NodeInspector
	{
		protected override void onGUI()
		{
			Logical myTarget = (Logical)target;

            EditorGUIExtension.SimpleBox("", 5, "", delegate ()
            {
                myTarget.LogicalMode = (Logical.Mode)EditorGUILayout.EnumPopup("Logical", myTarget.LogicalMode);                
            });
        }


	}
}

