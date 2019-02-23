using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runemark.VisualEditor.Actions
{
    [HelpURL("http://runemarkstudio.com/dialogue-system-documentation/#branch")]
    [System.Serializable] [Info("Branch", typeof(UnityEngine.EventSystems.EventSystem))]
	public class Branch : ExecutableNode
	{
		#region implemented abstract members of Node

		public override string Tooltip { get { return
				"Branch Statement\n" +
				"If condition is true, execution goes to True, otherwise it goes to False."; 
			} }
		
		protected override void OnInit()
		{
			PinCollection.AddInput("Condition", typeof(bool));
			PinCollection.AddOutTransition("True");
			PinCollection.AddOutTransition("False");
		}

		protected override Variable CalculateOutput(string name)
		{
			return null;
		}

		#endregion

		#region implemented abstract members of ExecutableNode

		protected override void OnEnter()
		{
			Variable input = GetInput("Condition");
            string name = (input.ConvertedValue<bool>()) ? "True" : "False";
            _calculatedNextNode = PinCollection.Get(name);
			IsFinished = true;
		}

		protected override void OnUpdate()
		{
			
		}

		protected override void OnExit()
		{
			
		}

		#endregion


	
	}
}