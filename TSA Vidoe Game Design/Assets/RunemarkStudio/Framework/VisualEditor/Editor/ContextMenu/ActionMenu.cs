using UnityEngine;
using System.Collections.Generic;
using Runemark.VisualEditor.Actions;
using Runemark.Common;
using UnityEditor;
using System.Linq;
using System.Reflection;


namespace Runemark.VisualEditor
{
	public class ActionMenu : Runemark.Common.ContextMenu
	{
		public static VisualEditor Editor;

        #region implemented abstract members of ContextMenu
		protected override string title { get { return "Actions"; 	}}
		protected override void OnItemSelected(MenuItem item)
		{
			string name = item.Name;
            if (item.args == null || !(item.args[0] is System.Type))
                Editor.CustomActionMenuCallback(name, item.args);
            else
            {
                System.Type type = (System.Type)item.args[0];
                System.Type subType = (item.args.Length > 1) ? (System.Type)item.args[1] : null;
                Editor.NodeCreate(name, type, subType);
            }
		}

		#endregion

        void OnEnable()
        {
            foreach (System.Type type in Assembly.GetAssembly(typeof(Node)).GetTypes().Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(typeof(Node))))
			{
				Info info = null;

				foreach (var attr in type.GetCustomAttributes(typeof(Info), false))
					info = attr as Info;
				if (info == null) continue;

				string p = info.DisplayName;
				List<string> path = p.Split('/').ToList();
				string name = path[path.Count - 1];

				// Follow & Create Hierarchy
				List<Menu> list = menu;
				for (int i = 0; i < path.Count-1; i++)
				{
					var group = list.Find(x => x.Name == path[i]) as MenuGroup;
					if (group == null)
					{
						group = new MenuGroup()
                        {
                            Name = path[i],
                            Items = new List<Menu>(),
                        };
						list.Add(group);
					}	
					list = group.Items;
				}

				Texture2D icon = null;
				int priority = info.Priority;

				if (info.SubTypes != null && info.SubTypes.Length > 0)
				{	
					foreach (System.Type subtype in info.SubTypes)
					{
						string prettyName = TypeUtils.GetPrettyName(subtype);
						if (info.Icon != "")
						{
							string iconPath = string.Format(info.Icon, prettyName.ToUpper());
							icon = LoadIcon(iconPath);
						}

						string suffix = " (" + prettyName + ")";
						list.Add(new MenuItem(){ Name = name + suffix, args = new object[]{ type, subtype }, Icon = icon, Priority = priority });
					}
				}
				else
				{
					icon = LoadIcon(info.Icon);
					list.Add(new MenuItem(){ Name = name, args = new object[]{ type }, Icon = icon, Priority = priority });
				}
			}

            // PASTE
            if(Editor != null && Editor.Clipboard != null && Editor.Clipboard.Count > 0)
            {
                menu.Add(new MenuItem()
                {
                    Name = "Paste",// + Editor.Clipboard.Count,
                    Priority = 2,
                    args = new object[] { "" }
                });
            }

           
                

            menu = menu.OrderByDescending(x => x.Priority).ThenBy(x => x.Name).ToList();
		}

        Texture2D LoadIcon(string path)
		{
			Texture2D icon = null;
			if (path != "")
				icon = Resources.Load<Texture2D>("Editor/"+path);
			
			if (path != "" && icon == null) 
				icon = (Texture2D)EditorGUIUtility.IconContent(path).image;
			return icon;
		}
	}
}
