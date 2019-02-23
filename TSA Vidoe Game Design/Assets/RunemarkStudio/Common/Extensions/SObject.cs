using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Runemark.Common
{
	[System.Serializable]
	public class SObject : ISerializationCallbackReceiver
	{
        public string Name { get; set;  }
        /// <summary>
        /// Value in string format to show in inspector debug mode.
        /// </summary>
        public string IValue { get; set;  }

		public object Value
        {
            get
            {
                IValue = _value + "";
                return _value;
            }
            set
            {
                _value = value;               
            }
        }
        object _value;

		[HideInInspector][SerializeField] Object _serializedObject = null;
        [HideInInspector][SerializeField] string _serializedString = "";

		BinaryFormatter _serializer = new BinaryFormatter();

        public SObject() { Name = "Unnamed SObject"; }
        public SObject(string name) { Name = name; }

		#region ISerializationCallbackReceiver implementation
		public void OnBeforeSerialize()
		{ 
			if (Value == null)
				_serializedString = "null";
			else
			{
				if (Value.GetType().IsSubclassOf(typeof(Object)))
				{
					_serializedString = "builtIn";
					_serializedObject = Value as Object;
				}
				else
				{
                    if (Value.GetType().IsSerializable)
                    {                      
                        using (var stream = new MemoryStream())
                        {
                            _serializer.Serialize(stream, Value);
                            stream.Flush();
                            _serializedString = System.Convert.ToBase64String(stream.ToArray());
                        }
                    }					
				}
			}
		}
		public void OnAfterDeserialize()
		{
			if (_serializedString == "null")
				Value = null;
			else if (_serializedString == "builtIn")
				Value = (object)_serializedObject;
			else
			{
				byte[] bytes = System.Convert.FromBase64String(_serializedString);
				using (var stream = new MemoryStream(bytes))
					Value = _serializer.Deserialize(stream);
			}
		}
        #endregion
    }
}