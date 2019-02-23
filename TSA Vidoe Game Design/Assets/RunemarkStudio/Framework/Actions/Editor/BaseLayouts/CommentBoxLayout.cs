using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Runemark.Common;

namespace Runemark.VisualEditor
{
	[CustomNodeLayout(typeof(CommentBox), true)]
	public class CommentBoxLayout : NodeLayout 
	{
        bool DEBUG_RESIZE_RECTS = false;

		protected override string Title { get { return _node.Name; } }

		protected sealed override float headerHeight { get { return 30; } }
		protected sealed override float bodyHeight { get { return height; } }
		protected float height;

		#region Resize Rects
		float rThick = 10f;

		Rect r_T { get { return new Rect(Rect.x, Rect.y, Rect.width, rThick); } }
		Rect r_B { get { return new Rect(Rect.x, Rect.y + Rect.height - rThick, Rect.width, rThick ); } }
		Rect r_L { get { return new Rect(Rect.x, Rect.y, rThick, Rect.height); } }
		Rect r_R { get { return new Rect(Rect.x + Rect.width - rThick, Rect.y, rThick, Rect.height ); } }

		#endregion

		Rect _left { get { return new Rect(Rect.x, Rect.y, rThick, Rect.height); } }
		Rect _right { get { return new Rect(Rect.x + Rect.width - rThick, Rect.y, rThick, Rect.height); }}
		Rect _top { get { return new Rect(Rect.x, Rect.y, Rect.width, rThick); }}
		Rect _bottom { get { return new Rect(Rect.x, Rect.y + Rect.height - rThick, Rect.width, rThick); } }


		protected override int DefaultOrder { get { return -1; } }
		CommentBox _node { get { return ConvertNode<CommentBox>(); } }

		List <NodeLayout> _layouts = new List<NodeLayout>();


		public CommentBoxLayout(Node node) : base(node)
		{
			SetColor();
			width = _node.Size.x;
			height = _node.Size.y;
		}


		public override void Move(Vector2 delta)
		{ 
			foreach (var l  in _layouts)
				l.Node.Position += delta;
			base.Move(delta);
		}

		protected override void OnSelectionChanged(bool selected)
		{
			base.OnSelectionChanged(selected);

			_layouts.Clear();

			Rect absrect = new Rect(_node.Position, bodyRect.size);
			foreach (var node in _node.Parent.Nodes.GetAll)
			{
				if (absrect.Contains(node.Position))
				{
					var layout = _editor.GetLayout(node);
					if (bodyRect.Contains(layout.Rect))
					{
						_layouts.Add(layout);
						layout.Select(selected, true);
					}
				}
			}
		}


	
		protected override void onGUIHeader()
		{		
			GUI.Label(headerRect, Title, titleStyle);
		}

		protected override void onGUIBody()
		{	
			if (_node.EditorParameterChanged)
			{
				SetColor();
				width = _node.Size.x;
				height = _node.Size.y;
				_node.EditorParameterChanged = false;
			}

			var TL = SetCursor(r_T, r_L, MouseCursor.ResizeUpLeft);
			var TR = SetCursor(r_T, r_R, MouseCursor.ResizeUpRight);
			var BL = SetCursor(r_B, r_L, MouseCursor.ResizeUpRight);
			var BR = SetCursor(r_B, r_R, MouseCursor.ResizeUpLeft);

			SetCursor(r_T, MouseCursor.ResizeVertical, TL, TR);
			SetCursor(r_L, MouseCursor.ResizeHorizontal, TL, BL);
			SetCursor(r_B, MouseCursor.ResizeVertical, BL, BR);
			SetCursor(r_R, MouseCursor.ResizeHorizontal, TR, BR);
		}

		Rect SetCursor(Rect r, Rect intersect, MouseCursor cursor)
		{
			return SetCursor(r.Intersection(intersect), cursor);
		}

		Rect SetCursor(Rect r, MouseCursor cursor, params Rect[] subtracts)
		{
			foreach (var s in subtracts)
			{
				if (r.width > r.height)
				{

					if (s.x <= r.x)
					{
						var diffX = s.x + s.width - r.x;
						r.x += diffX;
						r.width -= diffX;
					}
					else if (s.x + s.width >= r.x + r.width)
						r.width -= r.x + r.width - s.x;
				}
				else
				{
					if (s.y <= r.y)
					{
						var diffY = s.y + s.height - r.y;
						r.y += diffY;
						r.height -= diffY;
					}
					else if (s.y + s.height >= r.y + r.height)
						r.height -= r.y + r.height - s.y;
				}
			}            
			EditorGUIUtility.AddCursorRect(r, cursor);

            if(DEBUG_RESIZE_RECTS)
                EditorGUI.DrawRect(r, new Color(1,0,0,.25f));

            return r;
		}



		#region Resizing
		public override bool Resizeable { get { return true; } }

		public override void ResizeStart(Vector2 directionVector)
		{
			_resizing.UP = r_T.Contains(directionVector);
			_resizing.DOWN = r_B.Contains(directionVector);
			_resizing.LEFT = r_L.Contains(directionVector);
			_resizing.RIGHT = r_R.Contains(directionVector);

			_resizing.ACTIVE = _resizing.UP || _resizing.DOWN || _resizing.LEFT || _resizing.RIGHT;
		}

		public override void Resize(Vector2 delta)
		{
			if (!_resizing.ACTIVE) return;

			Vector2 dSize = Vector2.zero;
			Vector2 dPos = Vector2.zero;

			if (_resizing.UP)
			{
				dPos.y = delta.y;
				dSize.y = -delta.y;
			}
			else if (_resizing.DOWN)
				dSize.y = delta.y;			

			if (_resizing.LEFT)
			{
				dPos.x = delta.x;
				dSize.x = -delta.x;
			}
			else if (_resizing.RIGHT)
				dSize.x = delta.x;

            var size = _node.Size + dSize;
            size.x = Mathf.Max(100, size.x);
            size.y = Mathf.Max(50, size.y);

            _node.Size = size;          
            width = _node.Size.x;
			height = _node.Size.y;

			base.Move(dPos);
		}

		public override void ResizeEnd()
		{
			if (_resizing.ACTIVE)
				_resizing.ACTIVE = false;
		}

		#endregion

		protected override void InitStyle()
		{
			base.InitStyle();
			titleStyle.fontSize = 16;
		}	


		void SetColor()
		{
			headerColor = _node.Color;
			bodyColor = _node.Color;
			bodyColor.a = .5f;
		}
	}


	[CustomEditor(typeof(CommentBox))]
	public class CommentBoxInspector : NodeInspector
	{
		protected override bool NameEditable { get { return true; } }

		protected override void onGUI()
		{
			CommentBox myTarget = (CommentBox)target;
			myTarget.Color = EditorGUILayout.ColorField("Color", myTarget.Color);	
		}


	}
}