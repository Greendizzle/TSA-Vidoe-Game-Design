using Runemark.VisualEditor;
using UnityEngine;

namespace Runemark.DialogueSystem
{
    //[CreateAssetMenu(fileName = "DialogueSystemGlobals", menuName = "RunemarkDeveloper/Dialogue System Globals", order = 1)]
    [System.Serializable]
    public class DialogueSystemGlobals : ScriptableObject
    {
        #region GLOBAL VARIABLES
        public VariableCollection Variables = new VariableCollection();
        #endregion
    }
}
