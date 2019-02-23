using UnityEngine;

namespace Runemark.VisualEditor.Actions
{
    [HelpURL("http://runemarkstudio.com/dialogue-system-documentation/#call-event")]
    [System.Serializable] 
	[Info("Call Event", typeof(UnityEngine.EventSystems.EventTrigger))]
	public class EventCall : ExecutableNode
	{
		protected override void OnInit()
		{
			Variables.Add("EventName", "");            
		}

		protected override Variable CalculateOutput(string name) { return null; }
		public override string Tooltip { get { return ""; } }
	
		protected override void OnEnter()
		{
            Variable v = Variables["EventName"];
			IsFinished = true;
			Owner.CallEvent(v.ConvertedValue<string>());
		}

		protected override void OnUpdate(){}
		protected override void OnExit(){}

	}
}