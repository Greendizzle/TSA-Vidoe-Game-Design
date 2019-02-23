using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Runemark.Common;
using System.Linq;
using Runemark.VisualEditor.Actions;

namespace Runemark.VisualEditor
{
	[CustomEditorWindow(typeof(FunctionGraph), true)]
	public class VisualEditor : RunemarkEditorWindow
	{
		bool _enabled = true;
		double _nextRepaint = 0;
        bool _initialized
        {
            get
            {
                if (_nodeSelection == null)
                    return false;
                if (_zoomArea == null)
                    return false;    
                if (_grid == null)
                    return false;
                return true;
            }
        }

        void Init()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;          

            InitGraph();
            InitControls();
            InitGrid();

            // If no graph is loaded
            if (_loadedGraph == null)
            {
                // Try to get a graph that is selected.
                var selection = Selection.activeObject;
                if (selection != null)
                {
                    if (selection.GetType().IsSubclassOf(typeof(Node)))
                    {
                        Node n = (Node)selection;
                        LoadGraph(n.Root);
                    }
                    else
                    {
                        var selectedGO = Selection.activeGameObject;
                        if (selectedGO != null)
                        {
                            VisualEditorBehaviour b = selectedGO.GetComponent<VisualEditorBehaviour>();
                            if (b != null && b.Graph != null)
                                LoadGraph(b.Graph);
                        }
                    }
                }
            }            
        }

		public override void LoadGraph(object selectedObject)
		{
			_loadedGraph = (FunctionGraph)selectedObject;
			OnEnable();
		}

		void OnEnable()
		{
            Init();
		}
		void Update()
		{
            if (!_initialized)
            {
                Init();
                return;
            }

            if (!_enabled || _loadedGraph == null) return;

            _nodeSelection.Update();       

            if (_loadedGraph.HasChanges && _nextRepaint <= EditorApplication.timeSinceStartup)
			{
                foreach (var l in _layouts)
                    l.OnRepaint();

				Repaint();
				EditorUtility.SetDirty(_loadedGraph.Root);
				_loadedGraph.HasChanges = false;
				_nextRepaint = EditorApplication.timeSinceStartup + .1;            
			}
		}

		protected override void onGUI()
		{
            if (!_initialized)
            {
                Init();
                return;
            }

            Rect titleRect = new Rect(0, 0, position.width, 30);
			Rect sideRect = new Rect(0, 30, 225, this.position.height - 30);
			Workspace = new Rect(230, 30, this.position.width - 215, this.position.height - 45);

            _zoomArea.Draw(Workspace);

			if (!_enabled || _loadedGraph == null)
			{
				EditorGUI.HelpBox(new Rect(Workspace.x + Workspace.width / 2 - 100, Workspace.y + Workspace.height / 2 - 25, 200, 50), 
					"No graph is loaded. Please double click on a graph to load", MessageType.Info);
				return;
			}

			if(_marqueSelectionActive)
                EditorGUI.DrawRect(_marqueSelection, new Color(1, 1, 1, .2f));

			// TITLE
			GUI.Label(titleRect, ((Node)_loadedGraph).name, VisualEditorGUIStyle.WindowHeader);

			// SIDE BAR
			_localVariables.Draw(sideRect);


			// WORKSPACE
			GUI.Box(new Rect(Workspace.x + Workspace.width - 150, Workspace.y, 150f, 25f),
				"Zoom " + _zoomArea.ZoomStep + "(" + _zoomArea.ZoomValue + ")");

			updateControl();
		}	
        	
        
		#region GRID
		const float GRID_TILE_SIZE = 20f;
		const int GRID_SUBDIVISONS = 10;
		Color GRID_BACKGROUND_COLOR = new Color(.36f, .36f, .36f);
		Color GRID_SMALL_LINE_COLOR = new Color(.33f,.33f,.33f);
		Color GRID_LARGE_LINE_COLOR = new Color(.28f, .28f, .28f);

		GLGrid _grid;

