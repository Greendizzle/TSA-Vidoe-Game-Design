using UnityEngine;
using UnityEngine.UI;

namespace Runemark.DialogueSystem.UI
{
    [AddComponentMenu("Runemark Dialogue System/UI/Elements/Text Answer")]
    [RequireComponent(typeof(Button))]
    public class UIElementAnswerText : UIElementAnswer
    {
        public Text IndexUI;
        public Text TextUI;
              
        public override int Index
        {
            get { return (IndexUI.gameObject.activeSelf) ? int.Parse(IndexUI.text) : -1; }
            set { UnityUIUtility.SetText(IndexUI, value.ToString()); }
        }     

        public override void Set<T>(T value)
        {
            if(typeof(T) == typeof(string))
                UnityUIUtility.SetText(TextUI, value as string);
        }
    }
}