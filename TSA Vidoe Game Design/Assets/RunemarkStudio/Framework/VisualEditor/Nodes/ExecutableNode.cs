using Runemark.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runemark.VisualEditor
{
	[System.Serializable]
	public abstract class ExecutableNode : Node
	{		
		protected virtual bool AutoGenerateInTrans { get { return true; }}
		protected virtual bool AutoGenerateOutTrans { get { return true; }}

		public virtual ExecutableNode NextNode
		{
			get
			{
				ExecutableNode next = null;

				// If we already calculated return that value.
				if (_calculatedNextNode != null)
				{
                    if (_calculatedNextNode.Connections.Count > 0)
                    {
                        var id = _calculatedNextNode.Connections[0].NodeID;

                        var node = Parent.Nodes.Find(id, false);
                        next = node as ExecutableNode;
                    }
				}
				else
				{
					var transitions = PinCollection.Get(PinType.TransOut);
					if (transitions != null && transitions.Count > 0 && transitions[0].Connections.Count > 0)
					{
						var id = transitions[0].Connections[0].NodeID;
						next = Parent.Nodes.Find(id, false) as ExecutableNode;
					}
				}


				return next;
			}
		}

		public bool IsEntered;
		public bool IsFinished;

		[SerializeField] protected Pin _calculatedNextNode;

        public override void RuntimeInit(VisualEditorBehaviour owner)
        {
            base.RuntimeInit(owner);
            IsEntered = false;
            IsFinished = false;
            _calculatedNextNode = null;          
        }


        public void Enter()
		{
			IsEntered = true;
			IsFinished = false;

			time = 0f;
			foreach (var pin in PinCollection.Get(PinType.Output))
			{
                Variable result = CalculateOutput(pin.Name);
                result.Group = VariableGroups.SYSTEM;
                StoreVariable(pin.Name, result);
			}
			OnEnter();
		}

		public void Update()
		{
			time += Time.deltaTime;
			OnUpdate();
		}

		public void Exit()
		{
			IsEntered = false;
			OnExit();
		}

		#region Overrides
		protected sealed override void OnInstantiated()
		{
			if(AutoGenerateInTrans && PinCollection.Count(PinType.TransIn) == 0)
				PinCollection.AddInTransition("IN");
			if (AutoGenerateOutTrans && PinCollection.Count(PinType.TransOut) == 0)
				PinCollection.AddOutTransition("OUT");
		}		
		#endregion
        

		#region Abstract Methods
		/// <summary>
		/// Use this event for calculation when the execution flow enters this node. 
		/// This event is called only once.
		/// </summary>
		protected abstract void OnEnter();

		/// <summary>
		/// This event is called every frame.
		/// </summary>
		protected abstract void OnUpdate();

		/// <summary>
		/// Use this event for calculation when the execution flow leaves this node. 
		/// This event is called only once.
		/// </summary>
		protected abstract void OnExit();
		#endregion
        		
	}
}