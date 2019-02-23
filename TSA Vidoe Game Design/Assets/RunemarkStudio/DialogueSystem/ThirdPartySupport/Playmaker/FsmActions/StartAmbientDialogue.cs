#if PLAYMAKER
using HutongGames.PlayMaker;

namespace Runemark.DialogueSystem.Playmaker
{
    [HelpUrl("http://runemarkstudio.com/dialogue-system-documentation/#playmaker")]
    [ActionCategory("Runemark Dialogue System")]
    [Tooltip("Starts the Ambient Dialogue on the Actor")]
    public class StartAmbientDialogue : FsmStateAction
    {

        [CheckForComponent(typeof(DialogueBehaviour))]
        [Tooltip("The actor with a DialogueBehaviour on it.")]
        public FsmOwnerDefault Actor;        

        public override void Reset()
        {
            Actor = new FsmOwnerDefault();
            Actor.OwnerOption = OwnerDefaultOption.UseOwner;            
        }
      
        public override void OnEnter()
        {
             var go = Fsm.GetOwnerDefaultTarget(Actor);
            DialogueBehaviour b = go.GetComponent<DialogueBehaviour>();
            b.StartDialogue(b.AmbientDialogue, Trigger.Modes.Custom);

            Finish();
        }
    }
}
#endif