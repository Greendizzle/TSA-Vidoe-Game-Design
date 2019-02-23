using UnityEngine;
using UnityEditor;
using Runemark.Common;
using Runemark.VisualEditor;

namespace Runemark.DialogueSystem
{
	[CustomEditor(typeof(DialogueBehaviour), true)]
	public class DialogueBehaviourInspector : Editor
	{
		protected virtual string graphLabel { get { return "Dialogue"; }}

        ReorderableListGUI _cameras;

		GUIStyle _titleStyle;

        SkinNameList _skinNames = new SkinNameList();

        void OnEnable()
		{
			DialogueBehaviour myTarget = (DialogueBehaviour)target;
			//_cameras = new SimpleObjectReorderableList<Camera>("Cameras", myTarget.Conversation.CameraController.CameraList);

            _skinNames.CollectSkins(myTarget.Conversation.DefaultSkin, null, 0);
            myTarget.Conversation.CameraController.OnEnable();
        }


		public override void OnInspectorGUI()
		{
			if (_titleStyle == null)
			{
				_titleStyle = new GUIStyle(EditorStyles.boldLabel);
				_titleStyle.alignment = TextAnchor.MiddleCenter;
			}
		
			DialogueBehaviour myTarget = (DialogueBehaviour)target;
			float width = EditorGUIUtility.currentViewWidth;

            EditorGUIExtension.DrawInspectorTitle("Dialogue Behaviour",
                "This component compiles the dialogue graph in runtime, also it "+
                "commincates with the ui and other systems.");
	

			// DIALOGUE GRAPH
			GUILayout.BeginVertical("box");
			EditorGUILayout.Space();
                        
            myTarget.Graph = (DialogueGraph)EditorGUILayout.ObjectField(new GUIContent(graphLabel), myTarget.Graph, typeof(DialogueGraph), false);

			if (myTarget.Graph == null)
				EditorGUILayout.HelpBox("Step 1: Select a dialogue", MessageType.Error);

			EditorGUILayout.Space();
			GUILayout.EndVertical();

			if (myTarget.Graph == null)	return;

			EditorGUILayout.Space();

            // CUSTOM ACTOR
            EditorGUIExtension.FoldoutBox("Actor", ref myTarget.ActorFoldout,
                                         (myTarget.ActorEnabled) ? 1 : 0, delegate ()
            {
                EditorGUI.indentLevel--;

                myTarget.ActorEnabled = EditorGUILayout.Toggle("Enable", myTarget.ActorEnabled);

                if (myTarget.ActorEnabled)
                {
                    GUIContent label = new GUIContent("Actor Name", "The dialogue system will use this name as Actor name. If it's not set, the name will be the name of the GameObject.");
                    myTarget.ActorName = EditorGUILayout.TextField(label, myTarget.ActorName);
                    myTarget.ActorPortrait = (Sprite)EditorGUILayout.ObjectField("Portrait", myTarget.ActorPortrait, typeof(Sprite), false);
                }

                EditorGUI.indentLevel++;
            });

            EditorGUILayout.Space();

            // CONVERSATION	
            EditorGUIExtension.FoldoutBox("Conversation", ref myTarget.ConversationFoldout,
									      (myTarget.Conversation.Enabled) ? 1 : 0, delegate() {		

				EditorGUI.indentLevel--;

				myTarget.Conversation.Enabled = EditorGUILayout.Toggle("Enable", myTarget.Conversation.Enabled);

                // CONVERSATION
				if (myTarget.Conversation.Enabled)
				{
					EditorGUILayout.Space();

					myTarget.Conversation.Trigger.Mode = (Trigger.Modes)EditorGUILayout.EnumPopup("Trigger", myTarget.Conversation.Trigger.Mode);

					EditorGUI.indentLevel++;

					// Player Tag
					if (myTarget.Conversation.Trigger.Mode == Trigger.Modes.TriggerEnter || myTarget.Conversation.Trigger.Mode == Trigger.Modes.Use)
						myTarget.Conversation.Trigger.PlayerTag = EditorGUILayout.TagField("Player Tag", myTarget.Conversation.Trigger.PlayerTag);

					// Distance
					if (myTarget.Conversation.Trigger.Mode == Trigger.Modes.Use)
						myTarget.Conversation.Trigger.Distance = EditorGUILayout.FloatField("Distance", myTarget.Conversation.Trigger.Distance); 

					if (myTarget.Conversation.Trigger.Mode == Trigger.Modes.Custom)
						EditorGUILayout.HelpBox("You can call the StartDialogue() method from script directly.", MessageType.Info);

                    // Use Auto Close
                    if (myTarget.Conversation.Trigger.Mode != Trigger.Modes.OnStart)
                    {
                        myTarget.Conversation.UseAutoExit = EditorGUILayout.Toggle(new GUIContent("Use Auto Exit"), myTarget.Conversation.UseAutoExit);
                        if (myTarget.Conversation.UseAutoExit)
                        {
                            myTarget.Conversation.ExitDelay = EditorGUILayout.FloatField(new GUIContent("Exit Delay", "You can set a number of second by with you can delay the close of the dialogue. 0 means the conversation closes instantly."), myTarget.Conversation.ExitDelay);

                            string s = "";
                            if (myTarget.Conversation.Trigger.Mode == Trigger.Modes.TriggerEnter)
                                s = " exitst the trigger.";
                            if (myTarget.Conversation.Trigger.Mode == Trigger.Modes.Use)
                                s = " is further from the actor than the use distance.";
                            if (myTarget.Conversation.Trigger.Mode == Trigger.Modes.Custom)
                                s = " is further from the actor than the exit distance.";
                            EditorGUILayout.HelpBox("The dialogue will close if the player" + s, MessageType.Info);
                        }
                     }
                    
                    // Exit Distance
                    if (myTarget.Conversation.Trigger.Mode == Trigger.Modes.Custom)
                        myTarget.Conversation.ExitDistance = EditorGUILayout.FloatField(new GUIContent("Exit Distance", "Distance between the player and the actor, when the dialogue should be closed."),
                                                                                        myTarget.Conversation.ExitDistance);


                    EditorGUI.indentLevel--;
					EditorGUILayout.Space();

                    // CONVERSATION - PLAYER POSITION
					myTarget.Conversation.OverridePlayerPosition = EditorGUILayout.Toggle(new GUIContent("Override Player Position", "You can specify a position for the player during this conversation"), myTarget.Conversation.OverridePlayerPosition);
					if (myTarget.Conversation.OverridePlayerPosition)
					{
						EditorGUI.indentLevel++;
						myTarget.Conversation.PlayerTag = EditorGUILayout.TagField("Player Tag", myTarget.Conversation.PlayerTag);
						myTarget.Conversation.PlayerPosition = (Transform)EditorGUILayout.ObjectField("Position", myTarget.Conversation.PlayerPosition, typeof(Transform), true);
						EditorGUI.indentLevel--;
					}
					EditorGUILayout.Space();

                    // CONVERSATION - CUSTOM CAMERA
                    myTarget.Conversation.CameraController.DrawInspectorGUI();

					/*myTarget.Conversation.CameraController.Enable = EditorGUILayout.Toggle("Use Custom Camera(s)", myTarget.Conversation.CameraController.Enable);
					if (myTarget.Conversation.CameraController.Enable)
					{
						EditorGUILayout.HelpBox("Default camera is the one that is used when the Text Node returns -1 as camera index. If the default camera index is -1, it uses the main camera.", MessageType.Info);

						myTarget.Conversation.CameraController.DefaultIndex = EditorGUILayout.IntSlider("Default Camera Index", myTarget.Conversation.CameraController.DefaultIndex, -1, myTarget.Conversation.CameraController.CameraList.Count-1);						
						_cameras.Draw();
					}*/

                    EditorGUILayout.Space();

                    // CONVERSATION - SKIN
                    myTarget.Conversation.UseDefaultSkin = EditorGUILayout.Toggle("Use Custom Skin", myTarget.Conversation.UseDefaultSkin);
                    if (myTarget.Conversation.UseDefaultSkin)
                    {
                        if (_skinNames.DrawGUI())
                        {
                            myTarget.Conversation.DefaultSkin = SkinDatabase.Instance.Skins[_skinNames.Index].Name;
                        }                          
                    } // +Culling
                 }
				EditorGUI.indentLevel++;
			});

			EditorGUILayout.Space();

			// BARKING
			EditorGUIExtension.FoldoutBox("Ambient Dialogue (Barking)", ref myTarget.BarkFoldout,
											(myTarget.AmbientDialogue.Enabled) ? 1 : 0, delegate() {	
				EditorGUI.indentLevel--;

				myTarget.AmbientDialogue.Enabled = EditorGUILayout.Toggle("Enable", myTarget.AmbientDialogue.Enabled);

				if (myTarget.AmbientDialogue.Enabled)
				{
					EditorGUILayout.Space();

                    myTarget.AmbientDialogue.Once = EditorGUILayout.Toggle(new GUIContent("Once", "If this option is turned on, the ambient dialogue only appears once when it's triggered"), myTarget.AmbientDialogue.Once);
                    if(!myTarget.AmbientDialogue.Once)
					    myTarget.AmbientDialogue.Time = EditorGUILayout.FloatField(new GUIContent("Time", "The ambient dialogue will activate after this amount of seconds (if not active currently)"), myTarget.AmbientDialogue.Time);

					EditorGUILayout.Space();

					myTarget.AmbientDialogue.Trigger.Mode = (Trigger.Modes)EditorGUILayout.EnumPopup("Trigger", myTarget.AmbientDialogue.Trigger.Mode);
					EditorGUI.indentLevel++;

					// Player Tag
					if (myTarget.AmbientDialogue.Trigger.Mode == Trigger.Modes.TriggerEnter || myTarget.AmbientDialogue.Trigger.Mode == Trigger.Modes.Use)
						myTarget.AmbientDialogue.Trigger.PlayerTag = EditorGUILayout.TagField("Player Tag", myTarget.AmbientDialogue.Trigger.PlayerTag);

					// Distance
					if (myTarget.AmbientDialogue.Trigger.Mode == Trigger.Modes.Use)
						myTarget.AmbientDialogue.Trigger.Distance = EditorGUILayout.FloatField("Distance", myTarget.AmbientDialogue.Trigger.Distance); 

					if (myTarget.AmbientDialogue.Trigger.Mode == Trigger.Modes.Custom)
						EditorGUILayout.HelpBox("You can call the StartBark() method from script directly.", MessageType.Info);
                                                    EditorGUI.indentLevel--;
                                                    
                    // OFFSET
                    EditorGUILayout.Space();
                    myTarget.AmbientDialogue.Offset = EditorGUILayout.Vector3Field(new GUIContent("Offset", "The offset position for the ambient dialogue ui."), myTarget.AmbientDialogue.Offset);
                    EditorGUILayout.Space();                



					// +Culling
				}
				EditorGUI.indentLevel++;

				});	

			EditorGUILayout.Space();

			// Events
			EditorGUIExtension.FoldoutBox("External Events " +((myTarget.ExternalEventEnable) ? "["+myTarget.Events.Count+"]" : ""), ref myTarget.ExternalEventFoldout,
										 (myTarget.ExternalEventEnable) ? 1 : 0, delegate() {	

				EditorGUI.indentLevel--;

				myTarget.ExternalEventEnable = EditorGUILayout.Toggle("Enable", myTarget.ExternalEventEnable);

				if (myTarget.ExternalEventEnable)
				{
					EditorGUILayout.Space();

					for ( int cnt = 0; cnt < myTarget.Events.Count; cnt++)
					{
						var e = serializedObject.FindProperty("Events").GetArrayElementAtIndex(cnt);

						EditorGUIExtension.SimpleBox("", 5, "ShurikenModuleBG", delegate() {

							EditorGUILayout.BeginHorizontal();
							myTarget.Events[cnt].EventName = EditorGUILayout.TextField(new GUIContent("Event Name", "This is the name you want to call from the graph"), myTarget.Events[cnt].EventName);
							if(GUILayout.Button("X", GUILayout.Width(20)))
							{
								myTarget.Events.RemoveAt(cnt);
								return;
							}
							EditorGUILayout.EndHorizontal();

							EditorGUILayout.PropertyField(e.FindPropertyRelative("uEvent"), new GUIContent("Event: "+ myTarget.Events[cnt].EventName));
						});

						GUILayout.Space(10);
					}

                    serializedObject.ApplyModifiedProperties();

					if(GUILayout.Button("Add Event"))
					{
						myTarget.Events.Add(new DialogueBehaviour.ExternalEvent());
						Repaint();
					}
              }

			});	
		}
	}
}