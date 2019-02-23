using UnityEditor;
using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Runemark.Common;
using Runemark.VisualEditor;

public class CreateClass
{	
	public static void CreateNodeLayout <T> () where T : NodeLayout
	{
		string parentClass = typeof(T).Name;

		List<string> content = new List<string>();

		content.Add("using UnityEngine;");
		content.Add("using UnityEditor;");
		content.Add("using Runemark.VisualEditor.Utility;");
		content.Add("using Runemark.VisualEditor.Actions;");
		content.Add("");
		content.Add("");
		content.Add("namespace Runemark.VisualEditor");
		content.Add("{ ");
		content.Add("\t[CustomNodeLayout(typeof({classname}), true)]");
		content.Add("\tpublic class {classname}Layout : " + parentClass);
		content.Add("\t{ ");	
		content.Add("\t\tpublic {classname}Layout (NodeWindow window) : base (window){ } ");
		content.Add("\t} ");
		content.Add("");
		content.Add("");
		content.Add("\t[CustomEditor(typeof({classname}))]");
		content.Add("\tpublic class {classname}Inspector : NodeInspector");
		content.Add("\t{");
		content.Add("\t\tprotected override void onGUI()");
		content.Add("\t\t{");
		content.Add("\t\t\t{classname} myTarget = ({classname})target;");
		content.Add("\t\t}");
		content.Add("\t}");
		content.Add("} "); // Namespace end

		string path = EditorUtility.SaveFilePanelInProject(
			"Create new " + parentClass,
			"name_of_node.cs",
			"cs", IOUtility.GetCurrentPath()
		);

		var pathArr = path.Split('/');
		var filename = pathArr[pathArr.Length - 1].Split('.');

		for (int l = 0; l < content.Count; l++)
			content[l] = content[l].Replace("{classname}", filename[0]);
		
		filename[0] = filename[0] + "Layout";
		pathArr[pathArr.Length - 1] = String.Join(".", filename);
		path = String.Join("/", pathArr);

		IOUtility.CreateScript(content, path);

		UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(path, 0);
	}

}