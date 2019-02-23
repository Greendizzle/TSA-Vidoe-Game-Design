using UnityEngine;
using UnityEditor;
using Runemark.VisualEditor.Utility;
using Runemark.VisualEditor.Actions;
using Runemark.VisualEditor;
using Runemark.Common;
using System.Collections.Generic;
using Runemark.Common.Utility;
using System.Linq;

namespace Runemark.DialogueSystem
{ 
	[CustomNodeLayout(typeof(TextNode), true)]
	public class TextNodeLayout : NodeLayout
	{ 
		protected sealed override float headerHeight { get { return 30; } }
		protected sealed override float bodyHeight 
		{
			get
			{ 
				return Mathf.Max(minBodyHeight, 
					_textHeight + 30 + 	_node.Answers.Count * (Pin.PIN_SIZE+5) + 10); 
			}
		}

		protected float minBodyHeight;
		Pin _iTrans;
		float _textHeight = 100;

		TextNode _node { get { return ConvertNode<TextNode>(); }}

		int _lastAnswerCnt = -1;

        Texture2D _portrait;
        string _portraitName;

		public TextNodeLayout(TextNode node) : base(node)
		{
			headerColor = new Color(0.27f, 0.43f, 0.53f, 1f);
			bodyColor = new Color(0, 0, 0, 1f);
	
			width = 300f;
			minBodyHeight = _textHeight + 100f;


			_iTrans = transitions.Find(x => x.PinType == PinType.TransIn);
		}

		protected override void onGUIHeader()
		{		
			GUI.Label(headerRect, Title, titleStyle);

            // Portrait      
            if (_node.Portrait != null)
            {
                // Get the portrait texture, if it's changed.
                if (_portraitName != _node.Portrait.name)
                {
                    string spriteSheet = AssetDatabase.GetAssetPath(_node.Portrait);
                    Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheet).OfType<Sprite>().ToArray();

                    if (sprites.Length == 1)
                        _portrait = _node.Portrait.texture;
                    else
                    {
                        var path = IOUtility.GetPath(_node.Portrait);
                        var importer = (TextureImporter)TextureImporter.GetAtPath(path);
                        if (importer.isReadable)
                        {
                            _portrait = new Texture2D((int)_node.Portrait.rect.width, (int)_node.Portrait.rect.height);
                            var pixels = _node.Portrait.texture.GetPixels((int)_node.Portrait.textureRect.x,
                                                                    (int)_node.Portrait.textureRect.y,
                                                                    (int)_node.Portrait.textureRect.width,
                                                                    (int)_node.Portrait.textureRect.height);
                            _portrait.SetPixels(pixels);
                            _portrait.Apply();
                        }
                        else
                            _portrait = _node.Portrait.texture;                  
                    }

                    _portraitName = _node.Portrait.name;
                }

                Rect r = new Rect(headerRect.x - 30, headerRect.y - 30, 60, 60);
                GUI.Label(r, _portrait);
            }

            // Actor Name
            if (_node.ActorName != "")
            {
                GUIContent label = new GUIContent(_node.ActorName);
                Vector2 size = _actorNameStyle.CalcSize(label);
                Color c = headerColor; c.a = .5f;

                float xPos = headerRect.x;
                if (_node.Portrait != null) xPos += 30;          

                Rect r = new Rect(xPos + 5, headerRect.y - size.y - 10, size.x + 10, size.y + 5);
                EditorGUI.DrawRect(r, c);
                GUI.Label(r, label, _actorNameStyle);
            }

            float x = headerRect.x + headerRect.width - 5;
            float y = headerRect.y + (headerRect.height - 20) / 2;


            // Audio Clip
            if (_node.Audio != null)
            {
                GUI.Label(new Rect(x - 20, y, 20, 20), EditorGUIUtility.IconContent("SceneviewAudio"));
                x -= 30;
            }

            // Custom Camera
            if (_node.CustomCameraEnable)
                GUI.Label(new Rect(x-20, y - 3, 40, 20), new GUIContent(_node.CameraIndex.ToString(), EditorGUIUtility.IconContent("Camera Icon").image), textStyle);
                             
        }

