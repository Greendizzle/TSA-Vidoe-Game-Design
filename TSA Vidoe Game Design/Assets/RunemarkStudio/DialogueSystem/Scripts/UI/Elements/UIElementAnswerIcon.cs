using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Runemark.DialogueSystem.UI
{
    [AddComponentMenu("Runemark Dialogue System/UI/Elements/Icon Answer")]
    [RequireComponent(typeof(Button))]
    public class UIElementAnswerIcon : UIElementAnswer
    {
        public List<IconData> Icons = new List<IconData>();
        public Image Image;

        IconLibrary _iconLibrary;

        public override void Init(IDialogueUIController controller)
        {
            base.Init(controller);

            _iconLibrary = GetComponentInParent<IconLibrary>();
        }


        public override void Set<T>(T value)
        {
            if(typeof(T) == typeof(string))
            {
                var list = (_iconLibrary != null) ? _iconLibrary.Icons : Icons;

                IconData data = list.Find(x => x.Name == value as string);
                if(data != null && data.Icon != null)
                    UnityUIUtility.SetImage(Image, data.Icon);
            }                
        }
    }

    [System.Serializable]
    public class IconData
    {
        public string Name;
        public Sprite Icon;
    }
}