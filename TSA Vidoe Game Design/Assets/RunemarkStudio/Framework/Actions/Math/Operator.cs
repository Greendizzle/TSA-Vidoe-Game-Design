using Runemark.Common;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace Runemark.VisualEditor.Actions
{
    [HelpURL("http://runemarkstudio.com/dialogue-system-documentation/#operators")]
    [Serializable]
    [Info("Operator", "Icons/Operator")]
    public class Operator : Node, ITypedNode
    {
        public enum Mode
        {
            Add, Subtract, Multiply, Divide, Modulus
        }

        public override string Tooltip
        {
            get
            {
                return "Addition (A + B).";
            }
        }
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
                    RunemarkDebug.Error("Operator node can't be set to type {0}.", value);
                    return;
                }

                // Set Type
                Type oldType = _type;
                _type = value;
                _serializedType = _type.ToString();

                if (oldType != null && oldType != _type)
                {
                    var a = PinCollection.Get("A");
                    if (a != null) a.VariableType = _type;

                    var b = PinCollection.Get("B");
                    if (b != null) b.VariableType = _type;

                    var result = PinCollection.Get("Result");
                    if (result != null) result.VariableType = _type;

                    if (OnTypeChanged != null) OnTypeChanged.Invoke();
                }
            }

        }
        Type _type;
        [SerializeField] string _serializedType = "";
        public Action OnTypeChanged { get; set; }

        public Mode OperatorMode;
                        
        protected override void OnInit()
        {
            Type = AllowedTypes[0];

            PinCollection.AddInput("A", Type);
            PinCollection.AddInput("B", Type);
            PinCollection.AddOutput("Result", Type);
        }

        protected override Variable CalculateOutput(string name)
        {
            if (name != "Result")
            {
                RunemarkDebug.Error("{0} node doesn't have {1} output", Name, name);
                return null;
            }

            var A = GetInput("A");
            var B = GetInput("B");

            Variable result = new Variable(Type);
            double d = Calc(Convert.ToDouble(A.Value), Convert.ToDouble(B.Value));
            result.Value = Convert.ChangeType(d, Type);
            return result;
        }

        double Calc(double a, double b)
        {
            double result = 0;
            switch (OperatorMode)
            {
                case Mode.Add: result = a + b; break;
                case Mode.Subtract: result = a - b; break;
                case Mode.Multiply: result = a * b; break;
                case Mode.Divide: result = a / b; break;
                case Mode.Modulus: result = a % b; break;
            }
            return result;
        }

        public override Node Copy(bool copyConnections = false)
        {
            Operator newNode = (Operator)base.Copy(copyConnections);
            newNode.Type = Type;
            newNode.OperatorMode = OperatorMode;
            return newNode;
        }
    }    
}