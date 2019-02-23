using System.Collections.Generic;
using UnityEngine;
using Runemark.VisualEditor;
using UnityEngine.Events;
using Runemark.DialogueSystem.UI;
using Runemark.VisualEditor.Actions;

namespace Runemark.DialogueSystem
{
    [HelpURL("http://runemarkstudio.com/dialogue-system-documentation/#dialogue-behaviour")]
    [AddComponentMenu("Runemark Dialogue System/Dialogue Behaviour")]
    public class DialogueBehaviour : VisualEditorBehaviour
    {
        #region Editor Stuffs
        // This part is used to store editor variables.
        public bool ActorFoldout;
        public bool ConversationFoldout;
        public bool BarkFoldout;
        public bool ExternalEventFoldout;
        #endregion


        /// <summary>
        /// Turns on or off the actor default parameters.
        /// </summary>
        public bool ActorEnabled;
        /// <summary>
        /// The name of the actor.
        /// </summary>
        public string ActorName;
        /// <summary>
        /// The portrait of the actor.
        /// </summary>
        public Sprite ActorPortrait;


        /// <summary>
        /// Conversation parameters.
        /// </summary>
        public ConversationFlow Conversation = new ConversationFlow();
        /// <summary>
        /// Ambient Dialogue parameters.
        /// </summary>
        public AmbientDialogue AmbientDialogue = new AmbientDialogue();

        /// <summary>
        /// Has the current text node Next Answer?
        /// </summary>
        public bool HasNextAnswer;
        /// <summary>
        /// Has the current text node Time Answer?
        /// </summary>
        public bool HasTimer;
        /// <summary>
        /// In case HasTimer is true, this is the time the player has to answer.
        /// </summary>
        public float TimeBack = -1f;

        /// <summary>
        /// The answer id for the timer.
        /// </summary>
        string _timerOutputName;

        bool _ambientDialogueActive;

        public bool IsConversationActive { get { return GetActiveNode(Conversation.EVENT_NAME) != null; } }
        public bool IsAmbientDialogueActive { get { return _ambientDialogueActive && !IsConversationActive; } }


        void Awake()
        {
            // Initalize and reset cameras.
        //    _mainCamera = Camera.main;
         //   ResetCamera(Conversation);

            // If custom actor is not enabled, use the game object name if needed.
            if (!ActorEnabled) ActorName = gameObject.name;

            Conversation.Behaviour = this;
            AmbientDialogue.Behaviour = this;

            Conversation.CameraController.Init();
        }

        /// <summary>
        /// In this method I handle Timed answers and bark auto triggers.
        /// You can add any time based functionality here.
        /// </summary>
        protected override void OnUpdate()
        {
            // If the TextNode has time it's handled here.
            if (HasTimer)
            {
                TimeBack -= Time.deltaTime;
                if (TimeBack <= 0f)
                    SelectAnswer(_timerOutputName);
            }

            // Ambient Dialogue automatically triggers after certain seconds.
            if (IsAmbientDialogueActive && !IsConversationActive && AmbientDialogue.TimeCheck(Time.time))
                CallEvent(AmbientDialogue.EVENT_NAME);

            // Auto close conversation
            if (IsConversationActive && Conversation.UseAutoExit && Conversation.Player != null)
            {
                float distance = Vector3.Distance(transform.position, Conversation.Player.position);
                if(Conversation.Trigger.Mode == Trigger.Modes.Custom)
                {
                    if (distance > Conversation.ExitDistance)
                        StopDialogue(Conversation);
                }
                else if (Conversation.Trigger.Mode == Trigger.Modes.Use)
                {
                    if (distance > Conversation.Trigger.Distance)
                        StopDialogue(Conversation);
                }
            }
        }