		void InitGrid() {  _grid = new GLGrid(GRID_SUBDIVISONS, GRID_SMALL_LINE_COLOR, GRID_LARGE_LINE_COLOR);  }
		void DrawGrid(Rect r, Vector2 zoomOffset, int zoomStep) 
		{
			_grid.SmallTileSize = GRID_TILE_SIZE * zoomStep;		
			_grid.Draw(r, zoomOffset);
		}
		#endregion
        
		#region GRAPH
		[SerializeField] INodeCollection _loadedGraph;
        LocalVariableReorderableList _localVariables;
		List<NodeLayout> _layouts = new List<NodeLayout>();

		void InitGraph()
		{
            _layouts.Clear(); // Remove node windows from the previous loaded graph.

            if (_loadedGraph == null)		
				return;	

			// Create node windows based on the nodes in the graph.
			if (_loadedGraph.Nodes != null)
			{
				// Get rid of the null nodes, we don't need them.
				_loadedGraph.Nodes.RemoveAll(x => x == null);
                foreach (var node in _loadedGraph.Nodes.GetAll)
                {
                    CreateLayout(node);
                    node.OnEditorOpen();           
                }
                ((Node)_loadedGraph).OnEditorOpen();
			}

			_localVariables = new LocalVariableReorderableList(_loadedGraph.Root.Variables.GetAll());
			_localVariables.onVariableSelected = OnVariableSelected;
			_localVariables.onVariableDeleted = OnVariableDeleted;
		}

		void CreateLayout(Node node)
		{
			var layoutType = NodeLayoutSelection.Get(node.GetType());
			var l = (NodeLayout)System.Activator.CreateInstance(layoutType, new object[] { node });
			l.onPinClicked = onPinClicked;
			l._editor = this;
			_layouts.Add(l);
		}


		public NodeLayout GetLayout(string nodeID) { return GetLayout(_loadedGraph.Nodes.Find(nodeID)); }

		public NodeLayout GetLayout(Node node)
		{
			foreach (var l in _layouts)
			{
				var nl = l as NodeLayout;
				if (nl != null && nl.Node == node)
					return l;
			}
			return null;
		}


		void OnVariableSelected(string id, string name)
		{
			_nodeSelection.Clear();
			_loadedGraph.SelectedVariableID = id;
			Selection.objects = new Object[]{ (FunctionGraph)_loadedGraph};
		}

		void OnVariableDeleted(string id, string name)
		{
			List<NodeLayout> list = new List<NodeLayout>();
			foreach (var l in _layouts)
			{
                if(l.Node is IVariableNode && ((IVariableNode)l.Node).VariableName == name)
                    list.Add(l);
			}
			nodeDelete(list);
			Repaint();
		}

		#endregion
        
		#region NODE

		void NodeCopy(object param)
		{
            Clipboard.Clear();
			var layouts = (List<NodeLayout>)param;
            foreach (var l in layouts)
            {
                var copy = l.Node.Copy();
                Clipboard.Add(copy);
            }
		}

		void NodeDuplicate(object param)
		{
			NodeCopy(param);
			NodePaste();	
			Repaint();
		}

		void NodeCut(object param)
		{
			NodeCopy(param);
			NodeDelete(param);	
			Repaint();
		}

		void NodeDelete(object param)
		{
			var layouts = (List<NodeLayout>)param;
			string names = "";
			foreach (var l in layouts) names += " - " + l.Node.Name+"\n";
						
			if (EditorUtility.DisplayDialog("Delete", "Do you want to delete these nodes? \n" + names, "Yes", "No"))
			{
				nodeDelete(layouts);
			}
			Repaint();
		}

		void nodeDelete(List<NodeLayout> list)
		{
			foreach (var l in list)
			{
				foreach (var pin in l.Node.PinCollection.Get())
					WireController.Disconnect(pin);

				_loadedGraph.ZoomOffset = _zoomArea.ZoomOffset;

				string path = AssetDatabase.GetAssetPath(l.Node.Root);
				_loadedGraph.Nodes.Remove(l.Node);
				AssetDatabase.ImportAsset(path);
				LoadGraph(_loadedGraph);
			}
		}

