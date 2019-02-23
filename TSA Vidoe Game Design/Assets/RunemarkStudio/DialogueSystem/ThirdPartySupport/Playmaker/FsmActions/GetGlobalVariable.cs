#if PLAYMAKER
using HutongGames.PlayMaker;
using System.ComponentModel;
using System;
using Runemark.VisualEditor;

namespace Runemark.DialogueSystem.Playmaker
{
    [HelpUrl("http://runemarkstudio.com/dialogue-system-documentation/#playmaker")]
    [ActionCategory("Runemark Dialogue System")]
    [Tooltip("Gets the global or local string variable of the dialogue system.")]
    public class GetStringVariable : GetVariable<FsmString> { }

    [HelpUrl("http://runemarkstudio.com/dialogue-system-documentation/#playmaker")]
    [ActionCategory("Runemark Dialogue System")]
    [Tooltip("Gets the global or local int variable of the dialogue system.")]
    public class GetIntVariable : GetVariable<FsmInt> { }

    [HelpUrl("http://runemarkstudio.com/dialogue-system-documentation/#playmaker")]
    [ActionCategory("Runemark Dialogue System")]
    [Tooltip("Gets the global or local float variable of the dialogue system.")]
    public class GetFloatVariable : GetVariable<FsmFloat> { }

    [HelpUrl("http://runemarkstudio.com/dialogue-system-documentation/#playmaker")]
    [ActionCategory("Runemark Dialogue System")]
    [Tooltip("Gets the global or local bool variable of the dialogue system.")]
    public class GetBoolVariable : GetVariable<FsmBool> { }
    
    public class GetVariable<T> : FsmStateAction where T : NamedVariable
    {
        [RequiredField]
        [Tooltip("The name of the global variable.")]
        public FsmString VariableName;

        [Tooltip("Determines if we want to get GLobal or Local variable")]
        public FsmBool IsGlobal;

        [CheckForComponent(typeof(DialogueBehaviour))]
        [Tooltip("The actor with a DialogueBehaviour on it. Only needed if IsGlobal is checked")]
        public FsmOwnerDefault Actor;

        [ActionSection("Results")]
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The float value of the global variable")]
        public T result;

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
                return VariableName.Value + " doesn't exists in the " + ((IsGlobal.Value) ? "Dialogue Editor Global Variables" : "local...");

            // Variable type is not the right one
            if (!FsmVariableUtils.CheckType(variable, typeof(T)))
                return "The variable has a wrong type ("+variable.type+"), please use the right action";

            return base.ErrorCheck();
        }


        public override void OnEnter()
        {
            if (IsGlobal.Value)
            {
                var variable = DialogueSystem.GetGlobalVariable(VariableName.Value);
                FsmVariableUtils.SetFsmVariable(result, variable);
            }
            else
            {
                var go = Fsm.GetOwnerDefaultTarget(Actor);
                if (go != null)
                {
                    DialogueBehaviour b = go.GetComponent<DialogueBehaviour>();
                    var variable = b.GetLocalVariable(VariableName.Value);
                    FsmVariableUtils.SetFsmVariable(result, variable);
                }
            }
            Finish();
        }        
    }

   
}
#endif