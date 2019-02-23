using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runemark.VisualEditor.Actions
{
    [HelpURL("http://runemarkstudio.com/dialogue-system-documentation/#debug-log")]
    [System.Serializable] 
	[Info("DebugLog", "UnityEditor.ConsoleWindow")]
	public class DebugLog :  ExecutableNode
	{
		public bool EveryFrame = false;
        		
		#region implemented abstract members of Node
		public override string Tooltip { get { return
				"Log message to Unity console."; 
			} }

		protected override void OnInit()
		{
			PinCollection.AddInput("Message", typeof(string));
			Variables.Add("Message", "", "Input");
		}

		protected override Variable CalculateOutput(string name) { return null; }
		#endregion


		#region implemented abstract members of ExecutableNode
		protected override void OnEnter()
		{
            Variable msg = GetInput("Message");
			Debug.Log("DEBUGLOG: " + msg.ConvertedValue<string>());
		
			// Finish right after entered.
			if(!EveryFrame)
				IsFinished = true;
		}
        
		protected override void OnUpdate()
		{
            if (!EveryFrame) return;

            Variable msg = GetInput("Message");
            Debug.Log("DEBUGLOG: " + msg.ConvertedValue<string>());
			IsFinished = true;
		}
		protected override void OnExit(){}
		#endregion


		public override Node Copy(bool copyConnections = false)
		{
			DebugLog newNode = (DebugLog)base.Copy(copyConnections);
			newNode.EveryFrame = EveryFrame;
			return newNode;
		}
		
	}
}
