using UnityEngine;
using UnityEditor;


namespace Runemark.Common
{
	/// <summary>
	/// Helper class to create asset from scriptable object
	/// </summary>
	public static class AssetCreator 
	{
		public static T CreateAsset<T>() where T : ScriptableObject
		{
			return (T)CreateAsset(typeof(T));
		}

		public static T CreateAsset<T>(Object parent) where T : ScriptableObject
		{
			return (T)CreateAsset("temp", typeof(T), parent);
		}

		public static T CreateAsset<T>(string path) where T : ScriptableObject
		{
			return (T)CreateAsset(typeof(T), path);
		}

		public static T CreateAsset<T>(bool displayFilePanel) where T : ScriptableObject
		{
			return (T)CreateAsset(typeof(T), displayFilePanel);
		}


		public static ScriptableObject CreateAsset(System.Type type, bool displayFilePanel)
		{
			if (displayFilePanel)
			{
				string path = EditorUtility.SaveFilePanelInProject(
					"Create Asset of type "+type.Name,
					"New "+type.Name+".asset",
					"asset", ""
				);
				return CreateAsset(type, path);
			}
			return CreateAsset(type);
		}

		public static ScriptableObject CreateAsset(System.Type type)
		{
			string path = IOUtility.GetCurrentPath();				
			return CreateAsset(type, path + "/New "+type.Name + ".asset");
		}

		public static ScriptableObject CreateAsset(string name, System.Type type, Object parent)
		{
			if (!EditorUtility.IsPersistent(parent))
				return null;			
			ScriptableObject data = ScriptableObject.CreateInstance(type);
			data.name = name;
			AssetDatabase.AddObjectToAsset (data, parent);
			AssetDatabase.SaveAssets();
			return data;
		}

		public static ScriptableObject CreateAsset(System.Type type, string path)
		{
			if (string.IsNullOrEmpty(path))
				return null;

			ScriptableObject data = ScriptableObject.CreateInstance(type);
			AssetDatabase.CreateAsset(data, path);
			AssetDatabase.SaveAssets();
			return data;
		}



	}
}