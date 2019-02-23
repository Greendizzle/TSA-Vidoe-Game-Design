using UnityEngine;

namespace Runemark.VisualEditor.Actions
{
    [HelpURL("http://runemarkstudio.com/dialogue-system-documentation/#concatenate")]
    [System.Serializable] [Info("Concatenate", "Icons/Concatenate")]
	public class Concatenate : Node
	{
		#region implemented abstract members of Node
		public override string Tooltip { get { return
				"Concatenate two string together to make a new string."; 
			} }
		
		protected override void OnInit()
		{
			PinCollection.AddInput("A", typeof(string));
			PinCollection.AddInput("B", typeof(string));
			PinCollection.AddOutput("Result", typeof(string));
		}

		protected override Variable CalculateOutput(string name)
		{
            var a = GetInput("A");
            var b = GetInput("B");

            Variable result = new Variable(a.type);
            result.Value = a.ConvertedValue<string>() + b.ConvertedValue<string>();
            return result;
		}

		#endregion		
	}
}
