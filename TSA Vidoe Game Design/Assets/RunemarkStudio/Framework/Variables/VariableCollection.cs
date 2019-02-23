using Runemark.Common;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


namespace Runemark.VisualEditor
{
	[System.Serializable]
	public class VariableCollection 
	{
		[SerializeField]List<Variable> _variables = new List<Variable>();

		/// <summary>
		/// Gets or sets the variable with the specified name.
		/// </summary>
		/// <param name="name">Name.</param>
		public Variable this[string name]
		{
			get 
			{
				var v = GetByName(name);
				if (v != null)
					return v;
				return null;		
			}
			set 
			{ 
                
				var v = GetByName(name);
                int index = _variables.IndexOf(v);
                _variables[index] = value;
			}
		}
		public Variable this[int index]
		{
			get 
			{
				if (_variables.Count < index && index >= 0)
					return _variables[index];
				return null;		
			}
			set 
			{ 
				if (_variables.Count < index && index >= 0)
					_variables[index] = value;
			}
		}

		/// <summary>
		/// Number of variables in this collection.
		/// </summary>
		public int Count() { return _variables.Count; }

		/// <summary>
		/// Number of variables in this group.
		/// </summary>
		/// <param name="group">Group.</param>
		public int Count(string group) { return GetByGroup(group).Count; }

		/// <summary>
		/// Does this collection contain a variable with the specified name.
		/// </summary>
		/// <param name="name">Name.</param>
		public bool Contains(string name) {	return GetByName(name) != null;	}

		#region GET

		/// <summary>
		/// Gets the variable by ID.
		/// </summary>
		/// <returns>The by I.</returns>
		/// <param name="id">Identifier.</param>
		public Variable GetByID (string id)
		{
			foreach (var v in _variables)
			{
				if (v.ID == id)
					return v;
			}
			return null;	
		}


		/// <summary>
		/// Gets the variable by name.
		/// </summary>
		/// <returns>The by name.</returns>
		/// <param name="name">Name.</param>
		public Variable GetByName (string name)
		{
			foreach (var v in _variables)
			{
				if (v.Name == name)
					return v;
			}
			return null;	
		}

		/// <summary>
		/// Gets a list of variable with the specified group name.
		/// </summary>
		/// <returns>The by group.</returns>
		/// <param name="group">Group.</param>
		public List<Variable> GetByGroup (string group)
		{
			List<Variable> list = new List<Variable>();
			foreach (var v in _variables)
			{
				if (v.Group == group)
					list.Add(v);				
			}
			return list;
		}

		/// <summary>
		/// Gets all the variables from this collection.
		/// </summary>
		/// <returns>The all.</returns>
		public List<Variable> GetAll () { return _variables; }

        #endregion

        #region ADD      

		/// <summary>
		/// Create new variable.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="value">Value.</param>
		public string Add(string name, object value) { return Add(name, value, ""); }
		/// <summary>
		/// Create new variable.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="value">Value.</param>
		/// <param name="group">Group.</param>
		public string Add(string name, object value, string group)
		{
            System.Type t = value as System.Type;
            if (t != null)
				return Add(name, t, null, group);
			else
				return Add(name, value.GetType(), value, group); 
		}
        /// <summary>
        /// Create new variable.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="type">Type.</param>
        /// <param name="value">Value.</param>
        /// <param name="group">Group.</param>
        public string Add(string name, System.Type type, object value, string group)
		{
            if (!Contains(name))
			{
                if (value == null)
                    value = (type.IsValueType) ? System.Activator.CreateInstance(type) : null;

                if (group == "") group = VariableGroups.DEFAULT;
                Variable v = new Variable(type);
                v.Name = name;
                v.Group = group;
                v.Value = value;
                Add(v);
				return v.ID;
			}
			return "";
		}
        /// <summary>
        /// Create new variable.
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public string Add(Variable variable)
        {
            if (variable == null) return "";
            if (Contains(variable.Name)) return "";

            _variables.Add(variable);
            return variable.ID;
        }

        #endregion

        #region REMOVE

        /// <summary>
        /// Remove variable by name.
        /// </summary>
        /// <param name="name">Name.</param>
        public void RemoveByName(string name)
		{
			Remove(GetByName(name));
		}

		/// <summary>
		/// Remove variable by ID.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public void RemoveByID(string id)
		{
			Remove(GetByID(id));
		}

		/// <summary>
		/// Remove every variable in the given group.
		/// </summary>
		/// <param name="group">Group.</param>
		public void RemoveByGroup(string group)
		{
			_variables.RemoveAll(x => x.Group == group);
		}

		/// <summary>
		/// Remove the specified variable.
		/// </summary>
		/// <param name="v">V.</param>
		public void Remove(Variable v)
		{
			if (v != null)
				_variables.Remove(v);
		}
		#endregion

        
		/// <summary>
		/// Copy this collection.
		/// </summary>
		public VariableCollection Copy()
		{
			VariableCollection collection = new VariableCollection();
            foreach (var variable in _variables)
            {
                Variable v = variable.Copy();
                collection.Add(v);
            }
            collection.owner = owner;
			return collection;
		}

