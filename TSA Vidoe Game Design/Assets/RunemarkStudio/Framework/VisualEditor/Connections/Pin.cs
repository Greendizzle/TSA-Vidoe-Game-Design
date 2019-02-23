using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using Runemark.VisualEditor.Utility;
using UnityEditor;
#endif

namespace Runemark.VisualEditor
{
    /// <summary>
    /// Type of the pin.
    /// </summary>
    public enum PinType
    {
        Input, Output, TransIn, TransOut,
    }

    /// <summary>
    /// Data about the connected pin.
    /// </summary>
    [Serializable]
    public class ConnectionData
    {
        public string NodeID;
        public string PinName;

        public ConnectionData Copy()
        {
            var data = new ConnectionData();
            data.NodeID = NodeID;
            data.PinName = PinName;
            return data;
        }
    }
    
    /// <summary>
    /// This class contains every information and function for pins.
    /// </summary>
    [Serializable]
    public class Pin
    {
        /// <summary>
		/// The name of the connection pin.
		/// </summary>
        public string Name;
        /// <summary>
        /// Type of the pin.
        /// </summary>
        public PinType PinType;
        /// <summary>
        /// The pin collection that contains this pin.
        /// </summary>
        [NonSerialized] public PinCollection Collection;


        /// <summary>
        /// The type of the variable the pin handles.
        /// </summary>
        public Type VariableType
        {
            get
            {
                if (_variableType == null)
                    _variableType = Type.GetType(_variableTypeString);
                return _variableType;
            }
            set
            {
                bool changed = _variableType != value;
                _variableType = value;
                _variableTypeString = value.ToString();
                if (changed)  onTypeChanged();
            }
        }
        Type _variableType;
        [SerializeField] string _variableTypeString;

        /// <summary>
        /// Does this pin change its type based on connection?
        /// </summary>
        public bool IsDynamicType
        {
            get { return _isDynamicType;  }
            set { _isDynamicType = value;  }
        }
        [SerializeField] bool _isDynamicType = false;

        /// <summary>
        /// Does this pin connected to anything?
        /// </summary>
        public bool HasConnection { get { return Connections.Count > 0;  } }
        /// <summary>
		/// The connections goes from this pin to other(s).
		/// </summary>
        public List<ConnectionData> Connections = new List<ConnectionData>();

        public void Init()
        {
            onTypeChanged();
        }
                   
        /// <summary>
        /// Copy the pin.
        /// </summary>
        /// <returns></returns>
        public Pin Copy()
        {
            var copy = new Pin();
            copy.Name = Name;
            copy.PinType = PinType;

            copy.Connections = new List<ConnectionData>();
            foreach (var c in Connections)
                copy.Connections.Add(c.Copy());
            copy.VariableType = VariableType;
            return copy;
        }

        /// <summary>
        /// On type changed
        /// </summary>
        void onTypeChanged()
        {
        #if UNITY_EDITOR
            Color = BuiltInColors.Get(VariableType);
            float r = Color.r * .5f;
            float g = Color.g * .5f;
            float b = Color.b * .5f;

            _emptyColor = new Color(r, g, b);
            _labelStyle = null;

            if (IsDynamicType && OnTypeChanged != null)
                OnTypeChanged(this);
        #endif
        }
        public delegate void OnTypeChangedDelegate(Pin p);
        public OnTypeChangedDelegate OnTypeChanged;


#if UNITY_EDITOR
        public static float PIN_SIZE = 16;

        public Rect WireRect { get; private set; }
        public Vector2 Direction { get; private set;  }

        /// <summary>
        /// The label style.
        /// </summary>
        GUIStyle _labelStyle;
        /// <summary>
        /// Color of the pin (used for wires mostly)
        /// </summary>
        public Color Color { get; private set; }
        /// <summary>
        /// The empty color of the pin.
        /// </summary>
        Color _emptyColor;
        

        /// <summary>
        /// Draws gui in editor
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="dir"></param>
        /// <returns>True if the button is pressed.</returns>
        public bool DrawEditorGUI(Rect rect, Vector2 direction, bool hideLabel = false)
        {
            // Calculate and store
            float wireX = rect.x;
            //if (direction.x < 0) wireX = wireX - rect.width/2;
            WireRect = new Rect(wireX, rect.y - direction.y * rect.height / 2, rect.width, rect.height);
            Direction = direction;

            // Draw background
            EditorGUI.DrawRect(rect, Color);
            // Draw foreground
            EditorGUI.DrawRect(new Rect(rect.x + 2, rect.y + 2, rect.width - 4, rect.height - 4), (Connections.Count == 0) ? _emptyColor : Color);
            
            // Draw invisible button to handle pin clicks
            if (GUI.Button(rect, "", GUI.skin.label)) return true;

            // If label should be shown.
            if (!hideLabel)
            {
                // Init label style if it's null.
                if (_labelStyle == null)
                {
                    _labelStyle = new GUIStyle(GUI.skin.label);
                    _labelStyle.normal.textColor = Color;
                    _labelStyle.alignment = TextAlignment(direction);
                }

                // Calculate the size of the label, based on the Name lenght.
                var labelSize = _labelStyle.CalcSize(new GUIContent(Name));

                // Calculate x position
                float x = rect.x;
                if (direction.x > 0) x -= labelSize.x + 2;
                else if (direction.x < 0) x -= direction.x * (rect.width) - 2;

                // Calculate y position
                float y = rect.y;
                if (direction.y > 0) y -= rect.height + 2;
                else if (direction.y < 0) x -= direction.y * (rect.height) - 2;

                // Draw label
                GUI.Label(new Rect(x, y, labelSize.x, rect.height), Name, _labelStyle);
            }
            return false;
        }      
        
        /// <summary>
        /// Determine the text alignment based on the pin direction.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        TextAnchor TextAlignment(Vector2 direction)
        {
            if (direction.x > 0)
            {
                if (direction.y > 0) return TextAnchor.LowerRight;
                else if (direction.y < 0) return TextAnchor.UpperRight;
                else return TextAnchor.MiddleRight;
            }

            else if (direction.x < 0)
            {
                if (direction.y > 0) return TextAnchor.LowerLeft;
                else if (direction.y < 0) return TextAnchor.UpperLeft;
                else return TextAnchor.MiddleLeft;
            }

            else
            {
                if (direction.y > 0) return TextAnchor.LowerCenter;
                else if (direction.y < 0) return TextAnchor.UpperCenter;
                else return TextAnchor.MiddleCenter;
            }
        }
#endif

        public static void Disconnect(Pin pin)
        {
            FunctionGraph graph = pin.Collection.Parent.Root;
            foreach (var conn in pin.Connections)
            {
                var otherNode = graph.Nodes.Find(conn.NodeID);
                if (otherNode == null) continue;
                var otherPin = otherNode.PinCollection.Get(conn.PinName);
                if (otherPin == null) continue;
                otherPin.Connections.RemoveAll(x => x.NodeID == pin.Collection.Parent.ID && x.PinName == pin.Name);
            }
            pin.Connections.Clear();
        }
    }


}