        public void CustomActionMenuCallback(string name, params object[] args)
        {
            switch (name)
            {
                case "Paste": NodePaste();break;
                default:

                    string argsString = "\n [Arguments]";
                    foreach (var a in args)
                        argsString += "\n " + a+",";
                    RunemarkDebug.Error("Action Menu Callback {0} not implemented. {1}",
                        name, argsString);
                    break;
            }
        }

		public List<Node> NodeCreate(string name, System.Type type, System.Type subtype = null)
		{
			UnityEngine.Object o = _loadedGraph as FunctionGraph;
			if (o == null) o = _loadedGraph as MacroGraph;
			if (o == null)
			{
				RunemarkDebug.Error("Can't create node in the opened graph, since its not a INodeCollection");
				return new List<Node>();
			}
						 
			var node = (Node)AssetCreator.CreateAsset(name, type, o);			
			node.EditorInit(_loadedGraph, _zoomArea.AbsolutePosition(_lastMousePosition), subtype);
			_loadedGraph.Nodes.Add(node);
			CreateLayout(node);
			Repaint();
			return new List<Node>(){node};			
		}

        /// <summary>
        /// This method will fix the connection issue, by removing the pin from the node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="pin"></param>
        public void FixConnectionIssue(Node node, Pin pin)
        {         
            pin.Connections.Clear();
        }

		void NodePaste()
		{
			if (Clipboard.Count == 0) return;

			foreach (var node in Clipboard)
			{
                node.Position = _zoomArea.AbsolutePosition(_lastMousePosition);
                                   
                _loadedGraph.Nodes.Add(node);
                CreateLayout(node);                
                AssetDatabase.AddObjectToAsset(node, (FunctionGraph)((Node)_loadedGraph).Root);
            }
                       
            AssetDatabase.SaveAssets();
            Clipboard.Clear();
            Repaint();
        }



		#endregion
        
		#region CONTROLS
		public List<Node> Clipboard = new List<Node>();
		public Rect Workspace { get; private set; }

		[SerializeField] ZoomableArea _zoomArea;
		[SerializeField] WireController _wireController;
		[SerializeField] NodeSelection _nodeSelection;

		Vector2 _lastMousePosition;
		double _lastLeftClick = 0f;
		bool _mouseOverNode;

		bool _marqueSelectionActive;
		Rect _marqueSelection;

		GenericMenu _nodeMenu;
       

