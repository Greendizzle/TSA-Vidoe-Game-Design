using Runemark.VisualEditor.Actions;
using UnityEngine;
using UnityEditor;
using Runemark.VisualEditor.Utility;


namespace Runemark.VisualEditor
{
	[CustomNodeLayout(typeof(Branch), true)]
	public class BranchLayout : DefaultLayout 
	{
		public BranchLayout(Node node) : base(node)
		{
			headerColor = new Color(0.43f, 0.43f, 0.42f, 1f);
		}
	}


	[CustomEditor(typeof(Branch))]
	public class BranchInspector : NodeInspector
	{
		protected override void onGUI()
		{
			Branch myTarget = (Branch)target;
		}


	}
}

