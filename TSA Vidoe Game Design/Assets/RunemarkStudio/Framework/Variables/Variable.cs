using UnityEngine;
using Runemark.Common;

namespace Runemark.VisualEditor
{
	[System.Serializable]
	public class Variable
	{
        /// <summary>
        /// Name of the variable.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        [SerializeField]
        string _name;

        /// <summary>
        /// ID of the variable. Built up from its type, the date of the creation and a random number.
        /// </summary>
        /// <value>The I.</value>
        public string ID { get {  return GetType().ToString() + _idSuffix; }}
		[SerializeField]string _idSuffix;



		/// <summary>
		/// Group of the variable. 
		/// In some cases the variables are shown in groups.
		/// </summary>
		public string Group 
		{
			get { return _group; }
			set { _group = value; }
		}
		[SerializeField]string _group = "Default";

		/// <summary>
		/// Gets the type of the variable.
		/// </summary>
		/// <value>The type.</value>
		public System.Type type { 
			get{ return System.Type.GetType(_type); }
			set
            {
                _type = value.ToString();
                Value = TypeUtils.GetDefaultValue(value);
            }
		}
		[SerializeField]string _type;

        /// <summary>
        /// Determines whether or not this variable should be saved as player progress.
        /// This only works if this variable is Root or Global variable.
        /// </summary>
        public bool Save;

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Runemark.VisualEditor.Variable"/> is hidden.
		/// Hidden variables are not shown in the visual editor.
		/// </summary>
		/// <value><c>true</c> if hidden; otherwise, <c>false</c>.</value>
		public bool Hidden { get; set; }

		/// <summary>
		/// The default value.
		/// </summary>
		public SObject DefaultValue = new SObject("Variable.DefaultValue");

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public object Value 
		{
			get { return (_value.Value != null) ? _value.Value : DefaultValue.Value; }
			set 
			{ 
				if (value == null || CanConvertFrom(value.GetType()))
				{
					if (value == null)
						value = TypeUtils.GetDefaultValue(type);
					_value.Value = value;                
				}
				else
					RunemarkDebug.Error("{0} variable cannot be set to {1} becouse the type of the new value ({2})"+
                        "cannot be converted to the variable type ({3})",
                        Name, value, value.GetType(), type );
			}
		}
		[SerializeField]SObject _value = new SObject("Variable.Value");
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Runemark.VisualEditor.Variable"/> class.
		/// </summary>
		/// <param name="type">Type.</param>
		public Variable(System.Type type)
		{
			this.type = type;
            Value = TypeUtils.GetDefaultValue(type);
			this.Name = "New " + TypeUtils.GetPrettyName(type);
			CreateID();
		}

		/// <summary>
		/// Creates the ID
		/// </summary>
		public void CreateID()
		{
			_idSuffix = "-" + System.DateTime.Now.ToString() + "-" + Random.Range(1000, 9999);
		}
        
		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Runemark.VisualEditor.Variable"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Runemark.VisualEditor.Variable"/>.</returns>
		public override string ToString()
		{
			return string.Format("[Variable: Name={0}, Type={1}, Group={2}, Value={3}]", Name, type, Group, _value.Value);
		}
        

		/// <summary>
		/// Gets a value already converted to the given type (if possible).
		/// </summary>
		/// <returns>The value.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public T ConvertedValue<T>()
		{
            if (Value == null) return default(T);
			if(CanConvertTo(typeof(T))) return (T) Value;

            RunemarkDebug.Error("{0} variable is {1} that cannot be converted to {2}",
                        Name, Value.GetType(), typeof(T));
			return default(T);
		}

		/// <summary>
		/// Determines whether this variable's value can be converted to the specified type.
		/// </summary>
		/// <returns><c>true</c> if this instance can convert to the specified type; otherwise, <c>false</c>.</returns>
		/// <param name="type">Type.</param>
		public bool CanConvertTo(System.Type type)
		{
			return type == this.type || this.type.IsSubclassOf(type); 
		}

		/// <summary>
		/// Determines whether this variable's value can be converted from the specified type.
		/// </summary>
		/// <returns><c>true</c> if this instance can convert from the specified type; otherwise, <c>false</c>.</returns>
		/// <param name="type">Type.</param>
		public bool CanConvertFrom(System.Type type)
		{
			return type == this.type || type.IsSubclassOf(this.type); 
		}

		/// <summary>
		/// Copy this instance.
		/// </summary>
		public Variable Copy()
		{
			Variable nV = new Variable(type);
			nV.CreateID();
			nV.DefaultValue.Value = DefaultValue.Value;
            nV.Value = Value;
			nV.Group = Group;
			nV.Hidden = Hidden;
			nV.Name = Name;
            nV.Save = Save;            
			return nV;
		}
    }
}