using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Collections;

namespace Runemark.Common
{
	public abstract class ReorderableListGUI
	{
        protected virtual bool draggable { get { return true;  } }
        protected virtual bool displayHeader { get { return true; } }
        protected virtual bool displayAddButton { get { return true; } }
        protected virtual bool displayRemoveButton { get { return true; } }

        protected ReorderableList _reorderableList;

		bool _stylesInitialized;

		public ReorderableListGUI(IList list)
		{
			_reorderableList = new ReorderableList(list, typeof(string), draggable, displayHeader, displayAddButton, displayRemoveButton);
			_reorderableList.drawHeaderCallback = drawHeaderCallback;
			_reorderableList.drawElementCallback = drawElementCallback;

			if(!UseAddDropdown)
				_reorderableList.onAddCallback = onAddCallback;
			else
				_reorderableList.onAddDropdownCallback = onAddDropdownCallback;
			
			_reorderableList.onCanRemoveCallback = onCanRemoveCallback;
			_reorderableList.onRemoveCallback = onRemoveCallback;
			_reorderableList.onSelectCallback = onSelectCallback;
			_reorderableList.onReorderCallback = onReorderCallback;
		}

		public void Draw(Rect rect)
		{
			if (!_stylesInitialized)
				InitGUIStyles();

			_reorderableList.DoList(rect);
		}

		public void Draw()
		{
			if (!_stylesInitialized)
				InitGUIStyles();

			_reorderableList.DoLayoutList();
		}




		#region Abstract members
		public abstract string Title { get; }
		public abstract bool UseAddDropdown { get; }

		protected abstract void drawElementCallback(Rect rect, int index, bool isActive, bool isFocused);
		protected abstract void onAddCallback(ReorderableList list);
		protected abstract void onAddDropdownCallback(Rect buttonRect, ReorderableList list);
		protected abstract bool onCanRemoveCallback(ReorderableList list);
		protected abstract void onRemoveCallback(ReorderableList list);
		protected abstract void onSelectCallback(ReorderableList list);

		#endregion

		protected virtual void drawHeaderCallback(Rect rect)
		{
			EditorGUI.LabelField(rect, Title);
		}

		protected virtual void onReorderCallback(ReorderableList list)
		{
			
		}

		protected virtual void InitGUIStyles(){ _stylesInitialized = true; }
	}
}