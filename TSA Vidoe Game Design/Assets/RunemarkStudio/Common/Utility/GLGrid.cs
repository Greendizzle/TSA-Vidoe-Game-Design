using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runemark.Common
{
	public class GLGrid
	{
		public float SmallTileSize = 5f;

		int _subdivisons = 5;
		Color _smallLineColor;
		Color _largeLineColor;

		public GLGrid(int subdivisons, Color smallLine, Color largeLine)
		{
			_subdivisons = subdivisons;
			_smallLineColor = smallLine;
			_largeLineColor =  largeLine;
		}

		public void Draw(Rect rect, Vector2 offset)
		{
			float largeTileSize = SmallTileSize * (float)_subdivisons;
			float vLength = rect.height + Mathf.Abs(offset.y);
			float hLength = rect.width + Mathf.Abs(offset.x);
			float startX = (float)Mathf.FloorToInt(offset.x / largeTileSize) * largeTileSize;
			float startY = (float)Mathf.FloorToInt(offset.y / largeTileSize) * largeTileSize;

			GL.PushMatrix();
			GL.LoadPixelMatrix();

			for (float x = 0; x < rect.width + Mathf.Abs(offset.x); x += SmallTileSize)
			{
				GL.Begin(GL.LINES);
				GL.Color((x % largeTileSize == 0) ? _largeLineColor : _smallLineColor);

				float xPos = startX + x - offset.x;
				GL.Vertex3(xPos, 0, 0);
				GL.Vertex3(xPos, vLength, 0);

				GL.End();			
			}

			for (float y = 0; y < rect.height + Mathf.Abs(offset.y); y += SmallTileSize)
			{
				GL.Begin(GL.LINES);
				GL.Color((y % largeTileSize == 0) ? _largeLineColor : _smallLineColor);

				float yPos = startY + y - offset.y;
				GL.Vertex3(0, yPos, 0);
				GL.Vertex3(hLength, yPos, 0);

				GL.End();
			}

			GL.PopMatrix();
		}

	}
}