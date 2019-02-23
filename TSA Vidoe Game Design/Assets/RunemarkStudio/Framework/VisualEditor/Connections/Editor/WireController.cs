using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace Runemark.VisualEditor
{
	public class WireController
	{
        public bool IsPinSelected
        {
            get
            {
                return _selectedPin != null;
            }
        }

        VisualEditor _editor;
        INodeCollection _graph;
        Pin _selectedPin;

        public WireController(INodeCollection graph, VisualEditor editor)
        {
            _graph = graph;
            _editor = editor;
        }
        
        /// <summary>
        /// Selects the given pin, and starts draw a wire from it.
        /// </summary>
        /// <param name="pin"></param>
        public void Select(Pin pin)
        {
            _selectedPin = pin;
        }

        /// <summary>
        /// Releases the selected pin.
        /// </summary>
        /// <param name="pin"></param>
        public void Release()
        {
            _selectedPin = null;
        }

        /// <summary>
        /// Connects the selected pin to the given one.
        /// </summary>
        /// <param name="other"></param>
        public void Connect(Pin other)
        {
            if (!IsPinSelected) return;

            // Connection checks
            if (_selectedPin.Collection.Parent == other.Collection.Parent)
                return;
            if (_selectedPin.PinType == other.PinType)
                return;

            // Determine which one is the input, and which one is the output.
            bool selectedIsInput = _selectedPin.PinType == PinType.Input || _selectedPin.PinType == PinType.TransIn;
            Pin input = (selectedIsInput) ? _selectedPin : other;
            Pin output = (selectedIsInput) ? other : _selectedPin;

            // Handle Dynamic Type 
            if (input.IsDynamicType)
                input.VariableType = output.VariableType;
            if (output.IsDynamicType && output.Connections.Count == 0)
                output.VariableType = input.VariableType;

            if (input.VariableType == output.VariableType)
            {
                // If there is another connection and the type doesn't allow multi connection
                // disconnect the current connection
                if (input.PinType == PinType.Input && input.Connections.Count > 0)
                    Disconnect(input);
                if (output.PinType == PinType.TransOut && output.Connections.Count > 0)
                    Disconnect(output);

                input.Connections.Add(new ConnectionData()
                {
                    NodeID = output.Collection.Parent.ID,
                    PinName = output.Name
                });

                output.Connections.Add(new ConnectionData()
                {
                    NodeID = input.Collection.Parent.ID,
                    PinName = input.Name
                });

                Release();
            }
        }

        /// <summary>
        /// Disconnects the given pin.
        /// </summary>
        /// <param name="pin"></param>
        public static void Disconnect(Pin pin)
        {
            Pin.Disconnect(pin);
        }




        public void Draw()
        {
            // Iterate through nodes
            foreach (var node in _graph.Nodes.GetAll)
            {
                NodeLayout nodelayout = _editor.GetLayout(node) as NodeLayout;
                if (nodelayout == null) continue;

                // Iterate through the node pins
                foreach (var pin in node.PinCollection.Get())
                {
                    // If the pin is output or transition input continue
                    // We are using inputs and transition outputs only becouse those can
                    // have only a single wire.
                    if (pin.PinType == PinType.Output || pin.PinType == PinType.TransIn)
                        continue;

                    // Continue if the pin doesn't have any active connection
                    if (pin.Connections.Count == 0) continue;

                    var connection = pin.Connections[0];
                    var otherLayout = _editor.GetLayout(connection.NodeID);

                    // If the other layout doesn't exist the pin should be fixed.
                    if (otherLayout == null)
                    {
                        _editor.FixConnectionIssue(node, pin);
                        return;
                    }

                    // If the layout exists...
                    else
                    {
                        //... check if the layout has connection to this node (validate)
                        bool isValid = false;
                        var otherPin = otherLayout.Node.PinCollection.Get(connection.PinName);
                        foreach (var otherConn in otherPin.Connections)
                        {
                            if (otherConn.NodeID == node.ID && otherConn.PinName == pin.Name)
                                isValid = true;
                        }

                        if (isValid)
                        {
                            if (pin.VariableType != otherPin.VariableType)
                                Disconnect(pin);
                        }

                        // If the connection is valid, connect.
                        if (isValid)
                        {
                            var pinFrom = nodelayout.GetPin(pin.Name);
                            var pinTo = otherLayout.GetPin(otherPin.Name);

                            DrawWire(pinFrom.WireRect, pinTo.WireRect, pinFrom.Color, pinFrom.Direction);
                        }
                        else
                        {
                            _editor.FixConnectionIssue(node, pin);
                            return;
                        }
                    }
                }
            }
            if (IsPinSelected)
            {
                Event e = Event.current;

                Rect r = new Rect(0, 0, 16, 16);
                r.x = e.mousePosition.x;
                r.y = e.mousePosition.y;

                DrawWire(_selectedPin.WireRect, r, _selectedPin.Color, _selectedPin.Direction);
                _editor.Repaint();
            }
        }		
		void DrawWire(Rect start, Rect end, Color color, Vector2 direction)
		{
			Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
			Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);

			Vector3 startTan = startPos + new Vector3(direction.x, -direction.y, 0) * 50;
			Vector3 endTan = endPos - new Vector3(direction.x, -direction.y, 0) * 50;

			Handles.DrawBezier(startPos, endPos, startTan, endTan, color, null, 3);
		}
               
    }
}