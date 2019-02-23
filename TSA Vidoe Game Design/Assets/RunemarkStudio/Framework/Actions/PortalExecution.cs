using Runemark.Common;
using System;
using UnityEngine;

namespace Runemark.VisualEditor.Actions
{
    [HelpURL("http://runemarkstudio.com/dialogue-system-documentation/#portals")]
    [Serializable]
    [Info("Other/Portal (Execution)", "")]
    public class PortalExecution : ExecutableNode
    {
        protected override bool AutoGenerateInTrans { get { return IsInput; } }
        protected override bool AutoGenerateOutTrans { get { return !IsInput; } }
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
        [SerializeField] bool _isInput = false;

        public PortalExecution OutputPortal
        {
            get
            {
                var node = Root.Nodes.Find(_outputPortalID);
                if (node != null && node is PortalExecution)
                    return (PortalExecution)node;


                return null;
            }
            set
            {
                _outputPortalID = (value != null) ? value.ID : "";
                PortalName = (value != null) ? value.PortalName : "";
            }
        }
        [SerializeField] string _outputPortalID;

        protected override void OnInit()
        {
            
        }
        protected override void OnEnter()
        {
            // If this is an input portal...
            if (IsInput)
            {
                // ... find the output portal 
                if (OutputPortal == null)
                {
                    RunemarkDebug.Error("{0} ({1}) doesnt have output portal!", PortalName, ID);
                    return;
                }


                _calculatedNextNode = OutputPortal.PinCollection.Get("OUT");
                IsFinished = true;
            }                 
        }

        protected override Variable CalculateOutput(string name)
        {
            return null;
        }
                       
        void onInputTypeChanged(bool wasInput, bool isInput)
        {
            string removeName = (IsInput) ? "OUT" : "IN";
            var pin = PinCollection.Get(removeName);
            if (pin != null) Pin.Disconnect(pin);

            PinCollection.Clear();
            if (IsInput) PinCollection.AddInTransition("IN");
            else PinCollection.AddOutTransition("OUT");
        }

        public override Node Copy(bool runtime = false)
        {
            var copy = (PortalExecution)base.Copy(runtime);
            copy.PortalName = PortalName;
            copy.IsInput = IsInput;
            copy.OutputPortal = OutputPortal;
            return copy;
        }


        // unused
        protected override void OnExit()
        {

        }       
        protected override void OnUpdate()
        {

        }
    }
}
