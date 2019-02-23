using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Runemark.Common
{
	public static class IOUtility 
	{
		public static string GetCurrentPath()
		{
			string path = AssetDatabase.GetAssetPath (Selection.activeObject);
			if (path == "")	path = "Assets";
			else if (Path.GetExtension(path) != "")
				path = path.Replace(Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
			return path;
		}

        public static string GetPath(Object o)
        {
            return AssetDatabase.GetAssetPath(o);
        }

        public static void CreateScript(List<string> content, string path)
		{
			if( File.Exists(path) == false )
			{ 
				using (StreamWriter outfile = new StreamWriter(path))
				{
					foreach (var l in content)
						outfile.WriteLine(l);
				}
			}
			AssetDatabase.Refresh();
		}
	}

}