        protected override void onGUIBody()
		{	
			Rect r = new Rect(bodyRect.x + 10, bodyRect.y + 10, Pin.PIN_SIZE, Pin.PIN_SIZE);
		
			// Transition Input
			DrawPin(r, _iTrans, Vector2.left, true);
			// Text
			GUI.Label(new Rect(r.x + r.width + 10, r.y, width - r.width - 30, _textHeight), _node.Text, textStyle);
			r.y += _textHeight;


			// Answers
			if (_node.Answers.Count > 0)
			{
				Color c = headerColor;
				c.a = c.a / 2;
				EditorGUI.DrawRect(new Rect(bodyRect.x, r.y, width, 25), c);
				GUI.Label(new Rect(bodyRect.x, r.y, width, 25), "Answers:", _answersHeader);
				r.y += 30;

                foreach (var answer in _node.Answers)
                {
                    // INPUT
                    Pin input = _node.PinCollection.Get(answer.InputName);
                    if (input != null)
                        DrawPin(r, input, Vector2.left, true);

                    // LABEL
                    Variable variable = _node.Variables.GetByName(answer.VariableName);
                    string label = "[missing label]";

                    if (answer.Type == AnswerType.Answer) label = variable.ConvertedValue<string>();
                    else if (answer.Type == AnswerType.Time)
                    {
                        if (input != null && input.HasConnection)
                            label = "Time (calculated)";
                        else if (variable != null)
                            label = "Time (" + variable.ConvertedValue<float>() + " sec)";
                    }

                    Rect fRect = new Rect(r.x + r.width + 5, r.y, width - (2 * r.width + 30), 20);
                    GUI.Label(fRect, label, textStyle);                 

                    // OUTPUT
                    Pin output = _node.PinCollection.Get(answer.OutputName);
                    Rect tRect = new Rect(r.x + width - 35, r.y, r.width, r.height);

                    DrawPin(tRect, output, Vector2.right, true);
                    r.y += r.height + 5;
                }

				// Remap connections if not the same!
				if (_lastAnswerCnt != _node.Answers.Count)
				{
					_lastAnswerCnt = _node.Answers.Count;
					MapConnections();
				}
			}		
		}

        
        GUIStyle _actorNameStyle;
		GUIStyle _answersHeader;
		protected override void InitStyle()
		{
			base.InitStyle();
			titleStyle.fontSize = 16;			

			_answersHeader = new GUIStyle(textStyle);
			_answersHeader.alignment = TextAnchor.MiddleLeft;
			_answersHeader.padding = new RectOffset(10, 1, 1, 1);

            _actorNameStyle = new GUIStyle(textStyle);
            _actorNameStyle.alignment = TextAnchor.MiddleLeft;
            _actorNameStyle.padding = new RectOffset(10, 1, 1, 1);
            _actorNameStyle.fontStyle = FontStyle.Bold;
        }
	} 
    
	[CustomEditor(typeof(TextNode))]
	public class TextNodeInspector : NodeInspector
	{
		GUIStyle _textArea;
		AnswerReorderableList _answers;

		bool _audioIsPlaying;
		double _stopTime;

        SkinNameList _skinNames = new SkinNameList();
        bool _portraitWarning; 
       
        void OnEnable()
		{
			TextNode myTarget = (TextNode)target;
			_answers = new AnswerReorderableList(myTarget);
			EditorApplication.update += Update;
            _skinNames.CollectSkins(myTarget.Skin, null, -1);

            _portraitWarning = CheckSpriteProblem(myTarget.Portrait);
        }



		void Update()
		{
			if (_audioIsPlaying && _stopTime <= EditorApplication.timeSinceStartup)
			{
				AudioUtils.Stop();
				_audioIsPlaying = false;
			}
		}



        public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();

			if (_textArea == null)
			{
				_textArea = new GUIStyle(GUI.skin.textArea);
				_textArea.wordWrap = true;
			}

			TextNode myTarget = (TextNode)target;

			// NPC Details
			EditorGUIExtension.SimpleBox("", 5, "", delegate()
                {
                    myTarget.ActorName = EditorGUILayout.TextField(new GUIContent("Actor Name*", "Name of the Actor who 'says' the text. If no value is set, the system uses the name you defined in the Dialogue Behaviour Component."), myTarget.ActorName);

                    EditorGUI.BeginChangeCheck();
                    myTarget.Portrait = (Sprite)EditorGUILayout.ObjectField("Portrait", myTarget.Portrait, typeof(Sprite), false);

                    if (EditorGUI.EndChangeCheck())
                        _portraitWarning = CheckSpriteProblem(myTarget.Portrait);
    
                    if(_portraitWarning)
                    {
                        EditorGUILayout.HelpBox(
                                   "The visual editor can't visualize this sprite right above the node." +
                                   "To fix this, please set the Read/Write enable in the sprite import settings." +
                                   "\n This issue is not affects runtime functionality! ",
                                   MessageType.Warning);
                    }                          
                });

			EditorGUILayout.Space();

			EditorGUIExtension.SimpleBox("Text", 5, "", delegate()
				{
					myTarget.Text = EditorGUILayout.TextArea(myTarget.Text, _textArea,GUILayout.Height(100));

					EditorGUILayout.Space();

					_answers.Draw();
				});
			


			EditorGUILayout.Space();

			// AUDIO
			EditorGUIExtension.FoldoutBox("Audio", ref myTarget.AudioFoldout, (myTarget.Audio != null) ? 1 : 0 , delegate()
				{
					EditorGUI.indentLevel--;				
					EditorGUILayout.Space();

					EditorGUI.BeginChangeCheck();
					myTarget.Audio = (AudioClip)EditorGUILayout.ObjectField("Audio Clip", myTarget.Audio, typeof(AudioClip), false);
					if(EditorGUI.EndChangeCheck())
					{
						myTarget.AudioDelay = 0;
						myTarget.AudioEndTime = (myTarget.Audio != null) ? myTarget.Audio.length : 0;
						myTarget.AudioStartTime = 0;
					}

					EditorGUI.indentLevel++;

					if(myTarget.Audio != null)
					{
						myTarget.AudioDelay = EditorGUILayout.FloatField(new GUIContent("Play delay", "Seconds after the audio clip started to play after the textnode is appeared."), myTarget.AudioDelay);

						GUILayout.Space(10);

						EditorGUILayout.BeginHorizontal();
						GUILayout.Label(new GUIContent("Playback (seconds)", "Seconds of the audio clip where it should start"));

						myTarget.AudioStartTime = EditorGUILayout.FloatField(myTarget.AudioStartTime, GUILayout.Width(75));

						EditorGUILayout.MinMaxSlider(ref myTarget.AudioStartTime, ref myTarget.AudioEndTime, 0, myTarget.Audio.length);

						myTarget.AudioEndTime = EditorGUILayout.FloatField(myTarget.AudioEndTime, GUILayout.Width(75));

						EditorGUILayout.EndHorizontal();

						GUILayout.Space(10);

                        if(!_audioIsPlaying && GUILayout.Button(EditorGUIUtility.FindTexture("PlayButton")))
						{
							int start = (int)System.Math.Ceiling(myTarget.Audio.samples * (myTarget.AudioStartTime / myTarget.Audio.length));
							AudioUtils.Play(myTarget.Audio, start);
							_audioIsPlaying = true;
							_stopTime = EditorApplication.timeSinceStartup + myTarget.AudioEndTime;
						}

                        if (_audioIsPlaying && GUILayout.Button(EditorGUIUtility.FindTexture("PlayButton On")))
                        {
                            AudioUtils.Stop();
                            _audioIsPlaying = false;
                        }

                    }
						
					EditorGUILayout.Space();
				});		

			EditorGUILayout.Space();

			// CAMERA
			string title = "Custom Camera";
			if (myTarget.CustomCameraEnable)
				title += " [index: " + myTarget.CameraIndex + "]";

			EditorGUIExtension.FoldoutBox(title, ref myTarget.CameraFoldout, (myTarget.CustomCameraEnable) ? 1 : 0, delegate()
				{
					EditorGUI.indentLevel--;				
					EditorGUILayout.Space();

					myTarget.CustomCameraEnable = EditorGUILayout.Toggle("Enable", myTarget.CustomCameraEnable);
					if(myTarget.CustomCameraEnable)
					{
						myTarget.CameraIndex = EditorGUILayout.IntField("Camera Index", myTarget.CameraIndex);
					}

					EditorGUILayout.Space();
					EditorGUI.indentLevel++;
				});


            // ADVANCED
            EditorGUIExtension.FoldoutBox("Advanced Settings", ref myTarget.AdvancedSettingsFoldout, -1, delegate ()
            {
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();

                myTarget.CustomSkinEnable = EditorGUILayout.Toggle("Use Custom Skin", myTarget.CustomSkinEnable);
                if (myTarget.CustomSkinEnable)
                {
                    // NAME
                    if (_skinNames.DrawGUI())
                        myTarget.Skin = SkinDatabase.Instance.Skins[_skinNames.Index].Name;
                }

                EditorGUILayout.Space();
                EditorGUI.indentLevel++;
            });

            if (EditorGUI.EndChangeCheck())
				myTarget.HasChanges = true;
		}

        bool CheckSpriteProblem(Sprite sprite)
        {
            string spriteSheet = AssetDatabase.GetAssetPath(sprite);
            Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheet).OfType<Sprite>().ToArray();

            if (sprites.Length > 1)
            {
                var path = IOUtility.GetPath(sprite);
                var importer = (TextureImporter)TextureImporter.GetAtPath(path);
                return !importer.isReadable;
            }
            return false;                
        }
	}
} 
