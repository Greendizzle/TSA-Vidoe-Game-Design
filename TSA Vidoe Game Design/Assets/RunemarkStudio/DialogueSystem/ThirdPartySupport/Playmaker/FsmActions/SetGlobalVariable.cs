#if PLAYMAKER
using HutongGames.PlayMaker;
using Runemark.Common;
using Runemark.VisualEditor;

namespace Runemark.DialogueSystem.Playmaker
{
    [HelpUrl("http://runemarkstudio.com/dialogue-system-documentation/#playmaker")]
    [ActionCategory("Runemark Dialogue System")]
    [Tooltip("Sets the global or local string variable of the dialogue system.")]
    public class SetStringVariable : SetVariable<FsmString> {}

    [HelpUrl("http://runemarkstudio.com/dialogue-system-documentation/#playmaker")]
    [ActionCategory("Runemark Dialogue System")]
    [Tooltip("Sets the global or local string variable of the dialogue system.")]
    public class SetIntVariable : SetVariable<FsmInt> { }

    [HelpUrl("http://runemarkstudio.com/dialogue-system-documentation/#playmaker")]
    [ActionCategory("Runemark Dialogue System")]
    [Tooltip("Sets the global or local string variable of the dialogue system.")]
    public class SetFloatVariable : SetVariable<FsmFloat> { }

    [HelpUrl("http://runemarkstudio.com/dialogue-system-documentation/#playmaker")]
    [ActionCategory("Runemark Dialogue System")]
    [Tooltip("Sets the global or local string variable of the dialogue system.")]
    public class SetBoolVariable : SetVariable<FsmBool> { }

    public class SetVariable<T> : FsmStateAction where T : NamedVariable
    {
        [RequiredField]
        [Tooltip("The name of the global variable.")]
        public FsmString VariableName;

        [Tooltip("Determines if we want to get GLobal or Local variable")]
        public FsmBool IsGlobal;

        [CheckForComponent(typeof(DialogueBehaviour))]
        [Tooltip("The actor with a DialogueBehaviour on it. Only needed if IsGlobal is checked")]
        public FsmOwnerDefault Actor;

        [RequiredField]
        [Tooltip("The value of the global variable.")]
        public T Value;

        public override string ErrorCheck()
        {
            Variable variable = null;
            if (IsGlobal.Value) variable = DialogueSystem.GetGlobalVariable(VariableName.Value);
            else
            {
                var go = Fsm.GetOwnerDefaultTarget(Actor);
                if (go == null) return "Actor is needed for getting a local variable!";
                DialogueBehaviour b = go.GetComponent<DialogueBehaviour>();
                variable = b.GetLocalVariable(VariableName.Value);
            }

            // Variable doesn't exists
            if (variable == null)
                return VariableName.Value + " doesn't exists in the " + ((IsGlobal.Value) ? "Dialogue Editor Global Variables" : "");

            // Variable type is not the right one
            if (!FsmVariableUtils.CheckType(variable, typeof(T)))
                return "The value type should be " + variable.type;

            return base.ErrorCheck();
        }


        public override void OnEnter()
        {
            if (IsGlobal.Value)
            {
                var variable = DialogueSystem.GetGlobalVariable(VariableName.Value);
                FsmVariableUtils.SetVariable(variable, Value);
            }
            else
            {
                var go = Fsm.GetOwnerDefaultTarget(Actor);
                if (go != null)
                {
                    DialogueBehaviour b = go.GetComponent<DialogueBehaviour>();
                    var variable = b.GetLocalVariable(VariableName.Value);
                    FsmVariableUtils.SetVariable(variable, Value);
                }
            }


            Finish();
        }

        
    }
   
}
#endif