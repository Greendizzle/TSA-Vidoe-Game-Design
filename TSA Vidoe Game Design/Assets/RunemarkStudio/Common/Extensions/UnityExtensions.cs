using System.Collections.Generic;
using UnityEngine;

namespace Runemark.Common
{	
	public static class UnityExtensions
	{
        #region Interfaces
        public static I FindObjectOfInterface<I>() where I : class
        {
            var list = FindObjectsOfInterface<I>();
            if (list.Count > 0)
                return list[0];
            return null;
        }

        public static List<I> FindObjectsOfInterface<I>() where I : class
        {
            MonoBehaviour[] monoBehaviours = GameObject.FindObjectsOfType<MonoBehaviour>();
            List<I> list = new List<I>();

            foreach (MonoBehaviour behaviour in monoBehaviours)
            {
                I component = behaviour.GetComponent<I>();
                if (component != null)
                {
                    list.Add(component);
                }
            }
            return list;
        }
        #endregion


        #region RECT Extensions
        public static Vector2 TopLeft(this Rect rect)
		{
			return new Vector2(rect.x, rect.y);
		}
		public static Rect ScaleSizeBy(this Rect rect, float scale)
		{
			return rect.ScaleSizeBy(scale, rect.center);
		}
		public static Rect ScaleSizeBy(this Rect rect, float scale, Vector2 pivotPoint)
		{
			Rect result = rect;
			result.x -= pivotPoint.x;
			result.y -= pivotPoint.y;
			result.xMin *= scale;
			result.xMax *= scale;
			result.yMin *= scale;
			result.yMax *= scale;
			result.x += pivotPoint.x;
			result.y += pivotPoint.y;
			return result;
		}
		public static Rect ScaleSizeBy(this Rect rect, Vector2 scale)
		{
			return rect.ScaleSizeBy(scale, rect.center);
		}
		public static Rect ScaleSizeBy(this Rect rect, Vector2 scale, Vector2 pivotPoint)
		{
			Rect result = rect;
			result.x -= pivotPoint.x;
			result.y -= pivotPoint.y;
			result.xMin *= scale.x;
			result.xMax *= scale.x;
			result.yMin *= scale.y;
			result.yMax *= scale.y;
			result.x += pivotPoint.x;
			result.y += pivotPoint.y;
			return result;
		}
        
		public static float Left(this Rect rect) { return rect.x; }
		public static float Right(this Rect rect) { return rect.x + rect.width; }
		public static float Top(this Rect rect) { return rect.y; }
		public static float Bottom(this Rect rect) { return rect.y + rect.height; }
        
		public static bool Contains(this Rect rect, Rect other, bool allowInverse = false)
		{
			Vector2 tl = new Vector2(other.x, other.y);
			Vector2 bl = new Vector2(other.x, other.y + other.height);
			Vector2 tr = new Vector2(other.x + other.width, other.y);
			Vector2 br = new Vector2(other.x + other.width, other.y + other.height);

			return	rect.Contains(tl, allowInverse) && 
					rect.Contains(tr, allowInverse) &&
					rect.Contains(bl, allowInverse) && 
					rect.Contains(br, allowInverse);
		}

        public static Rect Normalize(this Rect rect)
		{
			if (rect.width < 0)
			{
				rect.x += rect.width;
				rect.width *= -1;
			}
			if (rect.height < 0)
			{
				rect.y += rect.height;
				rect.height *= -1;
			}
			return rect;
		}
        
		public static Rect Intersection(this Rect rect, Rect other)
		{
			Rect nRect = rect.Normalize();
			Rect nOther = other.Normalize();

			if (nRect.Overlaps(nOther))
			{
				float x = Mathf.Max(nRect.x, nOther.x);
				float y = Mathf.Max(nRect.y, nOther.y);

				float x2 = Mathf.Min(nRect.x + nRect.width, nOther.x + nOther.width);
				float y2 = Mathf.Min(nRect.y + nRect.height, nOther.y + nOther.height);			

				return new Rect(x, y, x2-x, y2-y);
			}

			return new Rect(0,0,0,0);
		}
        #endregion
    }
}