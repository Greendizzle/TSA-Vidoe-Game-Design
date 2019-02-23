using Runemark.Common;
using System;
using UnityEngine;

namespace Runemark.VisualEditor.Actions
{
    [HelpURL("http://runemarkstudio.com/dialogue-system-documentation/#logical-operators")]
    [Serializable]
    [Info("Logical", "Icons/Logical")]
    public class Logical : Node
    {
        public enum Mode
        {
            AND, OR, NOT
        }
        public override string Tooltip
        {
            get
            {
                return "Addition (A + B).";
            }
        }
        public Mode LogicalMode
        {
            get { return _logicalMode;  }
            set
            {
                var lastMode = _logicalMode;
                _logicalMode = value;

                if (lastMode != _logicalMode)
                {
                    if (_logicalMode == Mode.NOT)
                    {
                        var b = PinCollection.Get("B");
                        if (b != null)
                        {
                            Pin.Disconnect(b);
                            PinCollection.Remove(b);
                        }
                    }
                    else if (lastMode == Mode.NOT)
                        PinCollection.AddInput("B", typeof(bool));
                    HasChanges = true;
                }
            }
        }
        [SerializeField] Mode _logicalMode;

        protected override void OnInit()
        {
            PinCollection.AddInput("A", typeof(bool));
            PinCollection.AddInput("B", typeof(bool));

            PinCollection.AddOutput("Result", typeof(bool));
        }



        protected override Variable CalculateOutput(string name)
        {
            if (name != "Result")
            {
                RunemarkDebug.Error(Name + " this node doesn't have " + name + " output");
                return null;
            }

            var A = GetInput("A");
            var B = GetInput("B");

            Variable result = new Variable(typeof(bool));
            result.Value = Calc(A.ConvertedValue<bool>(), (B !=null)? B.ConvertedValue<bool>() : false);
            return result;
        }

        bool Calc(bool a, bool b)
        {
            bool result = false;
            switch (LogicalMode)
            {
                case Mode.AND: result = a && b; break;
                case Mode.OR: result = a || b; break;
                case Mode.NOT: result = !a; break;
            }
            return result;
        }
        public override Node Copy(bool copyConnections = false)
        {
            Logical newNode = (Logical)base.Copy(copyConnections);
            newNode.LogicalMode = LogicalMode;
            return newNode;
        }
    }
}