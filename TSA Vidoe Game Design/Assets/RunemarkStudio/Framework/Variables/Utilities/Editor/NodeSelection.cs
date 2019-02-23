using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace Runemark.VisualEditor
{
	public class NodeSelection 
	{
        public VisualEditor VisualEditor;
		public NodeLayout Selected { get { return (SelectionList.Count > 0) ? SelectionList[0] : null; } }
		public List<NodeLayout> SelectionList = new List<NodeLayout>();
		public bool Dragging { get; set; }
		NodeLayout _mouseOver;


		/// <summary>
		/// Is the mouse over a selected node?
		/// </summary>
		/// <returns><c>true</c>, if over selection was moused, <c>false</c> otherwise.</returns>
		/// <param name="mousePosition">Mouse position.</param>
		public bool MouseOverSelection(Vector2 mousePosition)
		{
			foreach (var l in SelectionList)
				if (l.Rect.Contains(mousePosition))	return true;
			return false;
		}

		public Pin MouseOverPin(Vector2 mousePosition)
		{
			if (_mouseOver != null && _mouseOver.Rect.Contains(mousePosition))
				return _mouseOver.MouseOverPin(mousePosition);
			return null;
		}

		public bool MouseOverIsSelected()
		{
			return IsSelected(_mouseOver);
		}

		/// <summary>
		/// Determines whether this node is selected or not.
		/// </summary>
		/// <returns><c>true</c> if this instance is selected the specified layout; otherwise, <c>false</c>.</returns>
		/// <param name="layout">Layout.</param>
		public bool IsSelected(NodeLayout layout) 
		{
			return layout != null && SelectionList.Contains(layout);
		}

		/// <summary>
		/// Sets a node that which have the mouse cursor over it.
		/// </summary>
		/// <param name="window">Window.</param>
		public void onMouseOver(NodeLayout window) { _mouseOver = window; }

		/// <summary>
		/// Selects the node below the cursor.
		/// </summary>
		/// <param name="additive">If set to <c>true</c> additive.</param>
		public void Select(bool additive)
		{
			if(!TrySelectNode(additive))
				Clear();
		}

		public bool TrySelectNode(bool additive)
		{
			if (_mouseOver != null)
			{
				Select(_mouseOver, additive);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Selects the specified node.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <param name="additive">If set to <c>true</c> additive.</param>
		public void Select(NodeLayout layout, bool additive)
		{
			if (IsSelected(layout))
			{
				if (additive) Remove(layout);
				else return;
			}

			if(!additive || layout == null) Clear();

			if (layout != null)
			{
				SelectionList.Add(layout);
				layout.Selected = true;				
			}
		}

		/// <summary>
		/// Select the specified list of nodes.
		/// </summary>
		/// <param name="list">List.</param>
		public void Select(List<NodeLayout> list)
		{
			Clear();
			foreach (var w in list)
				Select(w, true);
		}

		/// <summary>
		/// Removes the specified node from the selection.
		/// </summary>
		/// <param name="layout">Layout.</param>
		public void Remove(NodeLayout layout)
		{
			layout.Selected = false;
			SelectionList.Remove(layout);
		}

        /// <summary>
        /// Clear the selection.
        /// </summary>
        /// <param name="clearUnitySelection"></param>
        public void Clear(bool clearUnitySelection = true)
		{
			foreach (var w in SelectionList)
				w.Selected = false;					
			SelectionList.Clear();
            if(clearUnitySelection)
			    Selection.objects = new Object[0];
		}
                
        public void Update()
        {
            if (Selected != null)
            {
                bool clear = false;

                // If there is a GameObject selected clear the selection.
                if (Selection.gameObjects.Length > 0)
                    clear = true;

                // If there is an Object selected...
                else if (Selection.objects.Length > 0)
                {
                    // ... and clear the selection if...
                    foreach (var o in Selection.objects)
                    {
                        // ... any of the selected object is not a node.
                        if (!o.GetType().IsSubclassOf(typeof(Node)))
                        {
                            clear = true;
                            break;
                        }
                        // ... or any of the selected object is not selected by
                        // the node selection.
                        var selected = SelectionList.Find(x => x.Node == o || x.Node.Root == o);
                        if (selected == null)
                        {
                            clear = true;
                            break;
                        }
                    }
                }

                if (clear)
                {
                    Clear(false);
                    VisualEditor.Repaint();
                }                
            }         
        }
	}
}
