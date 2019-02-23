namespace Runemark.VisualEditor.Actions
{
	public abstract class EventListener : ExecutableNode
	{
		protected override bool AutoGenerateInTrans	{ get { return false; } }
	}
}