
namespace Runemark.VisualEditor
{
    public enum VariableScope { Local, Global }

    public interface IVariableNode
    {
        string VariableName { get; set; }
        FunctionGraph Root { get; }

        VariableScope Scope { get; set; }
        void ChangeScope(VariableScope scope);
        void ChangeVariable(string name);
    }
}