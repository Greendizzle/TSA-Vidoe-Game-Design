using Runemark.Common;
using Runemark.DialogueSystem;
using UnityEngine;

namespace Runemark.VisualEditor.Actions
{
    [HelpURL("http://runemarkstudio.com/dialogue-system-documentation/#variable-set-get")]
    [System.Serializable]
    [Info("Variables/Get", "")]
    public class GetVariableNode : Node, IVariableNode
    {
        public override string Tooltip
        {
            get
            {
                return string.Format("Read the value of the {0} {1} variable.",
                                Scope, Name);
            }
        }
        
        public string VariableName
        {
            get { return _variableName; }
            set { _variableName = value; }
        }
        [SerializeField] string _variableName;

        public VariableScope Scope
        {
            get { return _scope; }
            set { _scope = value; }
        }
        [SerializeField] VariableScope _scope;

        protected override void OnInit()
        {
            PinCollection.AddOutput("Value", typeof(object));
        }
        protected override Variable CalculateOutput(string name)
        {
            return GetVariable();
        }

        public void ChangeScope(VariableScope scope)
        {
            if (Scope != scope)
            {
                Scope = scope;
                ChangeVariable(VariableName);
                HasChanges = true;
                Root.HasChanges = true;
            }
        }
        public void ChangeVariable(string name)
        {
            VariableName = name;
            Variable v = GetVariable();
            if (v == null) VariableName = "";

            var value = PinCollection.Get("Value");
            value.VariableType = (v != null) ? v.type : typeof(object);
            HasChanges = true;
            Root.HasChanges = true;
        }
        public Variable GetVariable()
        {
            Variable variable = null;
            if (Scope == VariableScope.Local)
                variable = LocalVariables.GetByName(VariableName);
            else if (Scope == VariableScope.Global)
            {
                var list = Resources.LoadAll<DialogueSystemGlobals>("");
                var globals = (list.Length > 0) ? list[0] : null;
                if (globals != null)
                    variable = globals.Variables.GetByName(VariableName);
            }
            return variable;
        }
        public override Node Copy(bool runtime = false)
        {
            var copy = (GetVariableNode)base.Copy(runtime);
            copy.ChangeScope(Scope);
            copy.ChangeVariable(VariableName);
            return copy;
        }
    }      
}
