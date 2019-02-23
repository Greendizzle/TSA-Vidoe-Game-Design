using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Runemark.VisualEditor;
using UnityEditor;
using Runemark.Common;
using Runemark.VisualEditor.Utility;
using System.Linq;

namespace Runemark.VisualEditor
{
    public abstract class NodeLayout
    {
        public VisualEditor _editor;

        public double LastChanged = -1;

        public delegate void OnPinClicked(Pin pin);
        public OnPinClicked onPinClicked;

        public NodeLayout(Node node)
        {
            Node = node;
            position = node.Position;
            MapConnections();
            Order = DefaultOrder;
        }

        public virtual void OnRepaint()
        {

        }

        #region Node Handling
        public readonly Node Node;
        protected virtual string Title { get { return Node.Name; } }

        protected T ConvertNode<T>() where T : Node
        {
            if (Node.GetType() == typeof(T) || Node.GetType().IsSubclassOf(typeof(T)))
                return (T)Node;
            RunemarkDebug.Error("{0} node is {1} and can't be converted to type of {2}",
                Node.Name, Node.GetType(), typeof(T));
            return null;
        }
        #endregion

        #region Location
        public Vector2 AbsolutePosition { get { return Node.Position; } }

        /// <summary>
        /// Relative Rect.
        /// </summary>
		public Rect Rect { get { return new Rect(position, new Vector2(width, headerHeight + bodyHeight)); } }
        protected Rect headerRect { get { return new Rect(position, new Vector2(width, headerHeight)); } }
        protected Rect bodyRect { get { return new Rect(position.x, position.y + headerHeight, width, bodyHeight); } }

        protected float width;
        protected abstract float headerHeight { get; }
        protected abstract float bodyHeight { get; }

        /// <summary>
        /// Relative Position.
        /// </summary>
		protected Vector2 position;

        public virtual void Move(Vector2 delta) { Node.Position += delta; }
        #endregion

        #region Selection
        public bool MouseOverThis { get { return Rect.Contains(Event.current.mousePosition); } }
        public bool Selected { get { return _selected; } set { Select(value, false); } }
        bool _selected;
        bool _invisibleSelection;

        public int Order { get; private set; }
        protected virtual int DefaultOrder { get { return 0; } }


        public void Select(bool b, bool invisible)
        {
            _invisibleSelection = invisible;
            _selected = b;
            Order = DefaultOrder + ((_selected) ? 2 : 0);
            OnSelectionChanged(_selected);
        }

        protected virtual void OnSelectionChanged(bool selected)
        {
            if (_invisibleSelection)
                return;

            var list = Selection.objects.ToList();
            if (selected) list.Add(Node);
            else if (list.Contains(Node)) list.Remove(Node);
            Selection.objects = list.ToArray();
        }
        #endregion

        #region Resizing
        public virtual bool Resizeable { get { return false; } }
        public bool Resizing { get { return _resizing.ACTIVE; } }

        protected struct ResizingData { public bool ACTIVE, UP, DOWN, LEFT, RIGHT; }
        protected ResizingData _resizing = new ResizingData();

        public virtual void ResizeStart(Vector2 directionVector) { }
        public virtual void Resize(Vector2 delta) { }
        public virtual void ResizeEnd() { }
        #endregion

        #region Style
        protected Color headerColor;
        protected Color bodyColor;

        protected GUIStyle selectionStyle;
        protected virtual void InitSelectionStyle()
        {
            var background = VisualEditorGUIStyle.GetTexture(new Color(0.85f, 0.51f, 0.00f, 1.00f), new Color(0.85f, 0.51f, 0.00f, 0.00f), false, false, false, false);
			selectionStyle = new GUIStyle(GUI.skin.box);
            selectionStyle.normal.background = background.Texture;
			selectionStyle.fontStyle = FontStyle.Bold;
			selectionStyle.alignment = TextAnchor.MiddleCenter;
			selectionStyle.normal.textColor = Color.white;
			selectionStyle.border = background.BorderOffset;
        }

