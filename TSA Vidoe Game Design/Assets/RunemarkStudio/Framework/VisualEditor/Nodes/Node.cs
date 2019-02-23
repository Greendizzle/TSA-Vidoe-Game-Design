using UnityEngine;
using Runemark.Common;

namespace Runemark.VisualEditor
{
	[System.Serializable]
	public abstract class Node : ScriptableObject
	{
        public string ID
        {
            get
            {
                if (_id == "") CreateID();
                return _id;
            }
            set { _id = value; }
        }
        [SerializeField] string _id;

        public string Name 
		{
			get { return this.name; }
			set { this.name = value; }
		}

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		[SerializeField]string _description = "";
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		public INodeCollection Parent
		{
			get 
			{
				if (_parent == null) _parent = _parentObject.Value as INodeCollection;
				return _parent;
			}
			set
            {
                _parent = value;
                _parentObject.Value = value;              
            }
		}
		INodeCollection _parent;
		[SerializeField] SObject _parentObject = new SObject("Node.Parent");  // Needs to serialize an Interface
        
		public FunctionGraph Root 
		{ 
			get 
			{
				if (Parent == null)
				{
					if (GetType() == typeof(FunctionGraph) || GetType().IsSubclassOf(typeof(FunctionGraph)))
						return (FunctionGraph)this;
					else
					{
						RunemarkDebug.Error("{0} ({1}) has no Parent, and is not a function graph!",
                            Name, GetType());
                        return null;
					}

				}
				else
					return ((Node)Parent).Root; 
			}
		}

		public VisualEditorBehaviour Owner { get { return Root._owner; }}
		VisualEditorBehaviour _owner;

		public VariableCollection Variables = new VariableCollection();
		public VariableCollection LocalVariables { get { return Root.Variables; } }

        public PinCollection PinCollection;
      
        public bool IsInitialized;	

        public virtual void RuntimeInit(VisualEditorBehaviour owner)
		{
            if (Root == this)
            {
                LocalVariables.Reset();
                _owner = owner;
                Variables.owner = _owner.gameObject;

                foreach (var node in ((FunctionGraph)this).Nodes.GetAll)
                    node.RuntimeInit(owner);                          
            }
            PinCollection.Parent = this;
        }
		public void EditorInit(INodeCollection parent, Vector2 position, System.Type subType = null)
		{
			CreateID();
			PinCollection = new PinCollection(this);

			Parent = parent;
			this.Position = position;

			SetSubtype(subType);
			OnInit();

			OnInstantiated();

			this.IsInitialized = true;
		}
        public void CreateID()
		{
            ID = Name + "-" + System.DateTime.Now.ToString() + "-" + Random.Range(1000, 9999);
		}

        /// <summary>
        /// Grabs and calculates the input, and returns a variable as result.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual Variable GetInput(string name)
        {           
            var pin = PinCollection.Get(name);
            if (pin != null && pin.Connections.Count > 0)
            {
                Variable defaultVariable = new Variable(pin.VariableType);

                // Double check... but better check more than none.    
                if (pin.PinType != PinType.Input)
                {
                    RunemarkDebug.Error(name + " isn't input but it's used in the GetInputValue method!");
                    return defaultVariable;
                }
                
                // Get the connected node, if not exists, return a default value.
                var connectedNode = Root.Nodes.Find(pin.Connections[0].NodeID);
                if (connectedNode == null)
                {
                    RunemarkDebug.Error("{2} - No node is connected to {0}.{1}", Name, pin.Name, ID);
                    return defaultVariable;
                }
                // Get the output value of the connected node.
                var variable = connectedNode.GetOutput(pin.Connections[0].PinName);
                if (variable == null)
                {
                    RunemarkDebug.Error("{0}- {1}.{2} variable is null",
                        ID, connectedNode.Name, pin.Connections[0].PinName);
                    return defaultVariable;
                }
                if (variable.type != pin.VariableType)
                {
                   RunemarkDebug.Error("{2} - Connected Pin ({0}) and Pin ({1}) has different types",
                        variable.type, pin.VariableType, ID);
                    return defaultVariable;
                }

                return variable;
            }

           // Otherwise simply return a variable. Or null if it's not exists.
           return Variables.GetByName(name);          
        }
        /// <summary>
        /// Calculates the output and returns a result as a variable.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual Variable GetOutput(string name)
        {
            // Check if it has a stored value
            var stored = GetStoredVariable(name);
            if (stored != null)
                return stored;

            var pin = PinCollection.Get(name);
            if (pin != null)
            {
                Variable defaultVariable = new Variable(pin.VariableType);             

                // Double check... but better check more than none.    
                if (pin.PinType != PinType.Output)
                {
                    RunemarkDebug.Error(name + " isn't output but it's used in the GetOutput method!");
                    return defaultVariable;
                }
                return CalculateOutput(name);
            }

            RunemarkDebug.Error(Name +"." +name + " is not an output.");
            return null;
        }


        /// <summary>
        /// Stores a variable.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="v"></param>
        protected void StoreVariable(string name, Variable v)
        {
            string storage = name + "_STORED";
            if (Variables.Contains(storage))
                Variables[storage] = v;
            else
                Variables.Add(v);
        }
        /// <summary>
        /// Gets a stored variable
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected Variable GetStoredVariable(string name)
        {
            string storage = name + "_STORED";
            if (Variables.Contains(storage))
                return Variables[storage];
            return null;
        }
        /// <summary>
        /// Removes the stored variable.
        /// </summary>
        /// <param name="name"></param>
        protected void RemoveStoredVariable(string name)
        {
            string storage = name + "_STORED";
            if (Variables.Contains(storage))
                Variables.RemoveByName(storage);
        }
        
        /// <summary>
        /// Changes the node subtype (only if the node allowed to have a subtype)
        /// </summary>
        /// <param name="type"></param>
        protected virtual void SetSubtype(System.Type type)
        {
            
        }

        /// <summary>
        /// Copy the node.
        /// </summary>
        /// <param name="runtime"></param>
        /// <returns></returns>
        public virtual Node Copy(bool runtime = false)
		{
            var copy = (Node)CreateInstance(GetType());
			copy.Name = Name;	

			copy.Description = Description;
			copy.Position = Position;
			copy.Parent = Parent;
           
            if (runtime) copy.ID = ID;                
            else copy.CreateID();

            copy.PinCollection = PinCollection.Copy(copy);
            if (!runtime)
            {
                foreach (var pin in copy.PinCollection.Get())
                {
                    pin.Connections.Clear();
                }
            }

            copy.Variables = Variables.Copy();
           
            copy.CanCopy = CanCopy;
			copy.CanDelete = CanDelete;
			copy.Position = new Vector2(Position.x + 20, Position.y + 20);
			return copy;
		}



        public virtual void OnEditorOpen()
        {
            PinCollection.Parent = this;
        }
        


		#region Abstract Methods
		public abstract string Tooltip { get; }
		protected abstract void OnInit();
		protected abstract Variable CalculateOutput(string name);
		#endregion

		#region Editor Stuff
		public virtual bool HasChanges 
		{ 
			get { return _hasChanges; }
			set
			{
				_hasChanges = value;
				if (value && Root != this) Root.HasChanges = value;
			}
		}
		[SerializeField] bool _hasChanges;
		public bool EditorParameterChanged { get; set; }

		public Vector2 Position { get { return _position; } set { _position = value; EditorParameterChanged = true; }}
		[SerializeField] Vector2 _position = Vector2.zero;
		public float time;
		public bool CanDelete = true;
		public bool CanCopy = true;

		protected virtual void OnInstantiated(){}
        #endregion

	}
}