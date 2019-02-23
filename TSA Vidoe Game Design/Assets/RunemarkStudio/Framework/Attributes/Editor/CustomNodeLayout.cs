using Runemark.Common;
using System;
using System.Linq;
using UnityEngine;

namespace Runemark.VisualEditor
{	
    /// <summary>
    /// This attribute is defined what layout should the editor use for the
    /// node this attribute has. (basicly similat to but inverse of CustomEditor)
    /// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public class CustomNodeLayout : Attribute
	{
		public readonly Type InspectedType;
		public readonly bool EditForChildClasses;
		public CustomNodeLayout(Type inspectedType, bool editForChildClasses)
		{
			InspectedType = inspectedType;
			EditForChildClasses = editForChildClasses;
		}

		public CustomNodeLayout(Type inspectedType)
		{
			InspectedType = inspectedType;
			EditForChildClasses = false;
		}
	}

	public static class NodeLayoutSelection
	{
		public static Type Get(Type nodeType)
		{
			if (nodeType.IsSubclassOf(typeof(Node)))
			{
				Type[] types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
				Type[] nodeLayouts = (from Type type in types where type.IsSubclassOf(typeof(NodeLayout)) select type).ToArray();

				foreach (var l in nodeLayouts)
				{
					foreach (var attr in l.GetCustomAttributes(typeof(CustomNodeLayout), false))
					{
						var attribute = attr as CustomNodeLayout;
						if (nodeType == attribute.InspectedType || (attribute.EditForChildClasses && nodeType.IsSubclassOf(attribute.InspectedType)))
							return l;
					}
				}	
			}
			else
				RunemarkDebug.Error("{0} isn't subclass of the Node type.", nodeType);
			
			return typeof(DefaultLayout); // return the default node layout
		}
	}
}