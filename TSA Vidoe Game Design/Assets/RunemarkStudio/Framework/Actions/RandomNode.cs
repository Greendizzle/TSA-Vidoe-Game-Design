using Runemark.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runemark.VisualEditor.Actions
{
    [HelpURL("http://runemarkstudio.com/dialogue-system-documentation/#random")]
    [Serializable]
	[Info("Random", "Icons/Random")]
	public class RandomNode : Node, ITypedNode
    {
		public override string Tooltip { get { return "Returns a random number between Min [inclusive] and Max [exclusive]."; }}

        /// <summary>
        /// Allowed types this node can be set to.
        /// </summary>
        public List<Type> AllowedTypes
        {
            get
            {
                return new List<Type>() { typeof(int), typeof(float) };
            }
        }

        /// <summary>
        /// The type of the constant.
        /// </summary>
        public Type Type
        {
            get
            {
                if (_type == null && _serializedType != "")
                    _type = Type.GetType(_serializedType);
                if (_type == null)
                    Type = typeof(object);
                return _type;
            }
            set
            {
                if (_type == value) return;
                if (!AllowedTypes.Contains(value) && value != typeof(object))
                {
                    RunemarkDebug.Error("Random node can't be set to type {0}.", value);
                    return;
                }

                // Set Type
                Type oldType = _type;
                _type = value;
                _serializedType = _type.ToString();

                if (oldType != null && oldType != _type)
                {
                    var min = Variables.GetByName("Min");
                    if (min != null) min.type = _type;

                    var max = Variables.GetByName("Max");
                    if (max != null) max.type = _type;

                    var pin = PinCollection.Get("Random");
                    if (pin != null) pin.VariableType = _type;

                    if (OnTypeChanged != null) OnTypeChanged.Invoke();
                }
            }

        }
        Type _type;
        [SerializeField]
        string _serializedType = "";

        public Action OnTypeChanged { get; set; }


		protected override void OnInit()
		{
            Type = AllowedTypes[0];

            Variables.Add("Min", Type, null, "");
			Variables.Add("Max", Type, null, "");

			PinCollection.AddOutput("Random", Type);
		}

		protected override Variable CalculateOutput(string name)
		{
            Variable min = Variables["Min"];
            Variable max = Variables["Max"];
            Variable result = new Variable(Type);

			if(Type == typeof(int))
				result.Value = UnityEngine.Random.Range(min.ConvertedValue<int>(), max.ConvertedValue<int>());
			else if (Type == typeof(float))
                result.Value = UnityEngine.Random.Range(min.ConvertedValue<float>(), max.ConvertedValue<float>());

			return result;
		}

        public override Node Copy(bool runtime = false)
        {
            var copy = (RandomNode)base.Copy(runtime);
            copy.Type = Type;
            return copy;
        }


    }
}