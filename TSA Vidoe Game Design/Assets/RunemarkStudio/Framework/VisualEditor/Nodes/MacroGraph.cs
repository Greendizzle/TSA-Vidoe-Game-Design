using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Runemark.VisualEditor
{
	[System.Serializable]
	public class MacroGraph : Node, INodeCollection
	{
		#region Editor Stuff
		public Vector2 ZoomOffset { get { return _zoomOffset; } set { _zoomOffset = value; } }
		[SerializeField]Vector2 _zoomOffset = Vector2.zero;

		public string SelectedVariableID { get; set; }

		public List<CommentBox> CommentBoxes  { get { return _commentBoxes; } set { _commentBoxes = value; } }
		[SerializeField]List<CommentBox> _commentBoxes = new List<CommentBox>(); 

		#endregion

		public NodeCollection Nodes { get { return _nodes; }}
		[SerializeField] NodeCollection _nodes = new NodeCollection ();



	/*	[SerializeField]List<Node> _nodes = new List<Node>();

		#region INodeCollection implementation
		public List<Node> Nodes{ get { return _nodes; }}
		public List<Node> NodesRecursive 
		{
			get
			{
				List<Node> nodes = new List<Node>();
				if (Nodes.Count > 0)
					nodes.AddRange(Nodes);

				// Gather nodes from child graphs too
				foreach (FunctionGraph graph in Graphs)
					nodes.AddRange(graph.NodesRecursive);

				return nodes;
			}
		} 
		public List<FunctionGraph> Graphs { get { return (List<FunctionGraph>)Nodes.FindAll(node => node.GetType() == typeof(FunctionGraph)).Cast<FunctionGraph>(); } }
		public List<FunctionGraph> GraphsRecursive { get { return (List<FunctionGraph>)NodesRecursive.FindAll( node => node.GetType() == typeof(FunctionGraph)).Cast<FunctionGraph>(); } }

		public void Add(Node node)	{ Nodes.Add(node); }
		public bool Remove(string nodeID) {  if (Contains(nodeID)) return Remove(Find(nodeID)); return false; }
		public bool Remove(Node node)
		{ 
			bool b = Nodes.Remove(node);
			if(b)
				DestroyImmediate(node, true);
			return b;
		}

		public bool Contains(Node node) { return Nodes.Contains(node); }
		public bool Contains(string nodeID) { return Find(nodeID) != null; }
		public void Clear() { Nodes.Clear(); }

		public Node Find(string nodeID, bool recursive = false)
		{
			var list = (recursive) ? NodesRecursive : Nodes;
			foreach (var node in list)
				if (node.ID == nodeID)
					return node;
			return null;
		}

		public FunctionGraph FindGraph(string nodeID, bool recursive = false)
		{
			var list = (recursive) ? GraphsRecursive : Graphs;
			foreach (var graph in list)
				if (graph.ID == nodeID)
					return graph;
			return null;
		}
		#endregion
*/

		#region implemented abstract members of Node
		public override string Tooltip { get { return
				"Macro"; 
			} }

		/// <summary>
		/// Initializes the variables.
		/// </summary>
		protected override void OnInit()
		{
            // [Macro] - Init variables
		}

		protected override Variable CalculateOutput(string name)
		{
			// [Macro] - output calculation
			return Variables[name];
		}
		#endregion

		public override Node Copy(bool copyConnections = false)
		{
			MacroGraph newNode = (MacroGraph)base.Copy(copyConnections);

			foreach (var node in Nodes.GetAll)
				newNode.Nodes.Add(node.Copy(copyConnections));
			return newNode;
		}		
	}
}