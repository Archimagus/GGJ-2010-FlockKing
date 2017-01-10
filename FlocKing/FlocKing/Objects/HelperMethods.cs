using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FlocKing.Objects
{
	public static class HelperMethods
	{
		public static Vector2 ToVector2(this Vector3 vector)
		{
			return new Vector2(vector.X, vector.Y);
		}
	}
}
