using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runemark.VisualEditor.Actions
{
    [HelpURL("http://runemarkstudio.com/dialogue-system-documentation/#custom-event")]
    [System.Serializable]
    [Info("Custom Event", "Icons/CustomEvent")]
    public class SimpleEvent : EventListener
	{
		public override string Tooltip
        {
            get
            {
                return	Name + " event"; 
			}
        }
        protected override void OnInit()
		{
			
		}
		protected override Variable CalculateOutput(string name)
		{
			return null;
		}

		protected override void OnEnter()
		{
			// Finish right after entered.
			IsFinished = true;
		}

		protected override void OnUpdate()
		{
			
		}
		protected override void OnExit()
		{
			
		}
	}
}