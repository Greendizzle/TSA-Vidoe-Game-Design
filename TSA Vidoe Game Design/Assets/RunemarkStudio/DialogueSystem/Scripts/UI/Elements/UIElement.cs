using Runemark.Common;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Runemark.DialogueSystem.UI
{
    [HelpURL("http://runemarkstudio.com/dialogue-system-documentation/#ui-elements")]
    public class UIElement : MonoBehaviour
    {
        public bool FindInChildren = false;

        protected T Get<T>() where T : UIBehaviour
        {
            if (!FindInChildren)
                return GetComponent<T>();
          
            foreach(var e in GetComponentsInChildren<T>())
            {
                if (FindInChildren != (e.gameObject == gameObject))
                    return e;
            }
            return null;
        }      
        public virtual void Set<T>(T value)
        {
            RunemarkDebug.Error("{0} Set method is used but not implemented for value type of {1}",
                GetType().ToString(), typeof(T));
        }
        public virtual void UpdateValue<T>(T value)
        {
            RunemarkDebug.Error("{0} UpdateValue method is used but not implemented for value type of {1}",
               GetType().ToString(), typeof(T));
        }
    }   
}
