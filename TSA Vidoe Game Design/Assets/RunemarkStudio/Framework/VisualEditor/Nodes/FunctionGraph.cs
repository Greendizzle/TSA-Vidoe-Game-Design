using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Runemark.VisualEditor.Actions;
using Runemark.Common;

namespace Runemark.VisualEditor
{
	[System.Serializable]
	public class FunctionGraph : ExecutableNode, INodeCollection
	{
		#region Editor Stuff
		public Vector2 ZoomOffset { get { return _zoomOffset; } set { _zoomOffset = value; } }
		[SerializeField]Vector2 _zoomOffset = Vector2.zero;

		public string SelectedVariableID { get { return selectedVariableID; } set { selectedVariableID = value; } }
		[SerializeField]string selectedVariableID;

		public List<CommentBox> CommentBoxes  { get { return _commentBoxes; } set { _commentBoxes = value; } }
		[SerializeField]List<CommentBox> _commentBoxes = new List<CommentBox>();

        #endregion

        public override string Tooltip
        {
            get
            {
                return
"Custom function.";
            }
        }

        public NodeCollection Nodes { get { return _nodes; }}

		[SerializeField]
        NodeCollection _nodes = new NodeCollection ();

		public List<ExecutableNode> ExecQueue = new List<ExecutableNode>();

        public override void RuntimeInit(VisualEditorBehaviour owner)
        {
            base.RuntimeInit(owner);
            foreach (var n in Nodes.GetAll)
                n.Parent = this;
        }

       

		protected override void OnInit()
		{ 
			// [Function Graph] - init variables
		}
        
		/// <summary>
		/// Calculate the output value once when the execution flow enters this node.
		/// </summary>
		/// <returns>The output value.</returns>
		/// <param name="output">Output.</param>
		protected override Variable CalculateOutput(string name)
		{
			// [Function Graph] - calculate output
			return Variables[name];
		}
		protected override void OnEnter()
		{
            // do nothing
        }
        protected override void OnUpdate()
		{
			// do nothing
		}
		protected override void OnExit()
		{
            // do nothing
		}


		public EventListener OnEvent(string eventName)
		{
			foreach (var node in Nodes.GetAll)
			{
				if (node.GetType().IsSubclassOf(typeof(EventListener)) && node.Name == eventName)
					return (EventListener)node;					
			}
			return null;
		}



		public override Node Copy(bool copyConnections = false)
		{
			FunctionGraph newNode = (FunctionGraph)base.Copy(copyConnections);

			foreach (var node in Nodes.GetAll)
				newNode.Nodes.Add(node.Copy(copyConnections));

			return newNode;
		}    
        
           
    }
}