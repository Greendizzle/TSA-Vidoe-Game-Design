using Runemark.VisualEditor.Actions;
using UnityEngine;
using UnityEditor;
using Runemark.VisualEditor.Utility;


namespace Runemark.VisualEditor
{
	[CustomNodeLayout(typeof(EventCall), true)]
	public class EventCallLayout : DefaultLayout 
	{
		protected override string Title { get { return "Call Event"; } }

		Variable _eventName;

		public EventCallLayout(Node node) : base( node)
		{
			headerColor = new Color(0.43f, 0.10f, 0.10f, 1f);
			bodyColor = new Color(0,0,0,.50f);

			minBodyHeight = 40;
			width = 200;
			showTransitionInputLabel = false;
			showTransitionOutputLabel = false;

			_eventName = node.Variables.GetByName("EventName");
		}	

		protected override void onGUIBody()
		{
			base.onGUIBody();


			float x = bodyRect.x + Pin.PIN_SIZE + 10;
			float y = bodyRect.y + bodyRect.height / 2 - 15;
			float w = bodyRect.width - 2 * (Pin.PIN_SIZE + 10);
			float h = 30;

			GUI.Label(new Rect(x, y, w, h), _eventName.ConvertedValue<string>(), _textStyle);
		}

		GUIStyle _textStyle;
		protected override void InitStyle()
		{
			base.InitStyle();
			Color textColor = new Color(.8f, .8f, .8f, 1f);

			_textStyle = new GUIStyle(GUI.skin.label);
			_textStyle.fontSize = 16;
			_textStyle.normal.textColor = textColor;
			_textStyle.alignment = TextAnchor.MiddleCenter;
			_textStyle.wordWrap = true;
		}
	}


	[CustomEditor(typeof(EventCall))]
	public class EventCallLayoutInspector : NodeInspector
	{
		protected override void onGUI()
		{
			EventCall myTarget = (EventCall)target;

			myTarget.Variables["EventName"].Value = EditorGUILayout.TextField("Event Name", myTarget.Variables["EventName"].ConvertedValue<string>());
		}
	}
}