		void InitControls()
		{
			_nodeSelection = new NodeSelection();
            _nodeSelection.VisualEditor = this;

			_zoomArea = new ZoomableArea(1, 12, DrawZoomAreaContent);
			if(_loadedGraph != null)
				_zoomArea.PanTo(_loadedGraph.ZoomOffset);

			// Create the wire controller
			if(_loadedGraph != null)
				_wireController = new WireController(_loadedGraph, this);

			_nodeMenu = new GenericMenu();
        }
		void updateControl()
		{			
			var e = Event.current;

            if (!Workspace.Contains(e.mousePosition))
            {
                if (e.type == EventType.MouseUp)
                {
                    _nodeSelection.Clear();
                    _nodeSelection.Dragging = false;
                    _wireController.Release();
                }
                return;                
            }

			// WIRECONTROL
			if (e.button == (int)MouseButton.Left)
			{
				if (_wireController.IsPinSelected && 
					(e.button == (int)MouseButton.Left || e.button == (int)MouseButton.Right) && e.type == EventType.MouseUp ||
					e.keyCode == KeyCode.Escape && e.type == EventType.KeyUp)
				{
					_wireController.Release();
					e.Use();
				}
			}
			if (_wireController.IsPinSelected) return;
		
			// NODE SELECTION
			if (e.button == (int)MouseButton.Left)
			{
				if (e.type == EventType.MouseDown)
				{
                    GUIUtility.keyboardControl = 0;			
					if (!_nodeSelection.MouseOverIsSelected())
					{
						_nodeSelection.Select(e.shift || e.control); // if shift or control is hold down, allow multi selection.
						_lastLeftClick = EditorApplication.timeSinceStartup;
						e.Use();
					}

					// On double click open the graph
					else if (_nodeSelection.Selected is INodeCollection && _lastLeftClick >= 0 && _lastLeftClick + .3f <= EditorApplication.timeSinceStartup)
					{
						LoadGraph(_nodeSelection.Selected as INodeCollection);
						_lastLeftClick = -1f;
					}				
				}
			}
            
			// COMMENTBOX RESIZE
			if (e.button == (int)MouseButton.Left)
			{
				if (_nodeSelection.Selected == null && e.type == EventType.MouseDown)
					_nodeSelection.TrySelectNode(false);
				
				if (!_nodeSelection.Dragging && _nodeSelection.SelectionList.Count == 1 && _nodeSelection.Selected.Resizeable)
				{
					if (e.type == EventType.MouseDrag)
					{
						if (!_nodeSelection.Selected.Resizing)
							_nodeSelection.Selected.ResizeStart(e.mousePosition - Workspace.position);

						if (_nodeSelection.Selected.Resizing)
						{
							_nodeSelection.Selected.Resize(e.delta);
							e.Use();
						}
					}
					else if (e.type == EventType.MouseUp)
					{
						_nodeSelection.Selected.ResizeEnd();
						e.Use();
					}
				}
			}
            				
			// NODE DRAGGING
			if (e.button == (int)MouseButton.Left)
			{
				if (e.type == EventType.MouseDrag)
				{
					if (!_nodeSelection.Dragging && !_marqueSelectionActive && _nodeSelection.Selected != null)
						_nodeSelection.Dragging = true;

					if (_nodeSelection.Dragging)
					{
						foreach (var l in _nodeSelection.SelectionList)
							l.Move(e.delta / _zoomArea.ZoomValue);	
						e.Use();
					}
				}

				if (e.type == EventType.MouseUp && _nodeSelection.Dragging)
				{
					_nodeSelection.Dragging = false;
					e.Use();
				}
			}

			// MARQUE SELECTION
			if (e.button == (int)MouseButton.Left)
			{
				if (e.type == EventType.MouseDrag)
				{	
					if (!_marqueSelectionActive && (_nodeSelection.Selected == null || !_nodeSelection.MouseOverSelection(e.mousePosition)))
					{
						_marqueSelection = new Rect(e.mousePosition, new Vector2(0, 0));					
						_marqueSelectionActive = true;
						_nodeSelection.Clear();
					}

					if (_marqueSelectionActive)
					{
						_marqueSelection.width = e.mousePosition.x - _marqueSelection.x;
						_marqueSelection.height = e.mousePosition.y - _marqueSelection.y;

						Rect r = _marqueSelection;
						r.x -= Workspace.x;
						r.y -= Workspace.y;
                        r.width = r.width / _zoomArea.ZoomValue;
                        r.height = r.height / _zoomArea.ZoomValue;
         
						_nodeSelection.Select(_layouts.FindAll(x => r.Overlaps(x.Rect, true) && x.GetType() != typeof(CommentBoxLayout)));
						e.Use();
					}
				}
				else if (e.type == EventType.MouseUp)
				{
					if (_marqueSelectionActive) { _marqueSelectionActive = false;  e.Use(); }
				}
			}

			// CONTEXT MENU
			if (e.button == (int)MouseButton.Right)
			{
				if (e.type == EventType.MouseDown)
				{
					_nodeSelection.Select(false);

					if (_nodeSelection.Selected != null)
					{
						var canDelete = _nodeSelection.SelectionList.FindAll(x => !x.Node.CanDelete).Count == 0;
						var canCopy = _nodeSelection.SelectionList.FindAll(x => !x.Node.CanCopy).Count == 0;

						if(canCopy) 
							_nodeMenu.AddItem(new GUIContent("Copy"), false, NodeCopy, _nodeSelection.SelectionList);
						if(canDelete && canCopy)
							_nodeMenu.AddItem(new GUIContent("Cut"), false, NodeCut, _nodeSelection.SelectionList);
						if(canCopy)
							_nodeMenu.AddItem(new GUIContent("Duplicate"), false, NodeDuplicate, _nodeSelection.SelectionList);
						if(canDelete)
							_nodeMenu.AddItem(new GUIContent("Delete"), false, NodeDelete, _nodeSelection.SelectionList);

						if (canDelete || canCopy)
						{
							_nodeMenu.ShowAsContext();
							e.Use();
						}
					}
					else if(Workspace.Contains(e.mousePosition))
					{
                        ActionMenu.Editor = this;
                        var actionMenu = ScriptableObject.CreateInstance<ActionMenu>();                        
                        actionMenu.ShowAsDropDown(new Rect(e.mousePosition, new Vector2(-115, -150)), new Vector2(230, 300));
						_lastMousePosition = e.mousePosition;
						e.Use();
					}
				}
			}		

			// WORKSPACE PAN
			if (e.button == (int)MouseButton.Middle && e.type == EventType.MouseDrag)
			{
				_zoomArea.Pan(e.delta);
				e.Use();	
			}
			// WORKSPACE ZOOM
			if (e.type == EventType.ScrollWheel)
			{
				_zoomArea.Zoom(Workspace, e.delta, e.mousePosition);
				_loadedGraph.ZoomOffset = _zoomArea.ZoomOffset;
				e.Use();	
			}
			
            // CREATE COMMENT BOX            
			if (GUIUtility.keyboardControl == 0 && e.keyCode == KeyCode.C && e.type == EventType.KeyUp)
			{
				var comment = (CommentBox)NodeCreate("Comment", typeof(CommentBox))[0];			
				Rect cRect = new Rect(_lastMousePosition, new Vector2(200,100));

				foreach (var s in _nodeSelection.SelectionList)
				{
					var r = s.Rect;

					if (cRect == new Rect(0, 0, 0, 0)) cRect = new Rect(r.position, new Vector2(200, 100));

					if (r.xMin - 5 < cRect.xMin) cRect.xMin = r.xMin - 5;
					if (r.yMin - 35 < cRect.yMin) cRect.yMin = r.yMin - 35;

					if (r.xMax + 5 > cRect.xMax) cRect.xMax = r.xMax + 5;
					if (r.yMax + 5 > cRect.yMax) cRect.yMax = r.yMax + 5;
				}
                
				cRect = _zoomArea.AbsoluteRect(cRect);               

				comment.SetPositionNodesUneffected(cRect.position);
				comment.Size = cRect.size;
				e.Use();
			}	
		}

