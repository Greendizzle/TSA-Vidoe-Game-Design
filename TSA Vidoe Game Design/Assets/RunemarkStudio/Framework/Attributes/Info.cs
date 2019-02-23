using System;

namespace Runemark.VisualEditor
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public class Info : Attribute
	{
		/// <summary>
		/// The display name and the path in the menu.
		/// </summary>
		public string DisplayName = "";

		/// <summary>
		/// The icon of the node. You can set multiple icons for each allowed subtypes by seperating the icon path with '|' char
		/// </summary>
		public string Icon = "";

		/// <summary>
		/// (optional)The allowed sub types.
		/// </summary>
		public Type[] SubTypes = new Type[0];

		/// <summary>
		/// The priority.
		/// </summary>
		public int Priority = 0;

		public Info(string displayName){ Initialize(displayName, "", 0, new Type[0]); }	
		public Info(string displayName, Type[] subTypes){ Initialize(displayName, "", 0, subTypes);	}

		public Info(string displayName, int priority){ Initialize(displayName, "", priority, new Type[0]); }
		public Info(string displayName, int priority, Type[] subTypes){ Initialize(displayName, "", priority, subTypes); }

		public Info(string displayName, string iconPath){ Initialize(displayName, iconPath, 0, new Type[0]); }
		public Info(string displayName, string iconPath, Type[] subTypes){ Initialize(displayName, iconPath, 0, subTypes); }

		public Info(string displayName, string iconPath, int priority){ Initialize(displayName, iconPath, priority, new Type[0]); }
		public Info(string displayName, string iconPath, int priority, Type[] subTypes){ Initialize(displayName, iconPath, priority, subTypes); }

		public Info(string displayName, Type typeIcon){ Initialize(displayName, typeIcon, 0, new Type[0]); }
		public Info(string displayName, Type typeIcon, Type[] subTypes){ Initialize(displayName, typeIcon, 0, subTypes); }

		public Info(string displayName, Type typeIcon, int priority){ Initialize(displayName, typeIcon, priority, new Type[0]); }
		public Info(string displayName, Type typeIcon, int priority, Type[] subTypes){ Initialize(displayName, typeIcon, priority, subTypes); }

		void Initialize(string name, string icon, int priority, Type[] subTypes)
		{
			DisplayName = name;
			Icon = icon;
			Priority = priority;
			SubTypes = subTypes;
		}

		void Initialize(string name, Type icon, int priority, Type[] subTypes)
		{
			DisplayName = name;
			Icon = GetTypeIcon(icon);
			Priority = priority;
			SubTypes = subTypes;
		}



		string GetTypeIcon(System.Type type)
		{
			string icon = "";
			#if UNITY_EDITOR
			icon = UnityEditor.EditorGUIUtility.ObjectContent(null, type).image.name;
			#endif
			return icon;
		}
	}
}