		/// <summary>
		/// Reset all variable to default value.
		/// </summary>
		public void Reset()
		{
            foreach (var v in _variables)
            {
                v.Value = v.DefaultValue.Value;
            }
		}
        
        #region SAVE & LOAD
        public Object owner;               

        /// <summary>
        /// Saves the saveable variables to the player prefs.
        /// </summary>
        public void SaveToPlayerPrefs()
        {
            string prefix = "RunemarkDialogueSystem";
            if (owner.GetType() == typeof(VisualEditorBehaviour) || owner.GetType().IsSubclassOf(typeof(VisualEditorBehaviour)))
                prefix += "-"+((VisualEditorBehaviour)owner).ID;
            else if (owner.GetType() == typeof(GameObject))
            {
                var b = ((GameObject)owner).GetComponent<VisualEditorBehaviour>();
                prefix += "-"+b.ID;
            }

            foreach (var v in _variables)
            {
                if (v.Save)
                {                   
                    string key = prefix + "-" + v.Name;

                    RunemarkDebug.Log("{0} variable saved to player prefs", key);

                    if (v.type == typeof(int))
                        PlayerPrefs.SetInt(key, v.ConvertedValue<int>());
                    else if (v.type == typeof(float))
                        PlayerPrefs.SetFloat(key, v.ConvertedValue<float>());
                    else if (v.type == typeof(string))
                        PlayerPrefs.SetString(key, v.ConvertedValue<string>());
                    else
                    {
                        string s = SerializeToString(v.Value);
                        PlayerPrefs.SetString(key, s);
                    }                        
                }
            }            
        }
        /// <summary>
        /// Loads the saveable variables from the player prefs.
        /// </summary>
        public void LoadFromPlayerPrefs()
        {
            string prefix = "DialogueSystem-";
            if (owner.GetType() == typeof(VisualEditorBehaviour) || owner.GetType().IsSubclassOf(typeof(VisualEditorBehaviour)))
                prefix += ((VisualEditorBehaviour)owner).ID;
            else if (owner.GetType() == typeof(GameObject))
            {
                var b = ((GameObject)owner).GetComponent<VisualEditorBehaviour>();
                prefix += b.ID;
            }

            foreach (var v in _variables)
            {
                if (v.Save)
                {
                    string key = prefix + "-" + v.Name;
                    if (v.type == typeof(int))
                        v.Value = PlayerPrefs.GetInt(key);
                    else if (v.type == typeof(float))
                        v.Value = PlayerPrefs.GetFloat(key);
                    else if (v.type == typeof(string))
                        v.Value = PlayerPrefs.GetString(key);
                    else
                    {
                        string s = PlayerPrefs.GetString(key);
                        v.Value = DeserializeFromString(s);                       
                    }
                }
            }
        }

        /// <summary>
        /// Saves the saveable variables into a string.
        /// </summary>
        public string SaveToString()
        {
            List<VariableData> list = new List<VariableData>();
            foreach (var v in _variables)
            {
                if (v.Save)
                {
                    var data = new VariableData();
                    data.Name = v.Name;
                    data.Value = v.Value;
                    list.Add(data);
                }
            }
            return SerializeToString(list);
        }
        /// <summary>
        /// Loads the saveable variables from a string.
        /// </summary>
        public void LoadFromString(string s)
        {
            List<VariableData> list = (List<VariableData>)DeserializeFromString(s);

            foreach (var vd in list)
            {
                var v = GetByName(vd.Name);
                if(v.Save)
                    v.Value = vd.Value;
            }              
        }

        // XML: spec attr
        // SQLite

        string SerializeToString(object o)
        {
            if (o.GetType().IsSerializable)
            {
                BinaryFormatter serializer = new BinaryFormatter();
                using (var stream = new MemoryStream())
                {
                    serializer.Serialize(stream, o);
                    stream.Flush();
                    return System.Convert.ToBase64String(stream.ToArray());                    
                }
            }
            return "";
        }
        object DeserializeFromString(string s)
        {
            BinaryFormatter serializer = new BinaryFormatter();
            byte[] bytes = System.Convert.FromBase64String(s);
            using (var stream = new MemoryStream(bytes))
               return serializer.Deserialize(stream);         
        }

        #endregion 

        [System.Serializable]
        public class VariableData
        {
            public string Name;
            public string Type;
            public string ValueString;

            public object Value
            {
                get
                {
                    System.Type t = System.Type.GetType(Type);
                    return System.Convert.ChangeType(ValueString, t);
                }
                set
                {
                    Type = value.GetType().ToString();
                    ValueString = value.ToString();
                }
            }
        }
    }
}