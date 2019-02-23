using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Runemark.Common;

namespace Runemark.VisualEditor
{
	[System.Serializable]
	public class NodeCollection
	{
		[SerializeField]List<Node> _nodes = new List<Node>();

		public Node this [int index]
		{ 
			get { return _nodes[index]; } 
			set 
			{ 
				if (_nodes.Count == index)
					_nodes.Add(value);
				if (_nodes.Count < index || index < 0)
					return;
				_nodes[index] = value;
			}
		}



		public List<Node> GetAll{ get { return _nodes; }}
		public List<Node> GetAllRecursive 
		{
			get
			{
				List<Node> nodes = new List<Node>();
				if (GetAll.Count > 0)
					nodes.AddRange(GetAll);

				// Gather nodes from child graphs too
				foreach (FunctionGraph graph in Graphs)
					nodes.AddRange(graph.Nodes.GetAllRecursive);

				return nodes;
			}
		} 
		public List<FunctionGraph> Graphs { get { return (List<FunctionGraph>)GetAll.FindAll(node => node.GetType() == typeof(FunctionGraph)).Cast<FunctionGraph>(); } }
		public List<FunctionGraph> GraphsRecursive { get { return (List<FunctionGraph>)GetAllRecursive.FindAll( node => node.GetType() == typeof(FunctionGraph)).Cast<FunctionGraph>(); } }

		public void Add(Node node)	{ GetAll.Add(node); }
		public bool Remove(string nodeID) {  if (Contains(nodeID)) return Remove(Find(nodeID)); return false; }
		public bool Remove(Node node)
		{ 
			bool b = GetAll.Remove(node);
			if(b)
			 	Object.DestroyImmediate(node, true);
			return b;
		}

		public bool RemoveAll(System.Predicate<Node> match)
		{
			var list = _nodes.FindAll(match);
			foreach (var n in list)
				Remove(n);
			return true;
		}

		public bool Contains(Node node) { return GetAll.Contains(node); }
		public bool Contains(string nodeID) { return Find(nodeID) != null; }
		public void Clear() { GetAll.Clear(); }

		public Node Find(string nodeID, bool recursive = false)
		{
			var list = (recursive) ? GetAllRecursive : GetAll;
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

        public List<T> FindAll<T>(bool recursive = false) where T : Node
        {
            var list = (recursive) ? GetAllRecursive : GetAll;
            List<T> result = new List<T>();
            foreach (var node in list)
                if (node.GetType() == typeof(T))
                    result.Add((T)node);
            return result;
        }    
	}

}