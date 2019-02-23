using Runemark.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runemark.DialogueSystem.UI
{
    [AddComponentMenu("Runemark Dialogue System/UI/Controllers/Ambient Dialogue")]
    public class AmbientDialogueController : MonoBehaviour, IDialogueUIController
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

        Dictionary<string, DialogueSystemUISkin> _skins = new Dictionary<string, DialogueSystemUISkin>();
        Dictionary<DialogueBehaviour, DialogueSystemUISkin> _activeSkins = new Dictionary<DialogueBehaviour, DialogueSystemUISkin>();

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

            // Set up the listeners based on the type of the ui controller.
            foreach (var b in GameObject.FindObjectsOfType<DialogueBehaviour>())
            {
                b.AmbientDialogue.OnDialogueStart += OnDialogueStart;
                b.AmbientDialogue.OnTextChanged += OnTextChanged;
                b.AmbientDialogue.OnDialoguePaused += OnDialogueFinished;
                b.AmbientDialogue.OnDialogueFinished += OnDialogueFinished;

             }
        }
        void Update()
        {
            // Billboard
            if (Camera.main == null) return;
            foreach (var pair in _activeSkins)
            {
                var skin = pair.Value;
                var behaviour = pair.Key;

                skin.transform.position = behaviour.transform.position + Vector3.up * 3 + behaviour.AmbientDialogue.Offset;

                Vector3 dir = skin.transform.position - Camera.main.transform.position;
                skin.transform.LookAt(skin.transform.position + dir);
            }
        }
        
        DialogueSystemUISkin ActivateSkin(DialogueBehaviour behaviour, string skinName)
        {
            if (_activeSkins.ContainsKey(behaviour))
                Destroy(_activeSkins[behaviour]);
            else
                _activeSkins.Add(behaviour, null);
         
            DialogueSystemUISkin skin = null;
            if (_skins.ContainsKey(skinName))
            {
                var go = Instantiate(_skins[skinName].gameObject);
                skin = go.GetComponent<DialogueSystemUISkin>();
                go.transform.SetParent(transform);
                go.transform.localScale = Vector3.one;
                go.SetActive(true);
            }

            _activeSkins[behaviour] = skin;
            return skin;
        }
                
        public void OnTextChanged(DialogueFlow dialogue, DialogueText text, List<AnswerText> answers)
        {
            // Select and open a skin
            string skinName = text.Skin;
            if (skinName == "") skinName = dialogue.DefaultSkin;
            if (skinName == "") skinName = DefaultSkin;

            DialogueSystemUISkin skin;
            if (!_activeSkins.TryGetValue(dialogue.Behaviour, out skin) || skin.Name != skinName)
                skin = ActivateSkin(dialogue.Behaviour, skinName);
            if (skin != null)
            {               
                // Set the actor's name and portrait
                string name = text.ActorName;
                Sprite portrait = text.Portrait;
                if (dialogue.Behaviour.ActorEnabled)
                {
                    if (name == "")
                        name = dialogue.Behaviour.ActorName;
                    if (portrait == null)
                        portrait = dialogue.Behaviour.ActorPortrait;
                }
                if (name == "") name = dialogue.Behaviour.gameObject.name;

                skin.SetText(name, portrait, text.Text);
            }           
        }

        public void OnDialogueFinished(DialogueBehaviour owner)
        {
            if (_activeSkins.ContainsKey(owner) && _activeSkins[owner] != null)
            {
                Destroy(_activeSkins[owner].gameObject);
                _activeSkins.Remove(owner);
            }
        }


        public void OnAnswerSelected(string id)
        {
            // Don't do anything, since ambient dialogue doesn't have answers.
        }
        public void OnDialogueStart(DialogueBehaviour owner)
        {
            
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
}