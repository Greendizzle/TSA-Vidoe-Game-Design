using Runemark.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runemark.DialogueSystem.UI
{
    [HelpURL("http://runemarkstudio.com/dialogue-system-documentation/#dialogue-ui-controllers")]
    [RequireComponent(typeof(AudioSource))]
    [AddComponentMenu("Runemark Dialogue System/UI/Controllers/Conversation")]
    public class ConversationController : MonoBehaviour, IDialogueUIController
    {
        public string DefaultSkin
        {
            get { return _defaultSkin; }
            set
            {
                _defaultSkin = value;
                
            }
        }
        [SerializeField]
        string _defaultSkin;

        AudioSource _audioSource;
        DialogueBehaviour _behaviour;

        Dictionary<string, DialogueSystemUISkin> _skins = new Dictionary<string, DialogueSystemUISkin>();
        DialogueSystemUISkin _activeSkin;

        float _audioStopTime;
        float _closeTime = -1f;

        void Awake()
        {
            foreach (var s in GetComponentsInChildren<DialogueSystemUISkin>())
            {
                s.Initialize(this);
                _skins.Add(s.Name, s);
                s.gameObject.SetActive(false);
            }

            if (_skins.Count == 0)
            {
                Debug.LogError("You have to add a ui skin as a child to a UI Controller!");
                enabled = false;
                return;
            }

            SelectSkin("");
            _audioSource = GetComponent<AudioSource>();

            // Set up the listeners based on the type of the ui controller.
            foreach (var b in GameObject.FindObjectsOfType<DialogueBehaviour>())
            {
                b.Conversation.OnDialogueStart += OnDialogueStart;
                b.Conversation.OnTextChanged += OnTextChanged;
                b.Conversation.OnDialoguePaused += OnDialogueFinished;
                b.Conversation.OnDialogueFinished += OnDialogueFinished;
            }
        }
        void Update()
        {
            // Close the dialogue if we wanted to delayed.
            if (_closeTime > -1 && _closeTime <= Time.time)
                CloseDialogue();
      
            // Stop audio after a time.
            if (_audioSource.isPlaying && _audioSource.time >= _audioStopTime)
                _audioSource.Stop();

            // Update timer on the skin
            if (_activeSkin != null && _behaviour.HasTimer)
                _activeSkin.UpdateTimer(_behaviour.TimeBack);            
        }

        public  void OnDialogueStart(DialogueBehaviour owner)
        {
          //  Debug.LogFormat("{0} - Conversation Controller.OnDialogueStart()", name);
        }

        public void OnTextChanged(DialogueFlow dialogue, DialogueText text, List<AnswerText> answers)
        {       
            _behaviour = dialogue.Behaviour;

             // Select and open a skin
            string skinName = text.Skin;
            if (skinName == "" && dialogue.UseDefaultSkin) skinName = dialogue.DefaultSkin;
            if (skinName == "") skinName = DefaultSkin;
            SelectSkin(skinName);

            if (_activeSkin == null)
            {
                RunemarkDebug.Warning(skinName + " skin doesn't exists in this canvas: " + gameObject.name);
                return;
            }

            // Stop the audio if currently playing
            if (_audioSource.isPlaying) _audioSource.Stop();

            // Set the actor's name and portrait
            string name = text.ActorName;
            Sprite portrait = text.Portrait;
            if (_behaviour.ActorEnabled)
            {
                if (name == "")
                    name = _behaviour.ActorName;
                if (name == "")
                    name = _behaviour.gameObject.name;

                if (portrait == null)
                    portrait = _behaviour.ActorPortrait;
            }

            _activeSkin.SetText(name, portrait, text.Text);

            // Set the answers
            int cnt = 0;
            foreach (var a in answers)
            {        
                if (answers.Count > cnt)
                {
                    _activeSkin.SetAnswer(cnt, a.OutputName, a.Text);
                    cnt++;
                }
            }
            _activeSkin.HideAnswers(cnt);

            // Play Audio if any
            if (text.Audio != null)
            {
                _audioStopTime = text.AudioEndTime;
                _audioSource.clip = text.Audio;
                _audioSource.time = text.AudioStartTime;

                if (text.AudioDelay > 0)
                    _audioSource.PlayDelayed(text.AudioDelay);
                else
                    _audioSource.Play();
            }

            // Set the timer            
            if (_behaviour.HasTimer && cnt > 0)
                _activeSkin.StartTimer(_behaviour.TimeBack);
            else
                _activeSkin.HideTimer();
        }

        /// <summary>
        /// Closes the UI.
        /// </summary>
        public void OnDialogueFinished(DialogueBehaviour owner)
        {
            if (owner.Conversation.ExitDelay > 0)
                _closeTime = Time.time + owner.Conversation.ExitDelay;
            else
                CloseDialogue();          
        }

        public void CloseDialogue()
        {
            SelectSkin("");
            if (_audioSource.isPlaying)
                _audioSource.Stop();

            _closeTime = -1;
        }

        /// <summary>
        /// On Answer Selected Event
        /// </summary>
        /// <param name="id"></param>
        public void OnAnswerSelected(string id)
        {
            _behaviour.SelectAnswer(id);
        }

        /// <summary>
        /// Shows the skin with the given name, hides the other.
        /// If no name is given, it hides every skin.
        /// </summary>
        /// <param name="skinname"></param>
        void SelectSkin(string skinName)
        {
            if (_activeSkin != null)
            {
                _activeSkin.gameObject.SetActive(false);
                _activeSkin = null;
            }
            if (_skins.ContainsKey(skinName))
            {
                _activeSkin = _skins[skinName];
                _activeSkin.gameObject.SetActive(true);
            }
        }




        public System.Action onApplicationQuit { get; set; }
        public System.Action onInactivated { get; set; }
        private void OnApplicationQuit()
        {
            if (onApplicationQuit != null)
                onApplicationQuit();
        }

        private void OnDisable()
        {
            if (onInactivated != null)
                onInactivated();
        }
        private void OnDestroy()
        {
            if (onInactivated != null)
                onInactivated();
        }
    }


    public class AnswerText
    {
        public string Text;
        public string OutputName;
    }
}