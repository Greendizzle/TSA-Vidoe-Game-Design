using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runemark.VisualEditor.Actions
{
    [HelpURL("http://runemarkstudio.com/dialogue-system-documentation/#to-string")]
    [System.Serializable] [Info("ToString", "Icons/ToString")]
	public class ToString : Node
	{
		#region implemented abstract members of Node
		public override string Tooltip { get { return
				"Converts the input to string."; 
			} }

		protected override void OnInit()
		{
			PinCollection.AddInput("Input", typeof(object));
            var input = PinCollection.Get("Input");
            input.IsDynamicType = true;

			PinCollection.AddOutput("Result", typeof(string));
		}

		protected override Variable CalculateOutput(string name)
		{
            Variable result = new Variable(typeof(string));
            result.Value = GetString(GetInput("Input").Value);
            return result;
		}
		#endregion

	
		string GetString(object v)
		{
			if (v == null) return " null ";

			List<System.Type> _simpleConvert = new List<System.Type>(){ typeof(string), typeof(int), typeof(float), typeof(bool) };
			if(_simpleConvert.Contains(v.GetType()))
				return ""+v;
			else
				return v.ToString();
		}

		
	}
}