        #region Triggering Dialogues
        void Start()
        {
            // Start the dialogues if it's trigger set to OnStart.
            StartDialogue(Conversation, Trigger.Modes.OnStart);
            StartDialogue(AmbientDialogue, Trigger.Modes.OnStart);
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            StartDialogue(Conversation, Trigger.Modes.TriggerEnter, collision.transform);
            StartDialogue(AmbientDialogue, Trigger.Modes.TriggerEnter, collision.transform);
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            if (IsConversationActive && Conversation.UseAutoExit &&
               Conversation.Trigger.Mode == Trigger.Modes.TriggerEnter &&
               Conversation.Player == collision.transform)
            {
                StopDialogue(Conversation);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            StartDialogue(Conversation, Trigger.Modes.TriggerEnter, other.transform);
            StartDialogue(AmbientDialogue, Trigger.Modes.TriggerEnter, other.transform);
        }
        void OnTriggerExit(Collider other)
        {        
            if (IsConversationActive && Conversation.UseAutoExit && 
                Conversation.Trigger.Mode == Trigger.Modes.TriggerEnter &&
                Conversation.Player == other.transform)
            {
                StopDialogue(Conversation);
            }
        }
        public void Use(Transform other)
        {
            if (Conversation.Trigger.Use(other, transform))
                StartDialogue(Conversation, Trigger.Modes.Use, other.transform);
            if (AmbientDialogue.Trigger.Use(other, transform))
                StartDialogue(AmbientDialogue, Trigger.Modes.Use, other.transform);
        }

        /// <summary>
        /// Starts the dialogue. Only works if the trigger on the behaviour is set to Custom.
        /// </summary>
        public void StartDialogue()
        {
            if (Conversation.Trigger.Mode == Trigger.Modes.Custom)
                StartDialogue(Conversation, Trigger.Modes.Custom);
            if (AmbientDialogue.Trigger.Mode == Trigger.Modes.Custom)
                StartDialogue(AmbientDialogue, Trigger.Modes.Custom);
        }
        #endregion
        #region Camera Controll
     /*   Camera _mainCamera;
        Camera _activeCamera;

        /// <summary>
        /// Sets the camera that the Conversation Flow should use.
        /// </summary>
        /// <param name="flow"></param>
        void SetCamera(ConversationFlow flow, int index)
        {
            if (flow.CameraController.Enable)
            {
                if (index == -1 || index > flow.CameraController.CameraList.Count)
                    index = flow.CameraController.DefaultIndex;

                _activeCamera.enabled = false;

                Debug.Log("Camera index: " + index);

                if (index < 0)
                    _activeCamera = _mainCamera;
                else
                {
                    index = Mathf.Min(index, flow.CameraController.CameraList.Count);
                    _activeCamera = flow.CameraController.CameraList[index];
                }
                _activeCamera.enabled = true;
            }
        }

        /// <summary>
        /// Reset cameras to default: all turn off, except main camera.
        /// </summary>
        void ResetCamera(ConversationFlow flow)
        {
            if (_mainCamera != null)
            {
                _mainCamera.enabled = true;
                _activeCamera = _mainCamera;
            }

            if (flow.UseCustomCameras)
            {
                for (int cnt = 0; cnt < flow.ConversationCameras.Count; cnt++)
                    flow.ConversationCameras[cnt].enabled = false;
            }
        }*/
        #endregion

        


        /// <summary>
        /// Starts the dialogue with the given trigger mode, if it's allowed.
        /// </summary>
        /// <param name="dialogue"></param>
        /// <param name="mode"></param>
        public void StartDialogue(DialogueFlow dialogue, Trigger.Modes mode, Transform player = null)
        {
            if (dialogue.CheckTrigger(mode))
            {
                dialogue.Player = player;
                CallEvent(dialogue.EVENT_NAME);

                if (dialogue == Conversation && IsAmbientDialogueActive)
                    StopDialogue(AmbientDialogue);
            }
        }
        /// <summary>
        /// Stops the given dialogue flow.
        /// </summary>
        /// <param name="dialogue"></param>
        public void StopDialogue(DialogueFlow dialogue)
        {
            if (dialogue.EVENT_NAME == AmbientDialogue.EVENT_NAME)
                OnAmbientDialogueEnd();
            else
                OnEventFinished(dialogue.EVENT_NAME);
        }


        #region Event Handling

        public bool ExternalEventEnable;
        [System.Serializable]
        public class ExternalEvent
        {
            public string EventName;
            public UnityEvent uEvent;
        }
        public List<ExternalEvent> Events = new List<ExternalEvent>();

        public override void CallEvent(string eventName)
        {
            var e = Events.Find(x => x.EventName == eventName);

            if (e != null)
            {
                e.uEvent.Invoke();
            }
            else
                base.CallEvent(eventName);
        }
        #endregion

        public void ShowText(TextNode node, List<AnswerData> answers)
        {
            string flow = EventNameofNode(node);
            DialogueText t = new DialogueText(node);

            List<AnswerText> answerList = new List<AnswerText>();                
            foreach (var a in answers)
            {
                Variable show = node.GetInput(a.InputName);
                Variable text = node.Variables.GetByName(a.VariableName);

                if (a.Type == AnswerType.Time)
                {
                    Variable time = node.Variables.GetByName(a.VariableName);

                    HasTimer = true;
                    TimeBack = time.ConvertedValue<float>();
                    _timerOutputName = a.OutputName;
                    continue;
                }
                else if (show != null && !show.ConvertedValue<bool>())
                    continue;

                answerList.Add(new AnswerText()
                {
                    Text = text.ConvertedValue<string>(),
                    OutputName = a.OutputName
                });          
            }

            if (Conversation.EVENT_NAME == flow)
            {
                if (node.CustomCameraEnable || Conversation.CameraController.Enable)
                    Conversation.CameraController.Set((node.CustomCameraEnable) ? node.CameraIndex : -1);
                else
                    Conversation.CameraController.Reset();

                if (Conversation.OnTextChanged != null)
                    Conversation.OnTextChanged(Conversation, t, answerList);
            }

            else if (AmbientDialogue.EVENT_NAME == flow)
            {
                if (AmbientDialogue.OnTextChanged != null)
                    AmbientDialogue.OnTextChanged(AmbientDialogue, t, answerList);
            }
        }
        public void PauseText(string eventName)
        {
            if (Conversation.EVENT_NAME == eventName)
            {
               if (Conversation.OnDialoguePaused != null)
                    Conversation.OnDialoguePaused(this);
            }

            else if (AmbientDialogue.EVENT_NAME == eventName)
            {
                if (AmbientDialogue.OnDialoguePaused != null)
                    AmbientDialogue.OnDialoguePaused(this);
            }
        }

        public void SelectAnswer(string outputName)
        {
            var n = GetActiveNode(Conversation.EVENT_NAME);
            if (n == null)
                n = GetActiveNode(AmbientDialogue.EVENT_NAME);

            if (n != null && n.GetType() == typeof(TextNode))
                ((TextNode)n).SelectAnswer(outputName);

            HasTimer = false;
            HasNextAnswer = false;
        }

        protected override void OnEventStarted(string eventName)
        {
            if (Conversation.EVENT_NAME == eventName)
            {
                if (Conversation.OnDialogueStart != null)
                    Conversation.OnDialogueStart(this);

                // Handle reposition of the player.
                if (Conversation.OverridePlayerPosition && Conversation.PlayerPosition != null)
                {
                    var p = GameObject.FindGameObjectWithTag(Conversation.PlayerTag);
                    p.transform.position = Conversation.PlayerPosition.position;
                    p.transform.LookAt(new Vector3(Conversation.PlayerPosition.position.x, p.transform.position.y, Conversation.PlayerPosition.position.z));
                }
            }
            else if (AmbientDialogue.EVENT_NAME == eventName && AmbientDialogue.OnDialogueStart != null)
            {
                _ambientDialogueActive = true;
                AmbientDialogue.OnDialogueStart(this);
            }

            base.OnEventStarted(eventName);
        }
        protected override void OnEventFinished(string eventName)
        {
            if (eventName == Conversation.EVENT_NAME)
            {
                base.OnEventFinished(eventName);
                Conversation.CameraController.Reset();
                if (Conversation.OnDialogueFinished != null)
                    Conversation.OnDialogueFinished(this);
            }
            else if (eventName == AmbientDialogue.EVENT_NAME)
            {
                base.OnEventFinished(AmbientDialogue.EVENT_NAME);
                AmbientDialogue.OnDisapear(this, Time.time);                
            }
        }

        protected override void OnNodeActivated(string eventName, Node node)
        {
            if (node.GetType() == typeof(Wait))
            {
                PauseText(eventName);
            }            
        }

        void OnAmbientDialogueEnd()
        {
            base.OnEventFinished(AmbientDialogue.EVENT_NAME);
            if (AmbientDialogue.OnDialogueFinished != null)
                AmbientDialogue.OnDialogueFinished(this);
            _ambientDialogueActive = false;
        }
    }   
}