#if PLAYMAKER
using HutongGames.PlayMaker;
using Runemark.VisualEditor;
using System;

namespace Runemark.DialogueSystem.Playmaker
{
    public static class FsmVariableUtils 
    {
        public static bool CheckType(Variable variable, Type fsmVariableType)
        {
            if (variable.type == typeof(string) && fsmVariableType == typeof(FsmString)) return true;
            if (variable.type == typeof(int) && fsmVariableType == typeof(FsmInt)) return true;
            if (variable.type == typeof(float) && fsmVariableType == typeof(FsmFloat)) return true;
            if (variable.type == typeof(bool) && fsmVariableType == typeof(FsmBool)) return true;
            return false;
        }

        public static void SetFsmVariable(NamedVariable fsmVariable, Variable variable)
        {
            if (variable.type == typeof(string) && fsmVariable is FsmString)
                (fsmVariable as FsmString).Value = variable.ConvertedValue<string>();
            else if (variable.type == typeof(int) && fsmVariable is FsmInt)
                (fsmVariable as FsmInt).Value = variable.ConvertedValue<int>();
            else if (variable.type == typeof(float) && fsmVariable is FsmFloat)
                (fsmVariable as FsmFloat).Value = variable.ConvertedValue<float>();
            else if (variable.type == typeof(bool) && fsmVariable is FsmBool)
                (fsmVariable as FsmBool).Value = variable.ConvertedValue<bool>();            
        }

        public static void SetVariable(Variable variable, NamedVariable fsmVariable)
        {
            if (variable.type == typeof(string) && fsmVariable is FsmString)
                variable.Value = (fsmVariable as FsmString).Value;
            else if (variable.type == typeof(int) && fsmVariable is FsmInt)
                variable.Value = (fsmVariable as FsmInt).Value;
            else if (variable.type == typeof(float) && fsmVariable is FsmFloat)
                variable.Value = (fsmVariable as FsmFloat).Value;
            else if (variable.type == typeof(bool) && fsmVariable is FsmBool)
                variable.Value = (fsmVariable as FsmBool).Value;
        }



    }
}
#endif