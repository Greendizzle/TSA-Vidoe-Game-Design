using UnityEngine;
using Runemark.VisualEditor;

namespace Runemark.DialogueSystem.Playmaker
{
    [HelpURL("http://runemarkstudio.com/dialogue-system-documentation/#nodes-for-playmaker")]
    [System.Serializable]
#if PLAYMAKER
     [Info("3rd Party Support/Playmaker Event", "Icons/PlayMakerFSM Icon")]
#endif

    public class PlayMakerEventCall : ExecutableNode
    {
        protected override Variable CalculateOutput(string name) { return null; }
        public override string Tooltip { get { return ""; } }

        protected override void OnInit()
        {
            Variables.Add("FsmName", "");
            Variables.Add("EventName", "");
        }

        protected override void OnEnter()
        {
#if PLAYMAKER
            Variable nameVariable = Variables["FsmName"];
            string fsmName = nameVariable.ConvertedValue<string>();

            PlayMakerFSM fsm = null;
            if (fsmName != "")
            {
                foreach (var t in GameObject.FindObjectsOfType<PlayMakerFSM>())
                {
                    if (t.FsmName == fsmName)
                        fsm = t;
                }
            }
            else
                fsm = Owner.gameObject.GetComponent<PlayMakerFSM>();

            Variable eventName = Variables["EventName"];
            fsm.SendEvent(eventName.ConvertedValue<string>());
#endif
            IsFinished = true;

        }

        protected override void OnUpdate() { }
        protected override void OnExit() { }
    }
}