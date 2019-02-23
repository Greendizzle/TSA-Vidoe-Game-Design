 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Runemark.Common;

namespace Runemark.VisualEditor.Actions
{
    [HelpURL("http://runemarkstudio.com/dialogue-system-documentation/#constant")]
    [Serializable]
    [Info("Constant", "Icons/Constant")]
    public class Constant : Node, ITypedNode
    {
        public override string Tooltip
        {
            get
            {
                return "Constant value";
            }
        }

        /// <summary>
        /// Allowed types this node can be set to.
        /// </summary>
        public List<Type> AllowedTypes
        {
            get
            {
                return new List<Type>() { typeof(int), typeof(float), typeof(bool), typeof(string) };
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
                    RunemarkDebug.Error("Constant node can't be set to type {0}.", value);
                    return;
                }

                // Set Type
                Type oldType = _type;
                _type = value;
                _serializedType = _type.ToString();

                
                if (oldType != null && oldType != _type)
                {
                    var pin = PinCollection.Get("Value");
                    if (pin != null) pin.VariableType = _type;
                    var variable = Variables.GetByName("Value");
                    if (variable != null) variable.type = _type;

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
            PinCollection.AddOutput("Value", Type);
            Variables.Add("Value", Type, TypeUtils.GetDefaultValue(Type), "Input");
        }

        protected override Variable CalculateOutput(string name)
        {
            Variable v = Variables.GetByName(name);
            return v;
        }

        public override Node Copy(bool copyConnections = false)
        {
            Constant copy = (Constant)base.Copy(copyConnections);
            copy.Type = Type;
            return copy;
        }












        #region implemented abstract members of Node

        protected override void SetSubtype(System.Type type)
        {
            if (type != null) Type = type;
        }     
        #endregion       
    }    
}