		void DrawZoomAreaContent(Rect rect)
		{	
			var bgRect = new Rect(0, 0, rect.width / _zoomArea.ZoomValue, rect.height / _zoomArea.ZoomValue);
			EditorGUI.DrawRect(bgRect, GRID_BACKGROUND_COLOR);
			DrawGrid(rect, _zoomArea.ZoomOffset, _zoomArea.ZoomStep);

			if (_loadedGraph == null || _layouts == null) return;

			_mouseOverNode = false;

			// Draw nodes
			foreach (var layout in _layouts.OrderBy(x => x.Order))
			{
				if (layout == null)
					return;

				var p = _zoomArea.RelativePosition(layout.AbsolutePosition);
				layout.Draw(p);

				// Check if mouse is over this node
				if (layout.MouseOverThis)
				{
					_nodeSelection.onMouseOver(layout);
					_mouseOverNode = true;
				}
			}

			// Set selection to null, if mouse isn't over a node.
		if (!_mouseOverNode) _nodeSelection.onMouseOver(null);

			_wireController.Draw();
		}

		/// <summary>
		/// Bridge method to prevent initialization order issue.
		/// </summary>
		/// <param name="pin">Pin.</param>
		void onPinClicked(Pin pin)
		{
            Event e = Event.current;
            if (e.button == (int)MouseButton.Right)
                WireController.Disconnect(pin);
            else if (e.button == (int)MouseButton.Left)
            {
                if (_wireController.IsPinSelected)
                    _wireController.Connect(pin);
                else
                    _wireController.Select(pin);
            }
		}
		#endregion

	}

}