using Runemark.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runemark.VisualEditor.Actions
{
    [HelpURL("http://runemarkstudio.com/dialogue-system-documentation/#portals")]
    [Serializable]
    [Info("Other/Portal (Variable)", "")]
    public class PortalVariable : Node, ITypedNode
    {       
        public override string Tooltip
        {
            get
            {
                return "Portal";
            }
        }

        public string PortalName
        {
            get { return _portalName; }
            set { _portalName = value; }
        }
        [SerializeField] string _portalName = "";

        public bool IsInput
        {
            get { return _isInput; }
            set
            {
                bool old = _isInput;
                _isInput = value;
                if (old != _isInput)
                    onInputTypeChanged(old, _isInput);
            }
        }
        [SerializeField]  bool _isInput = true;

        public PortalVariable InputPortal
        {
            get
            {
                var node = Root.Nodes.Find(_inputPortalID);
                if (node != null && node is PortalVariable)
                    return (PortalVariable)node;
                return null;
            }
            set
            {
                _inputPortalID = (value != null) ? value.ID : "";
            }
        }

        [SerializeField] string _inputPortalID;

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
                    RunemarkDebug.Error("Portal node can't be set to type {0}.", value);
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
            onInputTypeChanged(IsInput, IsInput);
        }

        protected override Variable CalculateOutput(string name)
        {
            if (IsInput)
            {
                return GetInput(name);
            }
            else
            {
                if (InputPortal != null)
                    return InputPortal.CalculateOutput(name);
            }
            return new Variable(Type);
        }

        void onInputTypeChanged(bool wasInput, bool isInput)
        {
            var pin = PinCollection.Get("Value");
            if (pin != null) Pin.Disconnect(pin);

            PinCollection.Clear();
            if (IsInput) PinCollection.AddInput("Value", Type);
            else PinCollection.AddOutput("Value", Type);
        }

        public override Node Copy(bool runtime = false)
        {
            var copy = (PortalVariable)base.Copy(runtime);                       
            copy.PortalName = PortalName;
            copy._isInput = _isInput;
            copy._inputPortalID = _inputPortalID;
            return copy;
        }
    }


}
