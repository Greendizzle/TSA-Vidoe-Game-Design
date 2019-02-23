using Runemark.Common;
using Runemark.DialogueSystem;
using UnityEngine;

namespace Runemark.VisualEditor.Actions
{
    [HelpURL("http://runemarkstudio.com/dialogue-system-documentation/#variable-set-get")]
    [System.Serializable]
    [Info("Variables/Set", "")]
    public class SetVariableNode : ExecutableNode, IVariableNode
    {
        public override string Tooltip
        {
            get
            {
                return string.Format("Sets the value of the {0} {1} variable.",
                                Scope, VariableName);
            }
        }

        public string VariableName
        {
            get { return _variableName; }
            set { _variableName = value; }
        }
        [SerializeField]
        string _variableName;

        public VariableScope Scope
        {
            get { return _scope; }
            set { _scope = value; }
        }
        [SerializeField]
        VariableScope _scope;

        protected override void OnInit()
        {
            PinCollection.AddInput("Value", typeof(object));
            PinCollection.AddOutput("NewValue", typeof(object));
        }
        protected override void OnEnter()
        {
            // Get connected variable
            Variable variable = GetVariable();
            if (variable == null)
            {
                RunemarkDebug.Error("Set Node Variable is null");
                return;
            }

            // Get the input
            Variable input = GetInput("Value");
            if (input == null)
            {
                RunemarkDebug.Error("Set Node Input is null");
                return;
            }

            Debug.Log(variable.Name + " set to " + input.Value);

            variable.Value = input.Value;
            StoreVariable("NewValue", input);
            IsFinished = true;
        }

        /// <summary>
        /// Changes the scope to other one.
        /// </summary>
        /// <param name="scope"></param>
        public void ChangeScope(VariableScope scope)
        {
            if (Scope != scope)
            {
                Scope = scope;
                ChangeVariable(VariableName);
            }
        }
        /// <summary>
        /// Changes the variable to another one.
        /// </summary>
        /// <param name="name"></param>
        public void ChangeVariable(string name)
        {
            VariableName = name;
            Variable variable = GetVariable();
            if (variable == null) VariableName = "";

            var value = PinCollection.Get("Value");
            value.VariableType = (variable != null) ? variable.type : typeof(object);

            var newValue = PinCollection.Get("NewValue");
            newValue.VariableType = (variable != null) ? variable.type : typeof(object);

            RemoveStoredVariable("NewValue");

            HasChanges = true;
            Root.HasChanges = true;
        }
        /// <summary>
        /// Returns the variable the node is set to.
        /// </summary>
        /// <returns></returns>
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
            var copy = (SetVariableNode)base.Copy(runtime);
            copy.ChangeScope(Scope);
            copy.ChangeVariable(VariableName);
            return copy;
        }
        protected override Variable CalculateOutput(string name)
        {
            Variable stored = GetStoredVariable("NewValue");
            if (stored == null)
            {
                Variable value = GetInput("Value");
                stored = new Variable(value.type);
            }
            return stored;
        }

        protected override void OnExit()
        {

        }
        protected override void OnUpdate()
        {

        }
    }   
}
