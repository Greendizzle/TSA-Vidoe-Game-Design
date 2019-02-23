using UnityEngine;
using UnityEditor;
using Runemark.VisualEditor.Utility;
using Runemark.VisualEditor.Actions;
using Runemark.Common;
using System.Collections.Generic;

namespace Runemark.VisualEditor
{
    [CustomNodeLayout(typeof(PortalExecution))]
    public class PortalExecutionLayout : CompactLayout
    {
        protected override string Title { get { return _node.PortalName; } }

        PortalExecution _node { get { return ConvertNode<PortalExecution>(); } }
        public PortalExecutionLayout(PortalExecution node) : base(node)
        {
            headerColor = BuiltInColors.GetDark(typeof(ExecutableNode));
            width = 150;
        }

        public override void OnRepaint()
        {
            if (titleStyle != null)
            {
                GUIContent c = new GUIContent(_node.PortalName);
                var size = titleStyle.CalcSize(c);
                width = Mathf.Max(150, size.x + 2 * Pin.PIN_SIZE + 20);
                MapConnections();
            }
        }
    }

    [CustomEditor(typeof(PortalExecution))]
    public class PortalExecutionInspector : NodeInspector
    {
        List<PortalExecution> _outputPortals = new List<PortalExecution>();
        List<PortalExecution> _portals = new List<PortalExecution>();
        SmartPopup _portalNames;

        void OnEnable()
        {
            // Get all the portals from the graph.
            PortalExecution myTarget = (PortalExecution)target;
            _portals = myTarget.Root.Nodes.FindAll<PortalExecution>();
            _outputPortals = _portals.FindAll(x => !x.IsInput);


            // If the current target is an input...
            if (myTarget.IsInput)
                CollectPortalNames(); 
        }

        protected override void onGUI()
        {
            PortalExecution myTarget = (PortalExecution)target;
            
            EditorGUIExtension.SimpleBox("", 5, "", delegate ()
            {
                // IS INPUT
                if (_outputPortals.Count > 0)
                {
                    myTarget.IsInput = InputToggle(myTarget.IsInput);
                }
                else
                {
                    EditorGUILayout.Toggle("Is Input", myTarget.IsInput);
                    EditorGUILayout.HelpBox("You can't set this node to Input," +
                       "You need at least one output execution portal in the graph", MessageType.Info);
                }

                // PORTAL NAME 
                if (myTarget.IsInput)
                {
                    if (_portalNames.Draw("Portal Name"))
                    {
                        // Set the connected portal.       
                        myTarget.OutputPortal = _outputPortals[_portalNames.Index];   
                    }
                }
                else
                    myTarget.PortalName = PortalNameField(myTarget.PortalName);
            });
        }

        bool InputToggle(bool wasInput)
        {
            bool isInput = EditorGUILayout.Toggle("Is Input", wasInput);
            if (isInput != wasInput)
            {
                // If the node will be input, recollect portal names.
                if (isInput) CollectPortalNames();

                // If the node will be an output...
                else
                {
                    // Set its portal name to unique.
                    PortalExecution myTarget = (PortalExecution)target;
                    myTarget.PortalName = UniquePortalName(myTarget.PortalName);
                }
            }
            return isInput;
        }
        string PortalNameField(string portalName)
        {
            string newName = EditorGUILayout.TextField("Portal Name", portalName);
            if (newName != portalName)
            {
                // Check and fix portal name to be unique.
                newName = UniquePortalName(newName);

                // Change the connected inputs name to this.
                foreach (var portal in _portals)
                {
                    if (portal.IsInput && portal.PortalName == portalName)
                        portal.PortalName = newName;
                }
            }
            return newName;
        }

        string UniquePortalName(string name)
        {
            foreach (var portal in _outputPortals)
            {
                if (!portal.IsInput && portal.PortalName == name)
                    name = name + "_1";
            }
            return name;
        }

        void CollectPortalNames()
        {
            PortalExecution myTarget = (PortalExecution)target;

            // ... iterate through the portals, to get the names, and the current index.
            int index = -1;
            List<string> nameList = new List<string>();
            for (int cnt = 0; cnt < _outputPortals.Count; cnt++)
            {
                var portal = _outputPortals[cnt];
                if (portal == myTarget) continue;

                nameList.Add(portal.PortalName);

                if (myTarget.PortalName == portal.PortalName)
                    index = cnt;
            }
   
            _portalNames = new SmartPopup(nameList.ToArray(), index);

        }

    }


