using UnityEngine;

namespace Runemark.Common
{
	public class ZoomableArea
	{
		public float ZoomValue { get; private set; }
		public int ZoomStep { get; private set; }
		public Vector2 ZoomOffset { get; private set; }

		int _minStep = 1;
		int _maxStep = 10;

		public delegate void DrawContent(Rect rect);
		DrawContent _drawContent;

		Rect _rect;

		public ZoomableArea(int minStep, int maxStep, DrawContent drawContentDelegate)
		{
			_minStep = minStep;
			_maxStep = maxStep;
			_drawContent = drawContentDelegate;
			SetZoomStep(1);
		}

		public void Draw(Rect rect)
		{
			_rect = rect;
			EditorGUIExtension.BeginZoomArea(ZoomValue, rect);
			if (_drawContent != null)
				_drawContent(new Rect(0,0, rect.width, rect.height));
			EditorGUIExtension.EndZoomArea();
		}


		public void Zoom(Rect rect, Vector2 delta, Vector2 center)
		{
			float oldZoom = ZoomValue;

			// Change Zoom Step
			int d = Mathf.RoundToInt(delta.y / Mathf.Abs(delta.y));
			SetZoomStep(Mathf.Clamp(ZoomStep + d, _minStep, _maxStep));

			// Calculate zoom offset
			Vector2 zoomCoordsMousePos = AbsolutePosition(Event.current.mousePosition);

			// Keep the center of the workspace
			ZoomOffset += (zoomCoordsMousePos - ZoomOffset) - (oldZoom / ZoomValue) * (zoomCoordsMousePos - ZoomOffset);
		}

		public void PanTo(Vector2 pos)
		{
			ZoomOffset = pos;
		}

		public void Pan(Vector2 delta)
		{
			delta /= ZoomValue;
			ZoomOffset -= delta;
		}

		void SetZoomStep(int step)
		{
			ZoomStep = step;
			ZoomValue = .5f + (_maxStep - 1 - ZoomStep) * .05f;
		}


		/// <summary>
		/// Converts a position to a zoom rect position.
		/// </summary>
		/// <returns>The position.</returns>
		/// <param name="pos">Position.</param>
		public Vector2 RelativePosition(Vector2 pos)
		{
			pos.x = pos.x - ZoomOffset.x;
			pos.y = pos.y - ZoomOffset.y;

			pos += _rect.position;
			return pos;
		}

		/// <summary>
		/// Converts a rect to a zoom rect position.
		/// </summary>
		/// <returns>The rect.</returns>
		/// <param name="r">The red component.</param>
		public Rect RelativeRect(Rect r)
		{
			var pos = RelativePosition(r.position);
			return new Rect(pos.x, pos.y, r.width, r.height);
		}

		/// <summary>
		/// Converts the zoom area relative position to absolute.
		/// </summary>
		/// <returns>The position.</returns>
		/// <param name="pos">Position.</param>
		public Vector2 AbsolutePosition (Vector2 pos)
		{
			pos.x = pos.x + ZoomOffset.x;
			pos.y = pos.y + ZoomOffset.y;
			return pos - _rect.position;
		}

		/// <summary>
		/// Converts the zoom area relative rect to absolute.
		/// </summary>
		/// <returns>The position.</returns>
		/// <param name="pos">Position.</param>
		public Rect AbsoluteRect(Rect r)
		{
			var p = AbsolutePosition(r.position);
			return new Rect(p.x, p.y, r.width, r.height);
		}
	}
}
