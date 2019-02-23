using UnityEngine;
using UnityEditor;
using Runemark.VisualEditor.Utility;
using Runemark.VisualEditor.Actions;
using Runemark.Common;


namespace Runemark.VisualEditor
{ 
	[CustomNodeLayout(typeof(RandomNode), true)]
	public class RandomNodeLayout : DefaultLayout
	{ 
		protected override string Title
		{
			get
			{
				return "Random ("+_node.Variables.GetByName("Min").Value+
                    " - " + _node.Variables.GetByName("Max").Value+")";
			}
		}

        RandomNode _node { get { return ConvertNode<RandomNode>(); } }
        public RandomNodeLayout (Node node) : base(node)
		{
			width = 180;
			headerColor = BuiltInColors.GetDark(_node.Type);
			headerColor.a = 1f;

            _node.OnTypeChanged += OnTypeChanged;
        }

        void OnTypeChanged()
        {
            headerColor = BuiltInColors.GetDark(_node.Type);
        }
    } 


	[CustomEditor(typeof(RandomNode))]
	public class RandomNodeInspector : NodeInspector
	{
		protected override void onGUI()
		{
			RandomNode myTarget = (RandomNode)target;

            EditorGUIExtension.SimpleBox("", 5, "", delegate ()
            {
                var min = myTarget.Variables.GetByName("Min");
                var max = myTarget.Variables.GetByName("Max");

                VariableEditor.VariableField(min, null, true);
                VariableEditor.VariableField(max, null, true);
            });

            EditorGUILayout.Space();			
		}
	}
} 
