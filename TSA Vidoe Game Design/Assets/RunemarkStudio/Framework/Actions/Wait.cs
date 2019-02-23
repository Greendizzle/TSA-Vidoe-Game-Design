using UnityEngine;

namespace Runemark.VisualEditor.Actions
{
    [HelpURL("http://runemarkstudio.com/dialogue-system-documentation/#wait")]
    [System.Serializable] [Info("Wait", "Icons/Wait")]
	public class Wait : ExecutableNode
	{
		float _endTime;

		protected override void OnInit()
		{
			PinCollection.AddInput("Seconds", typeof(float));
			Variables.Add("Seconds", typeof(float), 0f, "Input");
		}
		protected override Variable CalculateOutput(string name)
		{
			return null;
		}
		public override string Tooltip
		{
			get
			{
				return "Suspend this executation flow for the given amount of seconds.";
			}
		}

        protected override void OnEnter()
		{
            Variable input = GetInput("Seconds");
            _endTime = Time.time + input.ConvertedValue<float>();            
		}
		protected override void OnUpdate()
		{
            if (_endTime <= Time.time)
            {
                IsFinished = true;
            }
		}
		protected override void OnExit()
		{
            _endTime = 0f;
		}
              
    }
}