        protected GUIStyle titleStyle;
        protected virtual void InitTitleStyle()
        {
            titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.normal.textColor = Color.white;
        }

        protected GUIStyle fieldStyle;
        protected virtual void InitFieldStyle()
        {
            var textColor = Color.white;
            var bg = VisualEditorGUIStyle.GetTexture(BuiltInColors.Default, new Color(0, 0, 0, 0), true, true, true, true);

            fieldStyle = new GUIStyle(GUI.skin.textField);
            fieldStyle.fontStyle = FontStyle.Bold;
            fieldStyle.alignment = TextAnchor.MiddleCenter;
            fieldStyle.border = bg.BorderOffset;
            fieldStyle.padding = new RectOffset(0, 0, 0, 0);
            fieldStyle.margin = new RectOffset(0, 0, 0, 0);

            fieldStyle.normal.textColor = textColor;
            fieldStyle.normal.background = bg.Texture;
            fieldStyle.active.textColor = textColor;
            fieldStyle.active.background = bg.Texture;
            fieldStyle.focused.textColor = textColor;
            fieldStyle.focused.background = bg.Texture;
            fieldStyle.hover.textColor = textColor;
            fieldStyle.hover.background = bg.Texture;

            
        }

        protected GUIStyle textStyle;
        protected virtual void InitTextStyle()
        {
            Color textColor = new Color(.8f, .8f, .8f, 1f);
            textStyle = new GUIStyle(GUI.skin.label);
            textStyle.normal.textColor = textColor;
            textStyle.wordWrap = true;
        }

        protected bool styleInitialized;
		protected virtual void InitStyle()
		{
            InitSelectionStyle();
            InitTitleStyle();
            InitFieldStyle();
            InitTextStyle();
            styleInitialized = true;
		}

		#endregion

		#region Drawing
		public void Draw(Vector2 relativePosition)
		{
            position = relativePosition;

			if (!styleInitialized) InitStyle();
            if (fieldStyle != null) GUI.skin.settings.cursorColor = fieldStyle.normal.textColor;

            // Drawing selection rect behind the layout
            if (Selected && !_invisibleSelection)
			{
				Rect r = new Rect(Rect.x - 2.5f, Rect.y - 2.5f, Rect.width + 5, Rect.height + 5);
				GUI.Box(r, new GUIContent("", Node.Tooltip + "\n" + Node.Description), selectionStyle);	
			}

			// Draw the background of the layout
			if (headerHeight > 0)
			{
				EditorGUI.DrawRect(headerRect, headerColor);
				onGUIHeader();	
			}
			if (bodyHeight > 0)
			{
				EditorGUI.DrawRect(bodyRect, bodyColor);
				onGUIBody();
			}

            if (DebugText != "")
				GUI.Label(new Rect(Rect.x, Rect.y - 20, 500, 20), DebugText);

		}

		protected abstract void onGUIHeader();
		protected abstract void onGUIBody();

		protected virtual string DebugText { get { return ""; } }
        #endregion
        
        #region Pins    
        protected List<Pin> inputs = new List<Pin>();
		protected List<Pin> outputs = new List<Pin>();
		protected List<Pin> transitions = new List<Pin>();

		protected bool showInputLabel;
		protected bool showOutputLabel;
		protected bool showTransitionInputLabel;
		protected bool showTransitionOutputLabel;
        
		public void MapConnections()
		{
            inputs.Clear();
            outputs.Clear();
            transitions.Clear();

            foreach (var pin in Node.PinCollection.Get())
            {
                pin.Init();
                switch (pin.PinType)
                {
                    case PinType.Input:
                        inputs.Add(pin);
                        break;

                    case PinType.Output:
                        outputs.Add(pin);
                        break;

                    case PinType.TransIn:
                    case PinType.TransOut:
                        transitions.Add(pin);
                        break;
                }
            }			
		}

