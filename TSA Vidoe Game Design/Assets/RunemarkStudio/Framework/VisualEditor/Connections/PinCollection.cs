using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Runemark.VisualEditor
{
	[System.Serializable]
	public class PinCollection
	{
        public Node Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
                foreach (var pin in _pins)
                    pin.Collection = this;
            }
        }
        Node _parent;
		[SerializeField] List<Pin> _pins = new List<Pin>();


        public PinCollection(Node parent)
        {            
            Parent = parent;
        }
        
		/// <summary>
		/// Number of connections in this collection.
		/// </summary>
		public int Count() { return _pins.Count; }

		/// <summary>
		/// Number of the specified type of connections.
		/// </summary>
		/// <param name="group">Group.</param>
		public int Count(PinType type)	{ return Get(type).Count; }

		/// <summary>
		/// Gets all the pins.
		/// </summary>
		public List<Pin> Get(){ return _pins; }

		/// <summary>
		/// Gets a single pin with the specified name.
		/// </summary>
		/// <param name="name">Name.</param>
		public Pin Get(string name)
		{
			foreach (var c in _pins)
			{
				if (c.Name == name)
					return c;
			}
			return null;
		}

		/// <summary>
		/// Gets a list of pins from the specified group.
		/// </summary>
		/// <param name="group">Group.</param>
		public List<Pin> Get (PinType type)
		{
			return _pins.FindAll(x => x.PinType == type);
		}

		public void AddInput(string name, System.Type type) { Add(name, PinType.Input, type); }
		public void AddOutput(string name, System.Type type) { Add(name, PinType.Output, type); }
		public void AddInTransition(string name) { Add(name, PinType.TransIn, typeof(ExecutableNode)); }
		public void AddOutTransition(string name) { Add(name, PinType.TransOut, typeof(ExecutableNode)); }
        
		public void Add(string name, PinType pinType, System.Type type)
		{
            Add(new Pin()
            {
                    Name = name,
                    Collection = this,
	    			PinType = pinType,
					VariableType = type,							
		    });
		}
        public void Add(Pin conn)
        {
            _pins.Add(conn);
        }


        
		public void Remove(string name)
		{			
			Remove(Get(name));
		}
		public void Remove(Pin pin)
		{
			_pins.Remove(pin);
		}

        public void Clear()
        {
            _pins.Clear();
        }	

        /// <summary>
		/// Copy this collection.
		/// </summary>
		public PinCollection Copy(Node parent)
        {          
            PinCollection collection = new PinCollection(parent);
            foreach (var conn in _pins)
            {
                var copy = conn.Copy();
                copy.Collection = collection;
                collection.Add(copy);
            }                
            return collection;
        }
    }
}
