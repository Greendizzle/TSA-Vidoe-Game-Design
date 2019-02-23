using Runemark.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runemark.VisualEditor.Actions
{
    [HelpURL("http://runemarkstudio.com/dialogue-system-documentation/#relations")]
    [Serializable]
    [Info("Relation", "Icons/Relation")]
    public class Relation : Node, ITypedNode
    {
        public enum Mode
        {
            Less, LessOrEqual, Equals, NotEqual, GreaterOrEqual, Greater
        }

        public override string Tooltip
        {
            get
            {
                return "Relation value";
            }
        }

        /// <summary>
        /// Allowed types this node can be set to.
        /// </summary>
        public List<Type> AllowedTypes
        {
            get
            {
                if (_allowedTypes == null || _allowedTypes.Count == 0)
                    UpdateAllowedTypes();
                return _allowedTypes;
            }
        }
        List<Type> _allowedTypes;

        public Mode RelationMode
        {
            get { return _relationMode; }
            set
            {
                var oldRM = _relationMode;          
                _relationMode = value;

                if(oldRM != _relationMode)
                    UpdateAllowedTypes();               
            }
        }
        [SerializeField] Mode _relationMode;

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
                    RunemarkDebug.Error("Relation node can't be set to type {0}.", value);
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

                    if (OnTypeChanged != null) OnTypeChanged.Invoke();
                }
            }

        }
        Type _type;
        [SerializeField] string _serializedType = "";

        public Action OnTypeChanged { get; set; }

        protected override void OnInit()
        {
            Type = AllowedTypes[0];

            PinCollection.AddInput("A", Type);
            PinCollection.AddInput("B", Type);
            PinCollection.AddOutput("Result", typeof(bool));
        }        
        protected override Variable CalculateOutput(string name)
        {
            if (name != "Result")
            {
                RunemarkDebug.Error("{0} node doesn't have {1} output", Name, name);
                return null;
            }

            Variable A = GetInput("A");
            Variable B = GetInput("B");
            Variable result = new Variable(typeof(bool));

            if (Type == typeof(string))
                result.Value = Compare(A.ConvertedValue<string>(), A.ConvertedValue<string>());
            else
                result.Value = Compare(Convert.ToDouble(A.Value), Convert.ToDouble(B.Value));
            return result;
        }
        bool Compare(double a, double b)
        {
            bool result = false;
            switch (RelationMode)
            {
                case Mode.Less: result = a < b; break;
                case Mode.LessOrEqual: result = a <= b; break;
                case Mode.Equals: result = a == b; break;
                case Mode.NotEqual: result = a != b; break;
                case Mode.GreaterOrEqual: result = a >= b; break;
                case Mode.Greater: result = a > b; break;
            }
            return result;
        }
        bool Compare(string a, string b)
        {
            bool result = false;
            switch (RelationMode)
            {
                case Mode.Equals: result = a == b; break;
                case Mode.NotEqual: result = a != b; break;
            }
            return result;
        }
        public override Node Copy(bool copyConnections = false)
        {
            Relation newNode = (Relation)base.Copy(copyConnections);
            newNode.Type = Type;
            newNode.RelationMode = RelationMode;
            return newNode;
        }

        void UpdateAllowedTypes()
        {
            _allowedTypes = new List<Type>() { typeof(int), typeof(float) };
            if (RelationMode == Mode.Equals || RelationMode == Mode.NotEqual)
            {
                _allowedTypes.Add(typeof(string));
                _allowedTypes.Add(typeof(bool));
            }

            if (!_allowedTypes.Contains(Type))
                Type = AllowedTypes[0];
        }

    }
	
}