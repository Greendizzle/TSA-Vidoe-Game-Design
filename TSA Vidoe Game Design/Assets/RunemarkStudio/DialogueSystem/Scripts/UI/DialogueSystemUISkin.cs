using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Runemark.Common;

namespace Runemark.DialogueSystem.UI
{
    /// <summary>
    /// Dialogue System UI skin
    /// </summary>
    [HelpURL("http://runemarkstudio.com/dialogue-system-documentation/#dialogue-ui-skin")]
    [AddComponentMenu("Runemark Dialogue System/UI/UISkin")]
	public class DialogueSystemUISkin : MonoBehaviour
	{
        public string Name;
        public DialogueUIType UIType;

        public UIElement ActorText;

        public bool UseActorName;
        public UIElement ActorName;

        public bool UseActorPortrait;
        public UIElement ActorPortrait;

        public bool UseTimer;
        public UIElement Timer;

        public bool DynamicAnswerNumber;        
        
        List<UIElementAnswer> _answers = new List<UIElementAnswer>();
        IDialogueUIController _controller;

        public void Initialize(IDialogueUIController controller)
        {
            _controller = controller;

            _answers = GetComponentsInChildren<UIElementAnswer>().ToList();
            foreach (var a in _answers)
                a.Init(_controller);

            // TESTING
            string message = "";
            if (Test(out message))
            {
                Debug.LogError(message);
                return;
            }
        }

        /// <summary>
        /// Testing the skin
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool Test(out string message)
        {
            message = Name + " skin test result: ";
            bool error = false;
            if (UseActorName && ActorName == null)
            {
                message += "\n- Use Actor Name is turned on but no ui element set as Actor Name";
                error = true;
            }
            if (UseActorPortrait && ActorPortrait == null)
            {
                message += "\n- Use Actor Portrait is turned on but no ui element set as Actor Portrait";
                error = true;
            }
            if (UIType == DialogueUIType.Conversation && UseTimer && Timer == null)
            {
                message += "\n- Use Timer is turned on but no ui element set as Timer";
                error = true;
            }
            if (!error) message += "OK";
            return error;
        }


        /// <summary>
        /// Sets the text element of the skin.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="portrait"></param>
        /// <param name="text"></param>
        public void SetText(string name, Sprite portrait, string text)
        {
            if (UseActorName && ActorName != null)
            {
                // If the name exists, set it.
                if (name != "")
                {
                    ActorName.gameObject.SetActive(true);
                    ActorName.Set<string>(name);
                }
                // If the name is empty string
                else
                {
                    ActorName.gameObject.SetActive(false);
                }
            }

            if (UseActorPortrait && ActorPortrait != null)
            {
                // If the portrait exists, set it.
                if (portrait != null)
                {
                    ActorPortrait.gameObject.SetActive(true);
                    ActorPortrait.Set<Sprite>(portrait);
                }
                // If the portrait is null
                else
                    ActorPortrait.gameObject.SetActive(false);
            }
                           
            ActorText.Set<string>(text);        
        }

        /// <summary>
        /// Sets the answer on the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="outputName"></param>
        /// <param name="text"></param>
        public void SetAnswer(int index, string outputName, string text)
        {
            if (_answers.Count <= index)
            {
                if (DynamicAnswerNumber)
                {
                    var a = Instantiate(_answers[0]);
                    a.transform.SetParent(_answers[0].transform.parent);
                    a.Init(_controller);
                    _answers.Add(a);
                }
                else
                    RunemarkDebug.Error("Can't show the {0}. answer, becouse the skin has less answer slots and it's not set to dynamical", 
                        index);
            }

            _answers[index].gameObject.SetActive(outputName != "");
            _answers[index].AnswerID = outputName;
            _answers[index].Set<string>(text);
            _answers[index].Index = index + 1;
        }

        /// <summary>
        /// Hides the answers begining the given index.
        /// </summary>
        /// <param name="fromIndex"></param>
        public void HideAnswers(int fromIndex)
        {
            for (int i = fromIndex; i < _answers.Count; i++)
                SetAnswer(i, "", "");
        }

        /// <summary>
        /// Starts the timer, if exists.
        /// </summary>
        /// <param name="totalTime"></param>
        public void StartTimer(float totalTime)
        {
            if (UseTimer)
            {
                Timer.gameObject.SetActive(true);
                Timer.Set<float>(totalTime);
            }
        }

        /// <summary>
        /// Hides the timer if exists.
        /// </summary>
        public void HideTimer()
        {
            if(Timer != null)
                Timer.gameObject.SetActive(false);
        }

        /// <summary>
        /// Updates the timer if extists.
        /// </summary>
        /// <param name="timeBack"></param>
        public void UpdateTimer(float timeBack)
        {
            if(Timer != null)
                Timer.UpdateValue<float>(timeBack);
        }

        void Update()
        {
         


        }

    }
}