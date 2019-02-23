using Runemark.Common;
using System.IO;
using UnityEngine;

namespace Runemark.DialogueSystem
{
    public class BasicSaveLoad : MonoBehaviour
    {
        /// <summary>
        /// Modes to save the dialogue system data.
        /// </summary>
        public enum Mode { PlayerPrefs, File }
        public Mode mode;

        /// <summary>
        /// Name of the file if the File mode is choosen.
        /// </summary>
        public string FileName = "RunemarkDialogueSystem_Saves";

        /// <summary>
        /// Should the system automatically save on exit?
        /// </summary>
        public bool SaveOnExit = true;

        /// <summary>
        /// Should the system automatically load on start?
        /// </summary>
        public bool LoadOnStart = true;

        private void OnEnable()
        {
            transform.SetAsLastSibling();
        }
        private void Start()
        {
            if (LoadOnStart)
                Load();
        }
        private void OnApplicationQuit()
        {
            if (SaveOnExit)
                Save();
        }

        /// <summary>
        /// Save every local and global variable value (that are marked as Saveable)
        /// </summary>
        public void Save()
        {
            if (mode == Mode.PlayerPrefs)
                DialogueSystemSaveLoad.SaveToPlayerPrefs();

            else if (mode == Mode.File)
            {
                string data = DialogueSystemSaveLoad.SerializeToString();
                string path = Path.Combine(Application.dataPath, FileName + ".save");
                File.WriteAllText(path, data);

                RunemarkDebug.Log("Saved to " + path);
            }
        }

        /// <summary>
        /// Save every local and global variable value (that are marked as Saveable)
        /// </summary>
        public void Load()
        {
            if (mode == Mode.PlayerPrefs)
                DialogueSystemSaveLoad.LoadFromPlayerPrefs();

            else if (mode == Mode.File)
            {
                string path = Path.Combine(Application.dataPath, FileName + ".save");
                string data = File.ReadAllText(path);
                DialogueSystemSaveLoad.DeserializeFromString(data);
            }
        }
    }
}
