using Runemark.VisualEditor;
using System.Collections.Generic;
using UnityEngine;

namespace Runemark.DialogueSystem
{
    [HelpURL("http://runemarkstudio.com/dialogue-system-documentation/#weighted-branch")]
    [System.Serializable]
    [Info("Weighted Branch", typeof(UnityEngine.EventSystems.EventSystem), 1)]
	public class WeightedBranch : ExecutableNode
	{       
        public List<WeightData> Weights = new List<WeightData>();        
        
		public override string Tooltip { get { return
				"Weighted Branch Statement\n" +
				"Selects an execution output based on an input number."; 
			} }	
		protected override void OnInit()
		{
			PinCollection.AddInTransition("IN");
			PinCollection.AddInput("Value", typeof(int));

            WeightData w1 = new WeightData() { VariableName = "Weight_0", OutputName = "Weight_0" };
            Variables.Add(w1.VariableName, 1, "Input");
            PinCollection.AddOutTransition(w1.OutputName);
            Weights.Add(w1);

            WeightData w2 = new WeightData() { VariableName = "Weight_1", OutputName = "Weight_1" };
            Variables.Add(w2.VariableName, 3, "Input");
            PinCollection.AddOutTransition(w2.OutputName);
            Weights.Add(w2);
        }
		protected override Variable CalculateOutput(string name)
		{
			return null;
		}

		protected override void OnEnter()
		{
            Variable var = GetInput("Value");
            int value = var.ConvertedValue<int>();

            int smallest = -1;
            Pin pin = null;
                    
            foreach (var w in Weights)
            {
                Variable wv = Variables.GetByName(w.VariableName);
                int weight = wv.ConvertedValue<int>();
                if (weight == value)
                {
                    pin = PinCollection.Get(w.OutputName);
                    break;
                }
                else if (weight > value && smallest > weight)
                {
                    pin = PinCollection.Get(w.OutputName);
                    smallest = weight;
                }
            }
            _calculatedNextNode = pin;
            IsFinished = true;
        }

		protected override void OnUpdate()
		{

		}
		protected override void OnExit()
		{

		}

        public override Node Copy(bool runtime = false)
        {
            WeightedBranch copy = (WeightedBranch)base.Copy(runtime);
            copy.Weights = new List<WeightData>();
            foreach (var w in Weights)
            {
                var copyW = new WeightData()
                {
                    OutputName = w.OutputName,
                    VariableName = w.VariableName
                };
                copy.Weights.Add(copyW);
            }
            return copy;
        }
    }

    [System.Serializable]
    public class WeightData
    {
        public string VariableName;
        public string OutputName;
    }
}
