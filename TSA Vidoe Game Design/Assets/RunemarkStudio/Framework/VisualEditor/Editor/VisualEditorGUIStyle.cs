using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Runemark.Common;

public class VisualEditorGUIStyle 
{
	public struct TextureInfo{ public Texture2D Texture; public RectOffset BorderOffset; }

	static Dictionary<string, TextureInfo> _textures = new Dictionary<string, TextureInfo>();

	public static TextureInfo GetTexture(Color border, Color fill, bool tl, bool tr, bool bl, bool br)
	{
		string key = border + "" + fill + "" + tl + "" + tr + "" + bl + "" + br;
		if (_textures.ContainsKey(key))
			return _textures[key];
		return CreateTexture(key, border, fill, tl, tr, bl, br);		
	}

	static TextureInfo CreateTexture(string key, Color border, Color fill, bool tl, bool tr, bool bl, bool br)
	{
		var tex = new RectangleTexture()
			{
				BorderColor = border,
				FillColor = fill,
				Resolution = 512,
				CornerRadius = 10,
				BorderThickness = 1,
				TL_isRounded = tl,
				TR_isRounded = tr,
				BL_isRounded = bl,
				BR_isRounded = br
			};
		var t = new TextureInfo()
		{
			BorderOffset = tex.BorderOffset,
			Texture = tex.Generate()
		};

		_textures.Add(key, t);
		return t;			
	}

	static GUIStyle _windowHeader;
	public static GUIStyle WindowHeader
	{
		get
		{
			if (_windowHeader == null)
			{
				_windowHeader = new GUIStyle(GUI.skin.label);
				_windowHeader.alignment = TextAnchor.MiddleCenter;
				_windowHeader.fontSize = 16;
				_windowHeader.fontStyle = FontStyle.Bold;
			}
			return _windowHeader;
		}		
	}
}
