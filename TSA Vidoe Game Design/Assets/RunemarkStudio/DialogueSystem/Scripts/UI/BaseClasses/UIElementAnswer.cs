using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Runemark.DialogueSystem.UI
{
    public class UIElementAnswer : UIElement
    {
        public virtual int Index { get; set; }

        IDialogueUIController _uiController;
        [HideInInspector] public string AnswerID;
        Button _button;

        public virtual void Init(IDialogueUIController controller)
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(Select);
            _uiController = controller;
        }

        
        /// <summary>
        /// Select this answer.
        /// </summary>
        public void Select()
        {
            _uiController.OnAnswerSelected(AnswerID);
        }
    }
}
