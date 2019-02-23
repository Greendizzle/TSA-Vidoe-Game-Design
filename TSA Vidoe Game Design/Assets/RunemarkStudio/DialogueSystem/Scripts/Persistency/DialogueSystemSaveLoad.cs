using System.Linq;
using UnityEngine;

namespace Runemark.DialogueSystem
{
    public static class DialogueSystemSaveLoad
    {
        /// <summary>
        /// Saves the saveable global variables and all saveable variables from
        /// the dialogue behaviours from the scene into the PlayerPrefs.
        /// </summary>
        public static void SaveToPlayerPrefs()
        {            
            DialogueSystem.SaveToPlayerPrefs();
            foreach (var b in GameObject.FindObjectsOfType<DialogueBehaviour>())
                b.Graph.Variables.SaveToPlayerPrefs();
        }
        /// <summary>
        /// Loads the saveable global variables and all saveable variables from
        /// the dialogue behaviours from the scene from the PlayerPrefs.
        /// </summary>
        public static void LoadFromPlayerPrefs()
        {
            DialogueSystem.LoadFromPlayerPrefs();
            foreach (var b in GameObject.FindObjectsOfType<DialogueBehaviour>())
                b.Graph.Variables.LoadFromPlayerPrefs();
        }
        /// <summary>
        /// Saves the saveable global variables and all saveable variables from
        /// the dialogue behaviours from the scene into a string.
        /// </summary>
        public static string SerializeToString()
        {
            string s = DialogueSystem.SaveToString() +"\n";
            foreach (var b in GameObject.FindObjectsOfType<DialogueBehaviour>())
            {
                if(b.Graph != null)
                    s += b.ID + "|" + b.Graph.Variables.SaveToString() + "\n";
            }
            return s;
        }
        /// <summary>
        /// Loads the saveable global variables and all saveable variables from
        /// the dialogue behaviours from the scene from a string.
        /// </summary>
        public static void DeserializeFromString(string s)
        {
            string[] vars = s.Split('\n');

            DialogueSystem.LoadFromString(vars[0]);
            var behaviours = GameObject.FindObjectsOfType<DialogueBehaviour>().ToList();

            foreach (var v in vars)
            {
                string[] variable = v.Split('|');
                var b = behaviours.Find(x => x.Graph != null && x.ID == variable[0]);
                if(b != null)
                    b.Graph.Variables.LoadFromString(variable[1]);
            }
        }
    }
}
