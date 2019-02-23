using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Runemark.Common
{
	
	public abstract class ContextMenu : EditorWindow 
	{	
		#region Abstracts
		new protected abstract string title { get; }
		protected abstract void OnItemSelected(MenuItem item);


		#endregion


		//[UnityEditor.MenuItem("Window/Runemark/TestContext")]
		public static void Open()
		{
			GetWindow<ContextMenu>();
		}

		protected class Menu { public string Name; public Texture2D Icon; public int Priority = 0;  }
		protected class MenuGroup : Menu { public List<Menu> Items; }
		protected class MenuItem : Menu { public object[] args; }

		protected List<Menu> menu = new List<Menu>();	

		List<MenuGroup> _group = new List<MenuGroup>();

		Styles _styles;
		string _searchString = string.Empty;
		Vector2 _scrollPosition;

		double _lastUpdate;

        Texture2D _emptyIcon;

		void OnGUI()
		{
			if (_styles == null)
				_styles = new Styles();
            if (_emptyIcon == null)
            {
                var text = new RectangleTexture();
                text.FillColor = new Color(0, 0, 0, 0);
                text.BorderColor = text.FillColor;
                text.Resolution = 16;
                _emptyIcon = text.Generate();
            }
            			
	
			// Background border
			GUI.Label(new Rect(0.0f, 0.0f, this.position.width, this.position.height), GUIContent.none, _styles.background);

			// SEARCH FIELD
			GUILayout.Space(7);
			GUILayout.BeginHorizontal();
			_searchString = GUILayout.TextField(_searchString, GUI.skin.FindStyle("SearchTextField"));
			var buttonStyle = _searchString == string.Empty ? GUI.skin.FindStyle("SearchCancelButtonEmpty") : GUI.skin.FindStyle("SearchCancelButton");

			if (GUILayout.Button(string.Empty, buttonStyle))
			{
				// Remove focus if cleared
				_searchString = string.Empty;
				GUI.FocusControl(null);
			}
			GUILayout.EndHorizontal();

			Rect panelRect = new Rect(1, 30, position.width - 2, position.height - 30);
			string t = _group.Count == 0 ? title : _group[_group.Count - 1].Name;
			List<Menu> list = (_group.Count == 0) ? menu : _group[_group.Count - 1].Items; 

			// MENU		
			GUILayout.BeginArea(panelRect);
			panelRect = GUILayoutUtility.GetRect(10f, 25f);

			// TITLE BAR
			if (GUI.Button(panelRect, (_searchString != string.Empty) ? "Search" : t, _styles.header)) HierarchyUp();
			if(_group.Count > 0) GUI.Label(new Rect(panelRect.x + 6.5f, panelRect.y + 6.5f, 13, 13), "", _styles.leftArrow);

			// LIST
			_scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

			// Apply search
			if (_searchString != string.Empty)
			{
				var searchString = _searchString.ToLower();
				List<Menu> visible = new List<Menu>();
				foreach (var item in menu)
					FilterMenuItem(item, searchString, ref visible);

				list = visible;
			}

            // Draw items
            int lastPriority = -1;
			foreach (var item in list)
			{
				if (lastPriority != item.Priority)
				{
					if (lastPriority > 0)
					{
						GUILayout.Box("", (GUIStyle)"AppToolbar", GUILayout.Width(position.width-5), GUILayout.Height(2));
					}
					lastPriority = item.Priority;
				}

				MenuGroup g = item as MenuGroup;

				var buttonRect = GUILayoutUtility.GetRect(16f, 20f, GUILayout.ExpandWidth(true));

                GUIContent c = new GUIContent(item.Name, (item.Icon != null) ? item.Icon : _emptyIcon);
				if (GUI.Button(buttonRect, c, _styles.componentButton))
				{
					if (g != null) HierarchyDown(g);
					else
					{
						OnItemSelected((MenuItem)item);
						Close();
					}
				}
                if (g != null)
                {
                    GUI.Label(new Rect(buttonRect.x + buttonRect.width - 26,
                                         buttonRect.y + 4f,
                                         13f, 13f),
                                         "", (GUIStyle)"AC RightArrow");
                }
			}
            


            GUILayout.EndScrollView();
			GUILayout.EndArea();	

			Repaint();
		}

		void FilterMenuItem(Menu item, string searchString, ref List<Menu> filteredList)
		{
			if (item == null) return;

			MenuGroup g = item as MenuGroup;
			if (g == null)
			{
				filteredList.Add(item);
				return;
			}

			foreach (var i in g.Items)
				FilterMenuItem(i, searchString, ref filteredList);			
		}

		void HierarchyDown(MenuGroup g) { _group.Add(g); }
		void HierarchyUp() { if(_group.Count > 0) _group.RemoveAt(_group.Count - 1); }
				








		public class Styles 
		{
			public GUIStyle header = new GUIStyle((GUIStyle)"In BigTitle");
			public GUIStyle componentButton = new GUIStyle((GUIStyle)"PR Label");
			public GUIStyle background = (GUIStyle)"grey_border";
			public GUIStyle previewBackground = (GUIStyle)"PopupCurveSwatchBackground";
			public GUIStyle previewHeader = new GUIStyle(EditorStyles.label);
			public GUIStyle previewText = new GUIStyle(EditorStyles.wordWrappedLabel);
			public GUIStyle rightArrow = (GUIStyle)"AC RightArrow";
			public GUIStyle leftArrow = (GUIStyle)"AC LeftArrow";
			public GUIStyle groupButton;

			public Styles()
            {
                var text = new RectangleTexture();               
                text.Resolution = 16;
                text.FillColor = new Color(0.24f, 0.49f, 0.91f, 1.00f);
                text.BorderColor = new Color(0.24f, 0.49f, 0.91f, 1.00f);
                Texture2D texture = text.Generate();



                this.header.font = EditorStyles.boldLabel.font;

				this.componentButton.alignment = TextAnchor.MiddleLeft;
				this.componentButton.padding.left -= 15;
				this.componentButton.fixedHeight = 20f;

				this.componentButton.hover.background = texture;
				this.componentButton.hover.textColor = Color.white;
				this.componentButton.focused.background = texture;
				this.componentButton.focused.textColor = Color.white;
				this.componentButton.active.background = texture;
				this.componentButton.active.textColor = Color.white;

				this.groupButton = new GUIStyle(this.componentButton);
				this.groupButton.padding.left += 17;
				this.previewText.padding.left += 3;
				this.previewText.padding.right += 3;
				++this.previewHeader.padding.left;
				this.previewHeader.padding.right += 3;
				this.previewHeader.padding.top += 3;
				this.previewHeader.padding.bottom += 2;
			}
		}
	}
}