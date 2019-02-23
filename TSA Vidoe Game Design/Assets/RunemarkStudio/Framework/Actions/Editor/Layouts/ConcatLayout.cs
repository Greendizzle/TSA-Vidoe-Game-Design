using Runemark.VisualEditor.Actions;
using UnityEngine;
using UnityEditor;
using Runemark.VisualEditor.Utility;


namespace Runemark.VisualEditor
{
	[CustomNodeLayout(typeof(Concatenate), true)]
	public class ConcatenateLayout : CompactLayout 
	{
		protected override string Title { get { return "CONCAT"; } }

		public ConcatenateLayout(Node node) : base(node)
		{			
			headerColor = BuiltInColors.DarkString;
			headerColor.a = .8f;
		}
	}


	[CustomEditor(typeof(Concatenate))]
	public class ConcatenateNodeInspector : NodeInspector
	{
		protected override void onGUI()
		{
			Concatenate myTarget = (Concatenate)target;
		}


	}
}

