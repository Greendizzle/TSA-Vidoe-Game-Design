using UnityEngine;

namespace Runemark.Common
{
	/// <summary>
	/// This class is very similiar to the unity built in Vector2 class, but
	/// this works with integers instead of floats.
	/// </summary>
	[System.Serializable]
	public class Point
	{
		/// <summary>
		/// The x, y and z coordinates.
		/// </summary>
		public int x, y;

		/// <summary>
		/// Initializes a new instance of the <see cref="GridSystem.Point"/> class.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public Point(int x, int y)  //constructor
		{
			this.x = x;
			this.y = y;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GridSystem.Point"/> class.
		/// </summary>
		public Point()
		{
			this.x = 0;
			this.y = 0;
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="GridSystem.Point"/>.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="GridSystem.Point"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current <see cref="GridSystem.Point"/>;
		/// otherwise, <c>false</c>.</returns>
		public override bool Equals(System.Object obj)
		{
			// If parameter is null return false.
			if (obj == null)
			{
				return false;
			}
			
			// If parameter cannot be cast to Point return false.
			Point p = obj as Point;
			if ((System.Object)p == null)
			{
				return false;
			}
			
			// Return true if the fields match:
			return (x == p.x) && (y == p.y);
		}

		/// <summary>
		/// Determines whether the specified <see cref="GridSystem.Point"/> is equal to the current <see cref="GridSystem.Point"/>.
		/// </summary>
		/// <param name="p">The <see cref="GridSystem.Point"/> to compare with the current <see cref="GridSystem.Point"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="GridSystem.Point"/> is equal to the current
		/// <see cref="GridSystem.Point"/>; otherwise, <c>false</c>.</returns>
		public bool Equals(Point p)
		{
			// If parameter is null return false:
			if ((object)p == null)
			{
				return false;
			}
			
			// Return true if the fields match:
			return (x == p.x) && (y == p.y);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode()
		{
			return x ^ y;
		}

		#region Operators
		// + 
		public static Point operator +(Point a, Point b)
		{
			return new Point(a.x + b.x, a.y+b.y);
		}
		
		// - 
		public static Point operator -(Point a, Point b)
		{
			return a + (-1 * b);
		}
		
		// *
		public static Point operator *(Point p, int num)
		{
			return new Point(p.x * num, p.y * num);
		}
		public static Point operator *(int num, Point c)
		{
			return c * num;
		}
		public static Vector2 operator *(Point p, float num)
		{
			return new Vector2(p.x * num, p.y * num);
		}
		public static Vector2 operator *(float num, Point p)
		{
			return p * num;
		}
		
		// /
		public static Point operator /(Point p, int num)
		{
			return p * (1/num);
		}
		public static Point operator /(int num, Point p)
		{
			return p / num;
		}
		public static Vector2 operator /(Point p, float num)
		{
			return p * (1/num);
		}
		public static Vector2 operator /(float num, Point p)
		{
			return p / num;
		}		
		
		// == 
		public static bool operator ==(Point a, Point b)
		{
			// If both are null, or both are same instance, return true.
			if (System.Object.ReferenceEquals(a, b))
			{
				return true;
			}
			
			// If one is null, but not both, return false.
			if (((object)a == null) || ((object)b == null))
			{
				return ((object)a == null) && ((object)b == null);
			}
			
			// Return true if the fields match:
			return a.x == b.x && a.y == b.y;
		}
		
		public static bool operator !=(Point a, Point b)
		{
			return !(a == b);
		}

        public static Point zero { get { return new Point(0, 0); } }
        public static Point one { get { return new Point(1, 1); } }

       #endregion

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="GridSystem.Point"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="GridSystem.Point"/>.</returns>
		public override string ToString ()
		{
			return x+","+y;
		}

		/// <summary>
		/// Parse a point from a specified string.
		/// </summary>
		/// <param name="s">S.</param>
		public static Point Parse(string s)
		{
			string[] vals = s.Split (',');
			Point point = new Point ();

			if(!int.TryParse(vals[0], out point.x) || !int.TryParse(vals[1], out point.y))
                RunemarkDebug.Log(s + "cannot be converted to point!");
			
			return point;
		}

		/// <summary>
		/// Normalize this point.
		/// </summary>
		public void Normalize()
		{
			x = normalized.x;
			y = normalized.y;
		}

		/// <summary>
		/// Gets the point as normalized vector 
		/// </summary>
		/// <value>The normalized.</value>
		public Point normalized
		{
			get
			{
				if (magnitude == 0) return this;

				Point p = new Point ();

				p.x = (x > 0) ? 1 : -1;
				if (x == 0)	p.x = 0;
					
				p.y = (y > 0) ? 1 : -1;
				if (y == 0)	p.y = 0;

				return p;
			}
		}

		/// <summary>
		/// Returns the length of the vector pointing to this point.
		/// </summary>
		/// <value>The magnitude.</value>
		public int magnitude {  get { return x * x + y * y; } }		       
	}
}