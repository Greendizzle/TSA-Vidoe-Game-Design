using Runemark.VisualEditor.Actions;
using UnityEngine;
using UnityEditor;
using Runemark.VisualEditor;
using Runemark.DialogueSystem.Playmaker;
using Runemark.Common;

namespace Runemark.DialogueSystem
{
	[CustomNodeLayout(typeof(PlayMakerEventCall), true)]
	public class PlayMakerEventLayout : DefaultLayout 
	{
		protected override string Title { get { return "Event Call"; } }

        Variable _fsmName;
        Variable _eventName;

        Texture2D _icon;

		public PlayMakerEventLayout(Node node) : base( node)
		{
			headerColor = new Color(0.26f, 0.00f, 0.26f, 1.00f);
			bodyColor = new Color(0,0,0,.50f);

			minBodyHeight = 40;
			width = 200;
			showTransitionInputLabel = false;
			showTransitionOutputLabel = false;

            _fsmName = node.Variables.GetByName("FsmName");
            _eventName = node.Variables.GetByName("EventName");

            _icon = Resources.Load<Texture2D>("Editor/Icons/PlayMakerFSM Icon");
		}

        protected override void onGUIHeader()
        {
            Rect iRect = new Rect(headerRect.x + 5, headerRect.y + 5 , headerRect.height, headerRect.height);
            GUI.Label(iRect, _icon);
            Rect tRect = new Rect(headerRect.x, headerRect.y+5, headerRect.width - iRect.width - 10, headerRect.height - 10);
            GUI.Label(tRect, Title, titleStyle);
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


	[CustomEditor(typeof(PlayMakerEventCall))]
	public class PlayMakerEventCallInspector : NodeInspector
	{
		protected override void onGUI()
		{
            PlayMakerEventCall myTarget = (PlayMakerEventCall)target;
            
            myTarget.Variables["FsmName"].Value = EditorGUILayout.TextField("Fsm Name (optional)", myTarget.Variables["FsmName"].ConvertedValue<string>());
            myTarget.Variables["EventName"].Value = EditorGUILayout.TextField("Event Name", myTarget.Variables["EventName"].ConvertedValue<string>());
		}
	}
}

