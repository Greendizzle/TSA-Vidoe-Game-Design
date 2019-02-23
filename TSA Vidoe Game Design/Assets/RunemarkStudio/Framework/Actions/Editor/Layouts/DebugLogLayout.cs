using Runemark.VisualEditor.Actions;
using UnityEngine;
using UnityEditor;
using Runemark.VisualEditor.Utility;


namespace Runemark.VisualEditor
{
	[CustomNodeLayout(typeof(DebugLog), true)]
	public class DebugLogLayout : DefaultLayout 
	{
		protected override string Title { get { return "Debug Log"; } }

		public DebugLogLayout(Node node) : base(node)
		{
			headerColor = BuiltInColors.GetDark(typeof(string));
		}

		protected override void InitStyle()
		{
			base.InitStyle();

			var background = VisualEditorGUIStyle.GetTexture(BuiltInColors.GetDark(typeof(string)), new Color(0, 0, 0, 0), true, true, true, true);

			fieldStyle.normal.background = background.Texture;
			fieldStyle.active.background = background.Texture;
			fieldStyle.focused.background = background.Texture;
			fieldStyle.hover.background = background.Texture;
		}
	}


	[CustomEditor(typeof(DebugLog))]
	public class DebugLogInspector : NodeInspector
	{
		protected override void onGUI()
		{
			DebugLog myTarget = (DebugLog)target;
		}


	}
}

