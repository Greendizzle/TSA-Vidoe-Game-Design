using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Runemark.VisualEditor
{
	public enum Priority : int { VeryHigh = 2, High = 1, Normal = 0, Low = -1, VeryLow = -2 }
	public enum MouseButton : int { None = -1, Left =  0, Right = 1,  Middle = 2 }
	public enum ModifierKey { None, Shift, Ctrl, Alt }

	public static class InputManager 
	{
		public delegate bool Callback(Event e);

		class InputListener
		{
			public int MouseButton;
			public KeyCode KeyCode;
			public bool Shift;
			public bool Alt;
			public bool Ctrl;
			public EventType Event;

			public int Priority;
			public Callback callback;

			public bool Check(Event e)
			{
				bool mouse = this.MouseButton != -1 && MouseButton == e.button || Event == EventType.ScrollWheel;
				bool key = KeyCode != KeyCode.None && KeyCode == e.keyCode  && GUIUtility.keyboardControl == 0;

				return (mouse || key) && Shift == e.shift && Ctrl == e.command && Alt == e.alt && Event == e.type;
			}
		}
		static List<InputListener> _listeners = new List<InputListener>();


		public static void AddListener(Priority priority, KeyCode code, EventType type, Callback callback, ModifierKey mod = ModifierKey.None)
		{
			_listeners.Add(new InputListener()
				{ 
					MouseButton = (int)MouseButton.None,
					KeyCode = code,
					Shift = mod == ModifierKey.Shift,
					Alt = mod == ModifierKey.Alt,
					Ctrl = mod == ModifierKey.Ctrl,
					Event = type,
					Priority = (int)priority,
					callback = callback
				});
		}

		public static void AddListener(Priority priority, MouseButton button, EventType type, Callback callback, ModifierKey mod = ModifierKey.None)
		{
			_listeners.Add(new InputListener()
				{ 
					MouseButton = (int)button,
					KeyCode = KeyCode.None,
					Shift = mod == ModifierKey.Shift,
					Alt = mod == ModifierKey.Alt,
					Ctrl = mod == ModifierKey.Ctrl,
					Event = type,
					Priority = (int)priority,
					callback = callback
				});
		}


		public static void HandleEvent(Event e)
		{
			var list = _listeners.FindAll(x => x.Check(e)).OrderByDescending(x => x.Priority);
			foreach (var l in list)
			{
				if (l != null && l.callback != null && l.callback(e))
				{
					e.Use();
					return;
				}
			}
		}
	}
}