    [CustomNodeLayout(typeof(PortalVariable))]
    public class PortalVariableLayout : CompactLayout
    {
        protected override string Title { get { return _node.PortalName; } }

        PortalVariable _node { get { return ConvertNode<PortalVariable>(); } }
        public PortalVariableLayout(PortalVariable node) : base(node)
        {
            headerColor = BuiltInColors.GetDark(_node.Type);
            width = 150;
        }

        public override void OnRepaint()
        {
            headerColor = BuiltInColors.GetDark(_node.Type);

            if (titleStyle != null)
            {
                GUIContent c = new GUIContent(_node.PortalName);
                var size = titleStyle.CalcSize(c);
                width = Mathf.Max(150, size.x + 2 * Pin.PIN_SIZE + 20);
            }
            else width = 150;

            MapConnections();
        }
    }

    [CustomEditor(typeof(PortalVariable))]
    public class PortalVariableInspector : NodeInspector
    {
        List<PortalVariable> _inputPortals = new List<PortalVariable>();
        List<PortalVariable> _portals = new List<PortalVariable>();
        SmartPopup _portalNames;

        void OnEnable()
        {
            // Get all the portals from the graph.
            PortalVariable myTarget = (PortalVariable)target;
            _portals = myTarget.Root.Nodes.FindAll<PortalVariable>();
            _inputPortals = _portals.FindAll(x => x.IsInput);


            // If the current target is an output...
            if (!myTarget.IsInput)
                CollectPortalNames();
        }

        protected override void onGUI()
        {
            PortalVariable myTarget = (PortalVariable)target;

            EditorGUIExtension.SimpleBox("", 5, "", delegate ()
            {
                // IS INPUT
                if (_inputPortals.Count > 0)
                {
                    myTarget.IsInput = InputToggle(myTarget.IsInput);
                }
                else
                {
                    EditorGUILayout.Toggle("Is Input", myTarget.IsInput);
                    EditorGUILayout.HelpBox("You can't set this node to Input," +
                       "You need at least one output execution portal in the graph", MessageType.Info);
                }

                // PORTAL NAME 
                if (!myTarget.IsInput)
                {
                    if (_portalNames.Draw("Portal Name"))
                    {
                        // Set the connected portal. 
                        myTarget.PortalName = _inputPortals[_portalNames.Index].PortalName;      
                        myTarget.InputPortal = _inputPortals[_portalNames.Index];
                    }
                }
                else
                    myTarget.PortalName = PortalNameField(myTarget.PortalName);
            });
        }

        bool InputToggle(bool wasInput)
        {
            bool isInput = EditorGUILayout.Toggle("Is Input", wasInput);
            if (isInput != wasInput)
            {
                // If the node will be input, recollect portal names.
                if (!isInput) CollectPortalNames();

                // If the node will be an output...
                else
                {
                    // Set its portal name to unique.
                    PortalVariable myTarget = (PortalVariable)target;
                    myTarget.PortalName = UniquePortalName(myTarget.PortalName);
                }
            }
            return isInput;
        }
        string PortalNameField(string portalName)
        {
            string newName = EditorGUILayout.TextField("Portal Name", portalName);
            if (newName != portalName)
            {
                // Check and fix portal name to be unique.
                newName = UniquePortalName(newName);

                // Change the connected inputs name to this.
                foreach (var portal in _portals)
                {
                    if (!portal.IsInput && portal.PortalName == portalName)
                        portal.PortalName = newName;
                }
            }
            return newName;
        }

        string UniquePortalName(string name)
        {
            foreach (var portal in _inputPortals)
            {
                if (portal.PortalName == name)
                    name = name + "_1";
            }
            return name;
        }

        void CollectPortalNames()
        {
            PortalVariable myTarget = (PortalVariable)target;

            // ... iterate through the portals, to get the names, and the current index.
            int index = -1;
            List<string> nameList = new List<string>();
            for (int cnt = 0; cnt < _inputPortals.Count; cnt++)
            {
                var portal = _inputPortals[cnt];
                if (portal == myTarget) continue;

                nameList.Add(portal.PortalName);

                if (myTarget.PortalName == portal.PortalName)
                    index = cnt;
            }

            _portalNames = new SmartPopup(nameList.ToArray(), index);
        }

    }

