using Runemark.VisualEditor;

namespace Runemark.DialogueSystem
{
	public class DialogueGraph : FunctionGraph
	{
		protected override void OnEnter()
		{
			Owner.CallEvent("OnOpen");
		}

	}
}