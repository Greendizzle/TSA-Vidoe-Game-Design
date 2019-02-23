using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Runemark.Common
{
	public class SimpleObjectReorderableList<T> : ReorderableListGUI where T : Object
	{
		public override string Title { get { return _title; } }
		public override bool UseAddDropdown { get { return false; } }

		string _title;
		List<T> _list;

		public SimpleObjectReorderableList(string title, List<T> list) : base (list)
		{
			_title = title;
			_list = list;
		}

		#region implemented abstract members of ReorderableListGUI
		protected override void drawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
		{
			_list[index] = (T)EditorGUI.ObjectField(rect, _list[index], typeof(T), true); 
		}
		protected override void onAddCallback(UnityEditorInternal.ReorderableList list)
		{
			ReorderableList.defaultBehaviours.DoAddButton(list);
		}
		protected override void onAddDropdownCallback(Rect buttonRect, UnityEditorInternal.ReorderableList list)
		{

		}
		protected override bool onCanRemoveCallback(UnityEditorInternal.ReorderableList list)
		{
			return true;
		}
		protected override void onRemoveCallback(UnityEditorInternal.ReorderableList list)
		{
			ReorderableList.defaultBehaviours.DoRemoveButton(list);
		}
		protected override void onSelectCallback(UnityEditorInternal.ReorderableList list)
		{
			
		}

		#endregion
		
	}
}
