using Runemark.VisualEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Runemark.DialogueSystem
{
	[System.Serializable]
    [HelpURL("http://runemarkstudio.com/dialogue-system-documentation/#text-node")]
    [Info("Text", "Icons/text", 1)]
	public class TextNode : ExecutableNode
	{
		protected override bool AutoGenerateInTrans  {	get { return true;  } }
		protected override bool AutoGenerateOutTrans {	get { return false; } }
        
		public override string Tooltip { get { return ""; }}

		/// <summary>
		/// The name of the NPC.
		/// </summary>
		public string ActorName = "";

		/// <summary> 
		/// Text that the 'npc' says.
		/// </summary>
		public string Text = "Write your text here...";
        
		/// <summary>
		/// The npc portrait, that appears with the text.
		/// </summary>
		public Sprite Portrait;

		/// <summary>
		/// Audio clip that is played when the text is shows up.
		/// </summary>
		public AudioClip Audio;

		/// <summary>
		/// The time (in seconds) where the audio playback should start.
		/// </summary>
		public float AudioStartTime = 0;

		/// <summary>
		/// The time (in seconds) how long the audio clip should be played.
		/// </summary>
		public float AudioEndTime;

		/// <summary>
		/// The time (in seconds) when the audio clip should be start to play after the text shows up.
		/// </summary>
		public float AudioDelay = 0f;

		/// <summary>
		///  If the custom camera is not enabled the system uses the main camera.
		/// </summary>
		public bool CustomCameraEnable;

		/// <summary>
		/// The index of the camera this node should used.
		/// </summary>
		public int CameraIndex = 0;
     

        /// <summary>
        /// Should the system use a custom skin?
        /// </summary>
        public bool CustomSkinEnable;

        /// <summary>
        /// The skin the system should use for this node.
        /// </summary>
        public string Skin = "";

        /// <summary>
        /// List of the answer data.
        /// </summary>
        public List<AnswerData> Answers = new List<AnswerData>();

		protected override void OnInit()
		{
			
		}

		protected override Variable CalculateOutput(string name)
		{
			return null;
		}

		protected override void OnEnter()
		{
            ((DialogueBehaviour)Owner).ShowText(this, Answers);           
		}
		protected override void OnUpdate(){}
		protected override void OnExit(){}		
        
		public void SelectAnswer(string outputName)
		{
            _calculatedNextNode = PinCollection.Get(outputName);
			IsFinished = true;
		}
        public override Node Copy(bool runtime = false)
        {
            var copy = (TextNode)base.Copy(runtime);
            copy.ActorName = ActorName;
            copy.Text = Text;
            copy.Portrait = Portrait;

            copy.Answers = new List<AnswerData>();
            foreach (var a in Answers)
            {
                AnswerData copyAnswer = new AnswerData()
                {
                    InputName = a.InputName,
                    OutputName = a.OutputName,
                    Type = a.Type,
                    VariableName = a.VariableName
                };
                copy.Answers.Add(copyAnswer);
            }

            copy.Audio = Audio;
            copy.AudioStartTime = AudioStartTime;
            copy.AudioEndTime = AudioEndTime;
            copy.AudioDelay = AudioDelay;

            copy.CustomCameraEnable = CustomCameraEnable;
            copy.CameraIndex = CameraIndex;

            copy.CustomSkinEnable = CustomSkinEnable;
            copy.Skin = Skin;

            return copy;
    }

        
        #region EditorStuffs
        public bool AudioFoldout;
		public bool CameraFoldout;
        public bool AdvancedSettingsFoldout;
        #endregion

    }
    
    public enum AnswerType { Answer, Time }

    [System.Serializable]
    public class AnswerData
    {
        public AnswerType Type;
        public string InputName;
        public string VariableName;
        public string OutputName;
    }



}