using UnityEditor;
using UnityEngine;
using Runemark.Common;

namespace Runemark.VisualEditor
{
	[CustomEditor(typeof(VisualEditorBehaviour), false)]
	public class VisualEditorBehaviourInspector : Editor
	{
		protected virtual string logoPath { get { return ""; }}
		protected virtual string graphLabel { get { return "Graph"; }}

		Texture2D _logo;

		void OnEnable()
		{
			_logo = Resources.Load<Texture2D>(logoPath);
		}


		public override void OnInspectorGUI()
		{
			GUILayout.Label(_logo);

			VisualEditorBehaviour myTarget = (VisualEditorBehaviour)target;

			myTarget.Graph = (FunctionGraph)EditorGUILayout.ObjectField(new GUIContent(graphLabel), myTarget.Graph, typeof(FunctionGraph), false);
            		
			myTarget.isDisabled = EditorGUILayout.Toggle("Is Disabled", myTarget.isDisabled);
            
			EditorGUIExtension.HorizontalLine(EditorGUIUtility.currentViewWidth);
            
		}
	}
}