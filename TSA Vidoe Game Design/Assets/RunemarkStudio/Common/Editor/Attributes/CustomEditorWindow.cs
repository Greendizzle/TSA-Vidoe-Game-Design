using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEditor.Callbacks;
using UnityEditor;

namespace Runemark.Common
{	
	public enum EditorWindowLayout { Default, Aux, Popup, Utility }

    /// <summary>
    /// This will define which class should be opened in the marked editor window.
    /// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public class CustomEditorWindow : Attribute
	{
		public readonly Type InspectedType;
		public readonly bool EditForChildClasses;
		public readonly EditorWindowLayout Layout;

		public CustomEditorWindow(Type inspectedType, bool editForChildClasses, EditorWindowLayout layout = EditorWindowLayout.Default)
		{
			InspectedType = inspectedType;
			EditForChildClasses = editForChildClasses;
			Layout = layout;
		}

		public CustomEditorWindow(Type inspectedType, EditorWindowLayout layout = EditorWindowLayout.Default)
		{
			InspectedType = inspectedType;
			EditForChildClasses = false;
			Layout = layout;
		}
	}



    /// <summary>
    /// This class handles the double click event on an asset.
    /// Based on the CustomEditorWindow attribute will open the right editor window.
    /// </summary>
	public static class CustomEditorWindowOpenHandler
	{
		[OnOpenAssetAttribute(1)]
		public static bool OnOpenAsset(int instanceID, int line)
		{
			var target = EditorUtility.InstanceIDToObject(instanceID);

			Type[] types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
			Type[] runemarkEditorWindows = (from Type type in types where type.IsSubclassOf(typeof(RunemarkEditorWindow)) select type).ToArray();

			foreach (var w in runemarkEditorWindows)
			{
				foreach (var attr in w.GetCustomAttributes(typeof(CustomEditorWindow), false))
				{
					var attribute = attr as CustomEditorWindow;
					if (target.GetType() == attribute.InspectedType || (attribute.EditForChildClasses && target.GetType().IsSubclassOf(attribute.InspectedType)))
					{					
						var window = (RunemarkEditorWindow)EditorWindow.GetWindow(w);
						window.LoadGraph(target);

						switch (attribute.Layout)
						{
							case EditorWindowLayout.Aux: window.ShowAuxWindow(); break;
							case EditorWindowLayout.Popup: window.ShowPopup(); break;
							case EditorWindowLayout.Utility: window.ShowUtility(); break;
							default: window.Show(); break;	
						}
						return true; //catch open file
					}
				}
			}
			return false; // let unity open the file
		}
	}
}