        protected void DrawPin(Rect r, Pin pin, Vector2 direction, bool hideLabel = false)
        {
            if (pin == null) return;

            // If the node has a variable input too, connecting the pin is optional.
            bool optional = pin.PinType == PinType.Input && Node.Variables.Contains(pin.Name);

            if (optional && !hideLabel)
                hideLabel = !pin.HasConnection;
 
            if (pin.DrawEditorGUI(r, direction, hideLabel))
                onPinClicked(pin);

            if (optional && !pin.HasConnection)
            {
                bool right = direction.x > 0;
                bool singlePin = (right) ? Node.PinCollection.Count(PinType.Input) + Node.PinCollection.Count(PinType.TransIn) == 0 :
                    Node.PinCollection.Count(PinType.Output) + Node.PinCollection.Count(PinType.TransOut) == 0;
                
                Variable v = Node.Variables.GetByName(pin.Name);
                float w = width - 5 - (Pin.PIN_SIZE + 10) * ((singlePin) ? 1 : 2);
                float h = 25;
                float x = (right) ? r.x - 5 - w : r.x + Pin.PIN_SIZE + 5;
                float y = r.y + (Pin.PIN_SIZE - h) / 2;
                DrawInput(new Rect(x, y, w, h), v);
            }          
        }
        protected void DrawInput(Rect r, Variable variable)
        {
            if (variable == null) return;
            VariableEditor.VariableField(r, variable, fieldStyle);         
        }
          
        public Pin MouseOverPin(Vector2 mousePosition)
		{
			foreach (var i in inputs) { if (i.WireRect.Contains(mousePosition)) { return i; } }
			foreach (var o in outputs) { if (o.WireRect.Contains(mousePosition)) { return o; } }
			foreach (var t in transitions) { if (t.WireRect.Contains(mousePosition)) { return t; } }
			return null;
		}        		
		public Pin GetPin(string name)
		{
            return Node.PinCollection.Get(name);
		}        		
		#endregion
	}


	#region Custom Inspector
	[CustomEditor(typeof(Node), true)]
	public class NodeInspector : Editor
	{
        protected virtual bool NameEditable { get { return false; } }
        SmartPopup _typePopup;
        
        protected override void OnHeaderGUI()
        {
            base.OnHeaderGUI();
            Node myTarget = (Node)target;
            EditorGUIExtension.DrawInspectorTitle(Utils.AddSpacesToSentence(myTarget.GetType().Name) + " (Action)", "");
        }

        public override void OnInspectorGUI()
		{
            Node myTarget = (Node)target;

            EditorGUI.BeginChangeCheck();

            if (myTarget is INodeCollection || NameEditable)
            {
                EditorGUIExtension.SimpleBox("", 5, "", delegate ()
                {
                    myTarget.Name = EditorGUILayout.TextField("Name", myTarget.Name);
                });
            }
            else if (myTarget is ITypedNode)
            {
                ITypedNode t = (ITypedNode)myTarget;
                EditorGUIExtension.SimpleBox("", 5, "", delegate ()
                {
                    if (_typePopup == null)
                    {
                        int index = 0;
                        List<string> names = new List<string>();
                        for(int cnt = 0; cnt < t.AllowedTypes.Count; cnt++)
                        {
                            if (t.Type == t.AllowedTypes[cnt]) index = cnt;
                            names.Add(TypeUtils.GetPrettyName(t.AllowedTypes[cnt]));
                        }
                        _typePopup = new SmartPopup(names, index);
                    }

                    if (_typePopup.Draw("Value Type"))
                    {
                        t.Type = t.AllowedTypes[_typePopup.Index];
                    }                  
                });
            }                   

            EditorGUILayout.Space();

            onGUI();

            if (EditorGUI.EndChangeCheck())
                myTarget.HasChanges = true;
        }

        protected void ResetTypePopup()
        {
            _typePopup = null;
            
        }

		protected virtual void onGUI(){}        
	}
	#endregion

}
