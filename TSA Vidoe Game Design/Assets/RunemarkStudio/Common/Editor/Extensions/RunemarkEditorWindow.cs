using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Runemark.Common
{
	public abstract class RunemarkEditorWindow : EditorWindow
	{
		public bool Initialized { get; protected set; }
		public abstract void LoadGraph(object selectedObject);

		void OnGUI()
		{
			onGUI();
			CheckSize();
		}
		protected abstract void onGUI();

		#region OnResize Event
		public delegate void OnResize(float newWidth, float newHeight);
		public OnResize onResize;
		Vector2 _lastSize;

		void CheckSize()
		{
			if (_lastSize.x != this.position.width || _lastSize.y != this.position.height)
			{
				_lastSize = new Vector2(this.position.width, this.position.height);
				if (onResize != null) onResize(_lastSize.x, _lastSize.y);
			}				
		}
		#endregion

	}
}