    /*
    [CustomNodeLayout(typeof(VariablePortal), true)]
    public class VariablePortalLayout : DefaultLayout
    {
        VariablePortal _node;

        public VariablePortalLayout(Node node) : base(node)
        {
            headerColor = BuiltInColors.DarkDefault;
            _node = (VariablePortal)node;

            minBodyHeight = Pin.PIN_SIZE + 10;

            showInputLabel = false;
            showOutputLabel = false;
        }

        public override void OnRepaint()
        {
            string pinName = (_node.IsInput) ? "IN" : "OUT";
            var pin = Node.PinCollection.Get(pinName);
            Color c = BuiltInColors.GetDark(pin.VariableType);
            headerColor = c;
        }

        protected override void onGUIBody()
        {
            base.onGUIBody();

            Rect r = bodyRect;
            r.width = r.width - Pin.PIN_SIZE - 10;
            r.x = r.x + 5 + ((_node.IsInput) ? Pin.PIN_SIZE + 5 : 0);
            GUI.Label(r, _node.PortalName, _textStyle);


            // Remap connections if not the same!
            if (_node.RemapPins)
            {               
                MapConnections();
                _node.RemapPins = false;
            }
        }

        GUIStyle _textStyle;
        protected override void InitStyle()
        {
            base.InitStyle();
            titleStyle.fontSize = 16;

            Color textColor = new Color(.8f, .8f, .8f, 1f);

            _textStyle = new GUIStyle(GUI.skin.label);
            _textStyle.normal.textColor = textColor;
            _textStyle.fontSize = 14;
            _textStyle.wordWrap = true;
            _textStyle.alignment = TextAnchor.MiddleCenter;
        }

    }
    [CustomEditor(typeof(VariablePortal))]
    public class VariablePortalInspector : NodeInspector
    {
        string[] _names;
        int _nameIndex;
        bool _nameError;

        string _lastName;
        int _lastIndex;

        bool _lastIsInput;

        private void OnEnable()
        {
            VariablePortal myTarget = (VariablePortal)target;
            var names = PortalUtils.GetNames(myTarget.Root, myTarget.IsExec);
            names.Insert(0, "");
            _names = names.ToArray();           

            if (myTarget.PortalName == "") _nameIndex = 0;
            else
            {
                for (int cnt = 0; cnt < _names.Length; cnt++)
                {
                    if (_names[cnt] == myTarget.PortalName)
                    {
                        _nameIndex = cnt;
                        break;
                    }
                }
            }                                
        }        

        protected override void onGUI()
        {
            VariablePortal myTarget = (VariablePortal)target;

            EditorGUI.BeginChangeCheck();

            EditorGUIExtension.SimpleBox("", 5, "", delegate ()
            {
                myTarget.IsInput = EditorGUILayout.Toggle("Input", myTarget.IsInput);

                if (myTarget.IsInput)
                {
                    myTarget.PortalName = EditorGUILayout.TextField("Portal Name", myTarget.PortalName);
                    if (_nameError)
                        EditorGUILayout.HelpBox("The name '"+myTarget.PortalName+"' already exists in this graph as a Variable Input Portal.", MessageType.Error);
                }
                else
                    _nameIndex = EditorGUILayout.Popup("Portal Name", _nameIndex, _names);
            });


            if (EditorGUI.EndChangeCheck())
            {               
                // Name Change
                if (myTarget.IsInput)
                {
                    if (myTarget.PortalName != _lastName)
                    {
                        if (!PortalUtils.HasName(myTarget.Root, myTarget))
                        {
                            _lastName = myTarget.PortalName;
                            _nameError = false;
                        }
                        else
                        {
                            _nameError = true;
                            myTarget.PortalName = _lastName;
                        }
                    }
                }
                else
                {
                    if (_nameIndex != _lastIndex && _nameIndex >= 0 && _nameIndex < _names.Length)
                    {
                        myTarget.PortalName = _names[_nameIndex];
                        _lastIndex = _nameIndex;
                    }
                }

                /// Input change
                if (myTarget.IsInput != _lastIsInput)
                { 
                    hasChanged = true;
                    myTarget.RemapPins = true;
                    _lastIsInput = myTarget.IsInput;
                }

            //    if (hasChanged)
            //        myTarget.HasChanges = true;
            }
        }
    }
    
    [CustomNodeLayout(typeof(Portal), true)]
	public class PortalLayout : DefaultLayout
	{ 
		public PortalLayout(Node node) : base(node)
		{
			headerColor = BuiltInColors.DarkDefault;
		} 
	} 
    
	[CustomEditor(typeof(Portal))]
	public class PortalInspector : NodeInspector
	{
		protected override void onGUI()
		{
            Portal myTarget = (Portal)target;
		}
	}
*/


} 
